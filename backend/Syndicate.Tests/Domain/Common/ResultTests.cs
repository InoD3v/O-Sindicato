using FluentAssertions;
using Syndicate.Domain.Common;

namespace Syndicate.Tests.Domain.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldReturnSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldReturnFailedResult()
    {
        var result = Result.Failure("Something went wrong");

        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Something went wrong");
    }

    [Fact]
    public void GenericSuccess_ShouldReturnValueAndSuccess()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void GenericFailure_ShouldReturnErrorAndNoValue()
    {
        var result = Result.Failure<int>("Error");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Error");
        result.Value.Should().Be(default(int));
    }

    [Fact]
    public void Success_WithError_ShouldThrow()
    {
        var act = () => new TestableResult(true, "error");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_WithoutError_ShouldThrow()
    {
        var act = () => new TestableResult(false, null);

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Concrete subclass to test the protected constructor.
    /// </summary>
    private class TestableResult : Result
    {
        public TestableResult(bool isSuccess, string? error) : base(isSuccess, error) { }
    }
}
