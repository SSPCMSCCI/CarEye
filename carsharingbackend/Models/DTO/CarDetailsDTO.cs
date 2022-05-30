using carsharingbackend.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carsharingbackend.Models.DTO
{
    public class CarDetailsDTO
    {
        public car car { get; set; }
        public carPosition carPosition { get; set; }
        public driver currentDriver { get; set; }
        public List<driverXCar> driverHistory { get; set; }
        public List<notification> notifications { get; set; }
        public List<component> components { get; set; }
    }
}