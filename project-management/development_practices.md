# ExanimaTools Development Practices

> **Note:** This file is the single source of truth for all coding, logging, and architectural best practices for the ExanimaTools project. All code files must refer to this file for best practices. Do not duplicate best practice notes in code files—always update and consult this file.

## Logging
- Use the injected `ILoggingService` for all logging throughout the codebase.
- Pass the logger via constructor injection to all view models and repositories.
- Log all important actions, errors, and state changes.
- Logging should be file-based, with logs written to the `logs` directory.
- Avoid static loggers or ad-hoc logging utilities.

## Dependency Injection
- Use constructor injection for all services (logging, repositories, etc.) to improve testability and maintainability.

## MVVM Pattern
- All UI logic must be in view models, with no business logic in the views.
- Use observable properties and proper property change notifications for all UI-bound data.

## Async/Await
- Use async methods for all I/O and database operations.

## Database Environment Awareness
- **Always use the correct database file for the current build configuration.**
    - In DEBUG mode, the database file is `exanima_tools_dev.db`.
    - In RELEASE/production mode, the database file is `exanima_tools.db`.
    - Use `DbManager.GetDbPath()` to obtain the correct path in all code and tools.
- When running tools or scripts (such as database dumps or seeds), always confirm you are targeting the correct database file for your environment.
- Document any environment-specific data or schema differences here.

## Unit Testing
- Use MSTest for all unit and UI logic tests.
- Ensure all public methods are testable and covered by tests.

## UI Consistency
- All ComboBox and dialog logic should be consistent and reusable.
- Default values for ComboBoxes must always be set programmatically.

## General
- No static state except for true singletons or utilities.
- Generous use of logging during development and in production code.
- All new features and bug fixes should include appropriate tests and logging.

## MVVM and Command Patterns
- **Do NOT use CommunityToolkit.Mvvm or its source generators** for commands, observable properties, or relay commands.
- Use explicit ICommand implementations (e.g., SimpleCommand, AsyncSimpleCommand) and assign them in the constructor, as in CompanyViewModel.
- Avoid any dependencies on source generators for core UI logic.
- Prefer explicit, manual patterns for all UI command and property binding.
- Document any workarounds for Avalonia or toolkit issues in this file.

---

## Legacy/Agile Practices (from agile_stories.md)

- All code must be written using modern async, event-driven, and MVVM patterns.
- Provide helpful tools for Exanima players in a tabbed UI.
- Team members are persisted to SQLite, with schema initialization handled automatically.
- Team member input validation is inline and always allows Save, with errors shown next to fields.
- Add Team Member dialog is always accessible from the Team Manager tab.
- All build/runtime errors must be resolved; dialog and team list update as expected.

---

*This file is maintained to ensure maintainability and reliability of the ExanimaTools codebase.*
