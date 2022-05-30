using carsharingbackend.Models.DTO;
using carsharingbackend.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace carsharingbackend.Controllers
{
    public class loginController : ApiController
    {
        private carsharingdbEntities db = new carsharingdbEntities();

        // POST: api/Login
        [ResponseType(typeof(carOwner))]
        public IHttpActionResult Post([FromBody] LoginDTO login)
        {
            carOwner user = db.carOwners.FirstOrDefault(u =>
                u.email == login.email &&
                u.password == login.password
            );

            if (user == null)
            {
                return NotFound();
            }
            if (user.active == false)
            {
                return NotFound();
            }

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
