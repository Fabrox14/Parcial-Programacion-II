﻿using RecetasSLN.dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasSLN.datos
{
    class HelperDAO
    {
        private static HelperDAO instancia;
        private string cadenaConexion;
        private HelperDAO()
        {
            cadenaConexion = @"Data Source=LAPTOP-8EMNHC7Q;Initial Catalog=db_recetas;Integrated Security=True";
        }

        public static HelperDAO ObtenerInstancia()
        {
            if (instancia == null)
            {
                instancia = new HelperDAO();
            }
            return instancia;
        }





        public DataTable ConsultaSQL(string nombreSP)
        {
            SqlConnection cnn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            DataTable tabla = new DataTable();

            try
            {
                cnn.ConnectionString = cadenaConexion;
                cnn.Open();

                // Command Productos
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = nombreSP;

                tabla.Load(cmd.ExecuteReader());

                return tabla;
            }
            catch (SqlException ex)
            {
                throw (ex);
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }

        }

        public int ProximoID(string nombreSP, string nombreParametro)
        {
            SqlConnection cnn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlParameter param = new SqlParameter();

            try
            {
                cnn.ConnectionString = cadenaConexion;
                cnn.Open();

                // Command proximo ID
                cmd.Connection = cnn;

                // Command Type para el Tipo de COmando que quiero ejecutar
                // cmd.CommandText = CommandType.Text;  ejecutamos sql como texto plano
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = nombreSP;

                param.ParameterName = nombreParametro;
                param.SqlDbType = SqlDbType.Int;
                param.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(param);
                cmd.ExecuteReader(); // no estoy esperando que el SP me devuelva un SELECT

                return (int)param.Value;
            }
            catch (SqlException ex)
            {
                throw (ex);
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
        }

        public bool Save(Receta oReceta)
        {
            SqlConnection cnn = new SqlConnection();
            SqlTransaction trans = null;
            bool resultado = true;

            try
            {
                cnn.ConnectionString = cadenaConexion;
                cnn.Open();
                trans = cnn.BeginTransaction();

                SqlCommand cmd = new SqlCommand("SP_INSERTAR_RECETA", cnn, trans);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_receta", oReceta.RecetaNro);
                cmd.Parameters.AddWithValue("@tipo_receta", oReceta.TipoReceta);
                cmd.Parameters.AddWithValue("@nombre", oReceta.Nombre);
                cmd.Parameters.AddWithValue("@cheff", oReceta.Cheff);


                cmd.ExecuteNonQuery();
                int cDetalles = 1; // es el ID que forma de la PK doble entre ID_PRESUPUESTO E ID_DETALLE
                int filasAfectadas = 0;


                foreach (DetalleReceta det in oReceta.Detalles)
                {
                    SqlCommand cmdDet = new SqlCommand("SP_INSERTAR_DETALLES", cnn);
                    cmdDet.CommandType = CommandType.StoredProcedure;
                    cmdDet.Transaction = trans;
                    cmdDet.Parameters.AddWithValue("@id_receta", oReceta.RecetaNro);
                    cmdDet.Parameters.AddWithValue("@id_ingrediente", cDetalles);
                    cmdDet.Parameters.AddWithValue("@cantidad", det.Ingrediente.IngredienteId);
                    cmdDet.ExecuteNonQuery();

                    cDetalles++;
                }


                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                resultado = false;
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }


            return resultado;
        }
    }
}
