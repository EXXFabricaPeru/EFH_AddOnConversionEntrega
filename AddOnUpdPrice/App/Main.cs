using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using AddOnUpdPrice.DB_Structure;
using AddOnUpdPrice.Functionality;

namespace AddOnUpdPrice.App
{
    public class Main
    {
        public Main()
        {
            //Globals.ReadTextFile("ini");
            Connect.SetApplication();
            Connect.ConnectToCompany();
            //Globals.ReadTextFile2();
            Globals.SAPVersion = Globals.oCompany.Version;
            Globals.SBO_Application.SetStatusBarMessage("Validando estructura de la Base de Datos", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            //if (Globals.IsHana() == true) Globals.RunQuery(AddOnBPC.Properties.Resources.HanaSari.ToString());
            //else Globals.RunQuery(AddOnBPC.Properties.Resources.SQLSari.ToString());
            //Globals.Addon = Globals.oRec.Fields.Item(0).Value.ToString();
            //Globals.version = Globals.oRec.Fields.Item(1).Value.ToString();
            //Globals.Release(Globals.oRec);
            #region Revisa Versión Cloud
            //if (Globals.Addon == "")
            //{
            Globals.Addon = Assembly.GetEntryAssembly().GetName().Name;
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            Globals.version = version.ToString().Replace(".0.0", "");
            //}
            #endregion
            #region Estructura
            Setup oSetup = new Setup();
            Globals.Actual = oSetup.validarVersion(Globals.Addon, Globals.version);
            if (Globals.Actual == false)
            {
                CreateStructure.CreateStruct();
                oSetup.confirmarVersion(Globals.Addon, Globals.version);
                oSetup.confirmarVersionUpdate(Globals.Addon, Globals.version);
                Globals.continuar = 0;
            }
            else
                Globals.continuar = 0;
            #endregion
            #region I kill you >:(
            //System.Diagnostics.Process procesoActual = System.Diagnostics.Process.GetCurrentProcess();
            //oSetup.validarInstancias(procesoActual).ToString();
            //oSetup.cerrarInstancias(procesoActual);
            #endregion
            Connect.SetFilters();
            //ParametrosIniciales.ObtenerParametros();
            Globals.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            Globals.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
            Globals.SBO_Application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(SBO_Application_MenuEvent);
            Globals.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEvent);
            Menu.AddMenuItems();
            Globals.SBO_Application.StatusBar.SetText("El Add-On Update Price Lists está conectado.", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
        }

        private void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            if (EventType == SAPbouiCOM.BoAppEventTypes.aet_ShutDown)
            {
                System.Windows.Forms.Application.Exit();
            }
            if (EventType == SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged)
            {
                System.Windows.Forms.Application.Exit();
            }
            if (EventType == SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged)
            {
                System.Windows.Forms.Application.Exit();
            }
            if (EventType == SAPbouiCOM.BoAppEventTypes.aet_FontChanged)
            {
                System.Windows.Forms.Application.Exit();
            }
            if (EventType == SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition)
            {
                System.Windows.Forms.Application.Exit();
            }
        }

        private void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            SAPbouiCOM.MenuItem menu = null;
            SAPbouiCOM.Form oForm = null;
            BubbleEvent = true;

            try
            {

                if (pVal.BeforeAction == true)
                {
                    //menu = Globals.SBO_Application.Menus.Item("47616");
                    switch (pVal.MenuUID)
                    {
                        #region SBAControlComp
                        case "EXX_AUPP1":
                            PriceLists.LoadForm();
                            break;
                        case "EXX_AUPP0":
                            PriceLists.LoadFormEstr();
                            break;
                        default:
                            break;

                            #endregion
                    }
                }
                else
                {
                    switch (pVal.MenuUID)
                    {
                        case "1282":
                            if (Globals.SBO_Application.Forms.ActiveForm.TypeEx == "UDO_FT_EXX_AUPP_LSPR")
                                PriceLists.OnClickAddMenu(Globals.SBO_Application.Forms.ActiveForm);
                            break;
                        case "EXX_AUPP_LSPR_Add_Line":
                            PriceLists.MostrarCantidadRegistros(Globals.SBO_Application.Forms.ActiveForm);
                            break;
                        case "EXX_AUPP_LSPR_Remove_Line":
                            PriceLists.MostrarCantidadRegistros(Globals.SBO_Application.Forms.ActiveForm);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                BubbleEvent = false;
                if (ex.Message.IndexOf("Form - Not found  [66000-9]") != -1)
                {
                    Globals.Error = "SYP: Activar campos de usuario al crear un documento";
                    Globals.SBO_Application.SetStatusBarMessage(Globals.Error, SAPbouiCOM.BoMessageTime.bmt_Short, true);
                }
                else
                {
                    Globals.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
                }
            }
            finally
            {
                GC.Collect();
            }
        }

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //if (pVal.FormTypeEx != "0")
            //{
            try
            {
                SAPbouiCOM.Form oForm = Globals.SBO_Application.Forms.Item(pVal.FormUID);

                if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED & pVal.FormTypeEx == Globals.fmrUdoListaPrecios)
                {
                    PriceLists.Actions(oForm, pVal);
                }
                if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED & pVal.FormTypeEx == Globals.fmrUdoEstructuras)
                {
                    PriceLists.Actions(oForm, pVal);
                }
                if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED & pVal.FormTypeEx == "155")
                {
                    PriceLists.Actions(oForm, pVal);
                }
                if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_VALIDATE && pVal.FormTypeEx == Globals.fmrUdoListaPrecios)
                {
                    BubbleEvent = PriceLists.ValidaCodigoMoneda(pVal, Globals.SBO_Application.Forms.Item(pVal.FormUID));
                }
                if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST && pVal.FormTypeEx == Globals.fmrUdoListaPrecios)
                {
                    PriceLists.Actions(oForm, pVal);
                }
                //if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD & pVal.FormTypeEx == Globals.fmrUdoListaPrecios)
                //{
                //    PriceLists.Actions(oForm, pVal);
                //}

            }

            catch (Exception ex)
            {
                BubbleEvent = false;
                if (ex.Message.IndexOf("Form - Not found  [66000-9]") != -1)
                {
                    Globals.Error = "EXX: Activar campos de usuario al crear un documento";
                    Globals.SBO_Application.SetStatusBarMessage(Globals.Error, SAPbouiCOM.BoMessageTime.bmt_Short, true);
                }
                else
                {
                    Globals.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
                }
            }
            //}
        }

        private void SBO_Application_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                SAPbouiCOM.Form oForm = Globals.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                #region ADD


                #endregion

                if (BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD)
                {
                    PriceLists.ActionsDataEvent(oForm, BusinessObjectInfo);
                }
            }
            catch (Exception ex)
            {
                BubbleEvent = false;
                Globals.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }
    }
}
