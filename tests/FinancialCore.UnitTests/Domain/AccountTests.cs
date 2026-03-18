// tests/FinancialCore.UnitTests/Domain/AccountTests.cs
using Domain.Entities;
using FluentAssertions;

namespace FinancialCore.UnitTests.Domain;

public class AccountTests
{
    [Fact]
    public void Constructor_WithValidCurrency_ShouldInitializeWithZeroBalance()
    {
        // Arrange (Preparar)
        var currency = "USD";

        // Act (Actuar)
        var account = new Account(currency);

        // Assert (Afirmar usando FluentAssertions)
        account.Currency.Should().Be("USD");
        account.Balance.Should().Be(0);
        account.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Deposit_WithStrictlyPositiveAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var account = new Account("ARS");
        decimal depositAmount = 1500.50m;

        // Act
        account.Deposit(depositAmount);

        // Assert
        account.Balance.Should().Be(depositAmount);
    }

    [Fact]
    public void Withdraw_ExceedingBalance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var account = new Account("EUR");
        account.Deposit(100); // Balance inicial de 100

        // Act
        // Como esperamos una excepción, encapsulamos la acción en un Action
        Action act = () => account.Withdraw(150);

        // Assert
        // Demostramos el límite de nuestro invariante: no hay saldos negativos
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Insufficient funds.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void Deposit_WithZeroOrNegativeAmount_ShouldThrowArgumentException(decimal invalidAmount)
    {
        // Arrange
        var account = new Account("USD");

        // Act
        Action act = () => account.Deposit(invalidAmount);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("Deposit amount must be strictly positive.");
    }
}