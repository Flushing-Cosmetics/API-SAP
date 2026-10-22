using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS_SAP
{
    public class SAP_Documentos
    {
        public Resultado Documento(string Datos, string Tipo)
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
                        SAPbobsCOM.Documents oDocumento;
                        SAPbobsCOM.Documents oDocumentoResultado;
                        string DocEntry = "";
                        int NumeroLinea = 0;
                        int NumeroLineaGastoAdicional = 0;
                        switch (Tipo.ToLower())
                        {
                            case "ofv":
                                oDocumento = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oQuotations);
                                break;
                            case "ov":
                                oDocumento = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                                break;
                            case "fac":
                                oDocumento = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                                break;
                            default:
                                Resultado.Estatus = false;
                                Resultado.Msj = "No se selecciono ningun documento de SAP";
                                return Resultado;
                        }
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
                                    if (Obj.Value.ToString().ToLower() == "cerrar")
                                    {
                                        TipoAccion = "cerrar";
                                    }
                                    if (Obj.Value.ToString().ToLower() == "cancelar")
                                    {
                                        TipoAccion = "cancelar";
                                    }
                                    break;
                                //Campos de Encabezado de Documento
                                case "docentry":
                                    DocEntry = Obj.Value.ToString();
                                    if (TipoAccion == "actualizar" || TipoAccion == "cerrar" || TipoAccion == "cancelar")
                                    {
                                        if (Convert.ToInt32(DocEntry) > 0)
                                        {
                                            oDocumento.GetByKey(Convert.ToInt32(DocEntry));
                                        }
                                    }
                                    break;
                                case "cardcode":
                                    if (TipoAccion == "agregar")
                                    {
                                        oDocumento.CardCode = Obj.Value.ToString();
                                        oDocumento.FederalTaxID = "000000000000";
                                    }
                                    break;
                                case "serie":
                                    if (TipoAccion == "agregar")
                                    {
                                        oDocumento.Series = Convert.ToInt32(Obj.Value.ToString());
                                    }
                                    break;
                                case "doctype":
                                    if (Obj.Value.ToString().ToLower() == "s")
                                    {
                                        oDocumento.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;
                                    }
                                    else
                                    {
                                        oDocumento.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;
                                    }
                                    break;
                                case "docdate":
                                    if (TipoAccion == "agregar")
                                    {
                                        oDocumento.DocDate = Sap.ConvertirFecha(Obj.Value.ToString());
                                    }
                                    break;
                                case "docduedate":
                                    oDocumento.DocDueDate = Sap.ConvertirFecha(Obj.Value.ToString());
                                    break;
                                case "reqdate":
                                    oDocumento.RequriedDate = Sap.ConvertirFecha(Obj.Value.ToString());
                                    break;
                                case "taxdate":
                                    oDocumento.TaxDate = Sap.ConvertirFecha(Obj.Value.ToString());
                                    break;
                                case "numatcard":
                                    oDocumento.NumAtCard = Obj.Value.ToString();
                                    break;
                                case "project":
                                    oDocumento.Project = Obj.Value.ToString();
                                    break;
                                case "discprcnt":
                                    oDocumento.DiscountPercent = Convert.ToDouble(Obj.Value.ToString());
                                    break;
                                case "doccur":
                                    if (TipoAccion == "agregar")
                                    {
                                        oDocumento.DocCurrency = Obj.Value.ToString();
                                    }
                                    break;
                                case "docrate":
                                    oDocumento.DocRate = Convert.ToDouble(Obj.Value.ToString());
                                    break;
                                case "slpcode":
                                    oDocumento.SalesPersonCode = Convert.ToInt32(Obj.Value.ToString());
                                    break;
                                case "comments":
                                    oDocumento.Comments = Obj.Value.ToString();
                                    break;
                                //Campos de Lineas
                                case "lineas":
                                    JArray jaLineas = (JArray)jObject["lineas"];
                                    foreach (JObject ObjLineas in jaLineas)
                                    {
                                        foreach (var ObjLinea in ObjLineas)
                                        {
                                            switch (ObjLinea.Key.ToLower())
                                            {
                                                case "agregar":
                                                    if (TipoAccion == "agregar")
                                                    {
                                                        if (NumeroLinea == 0)
                                                        {
                                                            NumeroLinea += 1;
                                                        }
                                                        else
                                                        {
                                                            oDocumento.Lines.Add();
                                                        }
                                                    }
                                                    else if (TipoAccion == "actualizar")
                                                    {
                                                        oDocumento.Lines.Add();
                                                    }
                                                    break;
                                                case "actualizar":
                                                    oDocumento.Lines.SetCurrentLine(Convert.ToInt32(ObjLinea.Value.ToString()));
                                                    break;
                                                case "eliminar":
                                                    oDocumento.Lines.SetCurrentLine(Convert.ToInt32(ObjLinea.Value.ToString()));
                                                    oDocumento.Lines.Delete();
                                                    break;
                                                case "itemcode":
                                                    oDocumento.Lines.ItemCode = ObjLinea.Value.ToString();
                                                    break;
                                                case "dscription":
                                                    oDocumento.Lines.ItemDescription = ObjLinea.Value.ToString();
                                                    break;
                                                case "acctcode":
                                                    oDocumento.Lines.AccountCode = ObjLinea.Value.ToString();
                                                    break;
                                                case "basetype":
                                                    oDocumento.Lines.BaseType = Convert.ToInt32(ObjLinea.Value.ToString());
                                                    break;
                                                case "baseentry":
                                                    oDocumento.Lines.BaseEntry = Convert.ToInt32(ObjLinea.Value.ToString());
                                                    break;
                                                case "baseeline":
                                                    oDocumento.Lines.BaseLine = Convert.ToInt32(ObjLinea.Value.ToString());
                                                    break;
                                                case "quantity":
                                                    oDocumento.Lines.Quantity = Convert.ToDouble(ObjLinea.Value.ToString());
                                                    break;
                                                case "price":
                                                    oDocumento.Lines.Price = Convert.ToDouble(ObjLinea.Value.ToString());
                                                    break;
                                                case "priceaftervat":
                                                    oDocumento.Lines.PriceAfterVAT = Convert.ToDouble(ObjLinea.Value.ToString());
                                                    break;
                                                case "discprcnt":
                                                    oDocumento.Lines.DiscountPercent = Convert.ToDouble(ObjLinea.Value.ToString());
                                                    break;
                                                case "currency":
                                                    oDocumento.Lines.Currency = ObjLinea.Value.ToString();
                                                    break;
                                                case "rate":
                                                    oDocumento.Lines.Rate = Convert.ToDouble(ObjLinea.Value.ToString());
                                                    break;
                                                case "project":
                                                    oDocumento.Lines.ProjectCode = ObjLinea.Value.ToString();
                                                    break;
                                                case "ocrcode":
                                                    oDocumento.Lines.CostingCode = ObjLinea.Value.ToString();
                                                    break;
                                                case "taxcode":
                                                    oDocumento.Lines.TaxCode = ObjLinea.Value.ToString();
                                                    break;
                                                case "whscode":
                                                    oDocumento.Lines.WarehouseCode = ObjLinea.Value.ToString();
                                                    break;
                                                case "shipdate":
                                                    oDocumento.Lines.ShipDate = Sap.ConvertirFecha(ObjLinea.Value.ToString());
                                                    break;
                                                default:
                                                    string LineasValor = ObjLinea.Key.ToString().ToLower();
                                                    if (LineasValor.Contains("lineas_u_") == true)
                                                    {
                                                        string res = Sap.AsignarValorCampoUsuario(oDocumento.Lines.UserFields.Fields, ObjLinea.Key.ToString(), ObjLinea.Value.ToString());
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
                                    }
                                    break;
                                case "gastosadicionales":
                                    JArray jaGastosAdicionales = (JArray)jObject["gastosadicionales"];
                                    foreach (JObject ObjGastosAdicionales in jaGastosAdicionales)
                                    {
                                        foreach (var ObjGastoAdicional in ObjGastosAdicionales)
                                        {
                                            switch (ObjGastoAdicional.Key.ToLower())
                                            {
                                                case "agregar":
                                                    if (TipoAccion == "agregar")
                                                    {
                                                        if (NumeroLineaGastoAdicional == 0)
                                                        {
                                                            NumeroLineaGastoAdicional += 1;
                                                        }
                                                        else
                                                        {
                                                            oDocumento.Expenses.Add();
                                                        }
                                                    }
                                                    else if (TipoAccion == "actualizar")
                                                    {
                                                        oDocumento.Expenses.Add();
                                                    }
                                                    break;
                                                case "actualizar":
                                                    oDocumento.Expenses.SetCurrentLine(Convert.ToInt32(ObjGastoAdicional.Value.ToString()));
                                                    break;
                                                case "expnscode":
                                                    oDocumento.Expenses.ExpenseCode = Convert.ToInt32(ObjGastoAdicional.Value.ToString());
                                                    break;
                                                case "comments":
                                                    oDocumento.Expenses.Remarks = ObjGastoAdicional.Value.ToString();
                                                    break;
                                                case "distrbmthd":
                                                    switch (ObjGastoAdicional.Value.ToString().ToLower())
                                                    {
                                                        case "n":
                                                            oDocumento.Expenses.DistributionMethod = SAPbobsCOM.BoAdEpnsDistribMethods.aedm_None;
                                                            break;
                                                        case "q":
                                                            oDocumento.Expenses.DistributionMethod = SAPbobsCOM.BoAdEpnsDistribMethods.aedm_Quantity;
                                                            break;
                                                        case "v":
                                                            oDocumento.Expenses.DistributionMethod = SAPbobsCOM.BoAdEpnsDistribMethods.aedm_Volume;
                                                            break;
                                                        case "w":
                                                            oDocumento.Expenses.DistributionMethod = SAPbobsCOM.BoAdEpnsDistribMethods.aedm_Weight;
                                                            break;
                                                        case "e":
                                                            oDocumento.Expenses.DistributionMethod = SAPbobsCOM.BoAdEpnsDistribMethods.aedm_Equally;
                                                            break;
                                                        case "t":
                                                            oDocumento.Expenses.DistributionMethod = SAPbobsCOM.BoAdEpnsDistribMethods.aedm_RowTotal;
                                                            break;
                                                    }
                                                    break;
                                                case "taxcode":
                                                    oDocumento.Expenses.TaxCode = ObjGastoAdicional.Value.ToString();
                                                    break;
                                                case "ocrcode":
                                                    oDocumento.Expenses.DistributionRule = ObjGastoAdicional.Value.ToString();
                                                    break;
                                                default:
                                                    string GastoAdicionalValor = Obj.Key.ToString().ToLower();
                                                    if (GastoAdicionalValor.Contains("gastoadicional_u_") == true)
                                                    {
                                                        string res = Sap.AsignarValorCampoUsuario(oDocumento.Expenses.UserFields.Fields, ObjGastoAdicional.Key.ToString(), ObjGastoAdicional.Value.ToString());
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
                                    }
                                    break;
                                default:
                                    string Valor = Obj.Key.ToString().ToLower();
                                    if (Valor.Contains("u_") == true)
                                    {
                                        string res = Sap.AsignarValorCampoUsuario(oDocumento.UserFields.Fields, Obj.Key.ToString(), Obj.Value.ToString());
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
                        switch (TipoAccion)
                        {
                            case "agregar":
                                if (Convert.ToInt32(DocEntry) > 0)
                                {
                                    if (oDocumento.Add() != 0)
                                    {
                                        Resultado.Estatus = false;
                                        Resultado.Msj = "Error al crear Documento[" + Tipo + "]" + Sap.oCompany.GetLastErrorDescription();
                                    }
                                    else
                                    {
                                        switch (Tipo.ToLower())
                                        {
                                            case "ofc":
                                                oDocumentoResultado = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oQuotations);
                                                oDocumentoResultado.GetByKey(Convert.ToInt32(Sap.oCompany.GetNewObjectKey()));
                                                Resultado.Estatus = true;
                                                Resultado.Msj = "Se creo con exito el Documento[" + Tipo + "] DocNum:" + oDocumentoResultado.DocNum.ToString();
                                                Resultado.Datos.Add("DocEntry", Sap.oCompany.GetNewObjectKey().ToString());
                                                Resultado.Datos.Add("DocNum", oDocumentoResultado.DocNum.ToString());
                                                break;
                                            case "ov":
                                                oDocumentoResultado = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                                                oDocumentoResultado.GetByKey(Convert.ToInt32(Sap.oCompany.GetNewObjectKey()));
                                                Resultado.Estatus = true;
                                                Resultado.Msj = "Se creo con exito el Documento[" + Tipo + "] DocNum:" + oDocumentoResultado.DocNum.ToString();
                                                Resultado.Datos.Add("DocEntry", Sap.oCompany.GetNewObjectKey().ToString());
                                                Resultado.Datos.Add("DocNum", oDocumentoResultado.DocNum.ToString());
                                                break;
                                            case "fac":
                                                oDocumentoResultado = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                                                oDocumentoResultado.GetByKey(Convert.ToInt32(Sap.oCompany.GetNewObjectKey()));
                                                Resultado.Estatus = true;
                                                Resultado.Msj = "Se creo con exito el Documento[" + Tipo + "] DocNum:" + oDocumentoResultado.DocNum.ToString();
                                                Resultado.Datos.Add("DocEntry", Sap.oCompany.GetNewObjectKey().ToString());
                                                Resultado.Datos.Add("DocNum", oDocumentoResultado.DocNum.ToString());
                                                break;
                                        }
                                    }
                                }
                                break;
                            case "actualizar":
                                if (oDocumento.Update() != 0)
                                {
                                    Resultado.Estatus = false;
                                    Resultado.Msj = "Error al actualizar Documento[" + Tipo + "]" + Sap.oCompany.GetLastErrorDescription();
                                }
                                else
                                {
                                    Resultado.Estatus = true;
                                    Resultado.Msj = "Se actualizo con exito el Documento[" + Tipo + "]";
                                    Resultado.Datos.Add("DocEntry", Sap.oCompany.GetNewObjectKey().ToString());
                                    Resultado.Datos.Add("DocNum", oDocumento.DocNum.ToString());
                                }
                                break;
                            case "cancel":
                                if (Convert.ToInt32(DocEntry) > 0)
                                {
                                    if (oDocumento.Cancel() != 0)
                                    {
                                        Resultado.Estatus = false;
                                        Resultado.Msj = "Error Cancelar Documento[" + Tipo + "]" + Sap.oCompany.GetLastErrorDescription();
                                    }
                                    else
                                    {
                                        Resultado.Estatus = true;
                                        Resultado.Msj = "Se Cancelo con exito el Documento[" + Tipo + "]";
                                        Resultado.Datos.Add("DocNum", oDocumento.DocNum.ToString());
                                    }
                                }
                                break;
                            case "cerrar":
                                if (Convert.ToInt32(DocEntry) > 0)
                                {
                                    if (oDocumento.Close() != 0)
                                    {
                                        Resultado.Estatus = false;
                                        Resultado.Msj = "Error al cerrar Documento[" + Tipo + "]" + Sap.oCompany.GetLastErrorDescription();
                                    }
                                    else
                                    {
                                        Resultado.Estatus = true;
                                        Resultado.Msj = "Se cerro con exito el Documento[" + Tipo + "]";
                                        Resultado.Datos.Add("DocNum", oDocumento.DocNum.ToString());
                                    }
                                }
                                break;
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