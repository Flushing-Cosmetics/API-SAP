using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading;
using Microsoft.Extensions.Configuration;
namespace WS_SAP
{
    /// <summary>
    /// Clase de Auxiliar para transacciones SAP
    /// </summary>
    public class SAP
    {
        private static readonly Lazy<IConfiguration> Configuration = new(CargarConfiguracion);
        /// <summary>
        /// Variables de Servidores
        /// </summary>
        private readonly string SAPServer = Configuration.Value["SAP:Server"] ?? string.Empty;
        private readonly string SAPDatabase = Configuration.Value["SAP:Database"] ?? string.Empty;
        private readonly string SAPUser = Configuration.Value["SAP:User"] ?? string.Empty;
        private readonly string SAPPassword = Configuration.Value["SAP:Password"] ?? string.Empty;
        private readonly string SAPUserDB = Configuration.Value["SAP:DatabaseUser"] ?? string.Empty;
        private readonly string SAPPasswordDB = Configuration.Value["SAP:DatabasePassword"] ?? string.Empty;
        private readonly string SAPLicenseServer = Configuration.Value["SAP:LicenseServer"] ?? string.Empty;
        public SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();

        private static IConfiguration CargarConfiguracion()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("webconfig.json", optional: false, reloadOnChange: false)
                .Build();
        }
        /// <summary>
        /// Procedimiento para hacer una conexion a SAP
        /// </summary>
        /// <returns></returns>
        internal Resultado Conectar()
        {
            Resultado Resultado = new Resultado();
            int lRetCode;
            int lErrCode;
            string sErrMsg;
            int Retry = 0;
            try
            {
                if (oCompany != null)
                {
                    if (oCompany.Connected && oCompany.CompanyDB != SAPDatabase)
                    {
                        oCompany.Disconnect();
                    }
                    if (oCompany.Connected && oCompany.CompanyDB == SAPDatabase)
                    {
                        Resultado.Estatus = true;
                        Resultado.Msj = "Conexion Exitosa!!!";
                        return Resultado;
                    }
                }
                oCompany.Server = SAPServer;
                oCompany.CompanyDB = SAPDatabase;
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;
                oCompany.DbUserName = SAPUserDB;
                oCompany.DbPassword = SAPPasswordDB;
                oCompany.UserName = SAPUser;
                oCompany.Password = SAPPassword;
                oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La;
                oCompany.LicenseServer = SAPLicenseServer;

                Error:
                sErrMsg = "";
                lRetCode = oCompany.Connect();
                if (lRetCode != 0)
                {
                    oCompany.GetLastError(out lErrCode, out sErrMsg);
                    Resultado.Estatus = false;
                    Resultado.Msj = sErrMsg;
                    Retry += 1;
                    if (Retry < 10)
                    {
                        Thread.Sleep(1000);
                        goto Error;
                    }
                    oCompany.Disconnect();
                    return Resultado;
                }
                else
                {
                    Resultado.Estatus = true;
                    Resultado.Msj = "Conexion Exitosa!!!";
                    return Resultado;
                }
            }
            catch (Exception ex)
            {
                Resultado.Estatus = false;
                Resultado.Msj = ex.Message.ToString();
                return Resultado;
            }
        }
        /// <summary>
        /// Finalizacion de Sesion de sap
        /// </summary>
        /// <returns></returns>
        internal Resultado Finalizar()
        {
            Resultado Resultado = new Resultado();
            if (oCompany != null)
            {
                if (oCompany.Connected)
                {
                    Resultado.Estatus = true;
                    Resultado.Msj = "Conexion Finalizada!!!";
                    oCompany.Disconnect();
                }
            }
            return Resultado;
        }
        /// <summary>
        /// Conversion de hora, formato que utiliza sap
        /// </summary>
        /// <param name="Hora"></param>
        /// <returns></returns>
        internal DateTime ConvertirHora(string Hora)
        {
            //Obtener Fecha Actual
            DateTime Time = DateTime.Today.Date;
            string[] HoraResult;
            //Agregar hora y minutos a la fecha
            if (Hora != "")
            {
                HoraResult = Hora.Split(':');
                Time = Time.AddHours(Convert.ToDouble(HoraResult[0]));
                Time = Time.AddMinutes(Convert.ToDouble(HoraResult[1]));
            }
            else
            {
                Time = Time.AddHours(0);
                Time = Time.AddMinutes(0);
            }
            return Time;
        }
        /// <summary>
        /// Conversion de string a Fecha, formato que utiliza SAP
        /// </summary>
        /// <param name="Fecha"></param>
        /// <returns></returns>
        internal DateTime ConvertirFecha(string Fecha)
        {
            DateTime FechaResult = Convert.ToDateTime(Fecha);
            return FechaResult;
        }
        /// <summary>
        /// Conversion de string a objeto de SAP tyes o tno 
        /// </summary>
        /// <param name="Valor"></param>
        /// <returns></returns>
        internal SAPbobsCOM.BoYesNoEnum ConvertirYesNo(string Valor)
        {            
            Valor = Valor.ToLower();
            if (Valor == "tyes" || Valor == "y")
            {
                return SAPbobsCOM.BoYesNoEnum.tYES;
            }
            else
            {
                return SAPbobsCOM.BoYesNoEnum.tNO;
            }
        }
        internal string AsignarValorCampoUsuario(SAPbobsCOM.Fields Obj, string Campo, string Valor)
        {
            int i;
            bool Busqueda = false;
            Campo = Campo.ToLower();
            //Quitar datos antes del u_
            if (Campo.StartsWith("u_") == false)
            {
                i = Campo.IndexOf("u_");
                Campo = Campo.Substring(i);
            }
            string ver = Campo;
            //Busqueda si existe el campo de usuario
            for (i = 0; i <= Obj.Count - 1; i++)
            {
                if (Obj.Item(i).Name.ToLower() == Campo)
                {
                    Busqueda = true;
                    break;
                }
            }
            if (Busqueda == false)
            {
                //Error que no encuenta el campo solicitado
                return "Error asignando valor al campo de usuario, el campo [" + Campo + "] no existe en la base de datos.";
            }
            else
            {
                try
                {
                    //Asigna Valor al tipo de datos que utiliza campo de usuario
                    switch (Obj.Item(i).Type)
                    {
                        case SAPbobsCOM.BoFieldTypes.db_Alpha:
                            Obj.Item(i).Value = Valor;
                            break;
                        case SAPbobsCOM.BoFieldTypes.db_Memo:
                            Obj.Item(i).Value = Valor;
                            break;
                        case SAPbobsCOM.BoFieldTypes.db_Date:
                            //Verifica el SubType de la fecha y le asigna el valor correspondiente
                            if (Obj.Item(i).SubType == SAPbobsCOM.BoFldSubTypes.st_Time)
                            {
                                Obj.Item(i).Value = ConvertirHora(Valor);
                            }
                            else
                            {
                                Obj.Item(i).Value = ConvertirFecha(Valor);
                            }
                            break;
                        case SAPbobsCOM.BoFieldTypes.db_Float:
                            Obj.Item(i).Value = Valor;
                            break;
                        case SAPbobsCOM.BoFieldTypes.db_Numeric:
                            Obj.Item(i).Value = Valor;
                            break;
                    }
                    return "";
                }
                catch (Exception Ex)
                {
                    //Error de asignacion de campo de usuario
                    return "Error asignando valor a campo de usuario, campo [" + Campo + "], Valor: [" + Valor + "]; Error: " + Ex.Message.ToString();
                }
            }            
        }        
    }
}
