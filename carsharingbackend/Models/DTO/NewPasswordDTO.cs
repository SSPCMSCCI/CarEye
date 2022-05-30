using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carsharingbackend.Models.DTO
{
    public class NewPasswordDTO
    {
        public int userId { get; set; }
        public string newPassword { get; set; }
    }
}