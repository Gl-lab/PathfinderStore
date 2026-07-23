using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Money;

namespace Pathfinder.Commerce.Domain.Tests.Money;

public sealed class WalletTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 23, 10, 0, 0, TimeSpan.Zero );

    [Fact]
    public void AppliesCreditAndDebitWithImmutableLedger()
    {
        Wallet wallet = Wallet.Create( 7, 19, _now );

        wallet.ApplyAdjustment( Guid.NewGuid(), 1000, "Starting funds", 11, _now );
        wallet.ApplyAdjustment( Guid.NewGuid(), -250, "Correction", 11, _now );

        Assert.Equal( 750, wallet.BalanceCopper );
        Assert.Equal( 2, wallet.Version );
        Assert.Collection(
            wallet.Entries,
            entry => Assert.Equal( WalletTransactionKind.AdjustmentCredit, entry.Kind ),
            entry => Assert.Equal( WalletTransactionKind.AdjustmentDebit, entry.Kind ) );
    }

    [Fact]
    public void RepeatingOperationReturnsExistingEntryWithoutChangingBalance()
    {
        Wallet wallet = Wallet.Create( 7, 19, _now );
        Guid operationId = Guid.NewGuid();

        WalletLedgerEntry first = wallet.ApplyAdjustment(
            operationId,
            1000,
            "Starting funds",
            11,
            _now );
        WalletLedgerEntry repeated = wallet.ApplyAdjustment(
            operationId,
            1000,
            "Starting funds",
            11,
            _now );

        Assert.Same( first, repeated );
        Assert.Equal( 1000, wallet.BalanceCopper );
        Assert.Single( wallet.Entries );
    }

    [Fact]
    public void RejectsDebitBeyondBalance()
    {
        Wallet wallet = Wallet.Create( 7, 19, _now );

        CommerceException exception = Assert.Throws<CommerceException>(
            () => wallet.ApplyAdjustment(
                Guid.NewGuid(),
                -1,
                "Invalid debit",
                11,
                _now ) );

        Assert.Contains( "insufficient", exception.Message );
    }
}
