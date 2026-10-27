using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.API.Extentions
{
    public static class ControllerExtensions
    {
      
        // JWT Token-დან userId ამოიღებს
        public static int GetUserId(this ControllerBase controller)
        {
            var userIdClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            throw new InvalidOperationException("Invalid or missing user ID in token");
        }
    }
}
