#include <Arduino.h>
#include <USB.h>
#include <Wire.h>
#include <FS.h>
#include <SD_MMC.h>
#include <ESP_I2S.h>
#include <DFRobot_LIS.h>
#include <TinyGPSPlus.h>
#include <math.h>

// ============================================================
// FireBeetle 2 ESP32-P4
// PDM microphone + microSD + H3LIS200DL
// High-speed USB OTG CDC Modbus RTU slave
//
// IMPORTANT ARDUINO IDE SETTINGS FOR ESP32-P4:
//   USB CDC On Boot : Disabled
//   USB Mode        : USB-OTG (TinyUSB)
//
// USB Modbus RTU:
//   Slave ID        : 1
//   Format          : 8-N-1
//   Requested baud  : 460800 (USB CDC itself is packet based)
//
// Connect the Raspberry Pi/PC to the board connector marked:
//   HIGH-SPEED USB OTG 2.0
//
// The connector marked "USB CDC" remains the programming/debug port.
// Modbus does not use Serial and no Modbus bytes are sent to that port.
// ============================================================

//#if !defined(ARDUINO_USB_MODE) || ARDUINO_USB_MODE != 0
//#error Select Tools > USB Mode > USB-OTG (TinyUSB)
//#endif

//#if defined(ARDUINO_USB_CDC_ON_BOOT) && ARDUINO_USB_CDC_ON_BOOT
//#error Select Tools > USB CDC On Boot > Disabled
//#endif

// ---------------- USER SETTINGS ----------------

constexpr uint32_t RECORD_SECONDS = 0; // 0 = continuous

constexpr uint32_t AUDIO_SAMPLE_RATE_HZ = 16000;
constexpr uint16_t AUDIO_BITS_PER_SAMPLE = 16;
constexpr uint16_t AUDIO_CHANNELS = 1;

// 512 bytes = 256 PCM samples = 16 ms at 16 kHz mono/16-bit.
// This keeps Modbus response latency low.
constexpr size_t AUDIO_BUFFER_BYTES = 512;

constexpr uint16_t SENSOR_LOG_RATE_HZ = 50;
constexpr uint32_t SENSOR_PERIOD_US =
    1000000UL / SENSOR_LOG_RATE_HZ;

constexpr uint32_t FLUSH_PERIOD_MS = 1000;

// Spectrum
constexpr uint8_t SPECTRUM_BANDS = 32;
constexpr uint16_t SPECTRUM_SAMPLES = 256;
constexpr uint8_t SPECTRUM_EVERY_AUDIO_BLOCKS = 3;

// Spectrogram display tuning.
//
// The original code mapped -80..0 dBFS directly to 0..255.
// With the onboard PDM microphone this can make almost all bands zero.
// We keep the existing 32-band Modbus protocol, but use an adaptive
// 60 dB display window with slow temporal smoothing.
constexpr float SPECTRUM_DYNAMIC_RANGE_DB = 60.0f;
constexpr float SPECTRUM_MIN_FLOOR_DB = -120.0f;
constexpr float SPECTRUM_MAX_CEILING_DB = -15.0f;
constexpr float SPECTRUM_LEVEL_SMOOTHING = 0.72f;
constexpr float SPECTRUM_SCALE_SMOOTHING = 0.92f;

// High-speed USB OTG CDC Modbus RTU
constexpr uint8_t MODBUS_SLAVE_ID = 1;
constexpr uint32_t MODBUS_BAUD = 460800;
constexpr uint16_t MODBUS_REGISTER_COUNT = 80;
// USB CDC packets may be delayed while audio/SD work is running.
// 200 ms prevents valid Modbus frames from being discarded midway.
constexpr uint32_t MODBUS_RX_TIMEOUT_US = 200000;

// GPS UART
// GPS TX -> ESP32 GPIO38 (RX)
// GPS RX <- ESP32 GPIO37 (TX)
// Only GPS TX is required if the module is output-only.
constexpr int8_t GPS_RX_PIN = 38;
constexpr int8_t GPS_TX_PIN = 37;
constexpr uint32_t GPS_BAUD = 115200;

// ---------------- BOARD PINS ----------------

// Onboard PDM microphone
constexpr int8_t PDM_CLK_PIN = 12;
constexpr int8_t PDM_DATA_PIN = 9;

// External H3LIS200DL
constexpr uint8_t I2C_SDA_PIN = 7;
constexpr uint8_t I2C_SCL_PIN = 8;
constexpr uint8_t H3_ADDRESS = 0x18;

// ---------------- H3LIS200DL ----------------

constexpr float G_PER_LSB = 0.78125f; // ±100 g
constexpr uint8_t REG_STATUS = 0x27;
constexpr uint8_t H3_REG_OUT_X_L = 0x28;

DFRobot_H3LIS200DL_I2C accelerometer(&Wire, H3_ADDRESS);

// ---------------- OBJECTS ----------------

I2SClass I2S;
HardwareSerial GPSSerial(1);
TinyGPSPlus gps;
USBCDC ModbusUSB;
File wavFile;
File jsonFile;

uint8_t audioBuffer[AUDIO_BUFFER_BYTES];

// ---------------- RUNTIME STATE ----------------

uint32_t audioDataBytes = 0;
uint32_t sensorSampleNumber = 0;

uint32_t h3I2cErrors = 0;
uint32_t audioReadErrors = 0;
uint32_t audioWriteErrors = 0;
uint32_t jsonWriteErrors = 0;
uint32_t lateSensorSamples = 0;

uint32_t modbusValidRequests = 0;
uint32_t modbusCrcErrors = 0;
uint32_t modbusProtocolErrors = 0;

uint32_t recordingStartMs = 0;
uint32_t recordingStartUs = 0;
uint32_t nextSensorUs = 0;
uint32_t lastFlushMs = 0;

bool recording = false;

