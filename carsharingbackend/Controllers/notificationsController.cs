using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using carsharingbackend.Models.DTO;
using carsharingbackend.Models.EF;

namespace carsharingbackend.Controllers
{
    [RoutePrefix("api/notifications")]
    public class notificationsController : ApiController
    {
        private carsharingdbEntities db = new carsharingdbEntities();

        // GET: api/notifications
        public IQueryable<notification> Getnotifications()
        {
            return db.notifications.Where(c => c.active);
        }

        // GET: api/notifications/5
        [ResponseType(typeof(notification))]
        public IHttpActionResult Getnotification(int id)
        {
            notification notification = db.notifications.FirstOrDefault(c => c.active);
            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        // PUT: api/notifications/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putnotification(int id, notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != notification.id)
            {
                return BadRequest();
            }

            db.Entry(notification).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!notificationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/notifications/send
        [HttpPost]
        [Route("send")]
        public IHttpActionResult Postnotification(NotificacionDTO notificationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            component componenteAdjunto = db.components.FirstOrDefault(co => co.rfidTicketCode == notificationDTO.idComponent.ToString() && co.active);
            if (componenteAdjunto == null)
            {
                return Conflict();
            }
            componenteAdjunto.isThere = notificationDTO.isThere;

            car carroAdjunto = db.cars.Include("carOwner").FirstOrDefault(c => c.id == componenteAdjunto.idCar);
            if (carroAdjunto == null)
            {
                return Conflict();
            }

            carPosition carroPositionAdjunto = db.carPositions.FirstOrDefault(c => c.idCar == carroAdjunto.id);
            if (carroPositionAdjunto == null)
            {
                carroPositionAdjunto = new carPosition();
                carroPositionAdjunto.idCar = carroAdjunto.id;
                carroPositionAdjunto.lat = "-12.146236";
                carroPositionAdjunto.lng = "-77.009470";
            }

            driver conductorAdjunto = null;
            driverXCar conductoCarro = db.driverXCars.FirstOrDefault(
                dc =>
                DateTime.Today >= dc.lendStartDate
                && dc.idCar == carroAdjunto.id
                && DateTime.Today <= dc.lendEndDate
                && dc.active
            );

            if (conductoCarro == null)
            {
                conductorAdjunto = db.drivers.FirstOrDefault(d => d.dni.Trim() == "NO_ASIGANDO");
            }
            else
            {
                conductorAdjunto = db.drivers.FirstOrDefault(d => d.id == conductoCarro.idDriver);
                if (conductorAdjunto == null)
                {
                    return Conflict();
                }
            }

            notification nuevaNoti = new notification();
            nuevaNoti.idComponent = componenteAdjunto.id;
            nuevaNoti.idCar = carroAdjunto.id;
            nuevaNoti.idDriver = conductorAdjunto.id;
            nuevaNoti.notificationDate = DateTime.Now.AddHours(-5);
            nuevaNoti.lat = carroPositionAdjunto.lat;
            nuevaNoti.lng = carroPositionAdjunto.lng;
            nuevaNoti.isThere = notificationDTO.isThere;
            nuevaNoti.active = true;

            db.notifications.Add(nuevaNoti);
            db.SaveChanges();

            string cuerpo = "El componente " + componenteAdjunto.name + " a cambiado de estado a: "
                + (nuevaNoti.isThere ? "EN UBICACIÓN" : "FUERA DE UBICACIÓN")
                + "\nVehículo: " + carroAdjunto.placa
                + "\nConductor: " + conductorAdjunto.name;

            SendNotification.Send(carroAdjunto.carOwner.token, "CarEye", cuerpo);

            return Ok(nuevaNoti);
        }

        // DELETE: api/notifications/5
        [ResponseType(typeof(notification))]
        public IHttpActionResult Deletenotification(int id)
        {
            notification notification = db.notifications.Find(id);
            if (notification == null)
            {
                return NotFound();
            }

            db.notifications.Remove(notification);
            db.SaveChanges();

            return Ok(notification);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool notificationExists(int id)
        {
            return db.notifications.Count(e => e.id == id) > 0;
        }
    }
}