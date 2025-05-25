using System.Collections.Generic;

namespace ExanimaTools.Models;

public enum Sex
{
    Male,
    Female,
    Other
}

public enum Role
{
    Manager = 0,
    Fighter = 1,
    Physician = 2,
    Merchant = 3,
    Trainer = 4
}

public enum Rank
{
    Inept,
    Aspirant,
    Novice,
    Adept,
    Expert,
    Master
}

public enum MemberType
{
    Recruit,
    Hireling,
    Custom
}

public enum EquipmentSlot
{
    Head,
    Body,
    Shoulders,
    Elbows,
    Wrists,
    Hands,
    Legs,
    Feet
}

public enum ArmourLayer
{
    Clothing,
    Padding,
    Chainmail,
    Armour // Non-full and full armour
}

public enum StatType
{
    Coverage,
    ImpactResistance,
    SlashProtection,
    CrushProtection,
    PierceProtection,
    Encumbrance,
    Weight,
    Balance,
    Thrust,
    // Add more as needed
}

public class EquipmentPiece
{
    public string Name { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public EquipmentSlot Slot { get; set; }
    public ArmourLayer? Layer { get; set; } // null for weapons
    public Dictionary<StatType, float> Stats { get; set; } = new();
    public string Description { get; set; } = string.Empty; // make, quality, condition
    public string? Quality { get; set; }
    public string? Condition { get; set; }
}

public class EquipmentProfile
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<EquipmentSlot, List<EquipmentPiece>> EquippedItems { get; set; } = new(); // Layered per slot
}

public class TeamMember
{
    public string Name { get; set; } = string.Empty;
    public Role Role { get; set; }
    public Rank Rank { get; set; }
    public Sex Sex { get; set; }
    public MemberType Type { get; set; } = MemberType.Recruit;
    public Dictionary<Rank, EquipmentProfile> EquipmentProfiles { get; set; } = new(); // Per-rank loadouts
}
