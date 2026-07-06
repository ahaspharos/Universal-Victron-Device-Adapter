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
