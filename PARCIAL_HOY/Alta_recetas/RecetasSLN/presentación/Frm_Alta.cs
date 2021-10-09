
using RecetasSLN.datos;
using RecetasSLN.dominio;
using RecetasSLN.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecetasSLN
{
    public partial class Frm_Alta : Form
    {
        private Receta oReceta;
        private GestorReceta gestor;
        public Frm_Alta()
        {
            InitializeComponent();
            oReceta = new Receta();
            gestor = new GestorReceta(new DAOFactory());
        }


        private void Frm_Alta_Presupuesto_Load(object sender, EventArgs e)
        {
            CargarCombo();
            consultarUltimoPresupuesto();
            // Valores por defecto
            cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProducto.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void consultarUltimoPresupuesto()
        {
            lblNro.Text = "Receta #: " + gestor.ProximoPresupuesto();
        }

        private void CargarCombo()
        {
            DataTable tabla = gestor.ObtenerProductos();

            // tabla.Rows[0]; // cada fila que tenga va a ser un DataRow

            cboProducto.DataSource = tabla;
            cboProducto.DisplayMember = tabla.Columns[1].ColumnName; // n_producto
            cboProducto.ValueMember = tabla.Columns[0].ColumnName; // id_producto 
        }


        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (ExisteProductoEnGrilla(cboProducto.Text))
            {
                MessageBox.Show("Ingrediente ya agregado", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            DialogResult result = MessageBox.Show("Desea Agregar?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                DetalleReceta item = new DetalleReceta();
                item.Cantidad = (int)nudCantidad.Value;

                DataRowView oDataRow = (DataRowView)cboProducto.SelectedItem;

                Ingrediente oProducto = new Ingrediente();
                oProducto.IngredienteId = Int32.Parse(oDataRow[0].ToString());
                oProducto.Nombre = oDataRow[1].ToString();
                oProducto.Unidad = oDataRow[2].ToString();
                item.Ingrediente = oProducto;

                oReceta.AgregarDetalle(item);

                // te voy a dar un array de objetos
                dgvDetalles.Rows.Add(new object[] { "", oProducto.Nombre, item.Cantidad });

                calcularTotales();
            }

            calcularTotales();
        }

        private void calcularTotales()
        {
            int total = 0;
            foreach (DataGridViewRow fila in dgvDetalles.Rows)
            {
                total += 1;
            }
            lblTotal.Text = "Total de Ingredientes: " + total;
        }

        private bool ExisteProductoEnGrilla(string producto)
        {
            foreach (DataGridViewRow fila in dgvDetalles.Rows)
            {
                string col = fila.Cells["ingrediente"].Value.ToString();
                if (col.Equals(producto))
                {
                    return true;
                }
            }
            return false;
        }





        private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDetalles.CurrentCell.ColumnIndex == 3)
            {
                oReceta.QuitarDetalle(dgvDetalles.CurrentRow.Index);
                dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);
            }
        }



        





        private void btnAceptar_Click(object sender, EventArgs e)
        {

            if (dgvDetalles.Rows.Count < 3)
            {
                MessageBox.Show("Ha olvidado ingredientes?",
                "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtNombre.Text.Trim() == "")
            {
                MessageBox.Show("Debe ingresar un nombre de receta", "Validaciones", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNombre.Focus();
                return;
            }

            if (txtCheff.Text.Trim() == "")
            {
                MessageBox.Show("Debe ingresar un cheff", "Validaciones", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCheff.Focus();
                return;
            }

            GuardarPresupuesto();
            Limpiar();
            consultarUltimoPresupuesto();
        }

        private void Limpiar()
        {
            txtNombre.Text = "";
            txtCheff.Text = "";
            cboTipo.SelectedIndex = -1;
            cboProducto.SelectedIndex = -1;
            nudCantidad.Value = 1;
            dgvDetalles.Rows.Clear();
            lblTotal.Text = "Total de Ingredientes: ";
        }

        private void GuardarPresupuesto()
        {
            oReceta.Nombre = txtNombre.Text;
            oReceta.Cheff = txtCheff.Text;
            oReceta.TipoReceta = Convert.ToInt32(cboTipo.SelectedIndex);
            oReceta.RecetaNro = gestor.ProximoPresupuesto();

            if (gestor.ConfirmarPresupuesto(oReceta))
            {
                MessageBox.Show("Presupuesto registrado", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // this.Dispose();
            }
            else
            {
                MessageBox.Show("ERROR. No se pudo registrar el presupuesto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Está seguro que desea cancelar?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Dispose();

            }
            else
            {
                return;
            }
        }

        

        
    }
}
