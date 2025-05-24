# ExanimaTools Agile User Stories & Issues

## Team Management

### Add Team Member UI (3 points)
**As a user, I want to add new team members via the UI, so that I can build and manage my Exanima team.**
- Add a button to the Team Manager tab to open a dialog/form for entering new team member details (Name, Role, Rank, Sex, Type, Equipment Profile).
- Validate required fields and allow saving the new member to the team list.
- Show a confirmation or error message on save.

#### Subtask: Team Member Input Validation (2 points)
- Ensure all required fields are filled before allowing save.
- Show inline validation errors for missing or invalid data.

### Team Display UI (3 points)
**As a user, I want to see a list of all team members, each represented by a TeamMember control, so I can view and manage my team at a glance.**
- Display all team members in a scrollable list or panel on the Team Manager tab.
- Each member should use the TeamMember control, showing their characteristics and loadout dropdown.
- Support editing and removing team members from the list.

### Team Member Control (3 points)
**As a user, I want each team member to be displayed in a card/control showing their characteristics and a dropdown for their armour loadout per rank.**
- Show Name, Role, Rank, Sex, Type.
- Dropdown to show and select their armour loadout for each rank.
- Button to edit or remove the member.

#### Subtask: Edit/Remove Team Member (2 points)
- Implement edit dialog for existing members.
- Add remove button with confirmation dialog.

### Equipment Profile Management (3 points)
**As a user, I want to manage equipment profiles for team members, so I can assign and view their gear.**
- Allow adding, editing, and removing equipment profiles.
- Assign equipment profiles to team members.
- Show equipment stats (pips/half-pips, description, make, quality, condition).

#### Subtask: Equipment Profile Assignment (2 points)
- UI for assigning equipment profiles to team members.
- Persist assignment in data model.

## Equipment & Arsenal

### Add Equipment from Database to Arsenal (2 points)
**As a user, I want to add equipment from the database to my arsenal, so I can build a collection of available gear.**
- Provide a searchable/selectable list of equipment from the database.
- Allow users to add selected equipment to their arsenal.
- Prevent duplicate entries in the arsenal.

### Arsenal Display Controls (3 points)
**As a user, I want to view my arsenal in a clear, organized way, so I can see all available equipment at a glance.**
- Create an arsenal display control showing all equipment in the arsenal.
- Support sorting/filtering by type, stats, or name.
- Allow removing equipment from the arsenal.

### Equipment Display Controls (3 points)
**As a user, I want each piece of equipment to be displayed in a card/control showing its details and stats.**
- Show name, type, stats (pips/half-pips), description, make, quality, and condition.
- Use visual indicators for pips/half-pips.
- Support editing or removing equipment from the database (optional).

## Loadouts & Assignment

### Assign Gear from Arsenal to Team Members (per Rank) (3 points)
**As a user, I want to assign gear from my arsenal to team members for each rank, so I can manage their loadouts.**
- Allow selecting equipment from the arsenal for each team member and rank.
- Update the team member’s equipment profile accordingly.
- Show assigned gear in the team member’s display.

#### Subtask: Arsenal Selection UI (2 points)
- UI for searching/selecting equipment from arsenal for assignment.
- Prevent duplicate assignments.

### Create and Manage Per-Rank Loadouts (3 points)
**As a user, I want to create and manage different loadouts for each team member rank (Novice, Veteran, Elite, etc.), so I can optimize gear for each level.**
- Allow users to define a separate loadout for each rank.
- UI to switch between and edit loadouts per rank.
- Persist loadouts with the team member data.

#### Subtask: Loadout Persistence (2 points)
- Save/load per-rank loadouts to/from data store.

### Display Loadout and Final Stats (5 points)
**As a user, I want to view a team member’s current loadout and see the final stats for each attribute, summed from all equipment worn.**
- Display a summary of all equipped items for the selected rank.
- Calculate and show the total stats (e.g., total Impact, Cut, Pierce, etc.) by summing all equipped pieces.
- Update the display dynamically as loadout changes.

#### Subtask: Stat Calculation Logic (3 points)
- Implement logic to sum stats from all equipped items.
- Unit test stat calculation for accuracy.

#### Subtask: Dynamic UI Update (2 points)
- Update UI in real time as loadout changes.
