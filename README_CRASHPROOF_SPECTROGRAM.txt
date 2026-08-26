ERMIS II - CRASH-PROOF TELEMETRY + SPECTROGRAM

FIXES
=====

1. TELEMETRY DOES NOT STOP THE APP
RobustModbusRtuMaster now exposes TryReadTelemetryBatch().
Temporary LoRa/serial loss returns False instead of allowing an IOException
to cross the WinForms Task boundary.

ErmisMonitorForm retries with backoff and continues running.

2. SPECTROGRAM DIAGNOSTICS
The spectrogram header now shows:
FRAME | BINS | MIN | MAX | PEAK

Examples:

MIN 0 MAX 0
    No useful spectrum amplitude is arriving from ESP32-P4.
    Flash ERMIS_ESP32_P4_NORMAL_SPECTROGRAM.cpp.

MIN 3 MAX 12
    Very narrow dynamic range. The SCADA automatically expands it.

MIN 20 MAX 230
    Healthy spectral amplitude range.

3. HEATMAP FALLBACK
When incoming values have a small but non-zero range, the SCADA expands
that range locally to make the time-frequency structure visible.

4. ESP32-P4
The included P4 source uses DC removal, Hann window, adaptive dB range,
temporal smoothing and gamma lift while preserving the existing 32-band
Modbus register format.

IMPORTANT
=========
If the display says MIN 0 MAX 0 continuously, no drawing algorithm can
create a truthful spectrogram because all 32 transmitted amplitudes are
identical zero. Flash the included ESP32-P4 source first.
