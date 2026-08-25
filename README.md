The SCADA application is developed in VB.NET for a CubeSat participating in the EuRoC competition. Communication is performed serially through OpenLoRa using the Modbus RTU protocol.

The software works as a lightweight industrial-style SCADA adapted for CubeSat telemetry. It includes real-time monitoring, communication status, GPS information, and sensor validity checks.

The system transfers data such as:

Temperature
Humidity
Atmospheric pressure
Calculated altitude
PM1.0, PM2.5, PM4.0 and PM10
CO₂
VOC index
NOx index
GPS latitude
GPS longitude
GPS altitude
GPS satellites / fix status

A special feature of the code is the use of a compact telemetry protocol, designed to reduce the amount of data transmitted over the limited LoRa bandwidth. It also uses validity flags to indicate whether each sensor or measurement is available and valid.

Overall, the project combines SCADA, Modbus RTU, LoRa telemetry and real-time sensor monitoring in a compact aerospace application for CubeSat missions.
