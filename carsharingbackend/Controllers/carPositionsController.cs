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
    public class carPositionsController : ApiController
    {
        private carsharingdbEntities db = new carsharingdbEntities();

        // GET: api/carPositions
        public IQueryable<carPosition> GetcarPositions()
        {
            return db.carPositions.Where(c => c.active);
        }

        // GET: api/carPositions/5
        [ResponseType(typeof(carPosition))]
        public IHttpActionResult GetcarPosition(int id)
        {
            carPosition carPosition = db.carPositions.Find(id);
            if (carPosition == null)
            {
                return NotFound();
            }

            return Ok(carPosition);
        }

        // PUT: api/carPositions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutcarPosition(int id, carPosition carPosition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            carPosition finded = db.carPositions.FirstOrDefault(c => c.idCar == id && c.active);
            if (finded == null)
            {
                return NotFound();
            }

            finded.lat = carPosition.lat;
            finded.lng = carPosition.lng;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!carPositionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(finded);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool carPositionExists(int id)
        {
            return db.carPositions.Count(e => e.id == id) > 0;
        }
    }
}