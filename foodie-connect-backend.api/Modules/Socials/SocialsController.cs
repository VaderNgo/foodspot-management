using System.Security.Claims;
using foodie_connect_backend.Modules.Restaurants;
using foodie_connect_backend.Modules.Socials.Dtos;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.Socials;

[ApiController]
[Route("v1/restaurants/{restaurantId}/socials")]
public class SocialsController(SocialsService socialsService, RestaurantsService restaurantsService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<SocialLinkResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantSocialLinks(Guid restaurantId)
    {
        var result = await socialsService.GetRestaurantSocialLinksAsync(restaurantId);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SocialLinkResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Head")]
    public async Task<IActionResult> AddSocialLink(Guid restaurantId, CreateSocialDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var authorizationResult = await CheckRestaurantAuthorization(restaurantId, userId);
        if (authorizationResult != null)
        {
            return authorizationResult;
        }

        var result = await socialsService.AddSocialLinkAsync(restaurantId, dto);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return CreatedAtAction(
            nameof(GetRestaurantSocialLinks),
            new { restaurantId },
            result.Value
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SocialLinkResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Head")]
    public async Task<IActionResult> UpdateSocialLink(
        Guid restaurantId,
        string id,
        UpdateSocialDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var authorizationResult = await CheckRestaurantAuthorization(restaurantId, userId);
        if (authorizationResult != null)
        {
            return authorizationResult;
        }
        if (id != dto.Id) return BadRequest("ID mismatch");

        var result = await socialsService.UpdateSocialLinkAsync(restaurantId, dto);
        if (!result.IsSuccess) return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Head")]
    public async Task<IActionResult> DeleteSocialLink(Guid restaurantId, string id)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var authorizationResult = await CheckRestaurantAuthorization(restaurantId, userId);
        if (authorizationResult != null)
        {
            return authorizationResult;
        }
        var result = await socialsService.DeleteSocialLinkAsync(restaurantId, id);
        if (!result.IsSuccess) return NotFound(result.Error);

        return NoContent();
    }

    private async Task<IActionResult> CheckRestaurantAuthorization(Guid restaurantId, string userId)
    {
        var restaurant = await restaurantsService.GetRestaurantById(restaurantId);
        if (restaurant.IsFailure)
        {
            return NotFound(SocialError.RestaurantNotFound(restaurantId));
        }
        
        if (restaurant.Value.HeadId != userId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, AuthError.NotAuthorized());
        }
        
        return null;
    }
}
