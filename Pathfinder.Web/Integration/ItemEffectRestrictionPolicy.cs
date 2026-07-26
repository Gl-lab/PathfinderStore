using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinder.ItemCatalog.Domain.Configurations;

namespace Pathfinder.Web.Integration;

public sealed class ItemEffectRestrictionPolicy
{
    public const string BindingCurseCode = "curse.binding";

    public bool RequiresTransferRestriction(
        IReadOnlyCollection<PermanentUpgrade> upgrades )
    {
        return upgrades.Any( upgrade =>
            upgrade.Kind == PermanentUpgradeKind.TypedEffect &&
            upgrade.Visibility == PermanentUpgradeVisibility.Hidden &&
            String.Equals(
                upgrade.Code,
                BindingCurseCode,
                StringComparison.Ordinal ) );
    }
}