// Latest accelerometer snapshot
int16_t latestXCentig = 0;
int16_t latestYCentig = 0;
int16_t latestZCentig = 0;
uint16_t latestTotalCentig = 0;
uint8_t latestSensorStatus = 0;

// Latest spectrum snapshot
// Latest GPS snapshot. Integer scaling keeps Modbus deterministic.
std::int32_t latestGpsLatitudeE7 = 0;     // degrees x 10^7
std::int32_t latestGpsLongitudeE7 = 0;    // degrees x 10^7
std::int32_t latestGpsAltitudeCm = 0;     // metres x 100
std::uint32_t latestGpsSpeedCms = 0;      // m/s x 100
std::uint16_t latestGpsCourseCdeg = 0;    // degrees x 100
std::uint16_t latestGpsSatellites = 0;
std::uint16_t latestGpsHdopX100 = 0;
std::uint32_t latestGpsAgeMs = 0xFFFFFFFFUL;
std::uint16_t latestGpsFlags = 0;
std::uint32_t gpsCharactersProcessed = 0;

uint8_t latestSpectrum[SPECTRUM_BANDS] = {0};

// Smoothed dB values are kept separately from the 8-bit Modbus image.
// This makes the spectrogram stable instead of flickering frame by frame.
float smoothedSpectrumDb[SPECTRUM_BANDS] = {0.0f};
bool spectrumSmoothingInitialized = false;

float spectrumDisplayFloorDb = -110.0f;
float spectrumDisplayCeilingDb = -50.0f;
bool spectrumScaleInitialized = false;

uint32_t spectrumFrameNumber = 0;
uint32_t spectrumTimestampMs = 0;
uint16_t spectrumPeakFrequencyHz = 0;
uint8_t audioBlocksSinceSpectrum = 0;

// Pending command written by Modbus
uint16_t pendingCommand = 0;

// Modbus RX buffer
uint8_t modbusRx[256];
size_t modbusRxLength = 0;
uint32_t modbusLastByteUs = 0;

// Forward declaration
void finalizeRecording();

// ============================================================
// MODBUS REGISTER MAP
// ============================================================
//
// Holding registers, zero-based:
//
//  0   Device ID                    0x4652 ("FR")
//  1   Protocol version             0x0200
//  2   Status bits
//      bit 0 recording
//      bit 1 SD files open
//      bit 2 microphone active
//      bit 3 spectrum valid
//  3   Sensor log rate Hz
//  4-5 Uptime ms                    uint32, high/low
//  6-7 Sensor sample counter        uint32, high/low
//  8   X acceleration g x 100       int16
//  9   Y acceleration g x 100       int16
// 10   Z acceleration g x 100       int16
// 11   Total acceleration g x 100   uint16
// 12   H3 status
// 13-14 Audio data bytes            uint32
// 15-16 H3 I2C errors               uint32
// 17-18 Spectrum frame number       uint32
// 19-20 Spectrum timestamp ms       uint32
// 21   Peak frequency Hz
// 22   Spectrum band count          32
// 23   Spectrum sample count        256
// 24   Audio sample rate            16000
// 25-26 Modbus valid requests       uint32
// 27-28 Modbus CRC errors           uint32
// 29-30 Modbus protocol errors      uint32
// 31   Last/pending command
// 32-47 Spectrum bands:
//       high byte = band 0,2,4...
//       low byte  = band 1,3,5...
// 48   GPS flags:
//       bit 0 location valid
//       bit 1 altitude valid
//       bit 2 speed valid
//       bit 3 course valid
//       bit 4 satellites valid
//       bit 5 HDOP valid
//       bit 6 NMEA characters received
// 49-50 Latitude degrees x 10^7    int32
// 51-52 Longitude degrees x 10^7   int32
// 53-54 Altitude centimetres       int32
// 55-56 Speed centimetres/second   uint32
// 57   Course degrees x 100        uint16
// 58   Satellites                  uint16
// 59   HDOP x 100                  uint16
// 60-61 GPS location age ms        uint32
// 64   Command register, write:
//       1 = flush files
//       2 = finish recording
//
// Supported functions:
//   0x03 Read Holding Registers
//   0x06 Write Single Register
//   0x10 Write Multiple Registers
// ============================================================

uint16_t modbusCrc16(const uint8_t* data, size_t length)
{
    uint16_t crc = 0xFFFF;

    for (size_t i = 0; i < length; ++i)
    {
        crc ^= data[i];

        for (uint8_t bit = 0; bit < 8; ++bit)
        {
            if (crc & 0x0001)
            {
                crc = (crc >> 1) ^ 0xA001;
            }
            else
            {
                crc >>= 1;
            }
        }
    }

    return crc;
}

void modbusSend(const uint8_t* frame, size_t length)
{
    ModbusUSB.write(frame, length);
    ModbusUSB.flush();
}

void modbusSendException(uint8_t function, uint8_t exceptionCode)
{
    uint8_t response[5];

    response[0] = MODBUS_SLAVE_ID;
    response[1] = function | 0x80;
    response[2] = exceptionCode;

    const uint16_t crc = modbusCrc16(response, 3);
    response[3] = static_cast<uint8_t>(crc & 0xFF);
    response[4] = static_cast<uint8_t>(crc >> 8);

    modbusSend(response, sizeof(response));
}

uint16_t saturateU16(uint32_t value)
{
    return value > 0xFFFFUL
        ? 0xFFFFU
        : static_cast<uint16_t>(value);
}

int16_t floatToCentiG(float value)
{
    const float scaled = value * 100.0f;

    if (scaled > 32767.0f)
    {
        return 32767;
    }

    if (scaled < -32768.0f)
    {
        return -32768;
    }

    return static_cast<int16_t>(lroundf(scaled));
}

