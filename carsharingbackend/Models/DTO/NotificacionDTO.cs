using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carsharingbackend.Models.DTO
{
    public class NotificacionDTO
    {
        public int idComponent { get; set; } //recibe el rfidcode
        public bool isThere { get; set; }
    }
}