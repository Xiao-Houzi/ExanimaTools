using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExanimaTools.Models;
using ExanimaTools.Persistence;

namespace ExanimaTools.ViewModels;

public partial class CompanyViewModel : ObservableObject
{
    private readonly CompanyMemberRepository _companyMemberRepository;
    private readonly ILoggingService? _logger;

    public CompanyViewModel(ILoggingService? logger = null)
    {
        _logger = logger;
        var dbPath = ExanimaToolsApp.DbManager.GetDbPath();
        _companyMemberRepository = new CompanyMemberRepository($"Data Source={dbPath}");
        companyMembers = new ObservableCollection<CompanyMember>();
        NewCompanyMember = new CompanyMemberViewModel();
        OpenAddManagerDialogCommand = new SimpleCommand(OpenAddManagerDialog);
        OpenAddRecruitDialogCommand = new SimpleCommand(OpenAddRecruitDialog);
        OpenAddHirelingDialogCommand = new SimpleCommand(OpenAddHirelingDialog);
        CloseAddDialogCommand = new SimpleCommand(CloseAddDialog);
        AddCompanyMemberCommand = new SimpleCommand(AddCompanyMember);
        _ = InitializeAndLoadAsync();
        _logger?.LogOperation("CompanyViewModel", "Created");
    }

    public CompanyViewModel() : this(ExanimaTools.App.LoggingServiceInstance) { }

    // Ensure DB schema is initialized before loading team members
    private async Task InitializeAndLoadAsync()
    {
        await _companyMemberRepository.InitializeSchemaAsync();
        await LoadCompanyMembersAsync();
    }

    [ObservableProperty]
    private ObservableCollection<CompanyMember> companyMembers = new();

    [ObservableProperty]
    private CompanyMember? selectedCompanyMember;

    [ObservableProperty]
    private string? searchText;

    [ObservableProperty]
    private string? statusMessage;

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

    // Add dialog state
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
        if (NewCompanyMember != null)
            NewCompanyMember.PropertyChanged -= NewCompanyMember_PropertyChanged;
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
        NewCompanyMember.PropertyChanged += NewCompanyMember_PropertyChanged;
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

    public static Role[] AllRoles { get; } = (Role[])System.Enum.GetValues(typeof(Role));
    public static Rank[] AllRanks { get; } = (Rank[])System.Enum.GetValues(typeof(Rank));
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

// ICommand implementation for async commands
public class AsyncSimpleCommand : ICommand
{
    private readonly Func<Task> _execute;
    public AsyncSimpleCommand(Func<Task> execute) => _execute = execute;
    public event EventHandler? CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public async void Execute(object? parameter) => await _execute();
}