uint16_t readHoldingRegister(uint16_t reg)
{
    const uint32_t uptime = millis();
    const uint32_t samples = sensorSampleNumber;
    const uint32_t audioBytes = audioDataBytes;
    const uint32_t i2cErrors = h3I2cErrors;
    const uint32_t validRequests = modbusValidRequests;
    const uint32_t crcErrors = modbusCrcErrors;
    const uint32_t protocolErrors = modbusProtocolErrors;

    switch (reg)
    {
        case 0:
            return 0x4652;

        case 1:
            return 0x0200;

        case 2:
        {
            uint16_t status = 0;

            if (recording)
            {
                status |= (1U << 0);
                status |= (1U << 2);
            }

            if (wavFile && jsonFile)
            {
                status |= (1U << 1);
            }

            if (spectrumFrameNumber > 0)
            {
                status |= (1U << 3);
            }

            if (latestGpsFlags & 0x0001U)
            {
                status |= (1U << 4);
            }

            return status;
        }

        case 3:
            return SENSOR_LOG_RATE_HZ;

        case 4:
            return static_cast<uint16_t>(uptime >> 16);

        case 5:
            return static_cast<uint16_t>(uptime);

        case 6:
            return static_cast<uint16_t>(samples >> 16);

        case 7:
            return static_cast<uint16_t>(samples);

        case 8:
            return static_cast<uint16_t>(latestXCentig);

        case 9:
            return static_cast<uint16_t>(latestYCentig);

        case 10:
            return static_cast<uint16_t>(latestZCentig);

        case 11:
            return latestTotalCentig;

        case 12:
            return latestSensorStatus;

        case 13:
            return static_cast<uint16_t>(audioBytes >> 16);

        case 14:
            return static_cast<uint16_t>(audioBytes);

        case 15:
            return static_cast<uint16_t>(i2cErrors >> 16);

        case 16:
            return static_cast<uint16_t>(i2cErrors);

        case 17:
            return static_cast<uint16_t>(spectrumFrameNumber >> 16);

        case 18:
            return static_cast<uint16_t>(spectrumFrameNumber);

        case 19:
            return static_cast<uint16_t>(spectrumTimestampMs >> 16);

        case 20:
            return static_cast<uint16_t>(spectrumTimestampMs);

        case 21:
            return spectrumPeakFrequencyHz;

        case 22:
            return SPECTRUM_BANDS;

        case 23:
            return SPECTRUM_SAMPLES;

        case 24:
            return AUDIO_SAMPLE_RATE_HZ;

        case 25:
            return static_cast<uint16_t>(validRequests >> 16);

        case 26:
            return static_cast<uint16_t>(validRequests);

        case 27:
            return static_cast<uint16_t>(crcErrors >> 16);

        case 28:
            return static_cast<uint16_t>(crcErrors);

        case 29:
            return static_cast<uint16_t>(protocolErrors >> 16);

        case 30:
            return static_cast<uint16_t>(protocolErrors);

        case 31:
            return pendingCommand;

        case 48:
            return latestGpsFlags;

        case 49:
            return static_cast<uint16_t>(
                static_cast<uint32_t>(
                    latestGpsLatitudeE7
                ) >> 16
            );

        case 50:
            return static_cast<uint16_t>(
                static_cast<uint32_t>(
                    latestGpsLatitudeE7
                )
            );

        case 51:
            return static_cast<uint16_t>(
                static_cast<uint32_t>(
                    latestGpsLongitudeE7
                ) >> 16
            );

        case 52:
            return static_cast<uint16_t>(
                static_cast<uint32_t>(
                    latestGpsLongitudeE7
                )
            );

        case 53:
            return static_cast<uint16_t>(
                static_cast<uint32_t>(
                    latestGpsAltitudeCm
                ) >> 16
            );

        case 54:
            return static_cast<uint16_t>(
                static_cast<uint32_t>(
                    latestGpsAltitudeCm
                )
            );

        case 55:
            return static_cast<uint16_t>(
                latestGpsSpeedCms >> 16
            );

        case 56:
            return static_cast<uint16_t>(
                latestGpsSpeedCms
            );

        case 57:
            return latestGpsCourseCdeg;

        case 58:
            return latestGpsSatellites;

        case 59:
            return latestGpsHdopX100;

        case 60:
            return static_cast<uint16_t>(
                latestGpsAgeMs >> 16
            );

        case 61:
            return static_cast<uint16_t>(
                latestGpsAgeMs
            );

        default:
            if (reg >= 32 && reg <= 47)
            {
                const uint8_t firstBand =
                    static_cast<uint8_t>((reg - 32) * 2);

                return
                    (static_cast<uint16_t>(
                        latestSpectrum[firstBand]
                    ) << 8) |
                    latestSpectrum[firstBand + 1];
            }

            return 0;
    }
}

bool writeHoldingRegister(uint16_t reg, uint16_t value)
{
    if (reg != 64)
    {
        return false;
    }

    if (value != 1 && value != 2)
    {
        return false;
    }

    pendingCommand = value;
    return true;
}

void handleReadHoldingRegisters(
    const uint8_t* request,
    size_t requestLength
)
{
    if (requestLength != 8)
    {
        modbusProtocolErrors++;
        modbusSendException(request[1], 0x03);
        return;
    }

    const uint16_t start =
        (static_cast<uint16_t>(request[2]) << 8) |
        request[3];

    const uint16_t quantity =
        (static_cast<uint16_t>(request[4]) << 8) |
        request[5];

    if (
        quantity == 0 ||
        quantity > 64 ||
        start >= MODBUS_REGISTER_COUNT ||
        start + quantity > MODBUS_REGISTER_COUNT
    )
    {
        modbusProtocolErrors++;
        modbusSendException(request[1], 0x02);
        return;
    }

    uint8_t response[3 + 128 + 2];
    const uint8_t byteCount =
        static_cast<uint8_t>(quantity * 2);

    response[0] = MODBUS_SLAVE_ID;
    response[1] = 0x03;
    response[2] = byteCount;

    for (uint16_t i = 0; i < quantity; ++i)
    {
        const uint16_t value =
            readHoldingRegister(start + i);

        response[3 + i * 2] =
            static_cast<uint8_t>(value >> 8);

        response[4 + i * 2] =
            static_cast<uint8_t>(value & 0xFF);
    }

    const size_t payloadLength = 3 + byteCount;
    const uint16_t crc =
        modbusCrc16(response, payloadLength);

    response[payloadLength] =
        static_cast<uint8_t>(crc & 0xFF);

    response[payloadLength + 1] =
        static_cast<uint8_t>(crc >> 8);

    modbusSend(response, payloadLength + 2);
}

