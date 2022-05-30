using carsharingbackend.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carsharingbackend.Models.DTO
{
    public class DriverHistoryDTO
    {
        public List<driverXCar> driverHistory { get; set; }
        public driverXCar currentDriver { get; set; }
    }
}