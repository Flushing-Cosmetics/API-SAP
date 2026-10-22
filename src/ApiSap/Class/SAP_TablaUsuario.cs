using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS_SAP
{
    public class SAP_TablaUsuario
    {
        public Resultado Agregar(string Datos, string TablaUsuario)
        {
            Resultado Resultado = new Resultado();
            if (Datos != null)
            {
                try
                {
                    string TipoAccion = "";
                    SAP Sap = new SAP();
                    Resultado = Sap.Conectar();
                    if (Resultado.Estatus == true)
                    {
                        SAPbobsCOM.UserTable oTU = Sap.oCompany.UserTables.Item(TablaUsuario);
                        var jObject = JObject.Parse(Datos);
                        foreach (var Obj in jObject)
                        {
                            switch (Obj.Key.ToLower())
                            {
                                case "tipoaccion":
                                    if (Obj.Value.ToString().ToLower() == "agregar")
                                    {
                                        TipoAccion = "agregar";     
                                    }
                                    if (Obj.Value.ToString().ToLower() == "actualizar")
                                    {
                                        TipoAccion = "actualizar";
                                    }
                                    break;
                                case "code":
                                    if (TipoAccion == "agregar")
                                    {
                                        oTU.Code = Obj.Value.ToString();
                                    }
                                    if (TipoAccion == "actualizar")
                                    {
                                        if (oTU.GetByKey(Obj.Value.ToString().Trim()) == false)
                                        {
                                            Resultado.Estatus = false;
                                            Resultado.Msj = "Error Cargar datos de Tabla de Usuario:" + Obj.Value.ToString().Trim()
                                            + " Accion:Actualizar Error:" + Sap.oCompany.GetLastErrorDescription();
                                            return Resultado;
                                        }
                                    }                                                         
                                    break;
                                case "name":
                                    oTU.Name = Obj.Value.ToString();
                                    break;
                                default:
                                    string Valor = Obj.Key.ToString().ToLower();
                                    if (Valor.Contains("u_") == true)
                                    {
                                        string res = Sap.AsignarValorCampoUsuario(oTU.UserFields.Fields, Obj.Key.ToString(), Obj.Value.ToString());
                                        if (res != "")
                                        {
                                            Resultado.Estatus = false;
                                            Resultado.Msj = res;
                                            return Resultado;
                                        }
                                    }
                                    break;
                            }
                        }
                        if (TipoAccion == "agregar")
                        {
                            if (oTU.Add() != 0)
                            {
                                Resultado.Estatus = false;
                                Resultado.Msj = "Error de agregar a Tabla de Usuario, codigo error:" + Sap.oCompany.GetLastErrorCode()
                                    + " Descripcion error:" + Sap.oCompany.GetLastErrorDescription();  
                            }
                            else
                            {
                                Resultado.Estatus = true;
                                Resultado.Msj = "Dato agregado con exito";   
                            }
                        }
                        if (TipoAccion == "actualizar")
                        {
                            if (oTU.Update() != 0)
                            {
                                Resultado.Estatus = false;
                                Resultado.Msj = "Error de actualizar BP, codigo error:" + Sap.oCompany.GetLastErrorCode()
                                    + " Descripcion error:" + Sap.oCompany.GetLastErrorDescription();
                            }
                            else
                            {
                                Resultado.Estatus = true;
                                Resultado.Msj = "Tabla de Usuario actualizada con exito";
                            }
                        }
                        Sap.Finalizar();
                        return Resultado;
                    }
                    else
                    {
                        return Resultado;
                    }
                }
                catch (Exception Ex)
                {
                    Resultado.Estatus = false;
                    Resultado.Msj = "Error Exception:" + Ex.Message.ToString();
                    return Resultado;
                }
            }
            else
            {
                Resultado.Estatus = false;
                Resultado.Msj = "Error en datos recibidos";
                return Resultado;
            }
        }
    }
}