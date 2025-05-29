# Equipment Stat Expansion: Rank & Points (Completed)

<!-- Completed tickets moved to history on 2025-05-28 -->

---

[See agile_history.md for details.]

# Equipment Browser UI with Categorised Tree (Active)

**Goal:** Implement a UI feature that displays all added equipment in a browsable, hierarchical tree structure, allowing users to easily explore, filter, select, and edit items by category and type.

**Background:**
- Exanima's equipment is categorised (e.g., Weapons > Swords > Dagger, Weapons > Polearms > Billhook, Armour > Body > Plate Cuirass, etc.).
- Current UI does not provide a visual list or tree for browsing all added equipment.
- See `Weapons.md`, `Armour.md`, and `EquipmentSystems.md` for category and subcategory details.

**Requirements:**
- [x] Add a tree view to the equipment management UI, showing all equipment grouped by top-level category (Weapon, Armour, etc.), then by subcategory (e.g., Swords, Axes, Polearms for weapons; Body, Head, etc. for armour), and then by specific type (e.g., Dagger, Billhook).
- [x] Equipment data model must support category and subcategory fields (e.g., `Category`, `Subcategory`), and all items must be assigned appropriately (e.g., Dagger: Weapon > Swords > Dagger; Billhook: Weapon > Polearms > Billhook).
- [x] Tree nodes should be expandable/collapsible, and selecting an item should show its details in a dedicated UI panel.
- [x] Provide a way to edit equipment items directly from the tree (e.g., context menu or edit button).
- [x] Support searching/filtering by name, type, or stat.
- [x] Update persistence and tests as needed to support new fields.
- [x] Reference Exanima documentation for canonical categories and subcategories.

**Acceptance Criteria:**
- [x] All equipment is visible in a browsable tree, grouped by category and subcategory.
- [x] Selecting an item in the tree displays its details in a UI panel.
- [x] Equipment items can be edited directly from the tree.
- [x] Adding a new item places it in the correct tree location.
- [x] UI is responsive and supports search/filter.
- [x] Data model and persistence changes are fully tested.

**Notes:**
- UI, model, and persistence for the equipment tree are implemented and tested.
- Manual and automated tests cover tree interactions, editing, and filtering.
- Further polish, accessibility, and documentation improvements may be tracked as a separate story.

(See `Weapons.md`, `Armour.md`, and `EquipmentSystems.md` for category structure.)

# Arsenal Display Controls (Active)

**As a user, I want to view my arsenal in a clear, organized way, so I can see all available equipment at a glance.**
- Create an arsenal display control showing all equipment in the arsenal.
- Support sorting/filtering by type, stats, or name.
- Allow removing equipment from the arsenal.

---

[See agile_history.md for completed stories.]

