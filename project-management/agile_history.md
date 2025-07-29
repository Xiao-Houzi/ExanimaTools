# ExanimaTools Agile Story History

## Completed Stories

### Three-Tier Equipment Management System (5 points) ✅ **ENHANCED**
**As a user, I want to assign gear from my arsenal to team members for each rank, so I can manage their loadouts with maximum flexibility.**

**Enhanced Implementation (Completed: July 29, 2025):**
- **Three-tier workflow:** Company Arsenal → Member Personal Pool → Member Rank Loadout
- **Superior UI design:** Tab-based rank selection with visual equipment flow
- **Enhanced database:** Three-table architecture (Arsenal, MemberPersonalPool, MemberRankLoadout)
- **Automatic action injection:** Context-aware buttons injected during tree building
- **Optimized layout:** Side-by-side arsenal/personal pool, full-width rank loadout
- **Equipment staging:** Personal pools allow equipment preparation before rank assignment
- **All tests passing:** Enhanced system maintains 41/41 test pass rate

**Key Achievement:** Exceeded original ticket requirements with a superior three-tier system that provides better UX, flexibility, and organization than the originally requested dialog-based approach.

---
**As a developer, I want the TeamMember repository and model to support layered, per-slot, per-rank equipment, so the system matches Exanima's rules.**
- Refactor models and enums for new equipment system.
- Update persistence and tests for nested/layered equipment profiles.
- Remove legacy/duplicate code.

(Completed: 2025-05-25)

---

# Equipment Profile Management (Completed 2025-05-26)

**Goal:** Implement robust equipment profile management for ExanimaTools, ensuring equipment profiles (per-rank, per-slot, layered, with full stat details) are correctly persisted and loaded, matching Exanima’s requirements.

**Highlights:**
- Extended models and database schema to support layered, per-slot, per-rank equipment profiles.
- Ensured all equipment details and stats are persisted and round-tripped via tests.
- Refactored and expanded tests for reliability and coverage.
- All model and persistence logic for equipment profiles is now robust and ready for further UI or integration work.

(No pending items for this story.)

---

# Equipment Browser UI with Categorised Tree (Completed 2025-06-03)

**Goal:** Implement a UI feature that displays all added equipment in a browsable, hierarchical tree structure, allowing users to easily explore, filter, select, and edit items by category and type.

**Background:**
- Exanima's equipment is categorised (e.g., Weapons > Swords > Dagger, Weapons > Polearms > Billhook, Armour > Body > Plate Cuirass, etc.).
- Current UI did not provide a visual list or tree for browsing all added equipment.
- See `Weapons.md`, `Armour.md`, and `EquipmentSystems.md` for category and subcategory details.

**Requirements:**
- Add a tree view to the equipment management UI, showing all equipment grouped by top-level category (Weapon, Armour, etc.), then by subcategory (e.g., Swords, Axes, Polearms for weapons; Body, Head, etc. for armour), and then by specific type (e.g., Dagger, Billhook).
- Equipment data model must support category and subcategory fields (e.g., `Category`, `Subcategory`), and all items must be assigned appropriately (e.g., Dagger: Weapon > Swords > Dagger; Billhook: Weapon > Polearms > Billhook).
- Tree nodes should be expandable/collapsible, and selecting an item should show its details in a dedicated UI panel.
- Provide a way to edit equipment items directly from the tree (e.g., context menu or edit button).
- Support searching/filtering by name, type, or stat.
- Update persistence and tests as needed to support new fields.
- Reference Exanima documentation for canonical categories and subcategories.

**Acceptance Criteria:**
- All equipment is visible in a browsable tree, grouped by category and subcategory.
- Selecting an item in the tree displays its details in a UI panel.
- Equipment items can be edited directly from the tree.
- Adding a new item places it in the correct tree location.
- UI is responsive and supports search/filter.
- Data model and persistence changes are fully tested.

**Notes:**
- UI, model, and persistence for the equipment tree are implemented and tested.
- Manual and automated tests cover tree interactions, editing, and filtering.
- Further polish, accessibility, and documentation improvements may be tracked as a separate story.

(Completed: 2025-06-03)

---

# Add Team Member UI (Completed 2025-05-28)

