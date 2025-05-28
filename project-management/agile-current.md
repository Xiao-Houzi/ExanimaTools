# Add Team Member UI (Current)

**As a user, I want to add new team members via the UI, so that I can build and manage my Exanima team.**
- Add a button to the Team Manager tab to open a dialog/form for entering new team member details (Name, Role, Rank, Sex, Type, Equipment Profile).
- Validate required fields and allow saving the new member to the team list.
- Show a confirmation or error message on save.
- Team member input validation is inline and always allows Save, with errors shown next to fields.
- Add Team Member dialog is always accessible from the Team Manager tab.
- All CommunityToolkit.Mvvm dependencies removed from TeamManagerViewModel and TeamMemberViewModel; standard C# and Avalonia MVVM patterns used.
- Team members are persisted to SQLite, with schema initialization handled automatically.
- All build/runtime errors resolved; dialog and team list update as expected.

## Subtask: Team Member Input Validation
- Ensure all required fields are filled before allowing save.
- Show inline validation errors for missing or invalid data.
- Validation is now inline and always visible; Save is always enabled.
- Error messages are shown next to the relevant fields.
- All acceptance criteria are met and the feature is complete.

