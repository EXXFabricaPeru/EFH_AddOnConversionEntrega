using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using AddonConEntrega.conexion;

namespace AddonConEntrega.commons
{
    public class FormCommon
    {
        #region Generic
        private const string RS_VALUE = "Value";
        private const string RS_NAME = "Name";

        public SAPbouiCOM.Form CreateForm(SAPbobsCOM.Company company, SAPbouiCOM.Application application, string resource, string formName)
        {
            SAPbouiCOM.Form mForm = null;

            try
            {
                SAPbouiCOM.FormCreationParams fCreationParams = application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
                fCreationParams.XmlData = resource;
                fCreationParams.FormType = formName;
                fCreationParams.UniqueID = formName + DateTime.Now.ToString("hhmmss");
                mForm = application.Forms.AddEx(fCreationParams);
                mForm.Visible = false;
            }
            catch (Exception ex)
            {
                StatusMessageError("Error creando formulario " + formName + ". Excepción :" + ex.Message);
            }

            return mForm;
        }

        public static void StatusMessageError(string message)
        {
            Conexion.application.StatusBar.SetText(Constantes.PREFIX_MSG_ADDON + message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
        }
        public static void StatusMessageInfo(string message)
        {
            Conexion.application.StatusBar.SetText(Constantes.PREFIX_MSG_ADDON + message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_None);
        }
        public static void StatusMessageWarning(string message)
        {
            Conexion.application.StatusBar.SetText(Constantes.PREFIX_MSG_ADDON + message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
        }
        public static void StatusMessageSuccess(string message)
        {
            Conexion.application.StatusBar.SetText(Constantes.PREFIX_MSG_ADDON + message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
        }

        internal static void LiberarObjetoGenerico(Object objeto)
        {
            try
            {
                if (objeto != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objeto);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(Constantes.PREFIX_MSG_ADDON + " Error Liberando Objeto: " + ex.Message);
            }
        }

        internal static void InstanciateCombo(SAPbouiCOM.ComboBox ComboBox, string Query)
        {
            SAPbobsCOM.Recordset ComboRS = (SAPbobsCOM.Recordset)Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            while (ComboBox.ValidValues.Count != 0)
            {
                ComboBox.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
            }
            ComboRS.DoQuery(Query);
            while (!ComboRS.EoF)
            {
                ComboBox.ValidValues.Add((string)ComboRS.Fields.Item(RS_VALUE).Value.ToString(), (string)ComboRS.Fields.Item(RS_NAME).Value.ToString());
                ComboRS.MoveNext();
            }
            ComboBox.Item.Enabled = true;
            ComboBox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ComboRS);
        }

        internal bool OpenFileDialog(SAPbouiCOM.Form oForm, string FILE_TXT)
        {
            GetFileNameClass oGetFileName = new GetFileNameClass();
            oGetFileName.Filter = "Archivo excel (.xlsx)|*.xlsx";
            Thread threadGetFile = new Thread(new ThreadStart(oGetFileName.GetFileName));
            threadGetFile.SetApartmentState(ApartmentState.STA);
            try
            {
                threadGetFile.Start();
                while (!threadGetFile.IsAlive) ; // Wait for thread to get started
                Thread.Sleep(1);  // Wait a sec more
                threadGetFile.Join();    // Wait for thread to end
                SAPbouiCOM.EditText txRuta = (SAPbouiCOM.EditText)oForm.Items.Item(FILE_TXT).Specific;
                txRuta.Value = oGetFileName.FileName;

            }
            catch (Exception ex)
            {
                StatusMessageWarning(string.Format("openFileDialog():{0}", ex.Message));
            }
            finally
            {
                threadGetFile = null;
                oGetFileName = null;
            }
            return true;
        }
        #endregion

        #region Aux
        internal static bool CheckShowAux(string id)
        {
            SAPbobsCOM.Recordset oRS = null;
            try
            {
                oRS = Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRS.DoQuery(Consultas.GetCheckCFGAux(Conexion.company.DbServerType, id));
                return oRS.Fields.Item("Check").Value.ToString().Equals("Y");
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                LiberarObjetoGenerico(oRS);
            }
        }

        internal static string GetMainUM()
        {
            SAPbobsCOM.Recordset oRS = null;
            try
            {
                oRS = Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRS.DoQuery(Consultas.GetMainUM(Conexion.company.DbServerType));
                return oRS.Fields.Item("Value").Value.ToString();
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                LiberarObjetoGenerico(oRS);
            }
        }

        internal static string GetGrupoUM(string ItemCode)
        {
            SAPbobsCOM.Recordset oRS = null;
            try
            {
                oRS = Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRS.DoQuery(Consultas.GetGrupoUM(Conexion.company.DbServerType, ItemCode));
                return oRS.Fields.Item("Value").Value.ToString();
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                LiberarObjetoGenerico(oRS);
            }
        }


        internal static double GetTotalLotes(string ItemCode, string Whs)
        {
            SAPbobsCOM.Recordset oRS = null;
            double valor = 100000;
            try
            {
                oRS = Conexion.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRS.DoQuery(Consultas.GetTotalLote(Conexion.company.DbServerType, ItemCode, Whs));
                return oRS.Fields.Item("Value").Value;
            }
            catch (Exception)
            {
                return valor;
            }
            finally
            {
                LiberarObjetoGenerico(oRS);
            }
        }



        #endregion


    }
}
