ERMIS II SCADA - STABLE TELEMETRY + LIVE SPECTROGRAM

Changes in this build:
- One Modbus polling loop only.
- Telemetry MaxBatchRecords = 5.
- Adaptive batch starts at 3 and never exceeds 5.
- Normal polling delay = 200 ms.
- Live spectrum is requested at most every 350 ms (~2.85 fps).
- Spectrum and telemetry are serialized over the same RobustModbusRtuMaster.
- Latest scrolling viridis-style LiveSpectrogramControl included.
- SerialPort ReadTimeout raised to 250 ms.
- On link degradation adaptive batch falls 5 -> 3 -> 1.

Reason:
The ESP32-P4 may generate spectrum at ~6 fps, but requesting every frame over
the same LoRa/Modbus link as telemetry causes unnecessary traffic and can
trigger Compact LoRa batch retries.

Expected:
Telemetry remains active while START is pressed.
Live spectrogram updates roughly 2-3 times/sec.
The UI may skip ESP32 spectrum frame numbers; this is intentional.
