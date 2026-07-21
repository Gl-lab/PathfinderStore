using FluentValidation;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class InviteCampaignMemberCommandValidator : AbstractValidator<InviteCampaignMemberCommand>
{
    public InviteCampaignMemberCommandValidator()
    {
        RuleFor( command => command.ActingUserId )
            .GreaterThan( 0 );
        RuleFor( command => command.CampaignId )
            .GreaterThan( 0 );
        RuleFor( command => command.UserName )
            .NotEmpty()
            .MaximumLength( 256 );
    }
}
