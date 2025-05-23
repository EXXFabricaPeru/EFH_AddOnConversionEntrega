using AddonConEntrega.commons;
using AddonConEntrega.view;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AddonConEntrega.conexion
{
    public class Conexion
    {
        public static SAPbobsCOM.Company company;
        public static SAPbouiCOM.Application application;
        public static readonly Dictionary<string, IForm> formOpen;
        static Conexion()
        {
            formOpen = new Dictionary<string, IForm>();
        }
        public Conexion()
        {
            try
            {
                application = instanciarAplicacion();
                company = InstanciarCompania();
                InicializarFiltros();
                application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(Application_AppEvent);
                application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(Application_MenuEvent);
                application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(Application_ItemEvent);
                application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(Application_FormDataEvent);
                application.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(Application_RightClickEvent);
                CrearMenu(Properties.Resources.IconoModulo);
            }
            catch (Exception e)
            {
                application.MessageBox("Conexion: " + e.Message);
            }
        }

        private SAPbouiCOM.Application instanciarAplicacion()
        {
            var guiApi = new SAPbouiCOM.SboGuiApi();
            guiApi.Connect(Environment.GetCommandLineArgs().GetValue(1).ToString());
            return guiApi.GetApplication();
        }
        private SAPbobsCOM.Company InstanciarCompania()
        {
            try
            {
                return application.Company.GetDICompany();
            }
            catch (Exception e)
            {
                application.MessageBox(e.Message);
            }
            return null;
        }

        private void InicializarFiltros()
        {
            SAPbouiCOM.EventFilters filtros = new SAPbouiCOM.EventFilters();
            SAPbouiCOM.EventFilter filtroMenu = filtros.Add(SAPbouiCOM.BoEventTypes.et_MENU_CLICK);
            SAPbouiCOM.EventFilter filtroItem = filtros.Add(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED);
            filtroItem.AddEx(FormName.BATCH_OUT);
            filtroItem.AddEx(FormName.ENTREGAS);
            SAPbouiCOM.EventFilter filtroKeyDown = filtros.Add(SAPbouiCOM.BoEventTypes.et_KEY_DOWN);
            filtroKeyDown.AddEx(FormName.ENTREGAS);

            SAPbouiCOM.EventFilter filtroCFL = filtros.Add(SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST);
            //filtroCFL.AddEx(FormName.AUT_INTERESES);
            SAPbouiCOM.EventFilter filterMatrixLink = filtros.Add(SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED);
            //filterMatrixLink.AddEx(FormName.AUT_INTERESES);
            SAPbouiCOM.EventFilter filterCombo = filtros.Add(SAPbouiCOM.BoEventTypes.et_COMBO_SELECT);
            //filterCombo.AddEx(FormName.AUT_INTERESES);

            SAPbouiCOM.EventFilter filterLostFocus = filtros.Add(SAPbouiCOM.BoEventTypes.et_LOST_FOCUS);
            //filterLostFocus.AddEx(FormName.BATCH_OUT);
            SAPbouiCOM.EventFilter filterValidate = filtros.Add(SAPbouiCOM.BoEventTypes.et_VALIDATE);
            //filterValidate.AddEx(FormName.BATCH_OUT);
            SAPbouiCOM.EventFilter filterFormLoad = filtros.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD);
            filterFormLoad.AddEx(FormName.BATCH_OUT);

            SAPbouiCOM.EventFilter filterFormActivate = filtros.Add(SAPbouiCOM.BoEventTypes.et_GOT_FOCUS);

            //filterFormActivate.AddEx(FormName.ENTREGAS);
            SAPbouiCOM.EventFilter filterFormClose = filtros.Add(SAPbouiCOM.BoEventTypes.et_FORM_CLOSE);
            //filterFormClose.AddEx(FormName.BATCH_OUT);

            //filterFormLoad.AddEx("0");
            SAPbouiCOM.EventFilter filterAddData = filtros.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD);
            //filterAddData.AddEx(FormName.BATCH_OUT);

            //SAPbouiCOM.EventFilter filterClose = filtros.Add(SAPbouiCOM.BoEventTypes.et_FORM_CLOSE);

            application.SetFilter(filtros);
        }

        //Eventos de aplicación
        void Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            try
            {
                BubbleEvent = true;
                if (formOpen.ContainsKey(FormUID))
                {
                    BubbleEvent = formOpen[FormUID].HandleItemEvents(pVal);
                }

                switch (pVal.FormTypeEx)
                {
                    case FormName.BATCH_OUT:
                        BubbleEvent = new frmBatchOut().HandleItemEvents(pVal);
                        break;
                    //case FormName.PK_BATCH_OUT:
                    //    BubbleEvent = new frmBatchOut().HandleItemEvents(pVal);
                    //    break;
                    case FormName.ENTREGAS:
                        BubbleEvent = new frmDelivery().HandleItemEvents(pVal);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                BubbleEvent = true;
            }
        }

        void Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    company.Disconnect();
                    Environment.Exit(0);
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    company.Disconnect();
                    Environment.Exit(0);
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    company.Disconnect();
                    Environment.Exit(0);
                    break;
            }
        }

        void Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            var result = true;
            if (pVal.BeforeAction)
            {
                try
                {
                    switch (pVal.MenuUID)
                    {
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    application.MessageBox(e.Message);
                }
            }
            try
            {
   
                //Controles basados en el menu "Click derecho"
                if ( pVal.MenuUID == Constantes.Menu_Lote)
                {
                    if (pVal.BeforeAction)
                    {
                        var mForm = application.Forms.ActiveForm;
                        if (formOpen.ContainsKey(mForm.UniqueID))
                            result = formOpen[mForm.UniqueID].HandleMenuDataEvents(pVal);

                        switch (mForm.TypeEx)
                        {
                            case FormName.ENTREGAS:
                                BubbleEvent = new frmDelivery().HandleMenuDataEvents(pVal);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                application.MessageBox(e.Message);
            }
            BubbleEvent = result;
        }

        void Application_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            try
            {
                BubbleEvent = true;

                if (formOpen.ContainsKey(BusinessObjectInfo.FormUID))
                {
                    BubbleEvent = formOpen[BusinessObjectInfo.FormUID].HandleFormDataEvents(BusinessObjectInfo);
                }
                else
                {
                    switch (BusinessObjectInfo.FormTypeEx)
                    {
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                BubbleEvent = true;
            }
        }

        void Application_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = formOpen[eventInfo.FormUID].HandleRightClickEvent(eventInfo);
        }

        //Creación de menú
        private void CrearMenu(System.Drawing.Bitmap imageBMP = null)
        {
            SAPbouiCOM.Form frmEps = application.Forms.GetFormByTypeAndCount(169, 1);
            frmEps.Freeze(true);
            try
            {
                application.StatusBar.SetText(Constantes.PREFIX_MSG_ADDON + "Cargando opciones de menú", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_None);

                XmlDocument xmlMenu = new XmlDocument();
                xmlMenu.LoadXml(AddonConEntrega.Properties.Resources.Menu);
                application.LoadBatchActions(xmlMenu.InnerXml);
            }
            catch (Exception e)
            {
                application.StatusBar.SetText(Constantes.PREFIX_MSG_ADDON + e.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                frmEps.Freeze(false);
                frmEps.Update();
            }
        }

        public static void AddForm(string UID, IForm newForm)
        {
            formOpen.Add(UID, newForm);
        }
    }
}