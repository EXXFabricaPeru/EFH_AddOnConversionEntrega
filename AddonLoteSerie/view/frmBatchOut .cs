using AddonConEntrega.commons;
using AddonConEntrega.conexion;
using System;
using System.Globalization;

namespace AddonConEntrega.view
{
    public class frmBatchOut : FormCommon, IForm
    {
        #region variables
        private SAPbouiCOM.Form mForm, baseForm;
        private SAPbouiCOM.Matrix gridDetalle;
        private SAPbouiCOM.Matrix availableRegs;
        private SAPbouiCOM.Matrix gridLotes;
        private SAPbouiCOM.Matrix gridLotesSelected;

        private SAPbouiCOM.Columns availableRegsColumns;
        private SAPbouiCOM.Column oColumn;
        private SAPbouiCOM.Button rightButton;
        private bool useAlt = false;

        private const string GRD_ITM = "3", GRD_AVA = "4", GRD_SEL = "5";//ID Grid
        private const string BTN_RIGHT = "48", BTN_OK = "1"; //ID BOTONES STANDARD
        private const string UD_FilePath = "UD_File", UD_ChekFV = "UD_CheckFV", UD_ChekID = "UD_CheckID";//UserDefined
        private const string BASE_POS = "14", BTN_ACTPARENT = "btnActPar", EDT_UM = "btnLinUM", EDT_REQ = "btnLinReq", EDT_TOT = "btnTotalt", COL_CLINV = "btnTotalt";
        private const string UD_BaseType = "UD_TYBASE", UD_BaseId = "UD_IDBASE", UD_QtyCol = "UD_COLQTY", UD_QtyLineAlt = "UD_LQTY", UD_QtySelAlt = "UD_SQTY", UD_UMLineAlt = "UD_LUM";//UserDefined
        private const string GRID_DELIVERY = "38", COL_QTY_CHK = "234000368", COL_QTY_VAL = "10002117", COL_UM = "212"; //Col Delivery
        private const string GRID_LINES = "3", COL_ITEMCODE = "1", COL_QTY = "4", COL_QTY_REQ = "17", COL_QTY_SEL = "18", COL_QTY_ALT = "U_EXP_CtdAlt"; //Col Batch
        #endregion
        public frmBatchOut() { }


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
                    case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                        //result = WhenFormClose(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                        result = WhenItemPressed(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD:
                        result = WhenDataAdd(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_VALIDATE:
                        result = WhenValidate(itemEvent);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                        result = WhenLostFocus(itemEvent);
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


        private bool WhenFormLoad(SAPbouiCOM.ItemEvent oEvent)
        {
            if (oEvent.BeforeAction)
            {
                mForm = Conexion.application.Forms.Item(oEvent.FormUID);
                baseForm = Conexion.application.Forms.ActiveForm;
                if (baseForm.TypeEx.Equals(FormName.ENTREGAS))
                {
                    AddDataSource();
                    AddUI();
                }
                //availableRegsColumn = availableRegsColumns.Add("99", SAPbouiCOM.BoFormItemTypes.it_EDIT);
            }
            else
            {
                mForm = Conexion.application.Forms.Item(oEvent.FormUID);
                gridLotes = mForm.Items.Item(GRID_LINES).Specific;
                gridLotes.Item.Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                gridLotes.Item.Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                gridLotes.Columns.Item(0).Cells.Item(1).Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                availableRegs = mForm.Items.Item("4").Specific;
                availableRegs.AutoResizeColumns();
            }
            return true;
        }
        private void AddDataSource()
        {
            mForm.DataSources.UserDataSources.Add(UD_BaseType, SAPbouiCOM.BoDataType.dt_LONG_TEXT, 100);
            mForm.DataSources.UserDataSources.Add(UD_BaseId, SAPbouiCOM.BoDataType.dt_LONG_TEXT, 100);
            mForm.DataSources.UserDataSources.Add(UD_QtyCol, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
            mForm.DataSources.UserDataSources.Add(UD_QtyLineAlt, SAPbouiCOM.BoDataType.dt_QUANTITY);
            mForm.DataSources.UserDataSources.Add(UD_QtySelAlt, SAPbouiCOM.BoDataType.dt_QUANTITY);
            mForm.DataSources.UserDataSources.Add(UD_UMLineAlt, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 10);
            mForm.DataSources.UserDataSources.Item(UD_BaseId).Value = baseForm.UDFFormUID.Split('_')[0] + "_" + (int.Parse(baseForm.UDFFormUID.Split('_')[1]) - 1);
            mForm.DataSources.UserDataSources.Item(UD_BaseType).Value = baseForm.TypeEx;
        }

        private void AddUI()
        {
            SAPbouiCOM.Items oItems = mForm.Items;
            SAPbouiCOM.Item oGeneric;
            SAPbouiCOM.EditText oEditText;
            SAPbouiCOM.StaticText oStatic;
            SAPbouiCOM.Button oBtn;
            #region Matrix
            availableRegs = mForm.Items.Item(GRD_AVA).Specific;
            availableRegs.Clear();
            availableRegsColumns = availableRegs.Columns;
            availableRegs.AutoResizeColumns();
            oColumn = availableRegsColumns.Add(COL_CLINV, SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
            oColumn.TitleObject.Caption = "Selección UA";
            oColumn.Visible = true;
            oColumn.Editable = true;
            oColumn.RightJustified = true;
            oColumn.Width = 50;
            oColumn.DataBind.SetBound(true, "", UD_QtyCol);
            oColumn = availableRegs.Columns.Item(COL_QTY);
            oColumn.TitleObject.Caption = "Cant. Seleccionada KG";
            #endregion
            #region Fields
            oGeneric = oItems.Add(EDT_UM, SAPbouiCOM.BoFormItemTypes.it_EDIT);
            oGeneric.Top = oItems.Item(BASE_POS).Top;
            oGeneric.Left = oItems.Item(BASE_POS).Left + (oItems.Item(BASE_POS).Width) + 5;
            oGeneric.Width = oItems.Item(BASE_POS).Width / 2;
            oGeneric.LinkTo = BASE_POS;
            oGeneric.Enabled = false;
            oEditText = oGeneric.Specific;
            oEditText.DataBind.SetBound(true, "", UD_UMLineAlt);
            oGeneric = oItems.Add("lblUM", SAPbouiCOM.BoFormItemTypes.it_STATIC);
            oGeneric.Top = oItems.Item("6").Top;
            oGeneric.Left = oItems.Item(EDT_UM).Left;
            oGeneric.Width = oItems.Item(EDT_UM).Width;
            oGeneric.LinkTo = EDT_UM;
            oStatic = oGeneric.Specific;
            oStatic.Caption = "UM";
            oGeneric = oItems.Add(EDT_REQ, SAPbouiCOM.BoFormItemTypes.it_EDIT);
            oGeneric.Top = oItems.Item(EDT_UM).Top;
            oGeneric.Left = oItems.Item(EDT_UM).Left + (oItems.Item(EDT_UM).Width) + 5;
            oGeneric.Width = oItems.Item(BASE_POS).Width;
            oGeneric.LinkTo = EDT_UM;
            oGeneric.Enabled = false;
            oGeneric.RightJustified = true;
            oEditText = oGeneric.Specific;
            oEditText.DataBind.SetBound(true, "", UD_QtyLineAlt);
            oGeneric = oItems.Add("lblReq", SAPbouiCOM.BoFormItemTypes.it_STATIC);
            oGeneric.Top = oItems.Item("6").Top;
            oGeneric.Left = oItems.Item(EDT_REQ).Left;
            oGeneric.Width = oItems.Item(EDT_REQ).Width;
            oGeneric.LinkTo = EDT_UM;
            oStatic = oGeneric.Specific;
            oStatic.Caption = "Total Req.";
            oGeneric = oItems.Add(EDT_TOT, SAPbouiCOM.BoFormItemTypes.it_EDIT);
            oGeneric.Top = oItems.Item(EDT_REQ).Top;
            oGeneric.Left = oItems.Item(EDT_REQ).Left + (oItems.Item(EDT_REQ).Width) + 5;
            oGeneric.Width = oItems.Item(BASE_POS).Width;
            oGeneric.LinkTo = EDT_REQ;
            oGeneric.Enabled = false;
            oGeneric.RightJustified = true;
            oEditText = oGeneric.Specific;
            oEditText.DataBind.SetBound(true, "", UD_QtySelAlt);
            oGeneric = oItems.Add("lblSel", SAPbouiCOM.BoFormItemTypes.it_STATIC);
            oGeneric.Top = oItems.Item("6").Top;
            oGeneric.Left = oItems.Item(EDT_TOT).Left;
            oGeneric.Width = oItems.Item(EDT_TOT).Width;
            oGeneric.LinkTo = EDT_UM;
            oStatic = oGeneric.Specific;
            oStatic.Caption = "Total Sel.";
            #endregion

            #region Buttom
            oGeneric = oItems.Add(BTN_ACTPARENT, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
            oGeneric.Top = oItems.Item(EDT_TOT).Top;
            oGeneric.Left = oItems.Item(EDT_TOT).Left + (oItems.Item(EDT_TOT).Width) + 5;
            oGeneric.Width = oItems.Item(EDT_TOT).Width;
            oGeneric.LinkTo = EDT_REQ;

            oBtn = oGeneric.Specific;
            oBtn.Caption = "Sel. Lote UA";
            #endregion
            //oColumn.DataBind.SetBound(true, "OBTN", "U_EXP_CtdAlt");
        }
        private bool WhenItemPressed(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            switch (oEvent.ItemUID)
            {
                case GRD_ITM:
                    if (!oEvent.BeforeAction && oEvent.Row > 0) PressMatrixDetalles(oEvent);
                    break;
                case GRD_AVA:
                    PressMatrixAsignacion(oEvent);
                    break;
                case BTN_ACTPARENT:
                    if (!oEvent.BeforeAction) SeleccionarLotesUA(oEvent);
                    break;
                default:
                    break;
            }
            return res;
        }
        private bool WhenValidate(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            switch (oEvent.ItemUID)
            {
                case GRD_AVA:
                    break;
                default:
                    break;
            }
            return res;
        }


        private bool WhenLostFocus(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            switch (oEvent.ItemUID)
            {
                case GRD_AVA:
                    switch (oEvent.ColUID)
                    {
                        //case COL_CLINV:
                        //    mForm = Conexion.application.Forms.Item(oEvent.FormUID);
                        //    gridLotes = mForm.Items.Item(GRD_AVA).Specific;
                        //    if (!oEvent.BeforeAction)
                        //    {
                        //        try
                        //        {
                        //            mForm.Freeze(true);
                        //            mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value = "0";
                        //            double total = 0;
                        //            if (gridLotes.Columns.Item(COL_CLINV).Cells.Item(oEvent.Row).Specific.Checked)
                        //                gridLotes.Columns.Item("4").Cells.Item(oEvent.Row).Specific.Value = double.Parse(gridLotes.Columns.Item("3").Cells.Item(oEvent.Row).Specific.Value);

                        //            else
                        //                gridLotes.Columns.Item("4").Cells.Item(oEvent.Row).Specific.Value = "";

                        //            for (int i = 1; i <= gridLotes.RowCount; i++)
                        //            {
                        //                if (gridLotes.Columns.Item(COL_CLINV).Cells.Item(i).Specific.Checked)
                        //                    total += double.Parse(gridLotes.Columns.Item("U_EXP_CtdAlt").Cells.Item(i).Specific.Value);
                        //            }
                        //            mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value = total.ToString();
                        //        }
                        //        catch (Exception)
                        //        {
                        //            throw;
                        //        }
                        //        finally
                        //        {
                        //            mForm.Freeze(false);
                        //        }
                        //    }
                        //    break;
                    }
                    break;
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
        private bool PressMatrixDetalles(SAPbouiCOM.ItemEvent oEvent)
        {
            try
            {
                mForm = Conexion.application.Forms.Item(oEvent.FormUID);
                baseForm = Conexion.application.Forms.Item(mForm.DataSources.UserDataSources.Item(UD_BaseId).Value);
                gridDetalle = baseForm.Items.Item(GRID_DELIVERY).Specific;
                gridLotes = mForm.Items.Item(GRID_LINES).Specific;
                mForm.Items.Item(BTN_ACTPARENT).Enabled = false;

                gridLotesSelected = mForm.Items.Item(GRD_SEL).Specific;
                double cantalt = 0;


                for (int l = 1; l <= gridLotesSelected.VisualRowCount; l++)
                {
                    if (gridLotesSelected.Columns.Item("U_EXP_CtdAlt").Cells.Item(l).Specific.Value != null || string.IsNullOrEmpty(gridLotesSelected.Columns.Item("U_EXP_CtdAlt").Cells.Item(l).Specific.Value))
                    {
                        try
                        {

                            var a = gridLotesSelected.Columns.Item("U_EXP_CtdAlt").Cells.Item(l).Specific.Value;

                            cantalt = cantalt + Convert.ToDouble(gridLotesSelected.Columns.Item("U_EXP_CtdAlt").Cells.Item(l).Specific.Value);
                        }
                         catch
                        {
                            cantalt = 0;
                        }
                    }
                }  

				for (int i = 1; i < gridDetalle.RowCount; i++)
                {
                    if (gridDetalle.Columns.Item(COL_ITEMCODE).Cells.Item(i).Specific.Value.Equals(gridLotes.Columns.Item(COL_ITEMCODE).Cells.Item(oEvent.Row).Specific.Value))
                    {
                        mForm.DataSources.UserDataSources.Item(UD_UMLineAlt).Value = gridDetalle.Columns.Item(COL_UM).Cells.Item(i).Specific.Value;
                        mForm.DataSources.UserDataSources.Item(UD_QtyLineAlt).Value = gridDetalle.Columns.Item("11").Cells.Item(i).Specific.Value;
                        mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value = cantalt.ToString();
                        mForm.Items.Item(BTN_ACTPARENT).Enabled = true;//!GetMainUM().Equals(gridDetalle.Columns.Item(COL_UM).Cells.Item(i).Specific.Value);
                    }
                }
            }
            catch (Exception)
            {
            }
            return true;
        }
        private bool PressMatrixAsignacion(SAPbouiCOM.ItemEvent oEvent)
        {
            switch (oEvent.ColUID)
            {
                case COL_CLINV:
                    if (oEvent.BeforeAction)
                    {
                        mForm = Conexion.application.Forms.Item(oEvent.FormUID);
                        gridLotes = mForm.Items.Item(GRD_AVA).Specific;
                        try
                        {
                            mForm.Freeze(true);
                            if (oEvent.Modifiers == SAPbouiCOM.BoModifiersEnum.mt_SHIFT)
                            {
                                mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value = "0";
                                double total = 0;
                                int lastCheck = 0;
                                for (int i = gridLotes.RowCount; i > 0; i--)
                                {
                                    if (gridLotes.Columns.Item(COL_CLINV).Cells.Item(i).Specific.Checked) { lastCheck = i; break; }
                                }
                                for (int i = 1; i <= lastCheck; i++)
                                {
                                    if (gridLotes.Columns.Item(COL_CLINV).Cells.Item(i).Specific.Checked)
                                    {
                                        gridLotes.Columns.Item(COL_QTY).Cells.Item(i).Specific.Value = double.Parse(gridLotes.Columns.Item("3").Cells.Item(i).Specific.Value, CultureInfo.InvariantCulture);
                                        total += double.Parse(gridLotes.Columns.Item(COL_QTY_ALT).Cells.Item(i).Specific.Value, CultureInfo.InvariantCulture);
                                    }
                                    else
                                        if (double.Parse(gridLotes.Columns.Item(COL_QTY).Cells.Item(i).Specific.Value, CultureInfo.InvariantCulture) > 0) gridLotes.Columns.Item(COL_QTY).Cells.Item(i).Specific.Value = "";
                                }
                                mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value = total.ToString();
                                gridLotes.Columns.Item(0).Cells.Item(oEvent.Row).Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                            }
                            else
                            {
                                if (gridLotes.Columns.Item(COL_CLINV).Cells.Item(oEvent.Row).Specific.Checked)
                                {
                                    gridLotes.Columns.Item(COL_QTY).Cells.Item(oEvent.Row).Specific.Value =double.Parse(gridLotes.Columns.Item("3").Cells.Item(oEvent.Row).Specific.Value, CultureInfo.InvariantCulture);
                                    mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value = (double.Parse(mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value, CultureInfo.InvariantCulture) + double.Parse(gridLotes.Columns.Item(COL_QTY_ALT).Cells.Item(oEvent.Row).Specific.Value, CultureInfo.InvariantCulture)).ToString();
									//mForm.Items.Item("48").Enabled = true;
									rightButton = mForm.Items.Item(BTN_RIGHT).Specific;
                                    rightButton.Item.Enabled = true;
								}
								else
                                {
                                    if (double.Parse(gridLotes.Columns.Item(COL_QTY).Cells.Item(oEvent.Row).Specific.Value) > 0) gridLotes.Columns.Item(COL_QTY).Cells.Item(oEvent.Row).Specific.Value = "";
                                    mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value = (double.Parse(mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value) - double.Parse(gridLotes.Columns.Item(COL_QTY_ALT).Cells.Item(oEvent.Row).Specific.Value, CultureInfo.InvariantCulture)).ToString();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            mForm.Freeze(false);
                        }
                    }
                    break;
            }
            return true;
        }

        private bool SeleccionarLotesUA(SAPbouiCOM.ItemEvent oEvent)
        {
            bool res = true;
            mForm = Conexion.application.Forms.Item(oEvent.FormUID);
            try
            {

                if (double.Parse(mForm.DataSources.UserDataSources.Item(UD_QtyLineAlt).Value, CultureInfo.InvariantCulture) < double.Parse(mForm.DataSources.UserDataSources.Item(UD_QtySelAlt).Value, CultureInfo.InvariantCulture))
                {
                    res = (Conexion.application.MessageBox("La unidad alternativa seleccionada es superior", 1, "Continuar", "Cancelar", "") != 1);
                }
                if (res)
                {
                    if (mForm.DataSources.UserDataSources.Item(UD_UMLineAlt).Value != GetMainUM()) res = ActualizarPadre();
                    if (res)
                    {
						res = AsignarDetalle(); 
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return res;
        }
        private bool ActualizarPadre()
        {
            string ItemCodeLine = "", selectedItemCode = "";
            double ReqLine = 0, selectVal = 0; ;

            baseForm = Conexion.application.Forms.Item(mForm.DataSources.UserDataSources.Item(UD_BaseId).Value);
            gridDetalle = baseForm.Items.Item(GRID_DELIVERY).Specific;
            gridLotes = mForm.Items.Item(GRID_LINES).Specific;
            availableRegs = mForm.Items.Item(GRD_AVA).Specific;
            gridLotesSelected = mForm.Items.Item(GRD_SEL).Specific;

            double.TryParse(availableRegs.Columns.Item(COL_QTY).ColumnSetting.SumValue, out selectVal);
            for (int l = 1; l <= gridLotes.RowCount; l++)
            {
                if (gridLotes.IsRowSelected(l))
                {
                    ReqLine = double.Parse(gridLotes.Columns.Item(COL_QTY_REQ).Cells.Item(l).Specific.Value, CultureInfo.InvariantCulture);
                    selectedItemCode = gridLotes.Columns.Item(COL_ITEMCODE).Cells.Item(l).Specific.Value;
                }
            }
            if (selectVal > ReqLine)
            {
                Conexion.application.MessageBox("La cantidad en KG seleccionada es superior a la permitida. Favor cerrar la ventana de selección y quitar el 'check' de 'Cambiar Cantidad UM'");
                return false;
            }


            for (int l = 1; l <= gridLotesSelected.VisualRowCount; l++)
            {
                if (gridLotesSelected.Columns.Item("29").Cells.Item(l).Specific.Value != null || string.IsNullOrEmpty(gridLotesSelected.Columns.Item("29").Cells.Item(l).Specific.Value))
                {
                    selectVal = selectVal + Convert.ToDouble(gridLotesSelected.Columns.Item("29").Cells.Item(l).Specific.Value);
                }
            }

            if (ReqLine != selectVal && selectVal > 0)
            {
                for (int i = 1; i <= gridDetalle.RowCount; i++)
                {
                    ItemCodeLine = gridDetalle.Columns.Item(COL_ITEMCODE).Cells.Item(i).Specific.Value;
                    if (selectedItemCode == ItemCodeLine)
                    {
                        gridDetalle.Columns.Item(COL_QTY_CHK).Cells.Item(i).Specific.Checked = true;
                        gridDetalle.Columns.Item(COL_QTY_VAL).Cells.Item(i).Specific.Value = selectVal;
                        break;
                    }
                }
                StatusMessageSuccess("Se ha actualizado cantidad Unidad Alternativa a " + selectVal);
            }
            return true;
        }
        private bool AsignarDetalle()
        {
            try
            {
                //mForm.Freeze(true);
                rightButton = mForm.Items.Item(BTN_RIGHT).Specific;
                gridLotes = mForm.Items.Item(GRD_AVA).Specific;
                for (int i = 1; i <= gridLotes.RowCount; i++)
                {
                    //var b = gridLotes.Columns.Item("0").Cells.Item(i).Specific.Value;

					if (gridLotes.Columns.Item(COL_CLINV).Cells.Item(i).Specific.Checked && double.Parse(gridLotes.Columns.Item(COL_QTY).Cells.Item(i).Specific.Value, CultureInfo.InvariantCulture) > 0)
                    {
                        gridLotes.Columns.Item(0).Cells.Item(i).Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                        rightButton.Item.Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                        i--;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                mForm.Freeze(false);
            }
            return true;
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