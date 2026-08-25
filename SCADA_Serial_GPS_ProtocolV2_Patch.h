#pragma once
#include <array>
#include <cstdint>
#include <cmath>

// Compact Protocol v2 layout expected by the updated SCADA.
// Existing fields 0..14 stay unchanged.
// GPS fields are appended as 16-bit words so the old sensor layout is preserved.
//
// 15 Latitude high  (int32 degrees * 1e7)
// 16 Latitude low
// 17 Longitude high (int32 degrees * 1e7)
// 18 Longitude low
// 19 GPS altitude high (int32 centimetres)
// 20 GPS altitude low
// 21 Satellites (uint16)
//
// Protocol version byte must be 2.
// First record: 22 values = 44 bytes.
// Following records: 32-bit change mask instead of the old 16-bit mask.

namespace scada_gps_v2 {

constexpr std::uint8_t PROTOCOL_VERSION = 2;
constexpr std::size_t VALUE_COUNT = 22;

inline std::uint16_t highWord(std::uint32_t v) {
    return static_cast<std::uint16_t>(v >> 16);
}

inline std::uint16_t lowWord(std::uint32_t v) {
    return static_cast<std::uint16_t>(v & 0xFFFFu);
}

template <typename TelemetryRecordT>
inline void appendGpsWords(
    const TelemetryRecordT& r,
    std::array<std::uint16_t, VALUE_COUNT>& values)
{
    const std::int32_t lat_e7 = static_cast<std::int32_t>(
        std::llround(r.gps_latitude * 10000000.0));

    const std::int32_t lon_e7 = static_cast<std::int32_t>(
        std::llround(r.gps_longitude * 10000000.0));

    const std::int32_t alt_cm = static_cast<std::int32_t>(
        std::llround(static_cast<double>(r.gps_altitude_m) * 100.0));

    const auto lat_u = static_cast<std::uint32_t>(lat_e7);
    const auto lon_u = static_cast<std::uint32_t>(lon_e7);
    const auto alt_u = static_cast<std::uint32_t>(alt_cm);

    values[15] = highWord(lat_u);
    values[16] = lowWord(lat_u);
    values[17] = highWord(lon_u);
    values[18] = lowWord(lon_u);
    values[19] = highWord(alt_u);
    values[20] = lowWord(alt_u);
    values[21] = r.gps_satellites;
}

} // namespace scada_gps_v2
