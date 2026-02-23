using FluentAssertions;
using Syndicate.Domain.Entities;

namespace Syndicate.Tests.Domain.Entities;

public class TransactionTests
{
    [Fact]
    public void CreateGenesis_ShouldSet100Pikas()
    {
        var memberId = Guid.NewGuid();

        var tx = Transaction.CreateGenesis(memberId);

        tx.MemberId.Should().Be(memberId);
        tx.Amount.Should().Be(100m);
        tx.Type.Should().Be(TransactionType.Genesis);
        tx.Description.Should().Contain("Initial balance");
    }
}
