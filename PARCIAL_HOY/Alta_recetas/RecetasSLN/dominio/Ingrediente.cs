using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasSLN.dominio
{
    class Ingrediente
    {
        public int IngredienteId { get; set; }
        public string Nombre { get; set; }
        public string Unidad { get; set; }

        public Ingrediente()
        {

        }

        public override string ToString()
        {
            return IngredienteId.ToString() + "-" + Nombre;
        }
    }
}
