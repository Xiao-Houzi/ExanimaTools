using System.Collections.Generic;

namespace ExanimaTools.Models;

public enum EquipmentType
{
    Weapon,
    Armour
}

public class EquipmentStat
{
    public string Name { get; set; } = string.Empty; // e.g. "Impact", "Cut"
    public int FullPips { get; set; } // Number of full pips
    public bool HasHalfPip { get; set; } // True if there is a half pip
}

// EquipmentPiece is now defined in TeamMember.cs with the new system. Remove this duplicate definition to avoid redefinition errors.
