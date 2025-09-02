using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace ProductCatalog.Api.Authorization;

public record MinimumAge(int Age) : IAuthorizationRequirement; // Mark interface


public class MinimumAgeHandler : AuthorizationHandler<MinimumAge>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAge requirement)
    {

        if (context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
        {

            var birthdate = DateTime.Parse(context.User.FindFirst(ClaimTypes.DateOfBirth).Value);

            var age = DateTime.Today.Year - birthdate.Year;

            if (age >= requirement.Age)
            {
                context.Succeed(requirement);
            }

            else
            {
                context.Fail();
            }
        }

        return Task.CompletedTask;

    
    }
}