using System.ComponentModel;
using ExanimaTools.Models;

namespace ExanimaTools.ViewModels;

public class TeamMemberViewModel : INotifyPropertyChanged
{
    public TeamMember Model { get; }

    public TeamMemberViewModel()
    {
        Model = new TeamMember();
    }
    public TeamMemberViewModel(TeamMember model)
    {
        Model = model;
        name = model.Name;
        role = model.Role;
        rank = model.Rank;
        sex = model.Sex;
        type = model.Type;
    }

    private string name = string.Empty;
    public string Name
    {
        get => name;
        set { if (name != value) { name = value; OnPropertyChanged(nameof(Name)); } }
    }
    private Role role;
    public Role Role
    {
        get => role;
        set { if (!Equals(role, value)) { role = value; OnPropertyChanged(nameof(Role)); } }
    }
    private Rank rank;
    public Rank Rank
    {
        get => rank;
        set { if (!Equals(rank, value)) { rank = value; OnPropertyChanged(nameof(Rank)); } }
    }
    private Sex sex;
    public Sex Sex
    {
        get => sex;
        set { if (!Equals(sex, value)) { sex = value; OnPropertyChanged(nameof(Sex)); } }
    }
    private MemberType type = MemberType.Recruit;
    public MemberType Type
    {
        get => type;
        set { if (!Equals(type, value)) { type = value; OnPropertyChanged(nameof(Type)); } }
    }

    public TeamMember ToModel()
    {
        Model.Name = Name;
        Model.Role = Role;
        Model.Rank = Rank;
        Model.Sex = Sex;
        Model.Type = Type;
        return Model;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
