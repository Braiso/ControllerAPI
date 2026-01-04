# DataApp – RAPID Data Exporter

Small console tool to export RAPID data from an ABB controller (real or virtual)
to CSV files using the ABB RobotStudio PC SDK.

## What it does
- Connects to an ABB controller (Virtual Controller supported)
- Scans tasks and modules
- Exports RAPID persistent data to CSV
- Groups output by data type
- Reports non-implemented RAPID types

## Requirements

- Windows
- .NET Framework 4.8
- ABB RobotStudio installed (PC SDK)

## ABB RobotStudio SDK

This project depends on ABB RobotStudio PC SDK.

The ABB assemblies are **not included** in this repository.
You must have ABB RobotStudio installed on your machine.

Tested with:
- RobotStudio 2025
- PC SDK 2025

If you are using a different RobotStudio version, you will need to
update the project references to point to your local SDK installation
(e.g. `C:\Program Files (x86)\ABB\SDK\RobotStudio <version> SDK\`).

## How to run

1. Clone the repository
2. Open `DataApp.sln` in Visual Studio
3. Restore NuGet packages
4. Ensure ABB RobotStudio is installed
5. Update ABB SDK references if required
6. Build the solution in **Release**
7. Run `run.bat`

Output files are generated in a folder named:

ExportacionDatos <ControllerName>

## Notes
- This is a personal utility tool
- No installer required (xcopy / zip usage)
- Intended for local use and experimentation

## License
MIT
