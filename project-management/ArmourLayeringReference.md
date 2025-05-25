# Exanima Armour & Clothing Layering Reference

This document provides a comprehensive breakdown of all clothing and armour that can be worn in each slot in Exanima, including layering order, slot-by-slot options, and notable conflicts. Use this as a reference for modelling the full extent of the equipment system.

---

## Layering Order (Innermost to Outermost)
- **Clothing** (base layer)
- **Padding** (gambesons, arming caps)
- **Chainmail** (coifs, shirts, leggings, gauntlets)
- **Non-Full Armour** (piecemeal: cuirasses, spaulders, bracers, greaves, etc.)
- **Full Armour** (covers multiple slots, e.g. Plate Armour)

---

## Slot-by-Slot Breakdown

### Head
- **Clothing:** Hats
- **Padding:** Leather Cap, Arming Cap
- **Chainmail:** Coif
- **Non-Full Armour:** Metal Helms (various types)
- **Full Armour:** Plate Helm

**Layering Example:** Cap > Coif > Helm (or Plate Helm)

### Body (Torso)
- **Clothing:** Cloth Shirt, Suede Shirt, Silk Shirt, Tunic, Jacket
- **Padding:** Padded Gambeson, Cloth Gambeson
- **Chainmail:** Sleeveless Shirt, Short-Sleeved Shirt, Long-Sleeved Shirt
- **Non-Full Armour:** Leather Cuirass, Splint Cuirass, Brigandine, Plate Cuirass
- **Full Armour:** Plate Armour

**Layering Example:** Shirt > Gambeson > Chainmail Shirt > Plate Cuirass (or Plate Armour)

### Shoulders
- **Non-Full Armour:** Leather Spaulders, Splint Spaulders, Plate Spaulders

### Elbows
- **Non-Full Armour:** Splint Couters, Plate Couters

### Wrists/Forearms
- **Non-Full Armour:** Leather Bracers, Splint Bracers, Plate Bracers, Plate Vambraces

### Hands
- **Clothing:** Gloves
- **Padding:** Leather Gauntlets
- **Chainmail:** Chainmail Gauntlets
- **Non-Full Armour:** Plate Gauntlets

### Legs (Thighs, Knees, Shins)
- **Clothing:** Cloth Pants, Leather Trousers, Silk Trousers
- **Padding:** Padded Leggings, Cloth Leggings
- **Chainmail:** Chainmail Leggings
- **Non-Full Armour:** Splint Cuisses, Plate Cuisses, Leather Greaves, Splint Greaves, Plate Greaves
- **Full Armour:** Plate Leggings (with or without feet)

**Layering Example:** Trousers > Padded Leggings > Chainmail Leggings > Plate Cuisses/Greaves or Plate Leggings

### Feet
- **Clothing:** Sandals, Shoes, Boots
- **Non-Full Armour:** Plated Shoes
- **Full Armour:** Plate Sabatons

---

## Notable Layering Conflicts & Rules
- Some greaves (especially with high knee-guards) may prevent wearing cuisses.
- Full armours (e.g., Plate Armour, Plate Leggings) may replace several slots and prevent wearing some underlying pieces.
- Only one item per slot per layer (e.g., one chainmail shirt, one plate cuirass).
- Some items (like boots vs. sabatons) are mutually exclusive.

---

## Example Layering Table (Body)

| Layer      | Example Items                        |
|------------|--------------------------------------|
| Clothing   | Shirt, Tunic, Jacket                 |
| Padding    | Gambeson                             |
| Chainmail  | Chainmail Shirt                      |
| Armour     | Leather/Splint/Brigandine/Plate Cuirass, Plate Armour |

---

## Use in System Design
- Each slot should support a stack of items, one per layer, with rules for conflicts.
- Full armours should be modelled as covering multiple slots and blocking some layers.
- The system should allow for easy extension as new items or layers are added.
