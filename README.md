# VictronBridge

A generic .NET bridge for integrating external devices into Victron Venus OS through D-Bus.

## Vision

The Victron ecosystem offers excellent monitoring and control capabilities through Venus OS and GX devices. However, integrating third-party devices often requires custom Python scripts for each supported device.

VictronBridge aims to provide a generic, extensible and configuration-driven solution that allows virtually any data source to appear as a native Victron device in the GX dashboard.

Instead of creating one integration per vendor, VictronBridge focuses on a reusable architecture:

```text
Data Source
    ↓
Device Mapping
    ↓
Victron Device Model
    ↓
D-Bus Service
    ↓
GX Dashboard
```


## Goals

- Generic integration framework
- Self-contained .NET deployment
- Configuration-driven mappings
- Native Victron GX integration
- Open source community project
- Extensible source adapters
- Extensible device models

## Use Cases
### MQTT Battery

```text
MQTT
 └── battery/soc
 └── battery/voltage
 └── battery/current

↓

Victron Battery

 └── /Soc
 └── /Dc/0/Voltage
 └── /Dc/0/Current
```
### OpenDTU / Hoymiles
```text
MQTT
 ↓
PV Inverter
 ↓
Victron GX Dashboard
```
### Shelly EM
```text
MQTT
 ↓
Grid Meter
 ↓
Victron GX Dashboard
```
### Heat Pump
```
MQTT
 ↓
Temperature Sensor
 ↓
Victron GX Dashboard
```

## Supported Sources (Planned)
### MQTT

- MQTT 3.x
- MQTT 5
- TLS
- Authentication

### Modbus TCP

- Read Holding Registers
- Read Input Registers

### Modbus RTU

- RS485 devices

### HTTP / REST

- Polling APIs

### WebSocket

- Real-time integrations

### Custom Plugins

- User-defined adapters

## Supported Device Types (Planned)
### Battery
```
com.victronenergy.battery.*
```
### PV Inverter
```
com.victronenergy.pvinverter.*
```
### Grid Meter
```
com.victronenergy.grid.*
```
### Generator
```
com.victronenergy.generator.*
```
### Tank
```
com.victronenergy.tank.*
```
### Temperature Sensor
```
com.victronenergy.temperature.*
```
### Custom Device
User-defined D-Bus services

## Example Configuration
```
source:
  type: mqtt
  host: 192.168.1.100

device:
  type: battery
  serviceName: com.victronenergy.battery.mqtt01

mappings:
  soc: battery/soc
  voltage: battery/voltage
  current: battery/current
```
The framework automatically maps:
```
soc
    → /Soc

voltage
    → /Dc/0/Voltage

current
    → /Dc/0/Current
```
without requiring D-Bus knowledge from the user.

## Why .NET?
- Strong typing
- Excellent tooling
- Self-contained deployment
- Cross-platform
- Lower maintenance complexity
- Familiar ecosystem for many developers

## project Status
Early design phase



