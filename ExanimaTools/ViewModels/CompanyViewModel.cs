// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ExanimaTools.Models;
using ExanimaTools.Persistence;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace ExanimaTools.ViewModels;

public class CompanyViewModel : INotifyPropertyChanged
{
    private readonly CompanyMemberRepository _companyMemberRepository;
    private readonly ILoggingService _logger;
    private readonly ArsenalManagerViewModel _arsenalManagerViewModel;

    public CompanyViewModel(
        CompanyMemberRepository companyMemberRepository,
        ArsenalManagerViewModel arsenalManagerViewModel,
        ILoggingService logger)
    {
        _companyMemberRepository = companyMemberRepository ?? throw new ArgumentNullException(nameof(companyMemberRepository));
        _arsenalManagerViewModel = arsenalManagerViewModel ?? throw new ArgumentNullException(nameof(arsenalManagerViewModel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        companyMembers = new ObservableCollection<CompanyMember>();
        NewCompanyMember = new CompanyMemberViewModel();
        OpenAddManagerDialogCommand = new SimpleCommand(OpenAddManagerDialog);
        OpenAddRecruitDialogCommand = new SimpleCommand(OpenAddRecruitDialog);
        OpenAddHirelingDialogCommand = new SimpleCommand(OpenAddHirelingDialog);
        CloseAddDialogCommand = new SimpleCommand(CloseAddDialog);
        AddCompanyMemberCommand = new SimpleCommand(AddCompanyMember);
        
        _logger.LogOperation("CompanyViewModel", "Created via DI");
        _ = InitializeAndLoadAsync();
    }

    public ArsenalManagerViewModel ArsenalManagerViewModel => _arsenalManagerViewModel;

    // ...existing code...

    private async Task InitializeAndLoadAsync()
    {
        await _companyMemberRepository.InitializeSchemaAsync();
        await LoadCompanyMembersAsync();
    }

    private ObservableCollection<CompanyMember> companyMembers = new();
    public ObservableCollection<CompanyMember> CompanyMembers
    {
        get => companyMembers;
        set { if (companyMembers != value) { companyMembers = value; OnPropertyChanged(nameof(CompanyMembers)); } }
    }

    private CompanyMemberViewModel? selectedCompanyMemberViewModel;
    public CompanyMemberViewModel? SelectedCompanyMemberViewModel
    {
        get => selectedCompanyMemberViewModel;
        set
        {
            if (selectedCompanyMemberViewModel != value)
            {
                selectedCompanyMemberViewModel = value;
                OnPropertyChanged(nameof(SelectedCompanyMemberViewModel));
                selectedCompanyMemberViewModel?.UpdateFromModel();
            }
        }
    }

    private CompanyMember? selectedCompanyMember;
    public CompanyMember? SelectedCompanyMember
    {
        get => selectedCompanyMember;
        set
        {
            if (selectedCompanyMember != value)
            {
                selectedCompanyMember = value;
                _logger?.LogOperation("CompanyViewModel", $"SelectedCompanyMember changed: {(selectedCompanyMember != null ? selectedCompanyMember.Name : "null")}");
                OnPropertyChanged(nameof(SelectedCompanyMember));
                if (selectedCompanyMember != null)
                {
                    SelectedCompanyMemberViewModel = new CompanyMemberViewModel(selectedCompanyMember);
                    _logger?.LogOperation("CompanyViewModel", $"SelectedCompanyMemberViewModel set: {selectedCompanyMember.Name}");
                    _arsenalManagerViewModel.SelectedMemberId = selectedCompanyMember.Id;
                }
                else
                {
                    SelectedCompanyMemberViewModel = null;
                    _logger?.LogOperation("CompanyViewModel", "SelectedCompanyMemberViewModel set: null");
                    _arsenalManagerViewModel.SelectedMemberId = null;
                }
            }
        }
    }

    private string? searchText;
    public string? SearchText
    {
        get => searchText;
        set { if (searchText != value) { searchText = value; OnPropertyChanged(nameof(SearchText)); } }
    }

    private string? statusMessage;
    public string? StatusMessage
    {
        get => statusMessage;
        set { if (statusMessage != value) { statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); } }
    }

    private CompanyMemberViewModel newCompanyMember = new();
    public CompanyMemberViewModel NewCompanyMember
    {
        get => newCompanyMember;
        set { if (newCompanyMember != value) { newCompanyMember = value; OnPropertyChanged(nameof(NewCompanyMember)); } }
    }

    private bool isAddDialogOpen;
    public bool IsAddDialogOpen
    {
        get => isAddDialogOpen;
        set { if (isAddDialogOpen != value) { isAddDialogOpen = value; OnPropertyChanged(nameof(IsAddDialogOpen)); } }
    }

    private string? errorMessage;
    public string? ErrorMessage
    {
        get => errorMessage;
        set { if (errorMessage != value) { errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }
    }

    private string addDialogTitle = "Add New Member";
    public string AddDialogTitle { get => addDialogTitle; set { addDialogTitle = value; OnPropertyChanged(nameof(AddDialogTitle)); } }
    private readonly ObservableCollection<Role> addDialogRoles = new();
    public ObservableCollection<Role> AddDialogRoles => addDialogRoles;
    private readonly ObservableCollection<Rank> addDialogRanks = new();
    public ObservableCollection<Rank> AddDialogRanks => addDialogRanks;
    private bool isRoleSelectable = true;
    public bool IsRoleSelectable { get => isRoleSelectable; set { isRoleSelectable = value; OnPropertyChanged(nameof(IsRoleSelectable)); } }
    private bool isRankSelectable = true;
    public bool IsRankSelectable { get => isRankSelectable; set { isRankSelectable = value; OnPropertyChanged(nameof(IsRankSelectable)); } }

    public ICommand OpenAddManagerDialogCommand { get; }
    public ICommand OpenAddRecruitDialogCommand { get; }
    public ICommand OpenAddHirelingDialogCommand { get; }
    public ICommand CloseAddDialogCommand { get; }
    public ICommand AddCompanyMemberCommand { get; }

    public bool CanAddManager => !CompanyMembers.Any(m => m.Role == Role.Manager);
    public bool CanAddNonManager => CompanyMembers.Any(m => m.Role == Role.Manager);

    private void OpenAddMemberDialog(List<Role> roles, List<Rank> ranks, MemberType type, string title, bool isRoleSelectable, bool isRankSelectable)
    {
        AddDialogRoles.Clear();
        foreach (var r in roles) AddDialogRoles.Add(r);
        AddDialogRanks.Clear();
        foreach (var r in ranks) AddDialogRanks.Add(r);
        _logger?.LogOperation("OpenAddMemberDialog", $"Set AddDialogRoles: {string.Join(", ", AddDialogRoles)}");
        _logger?.LogOperation("OpenAddMemberDialog", $"Set AddDialogRanks: {string.Join(", ", AddDialogRanks)}");
        NewCompanyMember = new CompanyMemberViewModel();
        if (AddDialogRoles.Count > 0)
            NewCompanyMember.Role = AddDialogRoles[0];
        if (AddDialogRanks.Count > 0)
            NewCompanyMember.Rank = AddDialogRanks[0];
        NewCompanyMember.Type = type;
        AddDialogTitle = title;
        IsRoleSelectable = isRoleSelectable;
        IsRankSelectable = isRankSelectable;
        _logger?.LogOperation("OpenAddMemberDialog", $"Roles: {string.Join(", ", AddDialogRoles.Select(r => r.ToString()))}");
        _logger?.LogOperation("OpenAddMemberDialog", $"Ranks: {string.Join(", ", AddDialogRanks.Select(r => r.ToString()))}");
        _logger?.LogOperation("OpenAddMemberDialog", $"NewCompanyMember.Role: {NewCompanyMember.Role} (Type: {NewCompanyMember.Role.GetType()})");
        _logger?.LogOperation("OpenAddMemberDialog", $"NewCompanyMember.Rank: {NewCompanyMember.Rank} (Type: {NewCompanyMember.Rank.GetType()})");
        OnPropertyChanged(nameof(AddDialogRoles));
        OnPropertyChanged(nameof(AddDialogRanks));
        OnPropertyChanged(nameof(NewCompanyMember));
        IsAddDialogOpen = true;
        ErrorMessage = null;
    }

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
    }    private void NewCompanyMember_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // No longer needed
    }

