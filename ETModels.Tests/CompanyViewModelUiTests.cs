using System.Linq;
using ExanimaTools.Models;
using ExanimaTools.ViewModels;
using ExanimaTools.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ETModels.Tests;

[TestClass]
public class CompanyViewModelUiTests
{
    private class DummyLogger : ILoggingService
    {
        public void Log(string message) { }
        public void LogOperation(string operation, string? details = null) { }
        public void LogError(string message, System.Exception? ex = null) { }
        public void LogInformation(string message) { }
    }

    private static CompanyViewModel CreateTestCompanyViewModel()
    {
        var logger = new DummyLogger();
        var companyMemberRepo = new CompanyMemberRepository("Data Source=:memory:");
        var arsenalRepo = new ArsenalRepository("Data Source=:memory:");
        var equipmentRepo = new EquipmentRepository("Data Source=:memory:", logger);
        var arsenalManagerViewModel = new ArsenalManagerViewModel(equipmentRepo, arsenalRepo, logger);
        return new CompanyViewModel(companyMemberRepo, arsenalManagerViewModel, logger);
    }

    [TestMethod]
    public void AddManagerDialog_SetsDefaultRoleAndRank()
    {
        var vm = CreateTestCompanyViewModel();
        vm.OpenAddManagerDialog();
        Assert.AreEqual(1, vm.AddDialogRoles.Count);
        Assert.AreEqual(Role.Manager, vm.NewCompanyMember.Role);
        Assert.AreEqual(1, vm.AddDialogRanks.Count);
        Assert.AreEqual(Rank.Inept, vm.NewCompanyMember.Rank);
    }

    [TestMethod]
    public void AddRecruitDialog_SetsDefaultRoleAndRank()
    {
        var vm = CreateTestCompanyViewModel();
        vm.OpenAddRecruitDialog();
        Assert.AreEqual(1, vm.AddDialogRoles.Count);
        Assert.AreEqual(Role.Fighter, vm.NewCompanyMember.Role);
        Assert.AreEqual(vm.AddDialogRanks.First(), vm.NewCompanyMember.Rank);
    }

    [TestMethod]
    public void AddHirelingDialog_SetsDefaultRoleAndRank()
    {
        var vm = CreateTestCompanyViewModel();
        vm.OpenAddHirelingDialog();
        CollectionAssert.Contains(vm.AddDialogRoles, vm.NewCompanyMember.Role);
        Assert.AreEqual(vm.AddDialogRanks.First(), vm.NewCompanyMember.Rank);
    }
}
