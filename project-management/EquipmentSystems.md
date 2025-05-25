# Equipment Systems for ExanimaTools

This document describes the systems required to model, manage, and compute equipment in ExanimaTools, including weapon and armour layering, stat stacking, and extensibility for future features.

---

## 1. Equipment Model Overview

Equipment in Exanima is divided into two main categories:
- **Weapons**: Procedural and non-procedural, with various stats (weight, impact, balance, slash, crush, pierce, thrust).
- **Armour**: Layered system, with each piece providing different types of protection and encumbrance.

Each equipment piece has:
- **Type** (Weapon, Armour, etc.)
- **Slot** (Head, Body, Hands, Legs, Feet, etc.)
- **Stats** (see below)
- **Layer** (for armour)

---

## 2. Armour Layering System

Armour is worn in layers, and each layer can provide protection to the same or overlapping body parts. The main layers are:
- **Clothing**: Basic, minimal protection (e.g., shirts, trousers, boots)
- **Padding**: Absorbs blunt force, worn under chainmail/plate (e.g., gambesons)
- **Chainmail**: Excellent against slashing, worn over padding
- **Plate**: Heavy, best protection, reduces agility

### Example Layering (Body):
- Clothing > Padding > Chainmail > Plate

### Layering Rules:
- Only one item per slot per layer (e.g., one chainmail shirt, one plate cuirass)
- Some pieces may conflict (e.g., high greaves may prevent cuisses)
- Full armours may replace several slots (e.g., Plate Armour covers most of the body)

---

## 3. Weapon System

Weapons have the following stats:
- **Weight**: Affects handling, encumbrance, and damage
- **Impact**: Blunt force damage
- **Balance**: Handling and parry reliability
- **Slash**: Slashing damage
- **Crush**: Crushing damage
- **Pierce**: Piercing damage (swing)
- **Thrust**: Piercing damage (stab)

Weapons can be procedural (randomised stats/components) or special (fixed stats).

---

## 4. Stat Stacking and Calculation

### Armour Stat Stacking
- Each armour piece provides stats: Coverage, Impact Resistance, Slash/Crush/Pierce Protection, Encumbrance
- When multiple layers cover the same body part, their stats are **summed or combined**:
  - **Protection**: Additive or diminishing returns (e.g., total slash protection = sum of all layers' slash protection, possibly with diminishing returns for each additional layer)
  - **Encumbrance**: Additive
  - **Coverage**: Highest coverage among layers for a given body part

#### Example Calculation (Body):
- Padded Gambeson: Slash +2, Encumbrance +1
- Chainmail Shirt: Slash +4, Encumbrance +2
- Plate Cuirass: Slash +6, Encumbrance +3
- **Total Slash Protection**: 2 + 4 + 6 = 12
- **Total Encumbrance**: 1 + 2 + 3 = 6

### Weapon Stat Calculation
- Use the weapon's base stats
- Quality and condition may modify stats (e.g., +10% for "Well Made")

---

## 5. Equipment System Requirements

### Data Model
- EquipmentPiece: Name, Type, Slot, Layer, Stats (dictionary or struct), Quality, Condition
- TeamMember: List of equipped items (by slot/layer)

### Core Logic
- **Equip/Unequip**: Add/remove items from slots/layers, enforce conflicts
- **Stat Calculation**: For each body part, sum protection/encumbrance from all equipped items
- **Weapon Handling**: Use equipped weapon's stats for combat calculations

### Extensibility
- Support for new equipment types, stats, and layers
- Support for procedural generation (randomised stats/components)

---

## 6. Example API

```csharp
// Pseudocode for stat stacking
public class EquipmentPiece {
    public string Name;
    public EquipmentType Type;
    public EquipmentSlot Slot;
    public ArmourLayer? Layer; // null for weapons
    public Dictionary<StatType, float> Stats;
}

public class TeamMember {
    public List<EquipmentPiece> EquippedItems;

    public float GetTotalStat(StatType stat, BodyPart part) {
        return EquippedItems
            .Where(e => e.Covers(part))
            .Sum(e => e.Stats.TryGetValue(stat, out var v) ? v : 0);
    }
}
```

---

## 7. References
- See `Armour.md` and `Weapons.md` for detailed lists and stats.
- For more on Exanima's systems, see the [Exanima Wiki](https://exanima.fandom.com/wiki/Exanima_Wiki) and [Reddit discussions](https://www.reddit.com/r/Exanima/).

---

This system provides a flexible, extensible foundation for modelling Exanima's equipment in your tools and UI.
