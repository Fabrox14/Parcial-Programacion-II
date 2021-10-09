using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasSLN.datos
{
    class DAOFactory : AbstractDAOFactory
    {
        public override IRecetaDAO CrearRecetaDAO()
        {
            return new RecetaDAO();
        }
    }
}
