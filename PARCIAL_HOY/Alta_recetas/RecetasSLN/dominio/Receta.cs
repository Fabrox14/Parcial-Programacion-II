using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasSLN.dominio
{
    class Receta
    {
        public int RecetaNro { get; set; }
        public string Nombre { get; set; }
        public string Cheff { get; set; }
        public int TipoReceta { get; set; }
        public DateTime FechaBaja { get; set; }
        public List<DetalleReceta> Detalles { get; }

        public Receta()
        {
            // Generar la relacion 1 a muchos
            Detalles = new List<DetalleReceta>();
        }

        public void AgregarDetalle(DetalleReceta detalle)
        {
            Detalles.Add(detalle);
        }

        public void QuitarDetalle(int nro)
        {
            Detalles.RemoveAt(nro);
        }
        
    }
}
