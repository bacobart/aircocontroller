using System;
using System.Collections.Generic;
using System.Text;

namespace AircoController.Model.Request
{
    public class LoginRequest
    {
        public string Language { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
    }
}
