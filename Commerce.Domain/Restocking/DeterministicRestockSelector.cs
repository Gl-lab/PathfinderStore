using System.Buffers.Binary;
using System.Security.Cryptography;
namespace Pathfinder.Commerce.Domain.Restocking;

public sealed class DeterministicRestockSelector
{
    public IReadOnlyCollection<RestockCandidate> Select(
        RestockPolicyRevision revision,
        long seed,
        IReadOnlyCollection<RestockCandidate> candidates )
    {
        ArgumentNullException.ThrowIfNull( revision );
        ArgumentNullException.ThrowIfNull( candidates );
        List<RestockCandidate> remaining = candidates
            .OrderBy( candidate => candidate.ItemConfigurationId )
            .ToList();
        List<RestockCandidate> selected = [];
        long remainingBudgetCopper = revision.BudgetCopper;
        int counter = 0;
        while ( selected.Count < revision.TargetOfferCount )
        {
            List<RestockCandidate> eligible = remaining
                .Where( candidate =>
                    revision.Allows( candidate, remainingBudgetCopper ) &&
                    revision.GetSelectionWeight( candidate ) > 0 )
                .ToList();
            if ( eligible.Count == 0 )
            {
                break;
            }

            long totalWeight = eligible.Sum(
                candidate => ( long )revision.GetSelectionWeight( candidate ) );
            long selectedWeight = NextInt64( seed, counter, totalWeight );
            counter++;
            long cumulativeWeight = 0;
            RestockCandidate selectedCandidate = eligible[ 0 ];
            foreach ( RestockCandidate candidate in eligible )
            {
                cumulativeWeight += revision.GetSelectionWeight( candidate );
                if ( selectedWeight < cumulativeWeight )
                {
                    selectedCandidate = candidate;
                    break;
                }
            }

            selected.Add( selectedCandidate );
            remaining.Remove( selectedCandidate );
            remainingBudgetCopper -= selectedCandidate.UnitPriceCopper;
        }

        return selected;
    }

    private static long NextInt64( long seed, int counter, long exclusiveMaximum )
    {
        Span<byte> input = stackalloc byte[12];
        BinaryPrimitives.WriteInt64LittleEndian( input, seed );
        BinaryPrimitives.WriteInt32LittleEndian( input[ 8.. ], counter );
        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData( input, hash );
        ulong value = BinaryPrimitives.ReadUInt64LittleEndian( hash );
        return ( long )(value % ( ulong )exclusiveMaximum);
    }
}
