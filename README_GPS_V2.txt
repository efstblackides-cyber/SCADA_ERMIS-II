GPS COMPACT PROTOCOL V2
=======================

The SCADA project in this archive now accepts BOTH:
  - protocol v1 (existing 15-value packets)
  - protocol v2 (22-value packets with GPS)

V2 adds:
  values[15..16] = latitude  int32, degrees x 10^7
  values[17..18] = longitude int32, degrees x 10^7
  values[19..20] = altitude  int32, centimetres
  values[21]     = satellites uint16

For protocol v2 the sender must:
  1. Set compact response version byte to 2.
  2. Send 22 U16 values for the first record (44 value bytes).
  3. Use a 32-bit change mask for following delta-compressed records.
  4. Keep CRC16 Modbus exactly as before.

The Raspberry main already fills:
  scada_record.gps_latitude
  scada_record.gps_longitude
  scada_record.gps_altitude_m
  scada_record.gps_satellites

Therefore the remaining Raspberry-side change is inside
SCADA_Serial/ScadaSerialSlave.cpp (the compact encoder).
The helper SCADA_Serial_GPS_ProtocolV2_Patch.h contains the GPS word packing.
