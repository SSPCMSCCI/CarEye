using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using carsharingbackend.Models.DTO;
using carsharingbackend.Models.EF;

namespace carsharingbackend.Controllers
{
    [RoutePrefix("api/carOwners")]
    public class carOwnersController : ApiController
    {
        private carsharingdbEntities db = new carsharingdbEntities();

        // GET: api/carOwners
        public IQueryable<carOwner> GetcarOwners()
        {
            return db.carOwners.Where(c => c.active);
        }

        // GET: api/carOwners/5
        [ResponseType(typeof(carOwner))]
        public IHttpActionResult GetcarOwner(int id)
        {
            carOwner carOwner = db.carOwners.Find(id);
            if (carOwner == null)
            {
                return NotFound();
            }

            return Ok(carOwner);
        }

        // PUT: api/carOwners/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutcarOwner(int id, carOwner carOwner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != carOwner.id)
            {
                return BadRequest();
            }

            db.Entry(carOwner).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!carOwnerExists(id))
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

        // POST: api/carOwners
        [ResponseType(typeof(carOwner))]
        public IHttpActionResult PostcarOwner(carOwner carOwner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!dniExists(carOwner.dni) && !emailExists(carOwner.email))
            {
                carOwner.active = true;
                carOwner.email = carOwner.email.ToLower();
                db.carOwners.Add(carOwner);
                db.SaveChanges();

                return Ok(carOwner);
            }
            else
            {
                return Conflict();
            }
        }

        // DELETE: api/carOwners/5
        [ResponseType(typeof(carOwner))]
        public IHttpActionResult DeletecarOwner(int id)
        {
            carOwner carOwner = db.carOwners.Find(id);
            if (carOwner == null)
            {
                return NotFound();
            }

            //db.carOwners.Remove(carOwner);
            carOwner.active = false;
            db.SaveChanges();

            return Ok(carOwner);
        }

        // GET: api/carOwners/{id:int}/cars
        [HttpGet]
        [Route("{id:int}/cars")]
        public IEnumerable<car> GetCarOwnerCars(int id)
        {
            return db.cars
                .Where(c => c.idCarOwner == id && c.active)
                .AsEnumerable()
                .Reverse();
        }

        // GET: api/carOwners/{id:int}/drivers
        [HttpGet]
        [Route("{id:int}/drivers")]
        public IEnumerable<driver> GetCarOwnerDrivers(int id)
        {
            return db.drivers
                .Where(c => c.idCarOwner == id && c.active)
                .AsEnumerable()
                .Reverse();
        }

        // GET: api/carOwners/{id:int}/notifications
        [HttpGet]
        [Route("{id:int}/notifications")]
        public IEnumerable<notification> GetCarOwnerNotifications(int id)
        {
            return db.notifications
                .Include("car")
                .Include("component")
                .Include("driver")
                .Where(n => n.car.idCarOwner == id && n.active)
                .AsEnumerable()
                .Reverse();
        }

        // GET: api/carOwners/{id:int}/components
        [HttpGet]
        [Route("{id:int}/components")]
        public IEnumerable<component> GetcomponentsOwnerDrivers(int id)
        {
            return db.components
                .Where(c => c.car.idCarOwner == id && c.active)
                .AsEnumerable()
                .Reverse();
        }

        // GET: api/carOwners/{id:int}/driversAvailable
        [HttpGet]
        [Route("{id:int}/driversAvailable")]
        public IEnumerable<driver> GetDriversAvailableByCarOwner(int id)
        {
            var driversFinded = db.drivers
                .Where(c => c.idCarOwner == id && c.active)
                .AsEnumerable()
                .Reverse();

            var driversHistory = db.driverXCars
                .Where(dc => dc.car.idCarOwner == id && dc.active)
                .OrderBy(dc => dc.lendStartDate);

            foreach (var driverH in driversHistory)
            {
                var driverFound = driversFinded.FirstOrDefault(d => d.id == driverH.idDriver);
                if (driverFound != null)
                {
                    driverFound.available = !(DateTime.Today >= driverH.lendStartDate && DateTime.Today <= driverH.lendEndDate);
                }                    
            }

            db.SaveChanges();
            return driversFinded.Where(d => d.available);
        }

        // POST: api/carOwners/sendResetPasswordEmail
        [HttpPost]
        [Route("sendResetPasswordEmail")]
        public IHttpActionResult PostSendResetPasswordEmail(ResetPasswordDTO rp)
        {
            try
            {
                rp.email = rp.email.ToLower();
                carOwner userFinded = db.carOwners.FirstOrDefault(u => u.email == rp.email && u.dni == rp.dni);

                if (userFinded != null)
                {
                    string charsUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    string charsLower = "abcdefghijklmnopqrstuvwxyz";
                    string numbers = "0123456789";
                    string newPassword = "";

                    //creo la nueva contraseña
                    StringBuilder sbnp = new StringBuilder();
                    Random r = new Random();

                    sbnp.Append(numbers[r.Next(0, numbers.Length)]);
                    sbnp.Append(charsUpper[r.Next(0, charsUpper.Length)]);
                    for (int i = 0; i < 7; i++)
                        sbnp.Append(charsLower[r.Next(0, charsLower.Length)]);

                    newPassword = sbnp.ToString();
                    userFinded.password = newPassword;
                    db.SaveChanges();

                    string messageText = "Su nueva contraseña es: " + newPassword;
                    EnviaMail.Enviar(rp.email, messageText);
                    return Ok(new { response = "Ok" });
                }
                else
                {
                    return Conflict();
                }
            }
            catch (Exception e)
            {
                return Ok(new { error = e.StackTrace });
            }
        }

        // PUT: api/carOwners/resetPassword
        [HttpPut]
        [Route("resetPassword")]
        public IHttpActionResult PostResetPassword(NewPasswordDTO np)
        {
            carOwner userFinded = db.carOwners.FirstOrDefault(u => u.id == np.userId);

            if (userFinded != null)
            {
                userFinded.password = np.newPassword;
                db.SaveChanges();
                return Ok(new { response = "Ok" });
            }
            else
            {
                return Conflict();
            }
        }

        // POST: api/carOwners/tokenFCM
        [HttpPost]
        [Route("tokenFCM")]
        public IHttpActionResult PostTokenFcm(TokenFcmDTO tokenFcmDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userFinded = db.carOwners.FirstOrDefault(u => u.id == tokenFcmDTO.userId);
            if (userFinded != null)
            {
                userFinded.token = tokenFcmDTO.token;
                db.SaveChanges();
                return Ok(userFinded);
            }
            else
            {
                return Conflict();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool carOwnerExists(int id)
        {
            return db.carOwners.Count(e => e.id == id) > 0;
        }

        private bool dniExists(string dni)
        {
            return db.carOwners.Count(e => e.dni == dni) > 0;
        }

        private bool emailExists(string email)
        {
            return db.carOwners.Count(e => e.email == email) > 0;
        }
    }
}