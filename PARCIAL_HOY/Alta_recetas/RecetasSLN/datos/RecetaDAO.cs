using RecetasSLN.dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasSLN.datos
{
    class RecetaDAO : IRecetaDAO
    {
        public int ObtenerProximoNroPresupuesto()
        {
            return HelperDAO.ObtenerInstancia().ProximoID("SP_PROXIMO_ID", "@next");
        }

        public DataTable ListarProductos()
        {
            return HelperDAO.ObtenerInstancia().ConsultaSQL("SP_CONSULTAR_INGREDIENTES");
        }

        public bool Crear(Receta oReceta)
        {
            return HelperDAO.ObtenerInstancia().Save(oReceta);
        }
    }
}
