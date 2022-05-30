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
    [RoutePrefix("api/cars")]
    public class carsController : ApiController
    {
        private carsharingdbEntities db = new carsharingdbEntities();

        // GET: api/cars
        public IQueryable<car> Getcars()
        {
            return db.cars.Where(c => c.active); 
        }

        // GET: api/cars/5
        [ResponseType(typeof(car))]
        public IHttpActionResult Getcar(int id)
        {
            car car = db.cars.Find(id);
            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }

        // GET: api/cars/{id:int}/carPosition
        [HttpGet]
        [Route("{id:int}/carPosition")]
        public carPosition GetCarPosition(int id)
        {
            return db.carPositions.FirstOrDefault(c => c.idCar == id);
        }

        // GET: api/cars/{id:int}/details
        [HttpGet]
        [Route("{id:int}/details")]
        public CarDetailsDTO GetCarDetails(int id)
        {
            car carfinded = db.cars.FirstOrDefault(c => c.id == id);
            carPosition carPositionFinded = db.carPositions.FirstOrDefault(cp => cp.idCar == id);
            
            driverXCar driverXCarFinded = db.driverXCars.FirstOrDefault(dc => DateTime.Today >= dc.lendStartDate && DateTime.Today <= dc.lendEndDate);
            driver currentDriverFinded = null;
            if (driverXCarFinded != null)
                currentDriverFinded = driverXCarFinded.driver;

            List<driverXCar> driverHistoryFinded = db.driverXCars
                .Where(dc => dc.idCar == id)
                .OrderByDescending(dc => dc.lendStartDate)
                .ToList();

            List<component> componentsFinded = db.components.Where(n => n.idCar == id).ToList();
            List<notification> notificationsFinded = db.notifications.Where(n => n.idCar == id).ToList();

            CarDetailsDTO carDetailsDTO = new CarDetailsDTO();
            carDetailsDTO.car = carfinded;
            carDetailsDTO.carPosition = carPositionFinded;
            carDetailsDTO.currentDriver = currentDriverFinded;
            carDetailsDTO.driverHistory = driverHistoryFinded;
            carDetailsDTO.notifications = notificationsFinded;
            carDetailsDTO.components = componentsFinded;
            return carDetailsDTO;
        }

        // GET: api/cars/{id:int}/components
        [HttpGet]
        [Route("{id:int}/components")]
        public IEnumerable<component> GetComponentsByCar(int id)
        {
            return db.components
                .Where(c => c.idCar == id && c.active)
                .AsEnumerable()
                .Reverse();
        }

        // GET: api/cars/{id:int}/notifications
        [HttpGet]
        [Route("{id:int}/notifications")]
        public IEnumerable<notification> GetNotificationsByCar(int id)
        {
            return db.notifications
                .Where(n => n.idCar == id && n.active)
                .AsEnumerable()
                .Reverse();
        }

        // GET: api/cars/{id:int}/drivers
        [HttpGet]
        [Route("{id:int}/driverHistory")]
        public DriverHistoryDTO GetDriverHistoryByCar(int id)
        {
            //busco el historial de conductores asignados al carro
            List<driverXCar> driverHistoryFinded = db.driverXCars
                .Where(dc => dc.idCar == id && dc.active)
                .OrderByDescending(dc => dc.lendStartDate)
                .ToList();

            //del historial tomo el conductor que tiene hoy en el rango fecha de prestamo 
            driverXCar currentDriver = driverHistoryFinded
                .FirstOrDefault(
                dc => 
                DateTime.Today >= dc.lendStartDate 
                && DateTime.Today <= dc.lendEndDate
            );

            //elimino el conductor actual del historial de conductores
            if (currentDriver != null)
                driverHistoryFinded.Remove(currentDriver);

            DriverHistoryDTO driverHistoryDTO = new DriverHistoryDTO();
            driverHistoryDTO.driverHistory = driverHistoryFinded;
            driverHistoryDTO.currentDriver = currentDriver;

            return driverHistoryDTO;
        }

        // PUT: api/cars/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putcar(int id, car car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != car.id)
            {
                return BadRequest();
            }

            db.Entry(car).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!carExists(id))
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

        // POST: api/cars
        [ResponseType(typeof(car))]
        public IHttpActionResult Postcar(car car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carBuscado = db.cars.FirstOrDefault(
                c => c.active 
                && c.placa.Trim() == car.placa.Trim()
                && c.idCarOwner == car.idCarOwner
            );

            if (carBuscado != null)
            {
                return Conflict();
            }

            //agrego el carro
            car.active = true;
            car carroAgregado = db.cars.Add(car);
            db.SaveChanges();

            //agrego la posicion del carro
            carPosition carPosition = new carPosition();
            carPosition.idCar = carroAgregado.id;
            carPosition.lat = "-12.146236";
            carPosition.lng = "-77.009470";
            carPosition.active = true;
            db.carPositions.Add(carPosition);
            db.SaveChanges();

            return Ok(carroAgregado);
        }

        // DELETE: api/cars/5
        [ResponseType(typeof(car))]
        public IHttpActionResult Deletecar(int id)
        {
            car car = db.cars.Find(id);
            if (car == null)
            {
                return NotFound();
            }

            //db.cars.Remove(car);
            car.active = false;
            db.SaveChanges();

            return Ok(car);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool carExists(int id)
        {
            return db.cars.Count(e => e.id == id) > 0;
        }
    }
}