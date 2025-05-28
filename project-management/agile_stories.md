# ExanimaTools Agile User Stories & Issues

## Agile Story Tracking Procedure (added 2025-05-26)

- The file `agile-current.md` is used to track the single current active agile story, including its task description, progress, and completion state.
- When a story is completed, it is cleaned to a minimal description of work dons and moved from `agile-current.md` to `agile_history.md` (with a completion date), and a note is left in `agile-current.md` for traceability.
- `agile_stories.md` is used for backlog, future, or reference stories, and should not contain the current active story.
- This process ensures clear tracking of the current story, preserves history, and avoids duplication.

# (Moved to agile-current.md on 2025-05-26)

---

## Team Management

<!-- (Moved to agile-current.md on 2025-05-26) -->

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

### BUG: Pip stat editor UI does not log pip click events or update stats in some cases
**As a user, I expect clicking on a pip (or half-pip) in the stat editor to always log the event and update the stat value in the model and UI.**
- Sometimes, clicking a pip does not trigger a log entry or stat update (see log excerpt: no pip click events logged).
- This may indicate a bug in pointer event routing, DataContext, or event handler binding in PipDisplayControl.
- Repro: Open equipment editor, click pips in stat editor, observe missing log entries for pip clicks.
- Expected: Every pip click should log an operation ("Pip Click") and update the stat value.
- Severity: Medium (affects usability and auditability of stat editing)
- Acceptance: All pip clicks reliably log and update stats; covered by automated and manual tests.

---

# Equipment Browser UI with Categorised Tree (Planned)

**Goal:** Implement a UI feature that displays all added equipment in a browsable, hierarchical tree structure, allowing users to easily explore, filter, and select items by category and type.

**Background:**
- Exanima's equipment is categorised (e.g., Weapons > Swords > Dagger, Weapons > Polearms > Billhook, Armour > Body > Plate Cuirass, etc.).
- Current UI does not provide a visual list or tree for browsing all added equipment.
- See `Weapons.md`, `Armour.md`, and `EquipmentSystems.md` for category and subcategory details.

**Requirements:**
- Add a tree view to the equipment management UI, showing all equipment grouped by top-level category (Weapon, Armour, etc.), then by subcategory (e.g., Swords, Axes, Polearms for weapons; Body, Head, etc. for armour), and then by specific type (e.g., Dagger, Billhook).
- Equipment data model must support category and subcategory fields (e.g., `Category`, `Subcategory`), and all items must be assigned appropriately (e.g., Dagger: Weapon > Swords > Dagger; Billhook: Weapon > Polearms > Billhook).
- Tree nodes should be expandable/collapsible, and selecting an item should show its details.
- Support searching/filtering by name, type, or stat.
- Update persistence and tests as needed to support new fields.
- Reference Exanima documentation for canonical categories and subcategories.

**Acceptance Criteria:**
- All equipment is visible in a browsable tree, grouped by category and subcategory.
- Adding a new item places it in the correct tree location.
- UI is responsive and supports search/filter.
- Data model and persistence changes are fully tested.

(See `Weapons.md`, `Armour.md`, and `EquipmentSystems.md` for category structure.)

---

# Equipment Stat Expansion: Rank & Points (Planned)

**As a user, I want equipment to have a minimum rank requirement and a points value, so that team management and loadout rules match Exanima's gameplay.**

- Add a `Rank` stat to each equipment piece, representing the minimum team member rank required to equip it.
- Add a `Points` stat to each equipment piece, representing its loadout cost.
- Each team member rank will have a maximum allowed points value for equipped gear.
- Update the data model, UI, and persistence to support these new fields.
- Update documentation in `EquipmentSystems.md` and `Armour.md` to describe the new stats and their gameplay impact.
- Update the Equipment Manager UI to allow editing/viewing these stats.
- Validate that team members cannot equip gear above their rank or points limit.

**Acceptance Criteria:**
- Equipment pieces have `Rank` and `Points` fields in the model, database, and UI.
- Team members cannot equip gear above their rank or points limit.
- Documentation is updated to reflect new rules and fields.
- All changes are covered by tests.


