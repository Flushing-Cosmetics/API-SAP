using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Microsoft.CSharp;

namespace WS_SAP
{
    public class SAP_SociosNegocio
    {
        public Resultado Agregar(string Datos)
        {
            Resultado Resultado = new Resultado();
            if (Datos != null)
            {
                try
                {
                    string TipoAccion = "";
                    int Propiedad = 0;
                    SAP Sap = new SAP();
                    Resultado = Sap.Conectar();
                    if (Resultado.Estatus == true)
                    {
                        //SAPbobsCOM.BusinessPartners oBP;
                        SAPbobsCOM.BusinessPartners oBP = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                        var jObject = JObject.Parse(Datos);
                        foreach (var Obj in jObject)
                        {
                            switch (Obj.Key.ToLower())
                            {
                                case "tipoaccion":
                                    if (Obj.Value.ToString().ToLower() == "agregar")
                                    {
                                        TipoAccion = "agregar";
                                        oBP = Sap.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                                        oBP.FederalTaxID = "000000000000";
                                        oBP.SubjectToWithholdingTax = SAPbobsCOM.BoYesNoEnum.tNO;
                                    }
                                    if (Obj.Value.ToString().ToLower() == "actualizar")
                                    {
                                        TipoAccion = "actualizar";
                                    }
                                    break;
                                case "cardcode":
                                    if (TipoAccion == "agregar")
                                    {
                                        oBP.CardCode = Obj.Value.ToString();
                                    }
                                    if (TipoAccion == "actualizar")
                                    {
                                        if (oBP.GetByKey(Obj.Value.ToString().Trim()) == false)
                                        {
                                            Resultado.Estatus = false;
                                            Resultado.Msj = "Error Cargar datos de BP:" + Obj.Value.ToString().Trim()
                                            + " Accion:Actualizar Error:" + Sap.oCompany.GetLastErrorDescription();
                                            return Resultado;
                                        }
                                    }
                                    break;
                                case "groupcode": oBP.GroupCode = Convert.ToInt32(Obj.Value.ToString()); break;
                                case "cardtype":
                                    if (Obj.Value.ToString().ToLower() == "c")
                                    {
                                        oBP.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                                    }
                                    if (Obj.Value.ToString().ToLower() == "s")
                                    {
                                        oBP.CardType = SAPbobsCOM.BoCardTypes.cSupplier;
                                    }
                                    if (Obj.Value.ToString().ToLower() == "l")
                                    {
                                        oBP.CardType = SAPbobsCOM.BoCardTypes.cLid;
                                    }
                                    break;
                                case "cardname":
                                    oBP.CardName = Obj.Value.ToString();
                                    break;
                                case "cardfname":
                                    oBP.CardForeignName = Obj.Value.ToString();
                                    break;
                                case "currency":
                                    oBP.Currency = Obj.Value.ToString();
                                    break;
                                case "phone1":
                                    oBP.Phone1 = Obj.Value.ToString();
                                    break;
                                case "phone2":
                                    oBP.Phone2 = Obj.Value.ToString();
                                    break;
                                case "cellular":
                                    oBP.Cellular = Obj.Value.ToString();
                                    break;
                                case "fax":
                                    oBP.Fax = Obj.Value.ToString();
                                    break;
                                case "e-mail":
                                    oBP.EmailAddress = Obj.Value.ToString();
                                    break;
                                case "intrntsite":
                                    oBP.Website = Obj.Value.ToString();
                                    break;
                                case "validfor":
                                    oBP.Valid = Sap.ConvertirYesNo(Obj.Value.ToString());
                                    break;
                                case "validfrom":
                                    oBP.ValidFrom = Sap.ConvertirFecha(Obj.Value.ToString());
                                    break;
                                case "validto":
                                    oBP.ValidTo = Sap.ConvertirFecha(Obj.Value.ToString());
                                    break;
                                case "validcomm":
                                    oBP.ValidRemarks = Obj.Value.ToString();
                                    break;
                                case "frozenfro":
                                    oBP.Frozen = Sap.ConvertirYesNo(Obj.Value.ToString());
                                    break;
                                case "frozenfrom":
                                    oBP.FrozenFrom = Sap.ConvertirFecha(Obj.Value.ToString());
                                    break;
                                case "frozento":
                                    oBP.FrozenTo = Sap.ConvertirFecha(Obj.Value.ToString());
                                    break;
                                case "frozencomm":
                                    oBP.FrozenRemarks = Obj.Value.ToString();
                                    break;
                                case "slpcode":
                                    oBP.SalesPersonCode = Convert.ToInt32(Obj.Value.ToString());
                                    break;
                                case "groupnum":
                                    oBP.PayTermsGrpCode = Convert.ToInt32(Obj.Value.ToString());
                                    break;
                                case "listnum":
                                    oBP.PriceListNum = Convert.ToInt32(Obj.Value.ToString());
                                    break;
                                case "taxcode":
                                    oBP.VatGroupLatinAmerica = Obj.Value.ToString();
                                    break;
                                case "creditline":
                                    oBP.CreditLimit = Convert.ToDouble(Obj.Value.ToString());
                                    break;
                                case "notes":
                                    oBP.Notes = Obj.Value.ToString();
                                    break;
                                case "freetext":
                                    oBP.FreeText = Obj.Value.ToString();
                                    break;
                                case "direcciones":
                                    JArray jaDirecciones = (JArray)jObject["direcciones"];
                                    foreach (JObject ObjDirecciones in jaDirecciones)
                                    {                                        
                                        foreach (var ObjDireccion in ObjDirecciones)
                                        {                                          
                                            switch (ObjDireccion.Key.ToLower())
                                            {
                                                case "direcciones_agregar":
                                                    oBP.Addresses.Add();
                                                    break;
                                                case "direcciones_linenum":
                                                    oBP.Addresses.SetCurrentLine(Convert.ToInt32(ObjDireccion.Value.ToString()));
                                                    break;
                                                case "direcciones_address":
                                                    oBP.Addresses.AddressName = ObjDireccion.Value.ToString();
                                                    break;
                                                case "direcciones_addresstype":
                                                    if (ObjDireccion.Value.ToString().ToLower() == "b")
                                                    {
                                                        oBP.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_BillTo;
                                                    }
                                                    if (ObjDireccion.Value.ToString().ToLower() == "s")
                                                    {
                                                        oBP.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_ShipTo;
                                                    }
                                                    break;
                                                case "direcciones_street":
                                                    oBP.Addresses.Street = ObjDireccion.Value.ToString();
                                                    break;
                                                case "direcciones_block":
                                                    oBP.Addresses.Block = ObjDireccion.Value.ToString();
                                                    break;
                                                case "direcciones_city":
                                                    oBP.Addresses.City = ObjDireccion.Value.ToString();
                                                    break;
                                                case "direcciones_zipcode":
                                                    oBP.Addresses.ZipCode = ObjDireccion.Value.ToString();
                                                    break;
                                                case "direcciones_county":
                                                    oBP.Addresses.County = ObjDireccion.Value.ToString();
                                                    break;
                                                case "direcciones_country":
                                                    oBP.Addresses.Country = ObjDireccion.Value.ToString();
                                                    break;
                                                case "direcciones_state":
                                                    oBP.Addresses.State = ObjDireccion.Value.ToString();
                                                    break;
                                                case "direcciones_taxcode":
                                                    oBP.Addresses.TaxCode = ObjDireccion.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }                                        
                                    }
                                    break;
                                case "contactos":
                                    JArray jaContactos = (JArray)jObject["contactos"];
                                    foreach (JObject ObjContactos in jaContactos)
                                    {
                                        foreach (var ObjContacto in ObjContactos)
                                        {
                                            switch (ObjContacto.Key.ToLower())
                                            {
                                                case "contactos_agregar":
                                                    oBP.ContactEmployees.Add();
                                                    break;
                                                case "contactos_cntctcode":
                                                    oBP.ContactEmployees.SetCurrentLine(Convert.ToInt32(ObjContacto.Value.ToString()));
                                                    break;
                                                case "contactos_name":
                                                    oBP.ContactEmployees.Name = ObjContacto.Value.ToString();
                                                    break;
                                                case "contactos_title":
                                                    oBP.ContactEmployees.Title = ObjContacto.Value.ToString();
                                                    break;
                                                case "contactos_position":
                                                    oBP.ContactEmployees.Position = ObjContacto.Value.ToString();
                                                    break;
                                                case "contactos_address":
                                                    oBP.ContactEmployees.Address = ObjContacto.Value.ToString();
                                                    break;
                                                case "contactos_tel1":
                                                    oBP.ContactEmployees.Phone1 = ObjContacto.Value.ToString();
                                                    break;
                                                case "contactos_tel2":
                                                    oBP.ContactEmployees.Phone2 = ObjContacto.Value.ToString();
                                                    break;
                                                case "contactos_cellolar":
                                                    oBP.ContactEmployees.MobilePhone = ObjContacto.Value.ToString();
                                                    break;
                                                case "contactos_email":
                                                    oBP.ContactEmployees.E_Mail = ObjContacto.Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    string Valor = Obj.Key.ToString().ToLower();
                                    if (Valor.StartsWith("propiedad") == true)
                                    {
                                        Propiedad = Convert.ToInt16(Valor.Substring(9));
                                        oBP.Properties[Propiedad] = Sap.ConvertirYesNo(Obj.Value.ToString());
                                    }
                                    if (Valor.Contains("u_") == true)
                                    {
                                        string res = Sap.AsignarValorCampoUsuario(oBP.UserFields.Fields, Obj.Key.ToString(), Obj.Value.ToString());
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
                            if (oBP.Add() != 0)
                            {
                                Resultado.Estatus = false;
                                Resultado.Msj = "Error de agregar BP, codigo error:" + Sap.oCompany.GetLastErrorCode()
                                    + " Descripcion error:" + Sap.oCompany.GetLastErrorDescription();
                            }
                            else
                            {
                                Resultado.Estatus = true;
                                Resultado.Msj = "BP agregado con exito ";
                            }
                        }
                        if (TipoAccion == "actualizar")
                        {
                            if (oBP.Update() != 0)
                            {
                                Resultado.Estatus = false;
                                Resultado.Msj = "Error de actualizar BP, codigo error:" + Sap.oCompany.GetLastErrorCode()
                                    + " Descripcion error:" + Sap.oCompany.GetLastErrorDescription();
                            }
                            else
                            {
                                Resultado.Estatus = true;
                                Resultado.Msj = "BP actualizado con exito";
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