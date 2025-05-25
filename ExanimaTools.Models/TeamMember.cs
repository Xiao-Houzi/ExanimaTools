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

public class EquipmentProfile
{
    public string Name { get; set; } = string.Empty;
    public List<string> EquipmentPieces { get; set; } = new();
}

public class TeamMember
{
    public string Name { get; set; } = string.Empty;
    public Role Role { get; set; }
    public Rank Rank { get; set; }
    public Sex Sex { get; set; }
    public MemberType Type { get; set; } = MemberType.Recruit;
    public EquipmentProfile EquipmentProfile { get; set; } = new();
}