void handleWriteSingleRegister(
    const uint8_t* request,
    size_t requestLength
)
{
    if (requestLength != 8)
    {
        modbusProtocolErrors++;
        modbusSendException(request[1], 0x03);
        return;
    }

    const uint16_t reg =
        (static_cast<uint16_t>(request[2]) << 8) |
        request[3];

    const uint16_t value =
        (static_cast<uint16_t>(request[4]) << 8) |
        request[5];

    if (!writeHoldingRegister(reg, value))
    {
        modbusProtocolErrors++;
        modbusSendException(request[1], 0x02);
        return;
    }

    // Standard function 06 echo response.
    modbusSend(request, requestLength);
}

void handleWriteMultipleRegisters(
    const uint8_t* request,
    size_t requestLength
)
{
    if (requestLength < 9)
    {
        modbusProtocolErrors++;
        modbusSendException(request[1], 0x03);
        return;
    }

    const uint16_t start =
        (static_cast<uint16_t>(request[2]) << 8) |
        request[3];

    const uint16_t quantity =
        (static_cast<uint16_t>(request[4]) << 8) |
        request[5];

    const uint8_t byteCount = request[6];

    if (
        quantity == 0 ||
        byteCount != quantity * 2 ||
        requestLength != static_cast<size_t>(9 + byteCount)
    )
    {
        modbusProtocolErrors++;
        modbusSendException(request[1], 0x03);
        return;
    }

    for (uint16_t i = 0; i < quantity; ++i)
    {
        const uint16_t value =
            (static_cast<uint16_t>(
                request[7 + i * 2]
            ) << 8) |
            request[8 + i * 2];

        if (!writeHoldingRegister(start + i, value))
        {
            modbusProtocolErrors++;
            modbusSendException(request[1], 0x02);
            return;
        }
    }

    uint8_t response[8];

    response[0] = MODBUS_SLAVE_ID;
    response[1] = 0x10;
    response[2] = request[2];
    response[3] = request[3];
    response[4] = request[4];
    response[5] = request[5];

    const uint16_t crc = modbusCrc16(response, 6);
    response[6] = static_cast<uint8_t>(crc & 0xFF);
    response[7] = static_cast<uint8_t>(crc >> 8);

    modbusSend(response, sizeof(response));
}

void processModbusFrame(
    const uint8_t* request,
    size_t requestLength
)
{
    if (requestLength < 4)
    {
        modbusProtocolErrors++;
        return;
    }

    if (request[0] != MODBUS_SLAVE_ID)
    {
        return;
    }

    const uint16_t receivedCrc =
        static_cast<uint16_t>(
            request[requestLength - 2]
        ) |
        (
            static_cast<uint16_t>(
                request[requestLength - 1]
            ) << 8
        );

    const uint16_t calculatedCrc =
        modbusCrc16(request, requestLength - 2);

    if (receivedCrc != calculatedCrc)
    {
        modbusCrcErrors++;
        return;
    }

    modbusValidRequests++;

    switch (request[1])
    {
        case 0x03:
            handleReadHoldingRegisters(
                request,
                requestLength
            );
            break;

        case 0x06:
            handleWriteSingleRegister(
                request,
                requestLength
            );
            break;

        case 0x10:
            handleWriteMultipleRegisters(
                request,
                requestLength
            );
            break;

        default:
            modbusProtocolErrors++;
            modbusSendException(request[1], 0x01);
            break;
    }
}

size_t expectedModbusRequestLength()
{
    if (modbusRxLength < 2)
    {
        return 0;
    }

    switch (modbusRx[1])
    {
        case 0x03:
        case 0x06:
            return 8;

        case 0x10:
            if (modbusRxLength >= 7)
            {
                return static_cast<size_t>(
                    9 + modbusRx[6]
                );
            }
            return 0;

        default:
            // Unknown function requests commonly have an 8-byte format.
            return 8;
    }
}

void serviceUsbModbus()
{
    while (ModbusUSB.available() > 0)
    {
        const int value = ModbusUSB.read();

        if (value < 0)
        {
            break;
        }

        if (modbusRxLength >= sizeof(modbusRx))
        {
            modbusRxLength = 0;
            modbusProtocolErrors++;
        }

        modbusRx[modbusRxLength++] =
            static_cast<uint8_t>(value);

        modbusLastByteUs = micros();

        const size_t expected =
            expectedModbusRequestLength();

        if (expected > 0 && modbusRxLength == expected)
        {
            processModbusFrame(
                modbusRx,
                modbusRxLength
            );

            modbusRxLength = 0;
        }
        else if (
            expected > 0 &&
            modbusRxLength > expected
        )
        {
            modbusRxLength = 0;
            modbusProtocolErrors++;
        }
    }

    if (
        modbusRxLength > 0 &&
        static_cast<uint32_t>(
            micros() - modbusLastByteUs
        ) > MODBUS_RX_TIMEOUT_US
    )
    {
        modbusRxLength = 0;
        modbusProtocolErrors++;
    }
}


// ============================================================
// GPS UART
// ============================================================

