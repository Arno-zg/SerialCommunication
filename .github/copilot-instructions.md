# Copilot instructions for SerialCommunication

## Purpose

A bidirectional serial communication project with two components:
- **Arduino firmware**: Sketch that processes serial commands to control digital/analog I/O pins
- **C# desktop application**: Windows Forms app that communicates with the Arduino via serial port

## Build / test / lint

- No CI, test suite, or lint rules included in this repository.

**Arduino sketch** (arduino/SerialCommunication/):
- Compile: `arduino-cli compile --fqbn arduino:avr:uno "C:\Users\arno\Documents\Hogent - Arno\Technisch Programmeren\SerialCommunication\arduino\SerialCommunication"`
- Upload: `arduino-cli upload -p COM3 --fqbn arduino:avr:uno "C:\Users\arno\Documents\Hogent - Arno\Technisch Programmeren\SerialCommunication\arduino\SerialCommunication"`
- Alternative: PlatformIO (if platformio.ini is added): `platformio run --project-dir arduino\SerialCommunication`

**C# desktop application** (SerialCommunication/):
- Open SerialCommunication.slnx in Visual Studio and build/run from the IDE
- No command-line build targets provided; Visual Studio handles compilation and execution
- No tests present

## High-level architecture

**Arduino side:**
- Type: Arduino sketch using an embedded SerialCommand library (SerialCommand.h/.cpp) for command tokenization and routing
- Entry points: setup() initializes pins (digital 2-4 as OUTPUT, digital 5-7 as INPUT, PWM 9-11 as OUTPUT, analog A0-A5 as INPUT) and registers command handlers; loop() calls sCmd.readSerial() to process incoming commands
- Command execution: SerialCommand parses input and invokes handlers (onSet, onToggle, onGet, onPing, onHelp, onDebug)
- analog.c provides analogReadDelay() helper used by onGet to sample analog pins with configurable delay

**Desktop application side:**
- Windows Forms app (Form1.cs) provides a UI to send commands and display responses
- Uses System.IO.Ports.SerialPort for communication
- Form1_Load() discovers available COM ports; combobox dropdown updates port list on demand
- Serial port settings must match Arduino (115200 baud default)

## Key conventions and patterns

**Arduino code:**
- String flash optimisation: Use F("...") macro for flash-stored strings to preserve RAM. Maintain this pattern in all new messages
- Serial abstraction: Top-level macros define SerialPort and Baudrate; use these rather than direct Serial/115200 constants
- Command syntax reference:
  - `set d[2..4] [on|off|high|low|1|0]` – set digital pin
  - `set pwm[9..11] [0..255]` – set PWM value
  - `toggle d[2..4]` – toggle digital pin
  - `get d[2..7]` – read digital pin
  - `get a[0..5]` – read analog pin (returns 0–1023)
  - `ping`, `debug`, `help`
- Buffer limits: SERIALCOMMANDBUFFER = 32, MAXSERIALCOMMANDS = 10. Changing these requires rebuilding SerialCommand
- Debug flag: SERIALCOMMANDDEBUG is explicitly #undef'ed in SerialCommand.h. Comment out the #undef to enable
- Input handling: Only printable characters are buffered; terminator marks command end. Handlers call sCmd.clearBuffer() when needed
- Helper functions: isValidNumber() validates numeric strings; startsWith() checks command prefixes

**C# desktop app:**
- Uses .NET Framework 4.7.2 (see SerialCommunication.csproj)
- UI components are defined in Form1.Designer.cs (auto-generated); edit Form1.cs for logic only
- Serial port discovery: GetPortNames() returns available ports; combobox updates on dropdown expansion
- Exception handling: Generic try-catch blocks (no specific error logging); safe to extend with better error handling

## Files to inspect for implementation details

- arduino/SerialCommunication/SerialCommunication.ino – main sketch, command handlers
- arduino/SerialCommunication/SerialCommand.h – command library header and configuration
- arduino/SerialCommunication/SerialCommand.cpp – command library implementation
- arduino/SerialCommunication/analog.c – analog read with delay helper
- SerialCommunication/Form1.cs – desktop app logic
- SerialCommunication/Form1.Designer.cs – desktop app UI (auto-generated, do not edit)
- SerialCommunication/Program.cs – entry point (STAThread main)
