using carsharingbackend.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carsharingbackend.Models.DTO
{
    public class CarNotificationDTO
    {
        public static CarNotificationDTO from(notification notification)
        {
            CarNotificationDTO carNotificationDTO = new CarNotificationDTO();
            carNotificationDTO.id = notification.id;
            carNotificationDTO.idCar = notification.idCar;
            carNotificationDTO.idDriver = notification.idDriver;
            carNotificationDTO.idComponent = notification.idComponent;
            carNotificationDTO.notificationDate = notification.notificationDate;
            carNotificationDTO.active = notification.active;
            carNotificationDTO.car = notification.car;
            carNotificationDTO.component = notification.component;
            carNotificationDTO.driver = notification.driver;
            return carNotificationDTO;
        }

        public int id { get; set; }
        public int idCar { get; set; }
        public int idDriver { get; set; }
        public int idComponent { get; set; }
        public System.DateTime notificationDate { get; set; }
        public bool active { get; set; }
        public car car { get; set; }
        public component component { get; set; }
        public driver driver { get; set; }
    }
}