void initializeGps()
{
    GPSSerial.begin(
        GPS_BAUD,
        SERIAL_8N1,
        GPS_RX_PIN,
        GPS_TX_PIN
    );
}

void updateGpsSnapshot()
{
    std::uint16_t flags = 0;

    if (gps.location.isValid())
    {
        const double latitude =
            gps.location.lat();

        const double longitude =
            gps.location.lng();

        latestGpsLatitudeE7 =
            static_cast<std::int32_t>(
                llround(latitude * 10000000.0)
            );

        latestGpsLongitudeE7 =
            static_cast<std::int32_t>(
                llround(longitude * 10000000.0)
            );

        latestGpsAgeMs = gps.location.age();
        flags |= (1U << 0);
    }

    if (gps.altitude.isValid())
    {
        latestGpsAltitudeCm =
            static_cast<std::int32_t>(
                llround(
                    gps.altitude.meters() *
                    100.0
                )
            );

        flags |= (1U << 1);
    }

    if (gps.speed.isValid())
    {
        const double centimetresPerSecond =
            gps.speed.mps() * 100.0;

        latestGpsSpeedCms =
            static_cast<std::uint32_t>(
                constrain(
                    llround(
                        centimetresPerSecond
                    ),
                    0LL,
                    0xFFFFFFFFLL
                )
            );

        flags |= (1U << 2);
    }

    if (gps.course.isValid())
    {
        latestGpsCourseCdeg =
            static_cast<std::uint16_t>(
                constrain(
                    lround(
                        gps.course.deg() *
                        100.0
                    ),
                    0L,
                    35999L
                )
            );

        flags |= (1U << 3);
    }

    if (gps.satellites.isValid())
    {
        latestGpsSatellites =
            static_cast<std::uint16_t>(
                constrain(
                    gps.satellites.value(),
                    0UL,
                    65535UL
                )
            );

        flags |= (1U << 4);
    }

    if (gps.hdop.isValid())
    {
        latestGpsHdopX100 =
            static_cast<std::uint16_t>(
                constrain(
                    gps.hdop.value(),
                    0UL,
                    65535UL
                )
            );

        flags |= (1U << 5);
    }

    if (gpsCharactersProcessed > 0)
    {
        flags |= (1U << 6);
    }

    latestGpsFlags = flags;
}

void serviceGps()
{
    bool decodedSentence = false;

    while (GPSSerial.available() > 0)
    {
        const int incoming = GPSSerial.read();

        if (incoming < 0)
        {
            break;
        }

        gpsCharactersProcessed++;

        if (
            gps.encode(
                static_cast<char>(incoming)
            )
        )
        {
            decodedSentence = true;
        }
    }

    if (
        decodedSentence ||
        gps.location.isUpdated() ||
        gps.altitude.isUpdated() ||
        gps.speed.isUpdated() ||
        gps.course.isUpdated() ||
        gps.satellites.isUpdated() ||
        gps.hdop.isUpdated()
    )
    {
        updateGpsSnapshot();
    }
}

// ============================================================
// SPECTRUM ANALYSIS — 32 GOERTZEL BANDS
// ============================================================

