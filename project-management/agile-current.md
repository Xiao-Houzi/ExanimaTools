<!-- No current stories. -->

# UI Framework Migration: Avalonia to Alternative (Current)

**Goal:** Migrate the ExanimaTools UI from Avalonia to a more familiar and robust cross-platform UI framework to improve development velocity, stability, and maintainability.

**Background:**
- Recent development with Avalonia UI has encountered repeated issues with layout, property bindings, and XAML compatibility.
- Team is more experienced and productive with alternative frameworks (e.g., WPF, WinUI, MAUI, Uno Platform, or web-based UI).
- Migration will enable faster feature delivery, easier debugging, and better long-term support.

**Requirements:**
- Evaluate and select a suitable replacement UI framework (WPF, MAUI, Uno Platform, or web-based such as Blazor or React + Electron).
- Create a migration plan: identify reusable code (models, persistence, view models), and UI features to port.
- Set up a new project structure for the chosen framework.
- Port core UI features: equipment browser, add/edit forms, tree/grid views, and persistence integration.
- Ensure all existing features and workflows are preserved or improved.
- Update build/test/deployment scripts as needed.
- Document migration steps and any breaking changes.

**Acceptance Criteria:**
- New UI project builds and runs with core features ported.
- All major workflows (equipment browsing, adding, editing, persistence) are functional.
- Team is able to develop and maintain the new UI efficiently.
- Documentation is updated for the new framework.

## PROGRESS
- Decision: Migrate from Avalonia UI to .NET MAUI for improved cross-platform support (Windows, macOS, Android, iOS) and better tooling/community.
- MAUI is modern, actively developed by Microsoft, and will allow future expansion to mobile if desired. Linux support is experimental, but Windows/macOS are first-class.
- Next: Plan the migration steps and set up the new MAUI project structure.

## PLAN
1. **Evaluate MAUI Project Structure:**
   - Review .NET MAUI templates and decide on single-project or multi-project setup.
   - Identify which code (models, persistence, view models) can be reused directly.
2. **Create New MAUI Solution:**
   - Scaffold a new .NET MAUI app in a separate folder (e.g., ExanimaTools.MAUI).
   - Add references to shared projects (Models, Persistence).
3. **Port Core UI Features:**
   - Recreate the main window with tabbed navigation (Equipment, Arsenal, etc.).
   - Implement the Equipment Browser: tree view, add/edit forms, and data grid using MAUI controls.
   - Ensure category/subcategory dropdowns and tree structure work as in Avalonia.
4. **Integrate Persistence:**
   - Reuse EquipmentRepository and related logic for SQLite persistence.
   - Test loading/saving equipment in the new UI.
5. **Feature Parity & Testing:**
   - Ensure all workflows (add, edit, browse equipment) are functional.
   - Test on Windows and (optionally) macOS/Android.
6. **Update Documentation:**
   - Document new build/run instructions and any breaking changes.

## NEXT
- Scaffold the new .NET MAUI project and begin porting the Equipment Browser UI.
