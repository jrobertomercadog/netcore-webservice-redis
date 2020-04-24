using System;
namespace PulseResponse
{
    public class Login
    {
        public string Username { get; set; }
        public string HashPassword { get; set; }
        public string HashSalt { get; set; }
        public string LastLoginDate { get; set; }
    }
}