**As a user, I want to add new team members via the UI, so that I can build and manage my Exanima team.**
- Add a button to the Team Manager tab to open a dialog/form for entering new team member details (Name, Role, Rank, Sex, Type, Equipment Profile).
- Validate required fields and allow saving the new member to the team list.
- Show a confirmation or error message on save.
- Team member input validation is inline and always allows Save, with errors shown next to fields.
- Add Team Member dialog is always accessible from the Team Manager tab.
- All CommunityToolkit.Mvvm dependencies removed from CompanyManagerViewModel and CompanyMemberViewModel; standard C# and Avalonia MVVM patterns used.
- Team members are persisted to SQLite, with schema initialization handled automatically.
- All build/runtime errors resolved; dialog and team list update as expected.

## Subtask: Team Member Input Validation
- Ensure all required fields are filled before allowing save.
- Show inline validation errors for missing or invalid data.
- Validation is now inline and always visible; Save is always enabled.
- Error messages are shown next to the relevant fields.
- All acceptance criteria are met and the feature is complete.

---

# Equipment Stat Expansion: Rank & Points (Completed)

**Completed on 2025-05-28**

- Added Rank and Points fields to equipment model, UI, and persistence.
- Weight is now a float, edited via a slider, and matches Exanima's UI bar (0-1 range, abstracted).
- Rank dropdown defaults to Inept and shows all ranks.
- Points is available as an optional pip stat.
- Weight is not shown as a pip stat and has no numeric label in the UI.
- Documentation updated in EquipmentSystems.md.
- All changes covered by tests and validation.
- UI and code tidied for clarity and maintainability.

---

# Equipment Stat Expansion: Rank & Points (Completed)

**Completed on 2025-05-28**

- Added Rank and Points fields to equipment model, UI, and persistence.
- Weight is now a float, edited via a slider, and matches Exanima's UI bar (0-1 range, abstracted).
- Rank dropdown defaults to Inept and shows all ranks.
- Points is available as an optional pip stat.
- Weight is not shown as a pip stat and has no numeric label in the UI.
- Documentation updated in EquipmentSystems.md.
- All changes covered by tests and validation.
- UI and code tidied for clarity and maintainability.

---

# Equipment Stat Expansion: Rank & Points (Completed)

**Completed on 2025-05-28**

- [x] Expand equipment stats: minimum Rank, Points value, Weight as float (slider in UI)
- [x] Update Weight to float, slider 0–1, abstract bar, no numeric label
- [x] Add validation: team members cannot equip gear above rank/points
- [x] Improve error reporting for equipment loading
- [x] Update Equipment Manager UI: Rank/Weight at top, Weight slider, Points pip stat, no log files in git
- [x] Ensure Weight slider matches Exanima’s UI
- [x] Update documentation (EquipmentSystems.md)
- [x] Move completed ticket to agile_history.md and mark as completed in agile-current.md
- [x] Code tidy and UI clarity

---

# Add Equipment from Database to Arsenal (Completed 2025-05-28)

**As a user, I want to add equipment from the database to my arsenal, so I can build a collection of available gear.**

- Provide a searchable/selectable list of equipment from the database.
- Allow users to add selected equipment to their arsenal.
- Allow duplicate entries in the arsenal we may own more than one rusty dagger.

**Acceptance Criteria:**
- User can browse/search all equipment in the database.
- User can add equipment to their arsenal with a single action.
- Duplicate entries in the arsenal are allowed.
- UI and persistence are updated accordingly.
- Covered by integration/UI tests.

**Highlights:**
- Arsenal model and repository refactored to allow duplicate entries.
- Arsenal and equipment tests updated/added for duplicate handling and persistence.
- Test setup improved with unique DB files/names and cleanup script.
- Seeding script created, improved to check for empty table, and integrated into app startup.
- Dump script created and integrated into command-line entry point.
- All DB path usage unified via DbManager.GetDbPath().
- Extensive logging added to seeding, EquipmentManagerViewModel, and all user actions.
- Debug UI element added to show equipment count loaded from DB.
- Verified correct DB state and logging via dump and log inspection.
- All acceptance criteria for the ticket are now met; further polish will be handled as backlog stories.

(Completed: 2025-05-28)

---
