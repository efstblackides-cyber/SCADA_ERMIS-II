ERMIS II SCADA - SPECTROGRAM SINGLE POLL FIX

Main corrections:
1. Removed the separate SpectrumReaderLoopAsync / CancellationTokenSource.
2. Telemetry and spectrum are now read sequentially inside PollLoopAsync.
3. This prevents competing Modbus readers on the same COM/LoRa connection.
4. START SPECTRUM only enables spectrum reading after command transmission succeeds.
5. STOP/RESET disables local spectrum reads.
6. Duplicate ErmisMonitorForm_FormClosing definitions were merged into one.
7. MissionControlClient / MissionCommand / SpectrumFrame are Friend types to match
   the internal RobustModbusRtuMaster class.
8. Spectrum status displays Frame, Bin count and Peak frequency.
9. Spectrum RX is logged every 10 frames to avoid flooding the console.

Expected when working:
SPECTRUM RX: Frame=1530 Bins=32 Peak=178.2 Hz

If Frame changes but Bins=0, inspect Raspberry register 122.
