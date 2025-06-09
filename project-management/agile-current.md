<!-- Completed tickets moved to history on 2025-06-03 -->

---

# Assign Gear from Arsenal to Team Members (per Rank) (Active)

**Goal:** Allow users to assign gear from the arsenal to team members for each rank, managing their loadouts.

**Background:**
- Users need to select equipment from the arsenal for each team member and rank.
- Assigned gear should update the team member’s equipment profile and be visible in their display.

**Requirements:**
- Allow selecting equipment from the arsenal for each team member and rank.
- Update the team member’s equipment profile accordingly.
- Show assigned gear in the team member’s display.

#### Subtask: Arsenal Selection UI (2 points)
- UI for searching/selecting equipment from arsenal for assignment.
- Prevent duplicate assignments.

**Acceptance Criteria:**
- Users can assign gear from the arsenal to team members for each rank.
- Assigned gear is reflected in the team member’s display and data model.
- UI prevents duplicate assignments and supports searching/selecting equipment.
- All changes are persisted and tested.

(See agile_stories.md for backlog and future stories.)

## Universal Tree Control (Reusable, Builder-based, Filterable)

**Goal:** Create a universal tree control that can be tailored to any current or future tree view in the app, with a silver border, injectable item view, and filter-driven content.

**Acceptance Criteria:**
- A new Avalonia control (UniversalTreeControl) exists in Controls/.
- The control uses a builder pattern to generate its tree structure from any data source.
- The item view/template is injectable, allowing custom display for any node type.
- The control has a silver border by default (Avalonia styling).
- The control exposes a filter property or delegate, and updates its content dynamically based on the filter (compatible with existing filter viewmodels).
- Existing tree views (e.g., Arsenal, Equipment, Company) can be migrated to use this control with minimal changes.
- Usage and extension are documented for future developers.

**Steps:**
1. Design and implement UniversalTreeControl.axaml/.cs with border and template injection.
2. Implement UniversalTreeBuilder<T> to generate tree structures from flat or hierarchical data.
3. Add filter support (property or delegate) and ensure dynamic updates.
4. Replace one existing tree view as a proof of concept.
5. Document usage and extension in the project-management folder.

