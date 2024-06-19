using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIAgendaNeon.Models
{
    public class Telefono
    {
        public int id { get; set; }
        public int personaid { get; set; }
        public string tipo { get; set; }
        public string nrotelefono { get; set; }
        public string estado { get; set; }

        public Persona persona { get; set; }
    }
}

