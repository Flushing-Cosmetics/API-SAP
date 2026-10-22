using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WS_SAP
{
    internal class SAP_Pagos
    {
        internal Resultado Pagos(string db, string Datos)
        {
            Resultado Resultado = new Resultado();
            if (Datos != null)
            {
                try
                {
                    string TipoAccion = "";
                    SAP Sap = new SAP();
                    Resultado = Sap.Conectar(db: db);
                    if (Resultado.Estatus == true)
                    {
                        SAPbobsCOM.Payments oPago;
                        string DocEntry = "";
                        int NumeroLinea = 0;
                        int NumeroLineaCheque = 0;
                        int NumeroLineaTarjeta = 0;
                        oPago = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
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
                                            oPago.GetByKey(Convert.ToInt32(DocEntry));
                                        }
                                    }
                                    break;
                                case "cardcode":
                                    if (TipoAccion == "agregar")
                                    {
                                        oPago.CardCode = Obj.Value.ToString();
                                    }
                                    break;
                                case "cardname":
                                    if (TipoAccion == "agregar")
                                    {
                                        oPago.CardName = Obj.Value.ToString();
                                    }
                                    break;
                                case "address":
                                    oPago.Address = Obj.Value.ToString();
                                    break;
                                case "cntctcode":
                                    oPago.ContactPersonCode = Convert.ToInt32(Obj.Value.ToString());
                                    break;
                                case "series":
                                    if (TipoAccion == "agregar")
                                    {
                                        oPago.Series = Convert.ToInt32(Obj.Value.ToString());
                                    }
                                    break;
                                case "doctype":
                                    switch (Obj.Value.ToString().ToLower())
                                    {
                                        case "c":
                                            oPago.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;
                                            oPago.DocTypte = SAPbobsCOM.BoRcptTypes.rCustomer;
                                            break;
                                        case "s":
                                            oPago.DocType = SAPbobsCOM.BoRcptTypes.rSupplier;
                                            oPago.DocTypte = SAPbobsCOM.BoRcptTypes.rSupplier;
                                            break;
                                        case "a":
                                            oPago.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
                                            oPago.DocTypte = SAPbobsCOM.BoRcptTypes.rAccount;
                                            break;
                                    }
                                    break;
                                case "docdate":
                                    if (TipoAccion == "agregar")
                                    {
                                        oPago.DocDate = Sap.ConvertirFecha(Obj.Value.ToString());
                                    }
                                    break;
                                case "docduedate":
                                    oPago.DueDate = Sap.ConvertirFecha(Obj.Value.ToString());
                                    break;
                                case "taxdate":
                                    oPago.TaxDate = Sap.ConvertirFecha(Obj.Value.ToString());
                                    break;
                                case "counterref":
                                    oPago.CounterReference = Obj.Value.ToString();
                                    break;
                                case "prjcode":
                                    oPago.ProjectCode = Obj.Value.ToString();
                                    break;
                                case "doccur":
                                    if (TipoAccion == "agregar")
                                    {
                                        oPago.DocCurrency = Obj.Value.ToString();
                                    }
                                    break;
                                case "docrate":
                                    oPago.DocRate = Convert.ToDouble(Obj.Value.ToString());
                                    break;
                                case "comments":
                                    oPago.Remarks = Obj.Value.ToString();
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
                                                            NumeroLinea += 1;
                                                            oPago.Invoices.Add();
                                                        }
                                                    }
                                                    else if (TipoAccion == "actualizar")
                                                    {
                                                        oPago.Invoices.Add();
                                                    }
                                                    break;
                                                case "actualizar":
                                                    oPago.Invoices.SetCurrentLine(Convert.ToInt32(ObjLinea.Value.ToString()));
                                                    break;
                                                case "docentry":
                                                    oPago.Invoices.DocEntry = Convert.ToInt32(ObjLinea.Value.ToString());
                                                    oPago.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                                                    break;
                                                case "sumapplied":
                                                    oPago.Invoices.SumApplied = Convert.ToDouble(ObjLinea.Value.ToString());
                                                    break;
                                                case "dcount":
                                                    oPago.Invoices.DiscountPercent = Convert.ToDouble(ObjLinea.Value.ToString());
                                                    break;

                                            }
                                        }
                                    }
                                    break;
                                //Campos de Pago efectivo
                                case "efectivo":
                                    JArray jaPagoEfectivo = (JArray)jObject["efectivo"];
                                    foreach (JObject ObjPagoEfectivo in jaPagoEfectivo)
                                    {
                                        foreach (var ObjEfectivo in ObjPagoEfectivo)
                                        {
                                            switch (ObjEfectivo.Key.ToLower())
                                            {
                                                case "cashsum":
                                                    oPago.CashSum = Convert.ToDouble(ObjEfectivo.Value.ToString());
                                                    break;
                                                case "cashacct":
                                                    oPago.CashAccount = ObjEfectivo.Value.ToString();
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                // Campos de Pago en cheque
                                case "cheque":
                                    JArray jaPagoCheque = (JArray)jObject["cheque"];
                                    foreach (JObject ObjPagoCheque in jaPagoCheque)
                                    {
                                        foreach (var ObjCheque in ObjPagoCheque)
                                        {
                                            switch (ObjCheque.Key.ToLower())
                                            {
                                                case "agregar":
                                                    if (TipoAccion == "agregar")
                                                    {
                                                        if (NumeroLineaCheque == 0)
                                                        {
                                                            NumeroLineaCheque += 1;
                                                        }
                                                        else
                                                        {
                                                            oPago.Checks.Add();
                                                        }
                                                    }
                                                    else if (TipoAccion == "actualizar")
                                                    {
                                                        oPago.Checks.Add();
                                                    }
                                                    break;
                                                case "actualizar":
                                                    oPago.Checks.SetCurrentLine(Convert.ToInt32(ObjCheque.Value.ToString()));
                                                    break;
                                                case "checksum":
                                                    oPago.Checks.CheckSum = Convert.ToDouble(ObjCheque.Value.ToString());
                                                    break;
                                                case "acctnum":
                                                    oPago.Checks.CheckAccount = ObjCheque.Value.ToString();
                                                    break;
                                                case "duedate":
                                                    oPago.Checks.DueDate = Sap.ConvertirFecha(ObjCheque.Value.ToString());
                                                    break;
                                                case "checknum":
                                                    oPago.Checks.CheckNumber = Convert.ToInt32(ObjCheque.Value.ToString());
                                                    break;
                                                case "bankcode":
                                                    oPago.Checks.CheckNumber = Convert.ToInt32(ObjCheque.Value.ToString());
                                                    break;
                                                case "countrycode":
                                                    oPago.Checks.CountryCode = ObjCheque.Value.ToString();
                                                    break;
                                                case "trnsfrable":
                                                    oPago.Checks.Trnsfrable = Sap.ConvertirYesNo(ObjCheque.Value.ToString());
                                                    break;
                                                default:
                                                    string ValorCheque = ObjCheque.Key.ToString().ToLower();
                                                    if (ValorCheque.Contains("u_") == true)
                                                    {
                                                        string res = Sap.AsignarValorCampoUsuario(oPago.Checks.UserFields.Fields, ObjCheque.Key.ToString(), ObjCheque.Value.ToString());
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
                                //campos de pago en transferencia o deposito
                                case "transferencia":
                                    JArray jaPagoDeposito = (JArray)jObject["transferencia"];
                                    foreach (JObject ObjPagoDeposito in jaPagoDeposito)
                                    {
                                        foreach (var ObjDeposito in ObjPagoDeposito)
                                        {
                                            switch (ObjDeposito.Key.ToLower())
                                            {
                                                case "trsfracct":
                                                    oPago.TransferAccount = ObjDeposito.Value.ToString();
                                                    break;
                                                case "trsfrdate":
                                                    oPago.TransferDate = Sap.ConvertirFecha(ObjDeposito.Value.ToString());
                                                    break;
                                                case "trsfrref":
                                                    oPago.TransferReference = ObjDeposito.Value.ToString();
                                                    break;
                                                case "trsfrsum":
                                                    oPago.TransferSum = Convert.ToDouble(ObjDeposito.Value.ToString());
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                //Campos de pago con tarjeta
                                case "tarjetacredito":
                                    JArray jaPagoTarjeta = (JArray)jObject["tarjetacredito"];
                                    foreach (JObject ObjPagoTarjeta in jaPagoTarjeta)
                                    {
                                        foreach (var ObjTarjeta in ObjPagoTarjeta)
                                        {
                                            switch (ObjTarjeta.Key.ToLower())
                                            {
                                                case "agregar":
                                                    if (TipoAccion == "agregar")
                                                    {
                                                        if (NumeroLineaTarjeta == 0)
                                                        {
                                                            NumeroLineaTarjeta += 1;
                                                        }
                                                        else
                                                        {
                                                            oPago.CreditCards.Add();
                                                        }
                                                    }
                                                    else if (TipoAccion == "actualizar")
                                                    {
                                                        oPago.CreditCards.Add();
                                                    }
                                                    break;
                                                case "actualizar":
                                                    oPago.CreditCards.SetCurrentLine(Convert.ToInt32(ObjTarjeta.Value.ToString()));
                                                    break;
                                                case "creditcard":
                                                    oPago.CreditCards.CreditCard = Convert.ToInt32(ObjTarjeta.Value.ToString());                                                    
                                                    oPago.CreditCards.CreditType = SAPbobsCOM.BoRcptCredTypes.cr_Regular;
                                                    break;
                                                case "cardvalid":
                                                    oPago.CreditCards.CardValidUntil = Sap.ConvertirFecha(ObjTarjeta.Value.ToString());
                                                    break;
                                                case "crcardnum":
                                                    oPago.CreditCards.CreditCardNumber = ObjTarjeta.Value.ToString();
                                                    break;
                                                case "owneridnum":
                                                    oPago.CreditCards.OwnerIdNum = ObjTarjeta.Value.ToString();
                                                    break;
                                                case "ownerphone":
                                                    oPago.CreditCards.OwnerPhone = ObjTarjeta.Value.ToString();
                                                    break;
                                                case "crtypecode":
                                                    oPago.CreditCards.PaymentMethodCode = Convert.ToInt32(ObjTarjeta.Value.ToString());
                                                    break;
                                                case "creditsum":
                                                    oPago.CreditCards.CreditSum = Convert.ToDouble(ObjTarjeta.Value.ToString());
                                                    break;
                                                case "numofpmnts":
                                                    oPago.CreditCards.NumOfPayments = Convert.ToInt32(ObjTarjeta.Value.ToString());
                                                    oPago.CreditCards.NumOfCreditPayments = Convert.ToInt32(ObjTarjeta.Value.ToString());
                                                    break;
                                                case "firstsum":
                                                    oPago.CreditCards.FirstPaymentSum = Convert.ToDouble(ObjTarjeta.Value.ToString());
                                                    break;
                                                case "addpmntsum":
                                                    oPago.CreditCards.AdditionalPaymentSum = Convert.ToDouble(ObjTarjeta.Value.ToString());
                                                    break;
                                                case "vouchernum":
                                                    oPago.CreditCards.VoucherNum = ObjTarjeta.Value.ToString();
                                                    break;
                                                case "confnum":
                                                    oPago.CreditCards.ConfirmationNum = ObjTarjeta.Value.ToString();
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    string Valor = Obj.Key.ToString().ToLower();
                                    if (Valor.Contains("u_") == true)
                                    {
                                        string res = Sap.AsignarValorCampoUsuario(oPago.UserFields.Fields, Obj.Key.ToString(), Obj.Value.ToString());
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
                        string NuevoDocEntry = "";
                        List<KeyValuePair<string, string>> Data = new List<KeyValuePair<string, string>>();
                        switch (TipoAccion)
                        {
                            case "agregar":
                                if (oPago.Add() != 0)
                                {
                                    Resultado.Estatus = false;
                                    Resultado.Msj = "Error al crear documento[Pago]" + Sap.oCompany.GetLastErrorDescription();
                                }
                                else
                                {
                                    NuevoDocEntry = Sap.oCompany.GetNewObjectKey().ToString();
                                    oPago.GetByKey(Convert.ToInt32(NuevoDocEntry));
                                    Resultado.Estatus = true;
                                    Resultado.Msj = "Se creo con exito el documento[Pago] DocNum:" + oPago.DocNum.ToString();
                                    Data.Add(new KeyValuePair<string,string>("DocEntry", NuevoDocEntry));
                                    Data.Add(new KeyValuePair<string, string>("DocNum", oPago.DocNum.ToString()));                                    
                                    Resultado.Datos = Data;
                                }
                                break;
                            case "actualizar":
                                if (Convert.ToInt32(DocEntry) > 0)
                                {
                                    if (oPago.Update() != 0)
                                    {
                                        Resultado.Estatus = false;
                                        Resultado.Msj = "Error al actualizar documento[Pago]" + Sap.oCompany.GetLastErrorDescription();
                                    }
                                    else
                                    {
                                        Resultado.Estatus = true;
                                        Resultado.Msj = "Se actualizo con exito el documento[Pago]";
                                        Data.Add(new KeyValuePair<string, string>("DocEntry", DocEntry));
                                        Data.Add(new KeyValuePair<string, string>("DocNum", oPago.DocNum.ToString()));                                        
                                        Resultado.Datos = Data;
                                    }
                                }
                                else
                                {
                                    Resultado.Estatus = false;
                                    Resultado.Msj = "Error docentry debe ser valido";
                                }
                                break;
                            case "cancelar":
                                if (Convert.ToInt32(DocEntry) > 0)
                                {
                                    if (oPago.Cancel() != 0)
                                    {
                                        Resultado.Estatus = false;
                                        Resultado.Msj = "Error al cancelar documento[Pago]" + Sap.oCompany.GetLastErrorDescription();
                                    }
                                    else
                                    {
                                        Resultado.Estatus = true;
                                        Resultado.Msj = "Se cancelo con exito el documento[Pago]";
                                        Data.Add(new KeyValuePair<string, string>("DocEntry", DocEntry));
                                        Data.Add(new KeyValuePair<string, string>("DocNum", oPago.DocNum.ToString()));
                                        Resultado.Datos = Data;
                                    }
                                }
                                else
                                {
                                    Resultado.Estatus = false;
                                    Resultado.Msj = "Error docentry debe ser valido";
                                }
                                break;
                            case "cerrar":
                                if (Convert.ToInt32(DocEntry) > 0)
                                {
                                    if (oPago.Close() != 0)
                                    {
                                        Resultado.Estatus = false;
                                        Resultado.Msj = "Error al cerrar Documento[Pago]" + Sap.oCompany.GetLastErrorDescription();
                                    }
                                    else
                                    {
                                        Resultado.Estatus = true;
                                        Resultado.Msj = "Se cerro con exito el Documento[Pago]";
                                        Data.Add(new KeyValuePair<string, string>("DocEntry", DocEntry));
                                        Data.Add(new KeyValuePair<string, string>("DocNum", oPago.DocNum.ToString()));
                                        Resultado.Datos = Data;
                                    }
                                }
                                else
                                {
                                    Resultado.Estatus = false;
                                    Resultado.Msj = "Error docentry debe ser valido";
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
                    Resultado.Msj = "Error exception:" + Ex.Message.ToString();
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