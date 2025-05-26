# ExanimaTools Agile Story History

## Completed Stories

### Team Member Repository & Persistence Refactor (3 points)
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

# Equipment Browser UI with Categorised Tree (Completed 2025-05-26)

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

**Highlights:**
- Updated EquipmentPiece model to include `Category` and `Subcategory` fields for hierarchical grouping in the equipment browser tree.
- Added category and subcategory ComboBoxes to the add equipment form. Subcategory options update based on selected category. ComboBoxes are bound to the view model and update the new equipment item.
- ComboBox logic finalized: first dropdown is broad category (e.g., Sword, Axe, Shield for weapons; Body, Head, etc. for armour), second is subtype (e.g., Longsword, Buckler). Add Weapon/Add Armour buttons determine which set is shown. UI and view model logic are now correct and robust.
- All build errors and logging service issues are resolved. The add equipment form is fully functional and user-friendly.
- Equipment tree UI is implemented: all equipment is shown in a hierarchical tree grouped by category and subcategory. Selecting a tree item shows its details in the grid.
- Persistence is now fully integrated: equipment loads from the database on startup, and new equipment is saved to the database. The UI updates automatically after changes.
- Equipment tree border replaced with a Separator for a cleaner, more flexible layout. Tree now stretches to full width and is visually clear.
- All build/runtime errors (including XAML AVLN2000 from invalid Line element) are resolved. UI is tested and working as intended.

(No pending items for this story.)
