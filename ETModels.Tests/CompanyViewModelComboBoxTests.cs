using System;
using ExanimaTools.Models;
using ExanimaTools.ViewModels;
using ExanimaTools.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ETModels.Tests;

[TestClass]
public class CompanyViewModelComboBoxTests
{
    private static CompanyViewModel CreateTestCompanyViewModel()
    {
        // Use DummyLogger to avoid file conflicts during testing
        var logger = new DummyLogger();
        var companyMemberRepo = new CompanyMemberRepository("Data Source=:memory:");
        var arsenalRepo = new ArsenalRepository("Data Source=:memory:");
        var equipmentRepo = new EquipmentRepository("Data Source=:memory:", logger);
        var arsenalManagerViewModel = new ArsenalManagerViewModel(equipmentRepo, arsenalRepo, logger);
        return new CompanyViewModel(companyMemberRepo, arsenalManagerViewModel, logger);
    }

    private class DummyLogger : ILoggingService
    {
        public void Log(string message) { }
        public void LogOperation(string operation, string? details = null) { }
        public void LogError(string message, System.Exception? ex = null) { }
        public void LogInformation(string message) { }
    }

    [TestMethod]
    public void AddRecruitDialog_DefaultRoleAndRank_AppearInDialogRolesAndRanks()
    {
        var vm = CreateTestCompanyViewModel();
        vm.OpenAddRecruitDialog();
        // The default role and rank should be the first in the list and match the NewCompanyMember
        Assert.AreEqual(vm.AddDialogRoles.First(), vm.NewCompanyMember.Role, "Default Role should match first AddDialogRoles");
        Assert.AreEqual(vm.AddDialogRanks.First(), vm.NewCompanyMember.Rank, "Default Rank should match first AddDialogRanks");
        // The roles and ranks lists should not be empty
        Assert.IsTrue(vm.AddDialogRoles.Count > 0, "AddDialogRoles should not be empty");
        Assert.IsTrue(vm.AddDialogRanks.Count > 0, "AddDialogRanks should not be empty");
    }

    [TestMethod]
    public void AddHirelingDialog_DefaultRoleAndRank_AppearInDialogRolesAndRanks()
    {
        var vm = CreateTestCompanyViewModel();
        vm.OpenAddHirelingDialog();
        Assert.IsTrue(vm.AddDialogRoles.Contains(vm.NewCompanyMember.Role), "Default Role should be in AddDialogRoles");
        Assert.AreEqual(vm.AddDialogRanks.First(), vm.NewCompanyMember.Rank, "Default Rank should match first AddDialogRanks");
    }

    [TestMethod]
    public void AddManagerDialog_DefaultRoleAndRank_AppearInDialogRolesAndRanks()
    {
        var vm = CreateTestCompanyViewModel();
        vm.OpenAddManagerDialog();
        Assert.AreEqual(Role.Manager, vm.NewCompanyMember.Role, "Manager dialog should set Role.Manager");
        Assert.AreEqual(Rank.Inept, vm.NewCompanyMember.Rank, "Manager dialog should set Rank.Inept");
    }

    [TestMethod]
    public void AddRecruitDialog_RolesAndRanks_AreValidEnumValues()
    {
        var vm = CreateTestCompanyViewModel();
        vm.OpenAddRecruitDialog();
        foreach (var role in vm.AddDialogRoles)
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(Role), role), $"Role {role} should be a valid Role enum value");
        }
        foreach (var rank in vm.AddDialogRanks)
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(Rank), rank), $"Rank {rank} should be a valid Rank enum value");
        }
    }
}
