# Equipment Stat Expansion: Rank & Points (Current)

**As a user, I want equipment to have a minimum rank requirement and a points value, so that team management and loadout rules match Exanima's gameplay.**

- Add a `Rank` stat to each equipment piece, representing the minimum team member rank required to equip it.
- Add a `Points` stat to each equipment piece, representing its loadout cost.
- Each team member rank will have a maximum allowed points value for equipped gear.
- Update the data model, UI, and persistence to support these new fields.
- Update documentation in `EquipmentSystems.md` and `Armour.md` to describe the new stats and their gameplay impact.
- Update the Equipment Manager UI to allow editing/viewing these stats.
- Validate that team members cannot equip gear above their rank or points limit.
- **Update:** The `Weight` stat should be a float and represented in the UI by a slider for more precise adjustment.

**Acceptance Criteria:**
- Equipment pieces have `Rank` and `Points` fields in the model, database, and UI.
- Team members cannot equip gear above their rank or points limit.
- `Weight` is a float and is edited via a slider in the UI.
- Documentation is updated to reflect new rules and fields.
- All changes are covered by tests.

