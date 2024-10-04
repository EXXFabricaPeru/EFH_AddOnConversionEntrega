using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace AddOnUpdPrice.App
{
    public class Connect
    {

        public static void SetApplication()
        {
            try
            {
                SAPbouiCOM.SboGuiApi SboGuiApi = default(SAPbouiCOM.SboGuiApi);
                string sConnectionString = null;
                SboGuiApi = new SAPbouiCOM.SboGuiApi();
                if (Environment.GetCommandLineArgs().Length > 1)
                {
                    sConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1));
                }
                else
                {
                    sConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(0));
                }
                SboGuiApi.Connect(sConnectionString);
                Globals.SBO_Application = SboGuiApi.GetApplication();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static bool ConnectToCompany()
        {
            try
            {
                //Globals.oCompany = Globals.SBO_Application.Company.GetDICompany();

                Globals.oCompany = new SAPbobsCOM.Company();

                string cookie = Globals.oCompany.GetContextCookie();

                string conStr = Globals.SBO_Application.Company.GetConnectionContext(cookie);

                if (Globals.oCompany.Connected)
                {
                    Globals.oCompany.Disconnect();
                }

                int ret = Globals.oCompany.SetSboLoginContext(conStr);

                if (ret != 0)
                {
                    throw new Exception("Login context failed");
                }

                ret = Globals.oCompany.Connect();

                return true;
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
                return false;
            }
        }

        public static void SetFilters()
        {
            Globals.oFilters = new SAPbouiCOM.EventFilters();
            #region FORM_DATA_ADD
            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD);
            //Globals.oFilter.AddEx("720");
            //Globals.oFilter.AddEx("392");
            Globals.oFilter.AddEx("170");
            Globals.oFilter.AddEx("141");
            Globals.oFilter.AddEx("426");  //pagos efectuados
            Globals.oFilter.AddEx("504");  //asistente de pagos
            #endregion
            #region FORM_DATA_UPDATE
            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE);
            //Globals.oFilter.AddEx("720");
            Globals.oFilter.AddEx("170");  //pagos recibidos
            Globals.oFilter.AddEx("141");  //Factura de proveedores
            Globals.oFilter.AddEx("426");  //pagos efectuados
            Globals.oFilter.AddEx("504");  //asistente de pagos
            #endregion

            #region FORM_DATA_DELETE
            //Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_DELETE);
            //Globals.oFilter.AddEx("504");  //asistente de pagos
            #endregion



            #region COMBO_SELECT
            //Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_COMBO_SELECT);
            //Globals.oFilter.AddEx("SPE_ABC");
            #endregion

            #region FORM_CLOSE
            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_CLOSE);
            Globals.oFilter.AddEx("ProjectForm");
            Globals.oFilter.AddEx("FormAsientos");
            #endregion

            #region ITEM_PRESSED
            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED);
            Globals.oFilter.AddEx("155");
            Globals.oFilter.AddEx(Globals.fmrUdoEstructuras);
            Globals.oFilter.AddEx(Globals.fmrUdoListaPrecios);

            #endregion
            #region FORM_LOAD
            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD);
            //Globals.oFilter.AddEx(Globals.fmrUdoListaPrecios);
            #endregion
            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_MENU_CLICK);
            //Globals.oFilter.AddEx("ProjectForm");
            //Globals.oFilter.AddEx("FormAsientos");
            //Globals.oFilter.AddEx("FormAsiBorr");

            #region MATRIX_LINK_PRESSED
            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED);
            Globals.oFilter.AddEx("FormAsiBorr");
            #endregion

            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_VALIDATE);
            Globals.oFilter.AddEx(Globals.fmrUdoListaPrecios);

            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD);
            Globals.oFilter.AddEx(Globals.fmrUdoListaPrecios);

            Globals.oFilter = Globals.oFilters.Add(SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST);
            Globals.oFilter.AddEx(Globals.fmrUdoListaPrecios);

            Globals.SBO_Application.SetFilter(Globals.oFilters);

        }
    }
}
