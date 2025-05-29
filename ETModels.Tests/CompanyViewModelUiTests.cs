using System.Linq;
using ExanimaTools.Models;
using ExanimaTools.ViewModels;
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
    }

    [TestMethod]
    public void AddManagerDialog_SetsDefaultRoleAndRank()
    {
        var vm = new CompanyViewModel(new DummyLogger());
        vm.OpenAddManagerDialog();
        Assert.AreEqual(1, vm.AddDialogRoles.Count);
        Assert.AreEqual(Role.Manager, vm.NewCompanyMember.Role);
        Assert.AreEqual(1, vm.AddDialogRanks.Count);
        Assert.AreEqual(Rank.Inept, vm.NewCompanyMember.Rank);
    }

    [TestMethod]
    public void AddRecruitDialog_SetsDefaultRoleAndRank()
    {
        var vm = new CompanyViewModel(new DummyLogger());
        vm.OpenAddRecruitDialog();
        Assert.AreEqual(1, vm.AddDialogRoles.Count);
        Assert.AreEqual(Role.Fighter, vm.NewCompanyMember.Role);
        Assert.AreEqual(vm.AddDialogRanks.First(), vm.NewCompanyMember.Rank);
    }

    [TestMethod]
    public void AddHirelingDialog_SetsDefaultRoleAndRank()
    {
        var vm = new CompanyViewModel(new DummyLogger());
        vm.OpenAddHirelingDialog();
        CollectionAssert.Contains(vm.AddDialogRoles, vm.NewCompanyMember.Role);
        Assert.AreEqual(vm.AddDialogRanks.First(), vm.NewCompanyMember.Rank);
    }
}
