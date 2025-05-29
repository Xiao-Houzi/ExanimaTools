using ExanimaTools.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExanimaTools.ViewModels;

public partial class CompanyMemberViewModel : ObservableObject
{
    public CompanyMember Model { get; }

    public CompanyMemberViewModel()
    {
        Model = new CompanyMember();
    }
    public CompanyMemberViewModel(CompanyMember model)
    {
        Model = model;
    }
    public CompanyMember ToModel()
    {
        return Model;
    }
    public string Name
    {
        get => Model.Name;
        set => Model.Name = value;
    }
    public Role Role
    {
        get => Model.Role;
        set => Model.Role = value;
    }
    public Rank Rank
    {
        get => Model.Rank;
        set => Model.Rank = value;
    }
    public Sex Sex
    {
        get => Model.Sex;
        set => Model.Sex = value;
    }
    public MemberType Type
    {
        get => Model.Type;
        set => Model.Type = value;
    }
}
