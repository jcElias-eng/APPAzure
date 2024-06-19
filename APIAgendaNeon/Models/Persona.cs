using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIAgendaNeon.Models
{
    public class Persona
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public DateTime fechanacimiento { get; set; }
        public string ci { get; set; }

        public ICollection<Telefono> telefono { get; set; }
    }
}

