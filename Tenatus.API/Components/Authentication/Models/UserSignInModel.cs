using System.ComponentModel.DataAnnotations;

namespace Tenatus.API.Components.Authentication.Models
{
    public class UserSignInModel
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Email { get; set; }
    }
}