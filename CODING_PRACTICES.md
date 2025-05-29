# ExanimaTools Coding Practices

## MVVM and Command Patterns
- **Do NOT use CommunityToolkit.Mvvm or its source generators** for commands, observable properties, or relay commands. 
- Use explicit ICommand implementations (e.g., SimpleCommand, AsyncSimpleCommand) and assign them in the constructor, as in CompanyViewModel.
- Avoid any dependencies on source generators for core UI logic.

## General
- Prefer explicit, manual patterns for all UI command and property binding.
- Document any workarounds for Avalonia or toolkit issues in this file.

---

*This file is maintained to ensure maintainability and reliability of the ExanimaTools codebase.*
