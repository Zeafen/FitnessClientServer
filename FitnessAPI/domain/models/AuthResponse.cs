using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }

        public AuthResponse(string token, string role)
        {
            Token = token;
            Role = role;
        }
    }
}
