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


---

# ARCHITECTURE.md

````markdown
# Architecture

## Overview

VictronBridge is built around a simple principle:

> Separate data acquisition from Victron-specific representation.

The system consists of four layers.

┌─────────────────────┐
│ Source Adapters     │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ Mapping Engine      │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ Device Models       │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ D-Bus Backend       │
└─────────────────────┘
````

## Core Concepts
### Source Adapter

Source adapters collect data from external systems.

Examples:
- MQTT
- Modbus TCP
- Modbus RTU
- REST APIs
- WebSockets

A source adapter produces normalized values.

Example:

````
{
  "soc": 82,
  "voltage": 53.4,
  "current": -12.1
}
````
Source adapters know nothing about D-Bus.

## Mapping Engine
The mapping engine connects source values to a device model.

Example:
````
mappings:
  soc: battery/soc
  voltage: battery/voltage
  current: battery/current
````
Result:
````
{
  "soc": 82,
  "voltage": 53.4,
  "current": -12.1
}
````

The mapping engine remains independent from both MQTT and D-Bus.

## Device Models
Device models represent logical Victron devices.

Examples:
- Battery
- PV Inverter
- Grid Meter
- Tank
- Generator

A device model defines:
- required properties
- optional properties
- D-Bus paths
- validation logic

## Example: Battery Model
````
Soc
Voltage
Current
Power
Connected
ProductName
````
Responsible for generating:
````
/Soc
/Dc/0/Voltage
/Dc/0/Current
/Dc/0/Power
/Connected
/ProductName
````
the user never interacts directly with D-Bus paths

## D-Bus Backend
The D-Bus backend is responsible for:
- creating services
- updating values
- publishing signals
- handling reconnects

Example service:
````
com.victronenergy.battery.mqtt01
````
Published paths:
````
/Soc
/Dc/0/Voltage
/Dc/0/Current
````

## Suggested Solution Structure
````
src/

├── VictronBridge.Host
├── VictronBridge.Core
├── VictronBridge.Configuration
├── VictronBridge.DBus
├── VictronBridge.Mapping
├── VictronBridge.Models

├── Sources
│   ├── MQTT
│   ├── ModbusTcp
│   ├── ModbusRtu
│   └── Http

└── Devices
    ├── Battery
    ├── PvInverter
    ├── GridMeter
    ├── Tank
    └── Temperature
````

## Interfaces
### IDataSource
````c#
public interface IDataSource
{
    Task StartAsync(
        CancellationToken cancellationToken);
}
````
### IDeviceModel
````c#
public interface IDeviceModel
{
    string ServiceName { get; }

    IReadOnlyDictionary<string, object?>
        GetDbusValues();
}
````
### IDbusPublisher
````c#
public interface IDbusPublisher
{
    Task PublishAsync(
        string serviceName,
        IReadOnlyDictionary<string, object?> values);
}
````
## Future Plugin System
Long-term goal:
````
Plugins
 ├── Source Plugins
 ├── Device Plugins
 └── Custom Mappers
````
Allowing users to drop assemblies into a plugin directory.
````
/plugins
````
without rebuilding the application.

## Example Runtime Flow
````
MQTT Message

battery/soc = 78
battery/voltage = 52.8
battery/current = -14.2

↓

MQTT Adapter

↓

Mapping Engine

↓

Battery Device Model

↓

D-Bus Backend

↓

com.victronenergy.battery.mqtt01

↓

GX Dashboard
````
## MVP Scope
Version 0.1

### Included
- MQTT Source
- Battery device model
- D-Bus backend
- YAML configuration
- Docker build support
- Native Venus OS deployment

### Excluded
- Modbus
- REST
- Plugin system
- Web UI

## Version 1.0 Goals
- Multiple device instances
- Multiple source types
- Plugin architecture
- Hot reload configuration
- Discovery mechanisms
- Full documentation
- Community templates
