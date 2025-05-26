# Equipment Browser UI with Categorised Tree (Current)

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

## PROGRESS
- Equipment browser tree is fully implemented, visually clear, and stretches to full width. All build/runtime errors are resolved.
- All requirements and acceptance criteria are met: hierarchical tree, add form, persistence, and UI updates are complete and tested.

## DONE
- Equipment Browser UI with Categorised Tree is complete and merged to main.

<!-- Ticket closed: 2025-05-26 -->