void calculateSpectrum(
    const int16_t* samples,
    size_t sampleCount
)
{
    if (sampleCount < SPECTRUM_SAMPLES)
    {
        return;
    }

    // ------------------------------------------------------------
    // Remove the PCM DC component first.
    //
    // A microphone offset otherwise creates a large low-frequency
    // component and reduces useful contrast in the spectrogram.
    // ------------------------------------------------------------

    double meanAccumulator = 0.0;

    for (uint16_t n = 0; n < SPECTRUM_SAMPLES; ++n)
    {
        meanAccumulator +=
            static_cast<double>(samples[n]);
    }

    const float sampleMean =
        static_cast<float>(
            meanAccumulator /
            static_cast<double>(SPECTRUM_SAMPLES)
        );


    float frameDb[SPECTRUM_BANDS];

    float strongestDb =
        -160.0f;

    float weakestDb =
        0.0f;

    uint16_t strongestFrequency =
        0;


    // ------------------------------------------------------------
    // 32 Goertzel frequency bands.
    //
    // Existing protocol is preserved:
    // 125, 375, 625 ... 7875 Hz
    //
    // Keeping 32 bands means Raspberry and SCADA do not need a
    // transport/protocol change.
    // ------------------------------------------------------------

    for (
        uint8_t band = 0;
        band < SPECTRUM_BANDS;
        ++band
    )
    {
        const float frequency =
            125.0f +
            250.0f *
            static_cast<float>(band);

        const float omega =
            2.0f *
            PI *
            frequency /
            static_cast<float>(
                AUDIO_SAMPLE_RATE_HZ
            );

        const float coefficient =
            2.0f *
            cosf(omega);

        float q0 =
            0.0f;

        float q1 =
            0.0f;

        float q2 =
            0.0f;


        for (
            uint16_t n = 0;
            n < SPECTRUM_SAMPLES;
            ++n
        )
        {
            // Hann window.
            const float window =
                0.5f -
                0.5f *
                cosf(
                    2.0f *
                    PI *
                    static_cast<float>(n) /
                    static_cast<float>(
                        SPECTRUM_SAMPLES - 1
                    )
                );

            const float input =
                (
                    static_cast<float>(
                        samples[n]
                    ) -
                    sampleMean
                ) *
                window;

            q0 =
                coefficient *
                q1 -
                q2 +
                input;

            q2 =
                q1;

            q1 =
                q0;
        }


        float power =
            q1 * q1 +
            q2 * q2 -
            coefficient *
            q1 *
            q2;

        if (power < 0.0f)
        {
            power =
                0.0f;
        }


        // Hann coherent gain is approximately 0.5.
        // Compensating it gives a more useful dBFS estimate.
        const float amplitude =
            4.0f *
            sqrtf(power) /
            static_cast<float>(
                SPECTRUM_SAMPLES
            );


        float dbfs =
            20.0f *
            log10f(
                amplitude /
                32768.0f +
                1.0e-12f
            );


        if (!isfinite(dbfs))
        {
            dbfs =
                SPECTRUM_MIN_FLOOR_DB;
        }


        dbfs =
            constrain(
                dbfs,
                SPECTRUM_MIN_FLOOR_DB,
                0.0f
            );


        // --------------------------------------------------------
        // Temporal smoothing per frequency band.
        // --------------------------------------------------------

        if (!spectrumSmoothingInitialized)
        {
            smoothedSpectrumDb[band] =
                dbfs;
        }
        else
        {
            smoothedSpectrumDb[band] =
                SPECTRUM_LEVEL_SMOOTHING *
                smoothedSpectrumDb[band] +
                (
                    1.0f -
                    SPECTRUM_LEVEL_SMOOTHING
                ) *
                dbfs;
        }


        frameDb[band] =
            smoothedSpectrumDb[band];


        if (band == 0)
        {
            strongestDb =
                frameDb[band];

            weakestDb =
                frameDb[band];

            strongestFrequency =
                static_cast<uint16_t>(
                    lroundf(
                        frequency
                    )
                );
        }
        else
        {
            if (
                frameDb[band] >
                strongestDb
            )
            {
                strongestDb =
                    frameDb[band];

                strongestFrequency =
                    static_cast<uint16_t>(
                        lroundf(
                            frequency
                        )
                    );
            }

            if (
                frameDb[band] <
                weakestDb
            )
            {
                weakestDb =
                    frameDb[band];
            }
        }
    }


    spectrumSmoothingInitialized =
        true;


    // ------------------------------------------------------------
    // Adaptive display scale.
    //
    // Strongest signal is kept near the top of the colour scale.
    // The floor is 60 dB below it. The scale itself moves slowly,
    // so the image remains visually stable instead of "breathing".
    // ------------------------------------------------------------

    float targetCeilingDb =
        strongestDb +
        3.0f;

    targetCeilingDb =
        constrain(
            targetCeilingDb,
            -100.0f,
            SPECTRUM_MAX_CEILING_DB
        );


    float targetFloorDb =
        targetCeilingDb -
        SPECTRUM_DYNAMIC_RANGE_DB;

    targetFloorDb =
        max(
            targetFloorDb,
            SPECTRUM_MIN_FLOOR_DB
        );


    // If all bands are clustered very tightly, keep at least
    // some space below the weakest band for background contrast.
    targetFloorDb =
        min(
            targetFloorDb,
            weakestDb -
            6.0f
        );

    targetFloorDb =
        max(
            targetFloorDb,
            SPECTRUM_MIN_FLOOR_DB
        );


    if (!spectrumScaleInitialized)
    {
        spectrumDisplayFloorDb =
            targetFloorDb;

        spectrumDisplayCeilingDb =
            targetCeilingDb;

        spectrumScaleInitialized =
            true;
    }
    else
    {
        spectrumDisplayFloorDb =
            SPECTRUM_SCALE_SMOOTHING *
            spectrumDisplayFloorDb +
            (
                1.0f -
                SPECTRUM_SCALE_SMOOTHING
            ) *
            targetFloorDb;

        spectrumDisplayCeilingDb =
            SPECTRUM_SCALE_SMOOTHING *
            spectrumDisplayCeilingDb +
            (
                1.0f -
                SPECTRUM_SCALE_SMOOTHING
            ) *
            targetCeilingDb;
    }


    float displayRangeDb =
        spectrumDisplayCeilingDb -
        spectrumDisplayFloorDb;

    if (displayRangeDb < 20.0f)
    {
        displayRangeDb =
            20.0f;
    }


    // ------------------------------------------------------------
    // Convert the smoothed dB values to 8-bit heat-map intensity.
    //
    // 0   = background / purple
    // 255 = strongest / yellow
    // ------------------------------------------------------------

    for (
        uint8_t band = 0;
        band < SPECTRUM_BANDS;
        ++band
    )
    {
        float normalized =
            (
                frameDb[band] -
                spectrumDisplayFloorDb
            ) /
            displayRangeDb;


        normalized =
            constrain(
                normalized,
                0.0f,
                1.0f
            );


        // Gamma < 1 lifts weak details, which is important for
        // a readable live spectrogram.
        normalized =
            powf(
                normalized,
                0.72f
            );


        const int mapped =
            constrain(
                static_cast<int>(
                    lroundf(
                        normalized *
                        255.0f
                    )
                ),
                0,
                255
            );


        latestSpectrum[band] =
            static_cast<uint8_t>(
                mapped
            );
    }


    spectrumPeakFrequencyHz =
        strongestFrequency;

    spectrumTimestampMs =
        millis();

    spectrumFrameNumber++;
}

void serviceSpectrum(
    const uint8_t* pcmBytes,
    size_t byteCount
)
{
    if (byteCount < SPECTRUM_SAMPLES * sizeof(int16_t))
    {
        return;
    }

    audioBlocksSinceSpectrum++;

    if (
        audioBlocksSinceSpectrum <
        SPECTRUM_EVERY_AUDIO_BLOCKS
    )
    {
        return;
    }

    audioBlocksSinceSpectrum = 0;

    calculateSpectrum(
        reinterpret_cast<const int16_t*>(pcmBytes),
        SPECTRUM_SAMPLES
    );
}

// ============================================================
// WAV HEADER
// ============================================================

void writeLE16(File& file, uint16_t value)
{
    uint8_t bytes[2] = {
        static_cast<uint8_t>(value & 0xFF),
        static_cast<uint8_t>((value >> 8) & 0xFF)
    };

    file.write(bytes, sizeof(bytes));
}

