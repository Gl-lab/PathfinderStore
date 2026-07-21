using FluentValidation;
using Pathfinder.CampaignManagement.Domain.Campaigns;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor( command => command.UserId )
            .GreaterThan( 0 );
        RuleFor( command => command.Name )
            .NotEmpty()
            .MaximumLength( Campaign.NameMaxLength );
    }
}
