using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WS_SAP
{
    /// <summary>
    /// Clase para ejecuta consultas
    /// </summary>
    public class DAL
    {
        /// <summary>
        /// Procedimiento que ejecuta querys de base de datos
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        internal DataTable Ejecutar_t(string Query)
        {
            DataTable dt;
            DB ObjDB = new DB();
            ObjDB.Conectar();
            ObjDB.CrearComando(Query);
            dt = ObjDB.EjecutarDataTable();
            ObjDB.Desconectar();
            return dt;
        } 
    }
}