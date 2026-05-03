# Copilot instructions for SerialCommunication

## Purpose

This is a dual-component project:
- **Arduino sketch** (arduino/SerialCommunication/): command-based serial interface for digital I/O, PWM, and analog input control
- **C# Windows desktop app** (SerialCommunication/): GUI for communicating with the Arduino via serial port

## Build, test, and lint

No CI, test suite, or automated lint rules in this repository.

### Arduino sketch

Compile with Arduino CLI (adjust `--fqbn` and `-p` for your board and COM port):
```bash
arduino-cli compile --fqbn arduino:avr:uno "C:\Users\arno\Documents\Hogent - Arno\Technisch Programmeren\SerialCommunication\arduino\SerialCommunication"
arduino-cli upload -p COM3 --fqbn arduino:avr:uno "C:\Users\arno\Documents\Hogent - Arno\Technisch Programmeren\SerialCommunication\arduino\SerialCommunication"
```

Alternative: PlatformIO (if platformio.ini is added):
```bash
platformio run --project-dir arduino\SerialCommunication
```

### C# desktop app

Open `SerialCommunication.slnx` in Visual Studio. No automated build targets provided for CLI compilation.

## High-level architecture

### Arduino sketch architecture

- **Entry point**: `SerialCommunication.ino`
  - `setup()` initializes serial port (115200 baud), registers command handlers, and configures pin modes
  - `loop()` repeatedly calls `sCmd.readSerial()` to process incoming serial commands
- **Command parser**: `SerialCommand` library (SerialCommand.h/.cpp)
  - Tokenizes incoming serial commands and dispatches to registered handlers (onSet, onToggle, onGet, onPing, onHelp, onDebug)
  - Buffer size: 32 bytes; max 10 registered commands
  - Default handler for unrecognized commands
- **Pin utilities**: `analog.c` provides `analogReadDelay()` for sampling analog pins

### Command interface

Supported commands (case-sensitive):
- `set d2 high|on|1` — Set digital pin 2 high (pins 2-4 are outputs)
- `set d3 low|off|0` — Set digital pin low
- `set pwm9 128` — Set PWM on pin 9 to value 0-255 (pins 9-11 are PWM outputs)
- `toggle d2` — Toggle digital pin state
- `get d2` — Read digital pin 2 (pins 2-7; returns 0 or 1)
- `get a0` — Read analog pin A0 (pins A0-A5; returns 0-1023)
- `ping` — Echo "pong"
- `debug` — Toggle debug output
- `help` — List available commands

### C# desktop app

Windows Forms application with serial port selection, baud rate settings, and command/response display. Manages serial communication with Arduino.

## Key conventions and patterns

- **Flash string optimization**: All serial output messages use `F("...")` macro to store strings in flash memory. Preserve this pattern when adding new output.
- **Serial abstraction**: `.ino` defines `SerialPort` (macro) and `Baudrate` constants at the top. Use these rather than hardcoded `Serial` or baud values.
- **Error messages**: Handlers output to SerialPort using consistent `println(F("Error: ..."))` or `println(F("set done"))` patterns.
- **Helper functions**: `startsWith()`, `isValidNumber()` validate arguments before processing.
- **Buffer limits**: `SERIALCOMMANDBUFFER = 32`, `MAXSERIALCOMMANDS = 10`. Changing these requires rebuilding SerialCommand library code.
- **Debug mode**: `SERIALCOMMANDDEBUG` is explicitly `#undef`'ed in SerialCommand.h. Enable debugging by commenting out the `#undef`.
- **Input handling**: Only printable characters are buffered. Handlers should call `sCmd.clearBuffer()` after error or when required.

## Files to inspect

- `arduino/SerialCommunication/SerialCommunication.ino` — Main sketch with command handlers
- `arduino/SerialCommunication/SerialCommand.h` — SerialCommand library header (buffer sizes, debug flag)
- `arduino/SerialCommunication/SerialCommand.cpp` — SerialCommand implementation
- `arduino/SerialCommunication/analog.c` — Analog read utilities
- `SerialCommunication/Form1.cs` — C# desktop app UI and serial communication logic

## Notes

- No other AI assistant config files (CLAUDE.md, .cursorrules, AGENTS.md, etc.) are present
- The project uses standard Visual Studio .gitignore for C# projects
