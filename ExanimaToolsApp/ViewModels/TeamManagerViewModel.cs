using System.Collections.ObjectModel;
using ExanimaTools.Models;
using ExanimaTools.Persistence;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;

namespace ExanimaTools.ViewModels
{
    public class TeamManagerViewModel : INotifyPropertyChanged
    {
        private readonly TeamMemberRepository _teamMemberRepository;
        private const string DefaultDbFile = "exanima_tools.db";

        public TeamManagerViewModel()
        {
            var dbPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, DefaultDbFile);
            _teamMemberRepository = new TeamMemberRepository($"Data Source={dbPath}");
            TeamMembers = new ObservableCollection<TeamMember>();
            NewTeamMember = new TeamMemberViewModel();
            OpenAddDialogCommand = new SimpleCommand(OpenAddDialog);
            AddTeamMemberAsyncCommand = new AsyncSimpleCommand(AddTeamMemberAsync);
            CloseAddDialogCommand = new SimpleCommand(CloseAddDialog);
            _ = InitializeAndLoadAsync();
        }

        // Ensure DB schema is initialized before loading team members
        private async Task InitializeAndLoadAsync()
        {
            await _teamMemberRepository.InitializeSchemaAsync();
            await LoadTeamMembersAsync();
        }

        private ObservableCollection<TeamMember> teamMembers = new();
        public ObservableCollection<TeamMember> TeamMembers
        {
            get => teamMembers;
            set { if (teamMembers != value) { teamMembers = value; OnPropertyChanged(nameof(TeamMembers)); } }
        }

        private TeamMember? selectedTeamMember;
        public TeamMember? SelectedTeamMember
        {
            get => selectedTeamMember;
            set { if (selectedTeamMember != value) { selectedTeamMember = value; OnPropertyChanged(nameof(SelectedTeamMember)); } }
        }

        private TeamMemberViewModel newTeamMember = new();
        public TeamMemberViewModel NewTeamMember
        {
            get => newTeamMember;
            set { if (newTeamMember != value) { newTeamMember = value; OnPropertyChanged(nameof(NewTeamMember)); } }
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

        public ICommand OpenAddDialogCommand { get; }
        public ICommand AddTeamMemberAsyncCommand { get; }
        public ICommand CloseAddDialogCommand { get; }

        private void OpenAddDialog()
        {
            NewTeamMember = new TeamMemberViewModel();
            IsAddDialogOpen = true;
        }

        private async Task AddTeamMemberAsync()
        {
            if (string.IsNullOrWhiteSpace(NewTeamMember.Name))
            {
                ErrorMessage = "Name is required.";
                return;
            }
            var member = NewTeamMember.ToModel();
            await _teamMemberRepository.AddAsync(member);
            TeamMembers.Add(member);
            IsAddDialogOpen = false;
            ErrorMessage = null;
        }

        private async Task LoadTeamMembersAsync()
        {
            var members = await _teamMemberRepository.GetAllAsync();
            foreach (var member in members)
                TeamMembers.Add(member);
        }

        private void CloseAddDialog()
        {
            IsAddDialogOpen = false;
        }

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

    // ICommand implementation for async commands
    public class AsyncSimpleCommand : ICommand
    {
        private readonly Func<Task> _execute;
        public AsyncSimpleCommand(Func<Task> execute) => _execute = execute;
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public async void Execute(object? parameter) => await _execute();
    }
}
