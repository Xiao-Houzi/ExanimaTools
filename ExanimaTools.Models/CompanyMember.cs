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
        _logger?.LogOperation("Create CompanyMember", $"Name={_name}");
        Id = 0;
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
    public int Id { get; set; }

    public bool AssignEquipmentToProfile(Rank rank, EquipmentSlot slot, EquipmentPiece equipment, ArmourLayer? layer = null)
    {
        if (!EquipmentProfiles.ContainsKey(rank))
            EquipmentProfiles[rank] = new EquipmentProfile(_logger) { Name = $"{Name} {rank} Loadout" };
        var profile = EquipmentProfiles[rank];
        if (!profile.EquippedItems.ContainsKey(slot))
            profile.EquippedItems[slot] = new List<EquipmentPiece>();
        // Prevent duplicate assignment (by Id, slot, and layer)
        if (profile.EquippedItems[slot].Any(e => e.Id == equipment.Id && e.Layer == layer))
        {
            _logger?.LogOperation("AssignEquipmentToProfile", $"Duplicate prevented: {equipment.Name} (Id={equipment.Id}) already assigned to {slot} ({layer}) for {Name} [{rank}]");
            return false;
        }
        var eqCopy = new EquipmentPiece(_logger)
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Type = equipment.Type,
            Slot = slot,
            Layer = layer,
            Stats = new Dictionary<StatType, float>(equipment.Stats),
            Description = equipment.Description,
            Quality = equipment.Quality,
            Condition = equipment.Condition,
            Category = equipment.Category,
            Subcategory = equipment.Subcategory,
            Rank = equipment.Rank,
            Points = equipment.Points,
            ImagePath = equipment.ImagePath
        };
        profile.EquippedItems[slot].Add(eqCopy);
        _logger?.LogOperation("AssignEquipmentToProfile", $"Assigned {equipment.Name} (Id={equipment.Id}) to {Name} [{rank}] Slot={slot} Layer={layer}");
        return true;
    }
    public bool UnassignEquipmentFromProfile(Rank rank, EquipmentSlot slot, int equipmentId, ArmourLayer? layer = null)
    {
        if (!EquipmentProfiles.ContainsKey(rank))
            return false;
        var profile = EquipmentProfiles[rank];
        if (!profile.EquippedItems.ContainsKey(slot))
            return false;
        var removed = profile.EquippedItems[slot].RemoveAll(e => e.Id == equipmentId && (layer == null || e.Layer == layer));
        if (removed > 0)
        {
            _logger?.LogOperation("UnassignEquipmentFromProfile", $"Unassigned equipmentId={equipmentId} from {Name} [{rank}] Slot={slot} Layer={layer}");
            return true;
        }
        return false;
    }
}
