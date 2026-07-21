using FluentValidation;
using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class CreateCampaignPartyCommandValidator : AbstractValidator<CreateCampaignPartyCommand>
{
    public CreateCampaignPartyCommandValidator()
    {
        RuleFor( command => command.Name )
            .NotEmpty()
            .MaximumLength( CampaignParty.NameMaxLength );
    }
}
