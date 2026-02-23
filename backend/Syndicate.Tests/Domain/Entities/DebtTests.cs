using FluentAssertions;
using Syndicate.Domain.Entities;

namespace Syndicate.Tests.Domain.Entities;

public class DebtTests
{
    private static Debt CreatePendingDebt()
        => Debt.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 50m, "Test debt");

    [Fact]
    public void Create_ShouldReturnDebtWithPendingStatus()
    {
        var debt = CreatePendingDebt();

        debt.Amount.Should().Be(50m);
        debt.Description.Should().Be("Test debt");
        debt.Status.Should().Be(DebtStatus.Pending);
    }

    // ── Accept ──────────────────────────────────────────────

    [Fact]
    public void Accept_WhenPending_ShouldTransitionToActive()
    {
        var debt = CreatePendingDebt();

        var result = debt.Accept();

        result.IsSuccess.Should().BeTrue();
        debt.Status.Should().Be(DebtStatus.Active);
        debt.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Accept_WhenAlreadyActive_ShouldFail()
    {
        var debt = CreatePendingDebt();
        debt.Accept();

        var result = debt.Accept();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("pending");
    }

    [Fact]
    public void Accept_WhenSettled_ShouldFail()
    {
        var debt = CreatePendingDebt();
        debt.Accept();
        debt.Settle();

        var result = debt.Accept();

        result.IsFailure.Should().BeTrue();
    }

    // ── Settle ──────────────────────────────────────────────

    [Fact]
    public void Settle_WhenActive_ShouldTransitionToSettled()
    {
        var debt = CreatePendingDebt();
        debt.Accept();

        var result = debt.Settle();

        result.IsSuccess.Should().BeTrue();
        debt.Status.Should().Be(DebtStatus.Settled);
    }

    [Fact]
    public void Settle_WhenPending_ShouldFail()
    {
        var debt = CreatePendingDebt();

        var result = debt.Settle();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("active");
    }

    // ── Cancel ──────────────────────────────────────────────

    [Fact]
    public void Cancel_WhenPending_ShouldTransitionToCancelled()
    {
        var debt = CreatePendingDebt();

        var result = debt.Cancel();

        result.IsSuccess.Should().BeTrue();
        debt.Status.Should().Be(DebtStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenActive_ShouldFail()
    {
        var debt = CreatePendingDebt();
        debt.Accept();

        var result = debt.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("pending");
    }
}
