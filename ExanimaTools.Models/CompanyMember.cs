using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    Impact,
    Slash,
    Crush,
    Pierce,
    Points // Added for equipment points stat
    // Add more as needed
}

public enum EquipmentType
{
    Weapon,
    Armour,
    Shield,
    Accessory
}

public class EquipmentProfile
{
    private readonly ILoggingService? _logger;
    public EquipmentProfile(ILoggingService? logger = null)
    {
        _logger = logger;
        _logger?.LogOperation("Create EquipmentProfile", $"Name={_name}");
    }
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set { _name = value; _logger?.LogOperation("Set Profile Name", value); }
    }
    public Dictionary<EquipmentSlot, List<EquipmentPiece>> EquippedItems { get; set; } = new();
}

public class CompanyMember
{
    private readonly ILoggingService? _logger;
    public CompanyMember(ILoggingService? logger = null)
    {
        _logger = logger;
        _logger?.LogOperation("Create TeamMember", $"Name={_name}");
    }
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set { _name = value; _logger?.LogOperation("Set Name", value); }
    }
    public Role Role { get; set; }
    public Rank Rank { get; set; }
    public Sex Sex { get; set; }
    public MemberType Type { get; set; } = MemberType.Recruit;
    public Dictionary<Rank, EquipmentProfile> EquipmentProfiles { get; set; } = new();
}
