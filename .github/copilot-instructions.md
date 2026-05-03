# Copilot instructions for SerialCommunication

Purpose
- Short guidance for Copilot sessions working on this Arduino-based serial-communication sketch.

Build / test / lint
- No CI, test suite, or lint rules included in this repository.
- Compile with Arduino CLI (example, set --fqbn and COM port for your board):
  - Compile: arduino-cli compile --fqbn arduino:avr:uno "C:\Users\arno\Documents\Hogent - Arno\Technisch Programmeren\SerialCommunication\arduino\SerialCommunication"
  - Upload:  arduino-cli upload -p COM3 --fqbn arduino:avr:uno "C:\Users\arno\Documents\Hogent - Arno\Technisch Programmeren\SerialCommunication\arduino\SerialCommunication"
- PlatformIO (if you add platformio.ini): platformio run --project-dir arduino\SerialCommunication
- Visual Studio: open SerialCommunication.slnx (project wraps the sketch; no automated build targets provided here).
- Tests: none present. No single-test runner to invoke.

High-level architecture
- Type: Arduino sketch using an included SerialCommand library (SerialCommand.h/.cpp).
- Entry points: SerialCommunication.ino -> setup() registers command handlers and configures pins; loop() repeatedly calls sCmd.readSerial() to process incoming serial commands.
- Command parsing: SerialCommand tokenizes input and invokes registered handlers (onSet, onToggle, onGet, onPing, onHelp, onDebug).
- analog.c provides analogReadDelay() used by onGet to sample analog pins with a delay.
- Solution file SerialCommunication.slnx references a project wrapper but does not add tests or CI.

Key conventions and patterns
- String flash optimisation: code uses the F("...") macro for flash-stored strings. Preserve this pattern when adding messages.
- Serial abstraction: .ino defines SerialPort (macro) and Baudrate at top; use these rather than direct Serial/baud constants when modifying the sketch.
- Command syntax: supported commands are:
  - set d[2..4] [on|off|high|low|1|0]
  - set pwm[9..11] [0..255]
  - toggle d[2..4]
  - get d[2..7]
  - get a[0..5]
  - ping, debug, help
- Buffer and command limits: SERIALCOMMANDBUFFER = 32, MAXSERIALCOMMANDS = 10. Changing these requires rebuilding SerialCommand library code.
- Debug flag: SERIALCOMMANDDEBUG is explicitly #undef'ed in SerialCommand.h. Enable debugging by commenting out the #undef.
- Input handling: only printable characters are buffered; terminator (term) is used to mark end of command. Handlers should call sCmd.clearBuffer() when appropriate.

Files to inspect for implementation details
- arduino/SerialCommunication/SerialCommunication.ino
- arduino/SerialCommunication/SerialCommand.h
- arduino/SerialCommunication/SerialCommand.cpp
- arduino/SerialCommunication/analog.c

AI / assistant config
- No CLAUDE.md, .cursorrules, AGENTS.md, .windsurfrules, CONVENTIONS.md, AIDER_CONVENTIONS.md, .clinerules or README files were found. Add repository-specific rules here if you want Copilot to follow additional conventions.

After creating this file: would you like an MCP server configured for this project (for example, an embedded device test harness)?

Summary
- Created concise Copilot instructions describing how to build, where to look for architecture, and repository-specific conventions. Reply if you want any area expanded or additional commands added.
