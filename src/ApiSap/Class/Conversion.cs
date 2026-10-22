using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace WS_SAP
{
    /// <summary>
    /// Clase para Convertir Tabla a Json
    /// </summary>
    public class Conversion
    {
        /// <summary>
        /// Procedimiento de Conversion de DataTable a Json
        /// </summary>        
        /// <param name="table"></param>
        /// <returns></returns>
        public string ConversionJson(DataTable table)
        {
           string JSONString=string.Empty;  
           JSONString = JsonConvert.SerializeObject(table);  
           return JSONString;  
        }
    }
}