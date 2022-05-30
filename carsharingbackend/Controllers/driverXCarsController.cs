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
using carsharingbackend.Models.EF;

namespace carsharingbackend.Controllers
{
    public class driverXCarsController : ApiController
    {
        private carsharingdbEntities db = new carsharingdbEntities();

        // GET: api/driverXCars
        public IQueryable<driverXCar> GetdriverXCars()
        {
            return db.driverXCars;
        }

        // GET: api/driverXCars/5
        [ResponseType(typeof(driverXCar))]
        public IHttpActionResult GetdriverXCar(int id)
        {
            driverXCar driverXCar = db.driverXCars.Find(id);
            if (driverXCar == null)
            {
                return NotFound();
            }

            return Ok(driverXCar);
        }

        // PUT: api/driverXCars/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutdriverXCar(int id, driverXCar driverXCar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != driverXCar.id)
            {
                return BadRequest();
            }

            db.Entry(driverXCar).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!driverXCarExists(id))
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

        // POST: api/driverXCars
        [ResponseType(typeof(driverXCar))]
        public IHttpActionResult PostdriverXCar(driverXCar driverXCar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            driverXCar.active = true;
            db.driverXCars.Add(driverXCar);

            driver driverFinded = db.drivers.FirstOrDefault(d => d.id == driverXCar.idDriver);
            if (driverFinded != null)
            {
                driverFinded.available = false;
            }
            db.SaveChanges();

            return Ok(driverXCar);
        }

        // DELETE: api/driverXCars/5
        [ResponseType(typeof(driverXCar))]
        public IHttpActionResult DeletedriverXCar(int id)
        {
            driverXCar driverXCar = db.driverXCars.Find(id);
            if (driverXCar == null)
            {
                return NotFound();
            }

            driverXCar.active = false;

            driver driverFinded = db.drivers.FirstOrDefault(d => d.id == driverXCar.idDriver);
            if (driverFinded != null)
            {
                driverFinded.available = true;
            }

            //db.driverXCars.Remove(driverXCar);
            db.SaveChanges();

            return Ok(driverXCar);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool driverXCarExists(int id)
        {
            return db.driverXCars.Count(e => e.id == id) > 0;
        }
    }
}