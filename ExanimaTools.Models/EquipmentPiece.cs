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

public class EquipmentPiece
{
    public string Name { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public List<EquipmentStat> Stats { get; set; } = new();
    public string Description { get; set; } = string.Empty; // make, quality, condition
}
