using RecetasSLN.datos;
using RecetasSLN.dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasSLN.Servicios
{
    class GestorReceta
    {
        private IRecetaDAO dao;

        public GestorReceta(AbstractDAOFactory factory)
        {
            dao = factory.CrearRecetaDAO();
        }





        public int ProximoPresupuesto()
        {
            return dao.ObtenerProximoNroPresupuesto();
        }

        public DataTable ObtenerProductos()
        {
            return dao.ListarProductos();
        }

        public bool ConfirmarPresupuesto(Receta oReceta)
        {
            return dao.Crear(oReceta);
        }
    }
}