void writeLE32(File& file, uint32_t value)
{
    uint8_t bytes[4] = {
        static_cast<uint8_t>(value & 0xFF),
        static_cast<uint8_t>((value >> 8) & 0xFF),
        static_cast<uint8_t>((value >> 16) & 0xFF),
        static_cast<uint8_t>((value >> 24) & 0xFF)
    };

    file.write(bytes, sizeof(bytes));
}

bool writeWavHeader(File& file, uint32_t pcmDataBytes)
{
    if (!file)
    {
        return false;
    }

    const uint32_t byteRate =
        AUDIO_SAMPLE_RATE_HZ *
        AUDIO_CHANNELS *
        AUDIO_BITS_PER_SAMPLE / 8;

    const uint16_t blockAlign =
        AUDIO_CHANNELS *
        AUDIO_BITS_PER_SAMPLE / 8;

    if (!file.seek(0))
    {
        return false;
    }

    file.write(
        reinterpret_cast<const uint8_t*>("RIFF"),
        4
    );

    writeLE32(file, 36 + pcmDataBytes);

    file.write(
        reinterpret_cast<const uint8_t*>("WAVE"),
        4
    );

    file.write(
        reinterpret_cast<const uint8_t*>("fmt "),
        4
    );

    writeLE32(file, 16);
    writeLE16(file, 1);
    writeLE16(file, AUDIO_CHANNELS);
    writeLE32(file, AUDIO_SAMPLE_RATE_HZ);
    writeLE32(file, byteRate);
    writeLE16(file, blockAlign);
    writeLE16(file, AUDIO_BITS_PER_SAMPLE);

    file.write(
        reinterpret_cast<const uint8_t*>("data"),
        4
    );

    writeLE32(file, pcmDataBytes);

    return true;
}

// ============================================================
// H3LIS200DL
// ============================================================

bool readH3Burst(
    int8_t& rawX,
    int8_t& rawY,
    int8_t& rawZ,
    uint8_t& status
)
{
    Wire.beginTransmission(H3_ADDRESS);
    Wire.write(REG_STATUS);

    if (Wire.endTransmission(false) != 0)
    {
        h3I2cErrors++;
        return false;
    }

    if (
        Wire.requestFrom(
            H3_ADDRESS,
            static_cast<uint8_t>(1)
        ) != 1
    )
    {
        h3I2cErrors++;
        return false;
    }

    status = Wire.read();

    Wire.beginTransmission(H3_ADDRESS);
    Wire.write(H3_REG_OUT_X_L | 0x80);

    if (Wire.endTransmission(false) != 0)
    {
        h3I2cErrors++;
        return false;
    }

    if (
        Wire.requestFrom(
            H3_ADDRESS,
            static_cast<uint8_t>(6)
        ) != 6
    )
    {
        h3I2cErrors++;

        while (Wire.available())
        {
            Wire.read();
        }

        return false;
    }

    Wire.read();
    rawX = static_cast<int8_t>(Wire.read());

    Wire.read();
    rawY = static_cast<int8_t>(Wire.read());

    Wire.read();
    rawZ = static_cast<int8_t>(Wire.read());

    return true;
}

bool writeSensorJson(uint32_t timestampUs)
{
    int8_t rawX = 0;
    int8_t rawY = 0;
    int8_t rawZ = 0;
    uint8_t status = 0;

    if (!readH3Burst(rawX, rawY, rawZ, status))
    {
        return false;
    }

    const float xG = rawX * G_PER_LSB;
    const float yG = rawY * G_PER_LSB;
    const float zG = rawZ * G_PER_LSB;

    const float totalG =
        sqrtf(
            xG * xG +
            yG * yG +
            zG * zG
        );

    latestXCentig = floatToCentiG(xG);
    latestYCentig = floatToCentiG(yG);
    latestZCentig = floatToCentiG(zG);

    latestTotalCentig =
        static_cast<uint16_t>(
            constrain(
                lroundf(totalG * 100.0f),
                0L,
                65535L
            )
        );

    latestSensorStatus = status;

    const size_t before = jsonFile.position();

    jsonFile.print('{');

    jsonFile.print("\"sample\":");
    jsonFile.print(sensorSampleNumber);

    jsonFile.print(",\"time_us\":");
    jsonFile.print(timestampUs);

    jsonFile.print(",\"x_g\":");
    jsonFile.print(xG, 5);

    jsonFile.print(",\"y_g\":");
    jsonFile.print(yG, 5);

    jsonFile.print(",\"z_g\":");
    jsonFile.print(zG, 5);

    jsonFile.print(",\"total_g\":");
    jsonFile.print(totalG, 5);

    jsonFile.print(",\"spectrum_frame\":");
    jsonFile.print(spectrumFrameNumber);

    jsonFile.print(",\"peak_hz\":");
    jsonFile.print(spectrumPeakFrequencyHz);

    jsonFile.print(",\"i2c_errors\":");
    jsonFile.print(h3I2cErrors);

    jsonFile.println('}');

    if (jsonFile.position() <= before)
    {
        jsonWriteErrors++;
        return false;
    }

    sensorSampleNumber++;
    return true;
}

// ============================================================
// INITIALIZATION
// ============================================================

bool initializeSensor()
{
    if (!Wire.begin(I2C_SDA_PIN, I2C_SCL_PIN, 400000))
    {
        return false;
    }

    Wire.setTimeOut(10);

    if (!accelerometer.begin())
    {
        return false;
    }

    if (accelerometer.getID() != 0x32)
    {
        return false;
    }

    if (
        !accelerometer.setRange(
            DFRobot_LIS::eH3lis200dl_100g
        )
    )
    {
        return false;
    }

    accelerometer.setAcquireRate(
        DFRobot_LIS::eNormal_1000HZ
    );

    accelerometer.setHFilterMode(
        DFRobot_LIS::eShutDown
    );

    delay(100);
    return true;
}

