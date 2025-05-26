# ExanimaTools Agile Story History

## Completed Stories

### Team Member Repository & Persistence Refactor (3 points)
**As a developer, I want the TeamMember repository and model to support layered, per-slot, per-rank equipment, so the system matches Exanima's rules.**
- Refactor models and enums for new equipment system.
- Update persistence and tests for nested/layered equipment profiles.
- Remove legacy/duplicate code.

(Completed: 2025-05-25)

---

## Partially Completed Stories (backend/model only)

### Add New Equipment to Database (3 points)
**As a user, I want to add new equipment pieces (weapons or armour) to the equipment database, so I can expand my arsenal.**
- Add a UI form for entering equipment details (name, type, stats, description, make, quality, condition).
- Validate required fields and save new equipment to the database.
- Show confirmation or error messages on save.

// Model and persistence implemented. UI for adding arbitrary equipment is not complete.

### Equipment Profile Management (3 points)
**As a user, I want to manage equipment profiles for team members, so I can assign and view their gear.**
- Allow adding, editing, and removing equipment profiles.
- Assign equipment profiles to team members.
- Show equipment stats (pips/half-pips, description, make, quality, condition).

// Model and persistence implemented and tested. UI for profile management and stat display is not complete.

### Add Equipment from Database to Arsenal (2 points)
**As a user, I want to add equipment from the database to my arsenal, so I can build a collection of available gear.**
- Provide a searchable/selectable list of equipment from the database.
- Allow users to add selected equipment to their arsenal.
- Prevent duplicate entries in the arsenal.

// Arsenal UI and logic are not present. Not done.

### Equipment Display Controls (3 points)
**As a user, I want each piece of equipment to be displayed in a card/control showing its details and stats.**
- Show name, type, stats (pips/half-pips), description, make, quality, and condition.
- Use visual indicators for pips/half-pips.
- Support editing or removing equipment from the database (optional).

// Model supports it, but UI is likely incomplete.
