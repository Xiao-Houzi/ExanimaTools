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
    Leader,
    Fighter,
    Support,
    Custom
}

public enum Rank
{
    Novice,
    Veteran,
    Elite,
    Custom
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
