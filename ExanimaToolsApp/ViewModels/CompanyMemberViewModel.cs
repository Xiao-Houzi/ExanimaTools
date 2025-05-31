using ExanimaTools.Models;
using System.ComponentModel;

namespace ExanimaTools.ViewModels;

public class CompanyMemberViewModel : INotifyPropertyChanged
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
        set { if (Model.Name != value) { Model.Name = value; OnPropertyChanged(nameof(Name)); } }
    }
    public Role Role
    {
        get => Model.Role;
        set { if (Model.Role != value) { Model.Role = value; OnPropertyChanged(nameof(Role)); } }
    }
    public Rank Rank
    {
        get => Model.Rank;
        set { if (Model.Rank != value) { Model.Rank = value; OnPropertyChanged(nameof(Rank)); } }
    }
    public Sex Sex
    {
        get => Model.Sex;
        set { if (Model.Sex != value) { Model.Sex = value; OnPropertyChanged(nameof(Sex)); } }
    }
    public MemberType Type
    {
        get => Model.Type;
        set { if (Model.Type != value) { Model.Type = value; OnPropertyChanged(nameof(Type)); } }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