    private async Task LoadCompanyMembersAsync()
    {
        var members = await _companyMemberRepository.GetAllAsync();
        foreach (var member in members)
            CompanyMembers.Add(member);
        // Select the manager by default if present, else null
        var manager = CompanyMembers.FirstOrDefault(m => m.Role == Role.Manager);
        if (manager != null)
        {
            _logger?.LogOperation("CompanyViewModel", $"Defaulting selection to manager: {manager.Name} (Id={manager.Id})");
            SelectedCompanyMember = manager;
        }
        else
        {
            _logger?.LogOperation("CompanyViewModel", "No manager found, setting SelectedCompanyMember to null");
            SelectedCompanyMember = null;
        }
    }

    private void CloseAddDialog()
    {
        IsAddDialogOpen = false;
    }

    private void AddCompanyMember()
    {
        _logger?.LogOperation("AddCompanyMember", $"Name: {NewCompanyMember.Name}, Role: {NewCompanyMember.Role}, Rank: {NewCompanyMember.Rank}, Type: {NewCompanyMember.Type}");
        if (string.IsNullOrWhiteSpace(NewCompanyMember.Name))
        {
            ErrorMessage = "Name is required.";
            _logger?.LogError("[AddCompanyMember] Error: Name is required.");
            return;
        }
        if (NewCompanyMember.Role == Role.Manager && CompanyMembers.Any(m => m.Role == Role.Manager))
        {
            ErrorMessage = "A manager already exists.";
            _logger?.LogError("[AddCompanyMember] Error: A manager already exists.");
            return;
        }
        var member = NewCompanyMember.ToModel();
        _logger?.LogOperation("AddCompanyMember", $"Saving member: {member.Name}, Role: {member.Role}, Rank: {member.Rank}, Type: {member.Type}");
        _companyMemberRepository.AddAsync(member).GetAwaiter().GetResult();
        CompanyMembers.Add(member);
        IsAddDialogOpen = false;
        ErrorMessage = null;
        OnPropertyChanged(nameof(CanAddManager));
        OnPropertyChanged(nameof(CanAddNonManager));
    }

    public string DebugArsenalManagerVM => ArsenalManagerViewModel == null ? "null" : ArsenalManagerViewModel.GetType().Name;

    public static Role[] AllRoles { get; } = (Role[])System.Enum.GetValues(typeof(Role));
    public static Rank[] AllRanks { get; } = (Rank[])System.Enum.GetValues(typeof(Rank));

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

// Simple ICommand implementation for sync commands
public class SimpleCommand : ICommand
{
    private readonly Action _execute;
    public SimpleCommand(Action execute) => _execute = execute;
    public event EventHandler? CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _execute();
}

