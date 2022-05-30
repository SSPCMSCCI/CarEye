//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace carsharingbackend.Models.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class driver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public driver()
        {
            this.driverXCars = new HashSet<driverXCar>();
            this.notifications = new HashSet<notification>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string dni { get; set; }
        public string cellphone { get; set; }
        public string address { get; set; }
        public string pictureUrl { get; set; }
        public int idCarOwner { get; set; }
        public bool available { get; set; }
        public bool active { get; set; }
    
        [Newtonsoft.Json.JsonIgnore] public virtual carOwner carOwner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Newtonsoft.Json.JsonIgnore] public virtual ICollection<driverXCar> driverXCars { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Newtonsoft.Json.JsonIgnore] public virtual ICollection<notification> notifications { get; set; }
    }
}