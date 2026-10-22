using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WS_SAP
{
    /// <summary>
    /// Clase para Devolver Encabezado de Resultado
    /// </summary>
    public class Resultado
    {
        /// <summary>
        /// Estatus de la Notificacion
        /// </summary>
        public bool Estatus { get; set; }
        /// <summary>
        /// Mensaje de la transacción
        /// </summary>
        public string Msj { get; set; }
        ///// <summary>
        ///// Llave principal del tipo de documento resultante
        ///// </summary>
        //public int CodKey { get; set; }
        /// <summary>
        /// Campos del Documento
        /// </summary>
        public Dictionary<string, string> Datos { get; set; }
    }
}