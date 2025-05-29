public void OpenAddManagerDialog()
{
    OpenAddMemberDialog(
        new List<Role> { Role.Manager },
        new List<Rank> { Rank.Inept },
        MemberType.Recruit,
        "Add Manager",
        false,
        false
    );
}
public void OpenAddRecruitDialog()
{
    OpenAddMemberDialog(
        new List<Role> { Role.Fighter },
        System.Enum.GetValues(typeof(Rank)).Cast<Rank>().ToList(),
        MemberType.Recruit,
        "Add Recruit",
        false,
        true
    );
}
public void OpenAddHirelingDialog()
{
    OpenAddMemberDialog(
        System.Enum.GetValues(typeof(Role)).Cast<Role>().Where(r => r != Role.Manager).ToList(),
        System.Enum.GetValues(typeof(Rank)).Cast<Rank>().ToList(),
        MemberType.Hireling,
        "Add Hireling",
        true,
        true
    );
}