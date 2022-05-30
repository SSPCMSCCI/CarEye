using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carsharingbackend.Models.DTO
{
    public class LoginDTO
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}