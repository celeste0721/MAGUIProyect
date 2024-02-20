using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaguiProyecto.Entidades
{
#pragma warning disable CS8981 // El nombre de tipo solo contiene caracteres ASCII en minúsculas. Estos nombres pueden reservarse para el idioma.
    public class iniciarsesion
#pragma warning restore CS8981 
    {
        public string CorreoElectronico { get; set; }
        public string HashContrasena { get; set; }

    }
}
