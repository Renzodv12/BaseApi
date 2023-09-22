using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Models
{
	public class LoginRequestModel
	{
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginResponseModel
    {
        public string Token { get; set; }
        public bool Login { get; set; }
        public List<string> Errors { get; set; }

    }
}

