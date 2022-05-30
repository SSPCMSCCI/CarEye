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
    public class componentsController : ApiController
    {
        private carsharingdbEntities db = new carsharingdbEntities();

        // GET: api/components
        public IQueryable<component> Getcomponents()
        {
            return db.components.Where(c => c.active);
        }

        // GET: api/components/5
        [ResponseType(typeof(component))]
        public IHttpActionResult Getcomponent(int id)
        {
            component component = db.components.FirstOrDefault(c => c.active);
            if (component == null)
            {
                return NotFound();
            }

            return Ok(component);
        }

        // PUT: api/components/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putcomponent(int id, component component)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != component.id)
            {
                return BadRequest();
            }

            db.Entry(component).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!componentExists(id))
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

        // POST: api/components
        [ResponseType(typeof(component))]
        public IHttpActionResult Postcomponent(component component)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //elimino los componentes con el mismo rfid 
            var componentesfinded = db.components.Where(c => c.rfidTicketCode == component.rfidTicketCode && c.active);
            foreach (var item in componentesfinded)
                item.active = false;

            //agrego el nuevo componente
            component.isThere = true;
            component.active = true;
            db.components.Add(component);
            db.SaveChanges();

            return Ok(component);
        }

        // DELETE: api/components/5
        [ResponseType(typeof(component))]
        public IHttpActionResult Deletecomponent(int id)
        {
            component component = db.components.Find(id);
            if (component == null)
            {
                return NotFound();
            }

            //db.components.Remove(component);
            component.active = false;
            db.SaveChanges();

            return Ok(component);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool componentExists(int id)
        {
            return db.components.Count(e => e.id == id) > 0;
        }
    }
}