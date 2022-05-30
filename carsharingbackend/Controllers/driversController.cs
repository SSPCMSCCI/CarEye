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
    [RoutePrefix("api/drivers")]
    public class driversController : ApiController
    {
        private carsharingdbEntities db = new carsharingdbEntities();

        // GET: api/drivers
        public IQueryable<driver> Getdrivers()
        {
            return db.drivers.Where(c => c.active);
        }

        // GET: api/drivers/5
        [ResponseType(typeof(driver))]
        public IHttpActionResult Getdriver(int id)
        {
            driver driver = db.drivers.FirstOrDefault(c => c.active);
            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }

        // PUT: api/drivers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putdriver(int id, driver driver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != driver.id)
            {
                return BadRequest();
            }

            db.Entry(driver).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!driverExists(id))
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

        // POST: api/drivers
        [ResponseType(typeof(driver))]
        public IHttpActionResult Postdriver(driver driver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var driverBuscado = db.drivers.FirstOrDefault(
                d => d.active 
                && d.dni.Trim() == driver.dni.Trim() 
                && d.idCarOwner == driver.idCarOwner
            );

            if (driverBuscado != null)
            {
                return Conflict();
            }

            driver.available = true;
            driver.active = true;
            db.drivers.Add(driver);
            db.SaveChanges();

            return Ok(driver);
        }

        // DELETE: api/drivers/5
        [ResponseType(typeof(driver))]
        public IHttpActionResult Deletedriver(int id)
        {
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                return NotFound();
            }

            //db.drivers.Remove(driver);
            driver.active = false;
            db.SaveChanges();

            return Ok(driver);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool driverExists(int id)
        {
            return db.drivers.Count(e => e.id == id) > 0;
        }
    }
}