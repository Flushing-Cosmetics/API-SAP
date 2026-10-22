using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace WS_SAP
{
    /// <summary>
    /// clase para llamada a la base de datos
    /// </summary>
    public class DB : Object
    {
        internal SqlConnection con;
        internal SqlDataAdapter da;
        internal SqlCommand cmd;            
        private readonly string serverValue = WebConfiguration.GetRequiredValue("Database:Server");
        private readonly string databaseValue = WebConfiguration.GetRequiredValue("Database:Name");
        private readonly string userValue = WebConfiguration.GetRequiredValue("Database:User");
        private readonly string passValue = WebConfiguration.GetRequiredValue("Database:Password");
        /// <summary>
        /// Procedimiento para conectar a la base de datos
        /// </summary>
        internal void Conectar()
        {
            string Conexion = ObtenerConexion();
            try
            {
                if ((con == null) || (con.State != ConnectionState.Open))
                {                    
                    con = new SqlConnection(Conexion);
                    con.Open();
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// Procedimientos para cerrar sesión de base datos
        /// </summary>
        internal void Desconectar()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// Procedimiento para ejecutar una consulta y retorna una tabla
        /// </summary>
        /// <returns></returns>
        internal DataTable EjecutarDataTable()
        {
            using (DataTable dt = new DataTable())
            {
                try
                {
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// proceimiento para crear un comando de una query
        /// </summary>
        /// <param name="consulta"></param>
        internal void CrearComando(string consulta)
        {
            try
            {
                cmd = new SqlCommand(consulta, con);
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// Procedimiento para obtener la conexion hacia la base de datos
        /// </summary>
        /// <returns></returns>
        internal string ObtenerConexion()
        {
            return "Server=" + serverValue + ";Database=" + databaseValue + ";User ID=" + userValue + ";Password=" + passValue + ";";
        }
    }
}
