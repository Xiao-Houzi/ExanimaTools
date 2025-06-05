using ExanimaTools.Models;
using System.ComponentModel;
using System.Windows.Input;

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
    public CompanyMemberViewModel(CompanyMember model, IEnumerable<EquipmentPiece>? arsenal = null)
    {
        Model = model;
        ArsenalEquipment = arsenal ?? new List<EquipmentPiece>();
        AssignEquipmentCommand = new RelayCommand(AssignEquipmentToProfile);
        UnassignEquipmentCommand = new RelayCommand(param => UnassignEquipmentFromProfile(param));
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
    // Arsenal equipment for assignment
    public IEnumerable<EquipmentPiece> ArsenalEquipment { get; set; } = new List<EquipmentPiece>();
    public EquipmentPiece? SelectedArsenalEquipment { get; set; }
    public bool AssignmentErrorVisible { get; set; }
    public ICommand? AssignEquipmentCommand { get; set; }
    public ICommand? UnassignEquipmentCommand { get; set; }
    public static Role[] AllRoles { get; } = (Role[])System.Enum.GetValues(typeof(Role));
    public static Rank[] AllRanks { get; } = (Rank[])System.Enum.GetValues(typeof(Rank));
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void AssignEquipmentToProfile()
    {
        AssignmentErrorVisible = false;
        if (SelectedArsenalEquipment == null) return;
        // Prevent assigning if already assigned to any member (TODO: implement global check in parent VM)
        var currentRank = Rank;
        var slot = SelectedArsenalEquipment.Slot;
        var layer = SelectedArsenalEquipment.Layer;
        if (!Model.AssignEquipmentToProfile(currentRank, slot, SelectedArsenalEquipment, layer))
        {
            AssignmentErrorVisible = true;
            OnPropertyChanged(nameof(AssignmentErrorVisible));
            return;
        }
        OnPropertyChanged(nameof(Model)); // Refresh UI
    }

    private void UnassignEquipmentFromProfile(object? param)
    {
        AssignmentErrorVisible = false;
        if (param is not EquipmentPiece eq) return;
        var currentRank = Rank;
        var slot = eq.Slot;
        var layer = eq.Layer;
        if (!Model.UnassignEquipmentFromProfile(currentRank, slot, eq.Id, layer))
        {
            AssignmentErrorVisible = true;
            OnPropertyChanged(nameof(AssignmentErrorVisible));
            return;
        }
        OnPropertyChanged(nameof(Model));
    }

    public string DebugInfo => $"VM: {Name}, {Role}, {Rank}";

    public void UpdateFromModel()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Role));
        OnPropertyChanged(nameof(Rank));
        OnPropertyChanged(nameof(Sex));
        OnPropertyChanged(nameof(Type));
        OnPropertyChanged(nameof(Model));
        OnPropertyChanged(nameof(DebugInfo));
    }
}