bool initializeSD()
{
    if (!SD_MMC.begin(
            "/sdcard",
            true,
            false,
            400,
            5
        ))
    {
        return false;
    }

    if (SD_MMC.cardType() == CARD_NONE)
    {
        SD_MMC.end();
        return false;
    }

    SD_MMC.remove("/audio.wav");
    SD_MMC.remove("/sensor.jsonl");

    wavFile = SD_MMC.open("/audio.wav", FILE_WRITE);
    jsonFile = SD_MMC.open("/sensor.jsonl", FILE_WRITE);

    if (!wavFile || !jsonFile)
    {
        return false;
    }

    if (!writeWavHeader(wavFile, 0))
    {
        return false;
    }

    if (!wavFile.seek(44))
    {
        return false;
    }

    jsonFile.println(
        "{\"type\":\"metadata\","
        "\"transport\":\"High-speed USB OTG CDC Modbus RTU\"," 
        "\"slave_id\":1,"
        "\"audio_sample_rate_hz\":16000,"
        "\"sensor_log_rate_hz\":50,"
        "\"spectrum_bands\":32,"
        "\"gps_uart_rx_gpio\":38,"
        "\"gps_uart_tx_gpio\":37,"
        "\"gps_baud\":9600}"
    );

    jsonFile.flush();
    return true;
}

bool initializeMicrophone()
{
    I2S.setPort(I2S_NUM_0);
    I2S.setPinsPdmRx(PDM_CLK_PIN, PDM_DATA_PIN);

    return I2S.begin(
        I2S_MODE_PDM_RX,
        AUDIO_SAMPLE_RATE_HZ,
        I2S_DATA_BIT_WIDTH_16BIT,
        I2S_SLOT_MODE_MONO
    );
}

// ============================================================
// RECORDING CONTROL
// ============================================================

void finalizeRecording()
{
    if (!recording)
    {
        return;
    }

    recording = false;

    if (wavFile)
    {
        wavFile.flush();
        writeWavHeader(wavFile, audioDataBytes);
        wavFile.flush();
        wavFile.close();
    }

    if (jsonFile)
    {
        jsonFile.flush();
        jsonFile.close();
    }

    I2S.end();
    SD_MMC.end();
}

void fatalStop()
{
    finalizeRecording();

    while (true)
    {
        serviceUsbModbus();
        delay(1);
    }
}

void servicePendingCommand()
{
    const uint16_t command = pendingCommand;

    if (command == 0)
    {
        return;
    }

    pendingCommand = 0;

    if (command == 1)
    {
        if (wavFile)
        {
            wavFile.flush();
        }

        if (jsonFile)
        {
            jsonFile.flush();
        }
    }
    else if (command == 2)
    {
        finalizeRecording();
    }
}

// ============================================================
// SETUP / LOOP
// ============================================================

void setup()
{
    // Create the Modbus virtual COM port on the separate high-speed
    // USB OTG connector. The baud value is host metadata for USB CDC.
    ModbusUSB.setRxBufferSize(sizeof(modbusRx));
    ModbusUSB.setTxTimeoutMs(50);
    ModbusUSB.begin(MODBUS_BAUD);

    USB.manufacturerName("DFRobot");
    USB.productName("FireBeetle P4 Modbus");
    USB.serialNumber("ERMIS-P4-MODBUS");
    USB.begin();

    delay(500);

    initializeGps();

    if (!initializeSensor())
    {
        fatalStop();
    }

    if (!initializeSD())
    {
        fatalStop();
    }

    if (!initializeMicrophone())
    {
        fatalStop();
    }

    recordingStartMs = millis();
    recordingStartUs = micros();
    nextSensorUs = recordingStartUs;
    lastFlushMs = millis();
    recording = true;
}

void loop()
{
    serviceGps();
    serviceUsbModbus();
    servicePendingCommand();

    if (!recording)
    {
        delay(1);
        return;
    }

    const size_t bytesRead =
        I2S.readBytes(
            reinterpret_cast<char*>(audioBuffer),
            AUDIO_BUFFER_BYTES
        );

    serviceGps();
    serviceUsbModbus();

    if (bytesRead == 0)
    {
        audioReadErrors++;
    }
    else
    {
        serviceSpectrum(audioBuffer, bytesRead);

        const size_t bytesWritten =
            wavFile.write(audioBuffer, bytesRead);

        if (bytesWritten != bytesRead)
        {
            audioWriteErrors++;
        }

        audioDataBytes += bytesWritten;
    }

    uint32_t nowUs = micros();
    uint8_t catchupCount = 0;

    while (
        static_cast<int32_t>(nowUs - nextSensorUs) >= 0 &&
        catchupCount < 4
    )
    {
        const uint32_t timestampUs =
            nextSensorUs - recordingStartUs;

        writeSensorJson(timestampUs);

        nextSensorUs += SENSOR_PERIOD_US;
        catchupCount++;
        nowUs = micros();

        serviceGps();
        serviceUsbModbus();
    }

    if (static_cast<int32_t>(nowUs - nextSensorUs) >= 0)
    {
        const uint32_t missed =
            (nowUs - nextSensorUs) /
            SENSOR_PERIOD_US + 1;

        lateSensorSamples += missed;
        nextSensorUs += missed * SENSOR_PERIOD_US;
    }

    const uint32_t nowMs = millis();

    if (nowMs - lastFlushMs >= FLUSH_PERIOD_MS)
    {
        wavFile.flush();
        jsonFile.flush();

        const size_t endPosition = wavFile.position();

        if (writeWavHeader(wavFile, audioDataBytes))
        {
            wavFile.seek(endPosition);
        }

        lastFlushMs = nowMs;
    }

    if (
        RECORD_SECONDS > 0 &&
        nowMs - recordingStartMs >=
            RECORD_SECONDS * 1000UL
    )
    {
        finalizeRecording();
    }

    serviceGps();
    serviceUsbModbus();
}