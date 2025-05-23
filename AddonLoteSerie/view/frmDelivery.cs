using AddonConEntrega.bean;
using AddonConEntrega.commons;
using AddonConEntrega.conexion;
using AddonConEntrega.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddonConEntrega.view
{
    public class frmDelivery : FormCommon, IForm
    {
        #region variables
        private SAPbouiCOM.Form mForm, baseForm;
        private SAPbouiCOM.Matrix gridDetalle;

        private const string GRD_ITM = "3", GRD_AVA = "4";//ID Grid
        private const string BTN_EXE = "btnExec", BTN_LOAD = "btnLoad", BTN_SELFV = "btnSelFV", CHK_FORFV = "chkForFV", CHK_IgnDD = "chkIgnDD";//ID BOTONES CUSTOM
        private const string BTN_RIGHT = "48", BTN_OK = "1"; //ID BOTONES STANDARD
        private const string UD_FilePath = "UD_File", UD_ChekFV = "UD_CheckFV", UD_ChekID = "UD_CheckID";//UserDefined
        private const string CheckForFVE = "U_CHK_FORFVE";//Variables
        private const string GRID_DELIVERY = "38", COL_QTY_CHK = "234000368", COL_QTY_VAL = "10002117", COL_UM = "212"; //Col Delivery
        private const int COL_ITM = 1; //Col Items
        #region 92
        //private const string TXT_PATH = "edtFile", LBL_PATH = "lblFile", BASE_POS = "16", SELECT_LBL = "7", COL_NEED = "55";
        //private const string COL_SEL = "0", COL_STOCK = "2", COL_DISP = "3", COL_EXPDATE = "15", COL_CREATEDATE = "27", COL_QTY = "4", COL_ASI = "24";//col lotes
        #endregion
        #region 93
        private const string TXT_PATH = "edtFile", LBL_PATH = "lblFile", BASE_POS = "16", SELECT_LBL = "7", COL_NEED = "234000021", COL_NEED_Alt = "55";
        private const string COL_SEL = "0", COL_STOCK = "2", COL_EXPDATE = "15", COL_CREATEDATE = "27", COL_INDATE = "13", COL_DISP = "234000058", COL_QTY = "234000059", COL_QTYCOM_DB = "234000062", COL_ASI = "234000061", COL_DISP_Alt = "3", COL_QTY_Alt = "4", COL_ASI_Alt = "24";//col lotes #endregion
        #endregion
        #endregion
        public frmDelivery() { }


        #region _EVENTOS_ITEMEVENT

        //Principal
        public bool HandleItemEvents(SAPbouiCOM.ItemEvent itemEvent)
        {
            var result = true;
            try
            {
                switch (itemEvent.EventType)
                {

                    case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                        result = WhenFormLoad(itemEvent);
                        break;

                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                        result = WhenItemPressed(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_KEY_DOWN:
                        result = WhenKeyPressed(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD:
                        result = WhenDataAdd(itemEvent);
                        break;
                }
            }
            catch (Exception ex)
            {
                result = false;
                StatusMessageError("HandleItemEvents() > " + ex.Message);
            }
            return result;
        }



        //Método maneja evento
        private void WhenLostFocus(SAPbouiCOM.ItemEvent oEvent)
        {
            switch (oEvent.ItemUID)
            {
                default:
                    break;
            }

        }


        private bool WhenFormLoad(SAPbouiCOM.ItemEvent oEvent)
        {
            if (oEvent.BeforeAction)
            {
            }
            return true;
        }
        private bool WhenKeyPressed(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            if (oEvent.BeforeAction && (oEvent.CharPressed == 9) && (oEvent.Modifiers == SAPbouiCOM.BoModifiersEnum.mt_CTRL))
            {
                ActualizarUnidadAlternativa();
            }
            return res;
        }
        private bool WhenItemPressed(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            switch (oEvent.ItemUID)
            {

                default:
                    break;
            }
            return res;
        }

        private bool WhenDataAdd(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            switch (oEvent.ItemUID)
            {
                default:
                    break;
            }
            return res;
        }

        #endregion

        public bool HandleFormDataEvents(SAPbouiCOM.BusinessObjectInfo oBusinessObjectInfo)
        {
            switch (oBusinessObjectInfo.EventType)
            {
                default:
                    break;
            }
            return true;
        }

        public bool HandleMenuDataEvents(SAPbouiCOM.MenuEvent menuEvent)
        {
            switch (menuEvent.MenuUID)
            {
                case Constantes.Menu_Lote:
                    ActualizarUnidadAlternativa();
                    break;
                default:
                    break;
            }

            return true;
        }

        #region _EVENTS_RIGHTCLICK
        public bool HandleRightClickEvent(SAPbouiCOM.ContextMenuInfo menuInfo)
        {
            var result = true;
            return result;
        }
        #endregion

        #region _METODOS_PROPIOS 
        private void ActualizarUnidadAlternativa()
        {
            baseForm = Conexion.application.Forms.ActiveForm;
            gridDetalle = baseForm.Items.Item(GRID_DELIVERY).Specific;
            for (int i = 1; i < gridDetalle.RowCount; i++)
            {
                string ItemCode = gridDetalle.Columns.Item("1").Cells.Item(i).Specific.Value.ToString();
                string GrupoUM = GetGrupoUM(ItemCode);

                if (GrupoUM!="Manual" && !gridDetalle.Columns.Item(COL_UM).Cells.Item(i).Specific.Value.Equals(GetMainUM()) && !gridDetalle.Columns.Item(COL_QTY_CHK).Cells.Item(i).Specific.Checked)
                {
                    gridDetalle.Columns.Item(COL_QTY_CHK).Cells.Item(i).Specific.Checked = true;
                    gridDetalle.Columns.Item(COL_QTY_VAL).Cells.Item(i).Specific.Value = GetTotalLotes(
                        gridDetalle.Columns.Item("1").Cells.Item(i).Specific.Value,
                        gridDetalle.Columns.Item("24").Cells.Item(i).Specific.Value);
                }
            }
        }
        private void AddUIExpOrder(SAPbouiCOM.Form oForm)
        {
            SAPbouiCOM.Items oItems = oForm.Items;
            SAPbouiCOM.Item oGeneric;
            SAPbouiCOM.CheckBox oChkForceFV;
            SAPbouiCOM.Button oBtn;

            try
            {
                oGeneric = oItems.Add(BTN_SELFV, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                oGeneric.Top = oItems.Item(BASE_POS).Top;//+ oItems.Item(BASE_POS).Height;
                oGeneric.Left = oItems.Item(BASE_POS).Left;
                oGeneric.Width = oItems.Item(BASE_POS).Width;
                oGeneric.Height = oItems.Item(BASE_POS).Height;
                //oGeneric.LinkTo = BASE_POS;
                oBtn = oGeneric.Specific;
                oBtn.Caption = "Selección x Logica";
                oGeneric = oItems.Item(BASE_POS);
                oGeneric.Visible = false;

                oForm.DataSources.UserDataSources.Add(UD_ChekID, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
                oGeneric = oItems.Add(CHK_IgnDD, SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
                oGeneric.Top = oItems.Item(SELECT_LBL).Top + oItems.Item(SELECT_LBL).Height + 5;
                oGeneric.Left = oItems.Item(SELECT_LBL).Left;
                oGeneric.Width = oItems.Item(SELECT_LBL).Width;

                oChkForceFV = oGeneric.Specific;
                oChkForceFV.Caption = "Ignorar Dias despacho";
                oChkForceFV.DataBind.SetBound(true, "", UD_ChekID);

                if (CheckShowAux(CheckForFVE))
                {
                    oForm.DataSources.UserDataSources.Add(UD_ChekFV, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
                    oGeneric = oItems.Add(CHK_FORFV, SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
                    oGeneric.Top = oItems.Item(CHK_IgnDD).Top;
                    oGeneric.Left = oItems.Item(CHK_IgnDD).Left - oItems.Item(CHK_IgnDD).Width / 2;
                    oGeneric.Width = oItems.Item(CHK_IgnDD).Width;

                    oChkForceFV = oGeneric.Specific;
                    oChkForceFV.Caption = "Permitir Vencidos";
                    oChkForceFV.DataBind.SetBound(true, "", UD_ChekFV);
                }
            }
            catch (Exception ex)
            {
                StatusMessageError("AddUIExpOrder() > " + ex.Message);
            }
            finally
            {
                LiberarObjetoGenerico(oItems);
            }
        }


        #endregion

        public string getFormUID()
        {
            if (mForm != null)
                return mForm.UniqueID;
            else
                return null;
        }
    }
}