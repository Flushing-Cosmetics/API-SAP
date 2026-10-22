using Newtonsoft.Json.Linq;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Web;


namespace WS_SAP
{
    /// <summary>
    /// 
    /// </summary>
    internal class SAP_TipoCambio
    {
        internal Resultado TipoCambio(string db, double Valor, string Moneda, string Fecha)
        {
            Resultado Resultado = new Resultado();
            StreamWriter sw;
            string fecha = System.DateTime.Now.ToString("yyyyMMdd");
            string path = HttpContext.Current.Request.MapPath("~/log/tipocambio/log_" + fecha + ".txt");
            sw = new StreamWriter(path, true);
            sw.WriteLine("//////////////////////////////////////////////////////");
            sw.WriteLine("INICIO DE DATOS");
            sw.WriteLine("BD:[" + db + "]");
            sw.WriteLine("Fecha Inicio:[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]");
            sw.WriteLine("Valor:" + Valor);
            sw.WriteLine("Moneda:" + Moneda);
            sw.WriteLine("Fecha:" + Fecha + "");
            try
            {
                SAP Sap = new SAP();
                Resultado = Sap.Conectar(db: db);
                if (Resultado.Estatus == true)
                {
                    SAPbobsCOM.SBObob oTC;
                    oTC = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                    oTC.SetCurrencyRate(Moneda, Sap.ConvertirFecha(Fecha), Valor, true);
                    Resultado.Estatus = true;
                    Resultado.Msj = "Tipo de Cambio Actualizado en BD[ " + db + " ]";    
                }
                else
                {
    
                }
                sw.WriteLine("Resultado Estatus:" + Resultado.Estatus.ToString());
                sw.WriteLine("Resultado MSJ:" + Resultado.Msj.ToString());
                sw.WriteLine("Fecha Final:[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]");
                sw.WriteLine("FIN DE DATOS");
                sw.Flush();
                sw.Close();
                return Resultado;
            }
            catch (Exception ex)
            {
                Resultado.Msj = ex.Message + " | " + ex.StackTrace;
                sw.WriteLine("Resultado Estatus:" + Resultado.Estatus.ToString());
                sw.WriteLine("Resultado MSJ:" + Resultado.Msj.ToString());
                sw.WriteLine("Fecha Final:[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]");
                sw.WriteLine("FIN DE DATOS");
                sw.Flush();
                sw.Close();
            
                return Resultado;
            }
        }
    }
}