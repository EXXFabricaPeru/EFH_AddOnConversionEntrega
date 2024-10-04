using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using AddOnUpdPrice.DB_Structure;
using AddOnUpdPrice.App;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace AddOnUpdPrice.Functionality
{
    public class PriceLists
    {
        private static bool isLoadingItems = false;

        public static void Actions(SAPbouiCOM.Form oForm, SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.ProgressBar pgrsBar = null;
            try
            {
                if (pVal.FormTypeEx == Globals.fmrUdoListaPrecios)
                {
                    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
                    {
                        if (pVal.Action_Success)
                        {
                            if (pVal.ItemUID == "btnFile") OpenFile(oForm);
                            if (pVal.ItemUID == "btnCargar")CargarFile(oForm);                           
                            if (pVal.ItemUID == "btnBuscar") CargarArticulos(oForm);
                            //if (pVal.ItemUID == "1") ActualizarPrecios(oForm);

                        }
                        if (oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE && pVal.ItemUID == "1" && pVal.BeforeAction)
                        {
                            pgrsBar = Globals.SBO_Application.StatusBar.CreateProgressBar("", 0, false);
                            ValidarRegistro(oForm);
                            ActualizarPrecios(oForm);
                            pgrsBar.Stop();
                        }


                        /*
                        if (pVal.ItemUID == "1")
                        {
                            ValidarRegistro(oForm);
                            ActualizarPrecios(oForm);

                        */

                        //if (pVal.BeforeAction)
                        //{
                        //    if (pVal.ItemUID == "1") ValidarRegistro(oForm);
                        //}
                    }
                    else if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                    {
                        var cflEvnt = (SAPbouiCOM.ChooseFromListEvent)pVal;
                        if (!isLoadingItems && cflEvnt.ColUID == "C_0_1" && cflEvnt.SelectedObjects != null)
                        {
                            int lLimaPU = 0, lLimaPxM = 0, lProvPU = 0, lProvPxM = 0;
                            string sMonLimaPU = "", sMonLimaPxM = "", sMonProvPU = "", sMonProvPxM = "";
                            ObtenerListas(ref lLimaPU, ref lLimaPxM, ref lProvPU, ref lProvPxM);
                            sMonLimaPU = ObtenerMonedaLista(lLimaPU);
                            sMonLimaPxM = ObtenerMonedaLista(lLimaPxM);
                            sMonProvPU = ObtenerMonedaLista(lProvPU);
                            sMonProvPxM = ObtenerMonedaLista(lProvPxM);

                            ((SAPbouiCOM.Matrix)oForm.Items.Item("0_U_G").Specific).SetCellWithoutValidation(cflEvnt.Row, "C_0_3", sMonLimaPU);
                            ((SAPbouiCOM.Matrix)oForm.Items.Item("0_U_G").Specific).SetCellWithoutValidation(cflEvnt.Row, "C_0_5", sMonLimaPxM);
                            ((SAPbouiCOM.Matrix)oForm.Items.Item("0_U_G").Specific).SetCellWithoutValidation(cflEvnt.Row, "C_0_7", sMonProvPU);
                            ((SAPbouiCOM.Matrix)oForm.Items.Item("0_U_G").Specific).SetCellWithoutValidation(cflEvnt.Row, "C_0_9", sMonProvPxM);
                            MostrarCantidadRegistros(oForm);
                        }
                    }
                    //if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD)
                    //{
                    //    if (pVal.Action_Success)
                    //    {
                    //        LoadComponentes(oForm);
                    //    }

                    //}
                }

                if (pVal.FormTypeEx == "FrmBusqueda")
                {
                    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
                    {
                        if (pVal.Action_Success)
                        {
                            if (pVal.ItemUID == "btnOK") SendData(oForm);
                        }
                    }
                }


                if (pVal.FormTypeEx == Globals.fmrUdoEstructuras)
                {
                    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
                    {
                        if (pVal.Action_Success)
                        {
                            if (pVal.ItemUID == "btnBF") CreateBF();
                        }
                    }
                }

                if (pVal.FormTypeEx == "155")
                {
                    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
                    {
                        if (pVal.BeforeAction)
                        {
                            if (pVal.ItemUID == "1") ValidarMaestroListas(oForm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (pgrsBar != null) pgrsBar.Stop();
                throw ex;
            }
        }

        public static void ActionsDataEvent(SAPbouiCOM.Form oForm, SAPbouiCOM.BusinessObjectInfo businessObjectInfo)
        {
            switch (businessObjectInfo.EventType)
            {
                case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD:
                    if (oForm.TypeEx == "UDO_FT_EXX_AUPP_LSPR" && !businessObjectInfo.BeforeAction && businessObjectInfo.ActionSuccess)
                    {
                        SetEditableColumnsMatrix(oForm, false);
                    }
                    break;
            }
        }

        public static bool ValidaCodigoMoneda(SAPbouiCOM.ItemEvent pVal, SAPbouiCOM.Form oForm)
        {
            if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_VALIDATE)
            {
                if (pVal.BeforeAction && !pVal.InnerEvent && pVal.ItemUID.Equals("0_U_G") && (pVal.ColUID.Equals("C_0_3") || pVal.ColUID.Equals("C_0_7")) && pVal.Row > 0)
                {
                    var mtxItems = (SAPbouiCOM.Matrix)oForm.Items.Item(pVal.ItemUID).Specific;
                    var currCode = ((SAPbouiCOM.EditText)mtxItems.GetCellSpecific(pVal.ColUID, pVal.Row)).Value;
                    if (!MonedaExisteEnSAP(currCode))
                    {
                        ((SAPbouiCOM.EditText)mtxItems.GetCellSpecific(pVal.ColUID, pVal.Row)).Item.Click();
                        throw new InvalidOperationException("Codigo de moneda no valido");
                    }
                    else
                        return true;
                }
            }
            return true;
        }

        public static void CreateBF()
        {
            try
            {
                Globals.SBO_Application.SetStatusBarMessage("Creando BFs...", SAPbouiCOM.BoMessageTime.bmt_Short, false);

                #region Let there be QCats UQ and FMS
                MDQueries oMDQueries = new MDQueries();

                #region QCats
                oMDQueries.CreateCategories("EXX_AddOn_UpdPrice");
                #endregion

                #region UQ
                #region UQ HANA
                if (Globals.IsHana() == true)
                {
                    #region
                    //no identificado aun
                    oMDQueries.CreateQueries("EXX_AddOn_UpdPrice", "BF_ObtenerArticulos", AddOnUpdPrice.Properties.Resources.BF_ObtenerArticulos);
                    oMDQueries.CreateQueries("EXX_AddOn_UpdPrice", "BF_ObtenerArticulosDesc", AddOnUpdPrice.Properties.Resources.BF_ObtenerArticulosDesc);
                    oMDQueries.CreateQueries("EXX_AddOn_UpdPrice", "BF_ObtenerMonedas", AddOnUpdPrice.Properties.Resources.BF_ObtenerMonedas);
                    #endregion
                }
                #endregion
                #region UQ SQL
                if (Globals.IsHana() == false)
                {
                    #region
                    oMDQueries.CreateQueries("EXX_AddOn_UpdPrice", "BF_ObtenerArticulos", AddOnUpdPrice.Properties.Resources.BF_ObtenerArticulos);
                    oMDQueries.CreateQueries("EXX_AddOn_UpdPrice", "BF_ObtenerArticulosDesc", AddOnUpdPrice.Properties.Resources.BF_ObtenerArticulosDesc);
                    oMDQueries.CreateQueries("EXX_AddOn_UpdPrice", "BF_ObtenerMonedas", AddOnUpdPrice.Properties.Resources.BF_ObtenerMonedas);
                    #endregion

                }
                #endregion
                #endregion

                #region Old FMS

                //oMDQueries.RemoveFMS("BF_ObtenerMonedas", Globals.fmrUdoListaPrecios, "0_U_G", "C_0_1", "", "Y", "EXX_AddOn_UpdPrice");

                #endregion

                #region FMS
                //oMDQueries.CreateFMS("BF_ObtenerArticulos", Globals.fmrUdoListaPrecios, "0_U_G", "C_0_1", "", "N");
                oMDQueries.CreateFMS("BF_ObtenerArticulosDesc", Globals.fmrUdoListaPrecios, "0_U_G", "C_0_2", "C_0_1", "N");
                oMDQueries.CreateFMS("BF_ObtenerMonedas", Globals.fmrUdoListaPrecios, "0_U_G", "C_0_3", "", "N");
                oMDQueries.CreateFMS("BF_ObtenerMonedas", Globals.fmrUdoListaPrecios, "0_U_G", "C_0_5", "", "N");
                oMDQueries.CreateFMS("BF_ObtenerMonedas", Globals.fmrUdoListaPrecios, "0_U_G", "C_0_7", "", "N");
                oMDQueries.CreateFMS("BF_ObtenerMonedas", Globals.fmrUdoListaPrecios, "0_U_G", "C_0_9", "", "N");
                #endregion

                #endregion

                Globals.SBO_Application.SetStatusBarMessage("BFs creadas correctamente", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public static void LoadComponentes(SAPbouiCOM.Form oForm)
        //{
        //    try
        //    {
        //        SAPbouiCOM.EditText eArt = oForm.Items.Item("Item_3").Specific;
        //            oForm.DataSources.UserDataSources.Add("EditDS2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT);
        //            AddChooseFromListTaxCode(oForm);
        //            SAPbouiCOM.Matrix oMatrix = oForm.Items.Item("0_U_G").Specific;
        //            SAPbouiCOM.Column ColTaxCode = oMatrix.Columns.Item("C_0_3");
        //            ColTaxCode.DataBind.SetBound(true, "", "EditDS2");
        //            ColTaxCode.ChooseFromListUID = "CFL2";
        //            ColTaxCode.ChooseFromListAlias = "CurrCode";
        //    }
        //    catch (Exception ex)
        //    {
        //       throw ex;
        //    }
        //}

        private static void AddChooseFromListTaxCode(SAPbouiCOM.Form oForm)
        {
            SAPbouiCOM.ChooseFromListCollection oCFLs;
            SAPbouiCOM.ChooseFromList oCFL;
            SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams;
            SAPbouiCOM.Conditions oCons;
            SAPbouiCOM.Condition oCon;

            oCFLs = oForm.ChooseFromLists;
            oCFLCreationParams = Globals.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

            oCFLCreationParams.MultiSelection = false;
            oCFLCreationParams.ObjectType = "37";
            oCFLCreationParams.UniqueID = "CFL2";
            oCFL = oCFLs.Add(oCFLCreationParams);
        }

        //envia los datos de la matrix y campos al formulario principal
        public static void SendData(SAPbouiCOM.Form oForm)
        {
            try
            {
                SAPbouiCOM.Matrix oMatrix = oForm.Items.Item("0_F_G").Specific;
                List<SelectedRows> ListRows = new List<SelectedRows>();

                if (oMatrix.RowCount > 0)
                {
                    for (int i = 1; i <= oMatrix.RowCount; i++)
                    {
                        if (oMatrix.IsRowSelected(i))
                        {
                            SelectedRows rows = new SelectedRows();

                            rows.Marcar = oMatrix.Columns.Item("Col_0").Cells.Item(i).Specific.Value;
                            rows.CDIT = oMatrix.Columns.Item("C_0_1").Cells.Item(i).Specific.Value;
                            rows.DSIT = oMatrix.Columns.Item("C_0_2").Cells.Item(i).Specific.Value;
                            rows.MNPU = oMatrix.Columns.Item("C_0_3").Cells.Item(i).Specific.Value;
                            rows.MNPM = oMatrix.Columns.Item("C_0_5").Cells.Item(i).Specific.Value;
                            rows.MNRU = oMatrix.Columns.Item("C_0_7").Cells.Item(i).Specific.Value;
                            rows.MNRM = oMatrix.Columns.Item("C_0_9").Cells.Item(i).Specific.Value;
                            ListRows.Add(rows);
                        }
                    }
                    oForm.Close();
                    CargarMatrixArticulos(ListRows);
                }
                else
                {
                    Globals.Error = "No existen datos en la matrix.";
                    throw new Exception(Globals.Error);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //cargar matrix en formulario principal
        public static void CargarMatrixArticulos(List<SelectedRows> ListRows)
        {
            try
            {
                SAPbouiCOM.Form oForm = default(SAPbouiCOM.Form);
                SAPbouiCOM.Matrix oMatrix = oForm.Items.Item("0_U_G").Specific;
                isLoadingItems = true;

                if (ListRows.Count>0)
                {
                    var lastRowEmpty = string.IsNullOrWhiteSpace(oMatrix.GetCellSpecific("C_0_1", oMatrix.RowCount).Value)
                        || string.IsNullOrWhiteSpace(oMatrix.GetCellSpecific("C_0_2", oMatrix.RowCount).Value);
                    for (int i = 0; ListRows.Count > i; i++)
                    {
                        //if (oMatrix.Columns.Item("C_0_1").Cells.Item(iRow).Specific.Value.ToString() != "")
                        //{
                        //    iRow++;
                        //    oMatrix.AddRow();
                        //}
                        var itemCode = ListRows[i].CDIT;
                        if (ArticuloExisteEnGrilla(oMatrix, itemCode))
                            Globals.SBO_Application.StatusBar.SetText($"Artículo: {itemCode}, se encuentra registrado en una de las lineas"
                                , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                        else
                        {
                            if (!lastRowEmpty) oMatrix.AddRow();
                            //oMatrix.Columns.Item("Col_0").Cells.Item(oMatrix.RowCount).Specific.Value = "Y";
                            oMatrix.Columns.Item("C_0_1").Cells.Item(oMatrix.RowCount).Specific.Value = itemCode;
                            oMatrix.Columns.Item("C_0_2").Cells.Item(oMatrix.RowCount).Specific.Value = ListRows[i].DSIT;
                            oMatrix.Columns.Item("C_0_3").Cells.Item(oMatrix.RowCount).Specific.Value = ListRows[i].LMPU;
                            oMatrix.Columns.Item("C_0_5").Cells.Item(oMatrix.RowCount).Specific.Value = ListRows[i].LMPM;
                            oMatrix.Columns.Item("C_0_7").Cells.Item(oMatrix.RowCount).Specific.Value = ListRows[i].PRPU;
                            oMatrix.Columns.Item("C_0_9").Cells.Item(oMatrix.RowCount).Specific.Value = ListRows[i].PRPM;
                            if (lastRowEmpty)
                            {
                                oMatrix.AddRow();
                                oMatrix.Columns.Item("C_0_1").Cells.Item(oMatrix.RowCount).Specific.Value = null;
                                oMatrix.Columns.Item("C_0_2").Cells.Item(oMatrix.RowCount).Specific.Value = null;
                            }
                        }

                    }
                    isLoadingItems = false;
                    MostrarCantidadRegistros(oForm);
                    Globals.Release(Globals.oRec);
                }

            }
            catch (Exception ex)
            {
                isLoadingItems = false;
                throw ex;
            }
        }


        //abrir nuevo form
        public static void OpenFrmBusqueda(string search)
        {
            try
            {
                SAPbouiCOM.Form oForm = default(SAPbouiCOM.Form);
                try
                {
                    oForm = Globals.SBO_Application.Forms.Item("FrmBusqueda");
                    Globals.SBO_Application.MessageBox("El formulario ya se encuentra abierto.");
                }catch
                {
                    SAPbouiCOM.FormCreationParams fcp = default(SAPbouiCOM.FormCreationParams);
                    fcp = Globals.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
                    fcp.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Sizable;
                    fcp.FormType = "FrmBusqueda";
                    fcp.UniqueID = "FrmBusqueda";
                    string FormName = "\\FrmBusqueda.srf";
                    fcp.XmlData = Globals.LoadFromXML(ref FormName);
                    oForm = Globals.SBO_Application.Forms.AddEx(fcp);
                    oForm.Top = 50;
                    oForm.Left = 345;
                    oForm.Visible = true;
                    //agregamos busqueda de articulos a la matrix
                    try
                    {

                        int lLimaPU = 0, lLimaPxM = 0, lProvPU = 0, lProvPxM = 0;
                        string sMonLimaPU = "", sMonLimaPxM = "", sMonProvPU = "", sMonProvPxM = "";
                        ObtenerListas(ref lLimaPU, ref lLimaPxM, ref lProvPU, ref lProvPxM);
                        sMonLimaPU = ObtenerMonedaLista(lLimaPU);
                        sMonLimaPxM = ObtenerMonedaLista(lLimaPxM);
                        sMonProvPU = ObtenerMonedaLista(lProvPU);
                        sMonProvPxM = ObtenerMonedaLista(lProvPxM);

                        SAPbouiCOM.Matrix oMatrix = oForm.Items.Item("0_F_G").Specific;
                        SAPbouiCOM.Column oColumn1, oColumn2, oColumn3, oColumn4, oColumn5, oColumn6, oColumn7, oColumn8, oColumn9, oColumn10, oColumn0;
                        SAPbouiCOM.UserDataSource Marcar, CDIT, DSIT, MNPU, LMPU, MNPM, LMPM, MNRU, PRPU, MNRM, PRPM;

                        Marcar = oForm.DataSources.UserDataSources.Add("Marcar", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
                        CDIT = oForm.DataSources.UserDataSources.Add("CDIT", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 100);
                        DSIT = oForm.DataSources.UserDataSources.Add("DSIT", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 100);
                        MNPU = oForm.DataSources.UserDataSources.Add("MNPU", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 100);
                        LMPU = oForm.DataSources.UserDataSources.Add("LMPU", SAPbouiCOM.BoDataType.dt_PRICE);
                        MNPM = oForm.DataSources.UserDataSources.Add("MNPM", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 100);
                        LMPM = oForm.DataSources.UserDataSources.Add("LMPM", SAPbouiCOM.BoDataType.dt_PRICE, 100);
                        MNRU = oForm.DataSources.UserDataSources.Add("MNRU", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 100);
                        PRPU = oForm.DataSources.UserDataSources.Add("PRPU", SAPbouiCOM.BoDataType.dt_PRICE, 100);
                        MNRM = oForm.DataSources.UserDataSources.Add("MNRM", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 100);
                        PRPM = oForm.DataSources.UserDataSources.Add("PRPM", SAPbouiCOM.BoDataType.dt_PRICE, 100);

                        oColumn0 = (SAPbouiCOM.Column)oMatrix.Columns.Item("Col_0");
                        oColumn0.DataBind.SetBound(true, "", "Marcar");
                        oColumn1 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_1");
                        oColumn1.DataBind.SetBound(true, "", "CDIT");
                        oColumn2 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_2");
                        oColumn2.DataBind.SetBound(true, "", "DSIT");
                        oColumn3 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_3");
                        oColumn3.DataBind.SetBound(true, "", "MNPU");
                        oColumn4 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_4");
                        oColumn4.DataBind.SetBound(true, "", "LMPU");
                        oColumn5 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_5");
                        oColumn5.DataBind.SetBound(true, "", "MNPM");
                        oColumn6 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_6");
                        oColumn6.DataBind.SetBound(true, "", "LMPM");
                        oColumn7 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_7");
                        oColumn7.DataBind.SetBound(true, "", "MNRU");
                        oColumn8 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_8");
                        oColumn8.DataBind.SetBound(true, "", "PRPU");
                        oColumn9 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_9");
                        oColumn9.DataBind.SetBound(true, "", "MNRM");
                        oColumn10 = (SAPbouiCOM.Column)oMatrix.Columns.Item("C_0_10");
                        oColumn10.DataBind.SetBound(true, "", "PRPM");


                        Globals.Query = "SELECT \"ItemCode\",\"ItemName\" FROM OITM WHERE UPPER(U_EXW_FILTRO) LIKE '%" + search.ToUpper() + "%'";
                        Globals.RunQuery(Globals.Query);


                        int iRecord = Globals.oRec.RecordCount;
                        int iRow = oMatrix.RowCount;
                        //for (int i = 0; i < iRecord; i++)
                        //{
                        //    oMatrix.AddRow();
                        //}
                        while (!Globals.oRec.EoF)
                        {
                            var itemCode = Globals.oRec.Fields.Item(0).Value.ToString();
                            if (ArticuloExisteEnGrilla(oMatrix, itemCode))
                                Globals.SBO_Application.StatusBar.SetText($"Artículo: { itemCode}, se encuentra registrado en una de las lineas"
                                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                            else
                            {
                                Marcar.Value = "Y";
                                CDIT.Value = itemCode;
                                DSIT.Value = Globals.oRec.Fields.Item(1).Value.ToString();
                                MNPU.Value = sMonLimaPU;
                                MNPM.Value = sMonLimaPxM;
                                MNRU.Value = sMonProvPU;
                                MNRM.Value = sMonProvPxM;
                                oMatrix.AddRow();
                            }

                            iRow++;
                            Globals.oRec.MoveNext();
                        }
                        isLoadingItems = false;
                        Globals.Release(Globals.oRec);
                    }
                    catch (Exception ex)
                    {
                        isLoadingItems = false;
                        throw ex;
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public static void CargarArticulos(SAPbouiCOM.Form oForm)
        {
            try
            {
                ValidarRegistro(oForm);

                int lLimaPU = 0, lLimaPxM = 0, lProvPU = 0, lProvPxM = 0;
                string sMonLimaPU = "", sMonLimaPxM = "", sMonProvPU = "", sMonProvPxM = "";
                ObtenerListas(ref lLimaPU, ref lLimaPxM, ref lProvPU, ref lProvPxM);
                sMonLimaPU = ObtenerMonedaLista(lLimaPU);
                sMonLimaPxM = ObtenerMonedaLista(lLimaPxM);
                sMonProvPU = ObtenerMonedaLista(lProvPU);
                sMonProvPxM = ObtenerMonedaLista(lProvPxM);

                SAPbouiCOM.Matrix oMatrix = oForm.Items.Item("0_U_G").Specific;
                SAPbouiCOM.EditText eArt = oForm.Items.Item("Item_3").Specific;
          
                if (eArt.Value.ToString() == "")
                {
                    throw new Exception("El campo 'Artículo' no puede estar vacío. Favor de revisar.");
                }

                isLoadingItems = true;

                string search = eArt.Value.ToString();
                //
                OpenFrmBusqueda(search);
                //

                /*Globals.Query = "SELECT \"ItemCode\",\"ItemName\" FROM OITM WHERE UPPER(U_EXW_FILTRO) LIKE '%" + eArt.Value.ToString().ToUpper() + "%'";
                Globals.RunQuery(Globals.Query);

                int iRecord = Globals.oRec.RecordCount;
                int iRow = oMatrix.RowCount;
                //for (int i = 0; i < iRecord; i++)
                //{
                //    oMatrix.AddRow();
                //}
                var lastRowEmpty = string.IsNullOrWhiteSpace(oMatrix.GetCellSpecific("C_0_1", oMatrix.RowCount).Value)
                    || string.IsNullOrWhiteSpace(oMatrix.GetCellSpecific("C_0_2", oMatrix.RowCount).Value);
                while (!Globals.oRec.EoF)
                {
                    //if (oMatrix.Columns.Item("C_0_1").Cells.Item(iRow).Specific.Value.ToString() != "")
                    //{
                    //    iRow++;
                    //    oMatrix.AddRow();
                    //}
                    var itemCode = Globals.oRec.Fields.Item(0).Value.ToString();
                    if (ArticuloExisteEnGrilla(oMatrix, itemCode))
                        Globals.SBO_Application.StatusBar.SetText($"Artículo: { itemCode}, se encuentra registrado en una de las lineas"
                            , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    else
                    {
                        if (!lastRowEmpty) oMatrix.AddRow();
                        //oMatrix.Columns.Item("Col_0").Cells.Item(oMatrix.RowCount).Specific.Value = "Y";
                        oMatrix.Columns.Item("C_0_1").Cells.Item(oMatrix.RowCount).Specific.Value = itemCode;
                        oMatrix.Columns.Item("C_0_2").Cells.Item(oMatrix.RowCount).Specific.Value = Globals.oRec.Fields.Item(1).Value.ToString();
                        oMatrix.Columns.Item("C_0_3").Cells.Item(oMatrix.RowCount).Specific.Value = sMonLimaPU;
                        oMatrix.Columns.Item("C_0_5").Cells.Item(oMatrix.RowCount).Specific.Value = sMonLimaPxM;
                        oMatrix.Columns.Item("C_0_7").Cells.Item(oMatrix.RowCount).Specific.Value = sMonProvPU;
                        oMatrix.Columns.Item("C_0_9").Cells.Item(oMatrix.RowCount).Specific.Value = sMonProvPxM;
                        if (lastRowEmpty)
                        {
                            oMatrix.AddRow();
                            oMatrix.Columns.Item("C_0_1").Cells.Item(oMatrix.RowCount).Specific.Value = null;
                            oMatrix.Columns.Item("C_0_2").Cells.Item(oMatrix.RowCount).Specific.Value = null;
                        }
                    }

                    iRow++;
                    Globals.oRec.MoveNext();
                }*/
                isLoadingItems = false;
                MostrarCantidadRegistros(oForm);
                Globals.Release(Globals.oRec);
            }
            catch (Exception ex)
            {
                isLoadingItems = false;
                throw ex;
            }
        }

        public static string ObtenerMonedaLista(int iList)
        {
            try
            {
                string sMoneda = "";

                Globals.Query = "SELECT \"PrimCurr\" FROM OPLN WHERE \"ListNum\" = " + iList.ToString();
                Globals.RunQuery(Globals.Query);
                sMoneda = Globals.oRec.Fields.Item(0).Value.ToString();
                Globals.Release(Globals.oRec);

                return sMoneda;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ActualizarPrecios(SAPbouiCOM.Form oForm)
        {
            try
            {
                int iTotal = 0, iProc = 0;
                int lLimaPU = 0, lLimaPxM = 0, lProvPU = 0, lProvPxM = 0;
                double PLimaPU= 0, PLimaPxM =0, PProvPU = 0, PProvPxM = 0;
                ObtenerListas(ref lLimaPU, ref lLimaPxM, ref lProvPU, ref lProvPxM);
                SAPbouiCOM.Matrix oMatrix = oForm.Items.Item("0_U_G").Specific;
                // Precios Globales para asignar a las lineas
                PLimaPU = double.Parse(oForm.Items.Item("Item_0").Specific.Value.ToString() == "" ? "0" : oForm.Items.Item("Item_0").Specific.Value.ToString());
                PLimaPxM = double.Parse(oForm.Items.Item("Item_4").Specific.Value.ToString() == "" ? "0" : oForm.Items.Item("Item_4").Specific.Value.ToString());
                PProvPU = double.Parse(oForm.Items.Item("Item_5").Specific.Value.ToString() == "" ? "0" : oForm.Items.Item("Item_5").Specific.Value.ToString());
                PProvPxM = double.Parse(oForm.Items.Item("Item_6").Specific.Value.ToString() == "" ? "0" : oForm.Items.Item("Item_6").Specific.Value.ToString());

                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    if (oMatrix.Columns.Item("C_0_1").Cells.Item(i).Specific.Value.ToString() == "") continue;

                    //checkbox agregado para solo actualizar los seleccionados
                    SAPbouiCOM.CheckBox oChkBox = (SAPbouiCOM.CheckBox)oMatrix.Columns.Item("Col_0").Cells.Item(i).Specific;
                    if (oChkBox.Checked == true )
                    {
                        SAPbobsCOM.Items oItems = (SAPbobsCOM.Items)Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
                        if (oItems.GetByKey(oMatrix.Columns.Item("C_0_1").Cells.Item(i).Specific.Value.ToString()))
                        {
                            bool bUpd = false;
                            int iListaActualizadas = 0;
                            for (int ilist = 0; ilist < oItems.PriceList.Count; ilist++)
                            {
                                if (iListaActualizadas == 4)
                                {
                                    break;
                                }
                                string sNumero = "";
                                oItems.PriceList.SetCurrentLine(ilist);
                                if (oItems.PriceList.PriceList == lLimaPU)
                                {
                                 
                                    sNumero = oMatrix.Columns.Item("C_0_4").Cells.Item(i).Specific.Value.ToString() == "" ? "0" : oMatrix.Columns.Item("C_0_4").Cells.Item(i).Specific.Value.ToString();
                                    if (double.Parse(sNumero) != 0 || PLimaPU != 0)
                                    {
                                        oItems.PriceList.Currency = oMatrix.Columns.Item("C_0_3").Cells.Item(i).Specific.Value.ToString();
                                        oItems.PriceList.Price = PLimaPU != 0 ? PLimaPU : double.Parse(sNumero);
                                        bUpd = true;
                                        iListaActualizadas++;
                                    }

                                }
                                else if (oItems.PriceList.PriceList == lLimaPxM)
                                {
                                    sNumero = oMatrix.Columns.Item("C_0_6").Cells.Item(i).Specific.Value.ToString() == "" ? "0" : oMatrix.Columns.Item("C_0_6").Cells.Item(i).Specific.Value.ToString();
                                    if (double.Parse(sNumero) != 0 || PLimaPxM != 0)
                                    {
                                        oItems.PriceList.Currency = oMatrix.Columns.Item("C_0_5").Cells.Item(i).Specific.Value.ToString();
                                        oItems.PriceList.Price = PLimaPxM != 0 ? PLimaPxM : double.Parse(sNumero);
                                        bUpd = true;
                                        iListaActualizadas++;
                                    }

                                }
                                else if (oItems.PriceList.PriceList == lProvPU)
                                {
                                    sNumero = oMatrix.Columns.Item("C_0_8").Cells.Item(i).Specific.Value.ToString() == "" ? "0" : oMatrix.Columns.Item("C_0_8").Cells.Item(i).Specific.Value.ToString();
                                    if (double.Parse(sNumero) != 0 || PProvPU != 0)
                                    {
                                        oItems.PriceList.Currency = oMatrix.Columns.Item("C_0_7").Cells.Item(i).Specific.Value.ToString();
                                        oItems.PriceList.Price = PProvPU != 0 ? PProvPU : double.Parse(sNumero);
                                        bUpd = true;
                                        iListaActualizadas++;
                                    }
                                }
                                else if (oItems.PriceList.PriceList == lProvPxM)
                                {
                                    sNumero = oMatrix.Columns.Item("C_0_10").Cells.Item(i).Specific.Value.ToString() == "" ? "0" : oMatrix.Columns.Item("C_0_10").Cells.Item(i).Specific.Value.ToString();
                                    if (double.Parse(sNumero) != 0 || PProvPxM != 0)
                                    {
                                        oItems.PriceList.Currency = oMatrix.Columns.Item("C_0_9").Cells.Item(i).Specific.Value.ToString();
                                        oItems.PriceList.Price = PProvPxM != 0 ? PProvPxM : double.Parse(sNumero);
                                        bUpd = true;
                                        iListaActualizadas++;
                                    }
                                }
                            }

                            if (bUpd)
                            {
                                if (oItems.Update().Equals(0))
                                {
                                    Globals.SBO_Application.SetStatusBarMessage("Línea  " + i + " actualizada con éxito.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                                    iProc++;
                                }
                                else
                                {

                                    Globals.oCompany.GetLastError(out Globals.lRetCode, out Globals.sErrMsg);
                                    Globals.SBO_Application.SetStatusBarMessage("Error al actualizar línea " + i + ": " + Globals.sErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, true);
                                }
                            }
                            else
                                Globals.SBO_Application.StatusBar.SetText($"El artículo: {oMatrix.Columns.Item("C_0_1").Cells.Item(i).Specific.Value}, no tiene asignado un precio en ninguna de las listas"
                                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                        }

                        Globals.Release(oItems);
                        iTotal++;
                    }                    
                }
                Globals.SBO_Application.MessageBox("Proceso culminado.\nActualizados correctamente " + iProc + "/" + iTotal);

                oForm.Items.Item("Item_0").Specific.value =0;
                oForm.Items.Item("Item_4").Specific.value =0;
                oForm.Items.Item("Item_5").Specific.value = 0;
                oForm.Items.Item("Item_6").Specific.value = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ObtenerListas(ref int lLimaPU, ref int lLimaPxM, ref int lProvPU, ref int lProvPxM)
        {
            try
            {
                //Validar Lima P.U
                Globals.Query = "SELECT \"ListNum\" FROM OPLN WHERE COALESCE(U_EXX_AUPP_TCTA,'-1')= '1' ";
                Globals.RunQuery(Globals.Query);
                lLimaPU = Int32.Parse(Globals.oRec.Fields.Item(0).Value.ToString());
                Globals.Release(Globals.oRec);

                //Validar Lima PxM
                Globals.Query = "SELECT \"ListNum\" FROM OPLN WHERE COALESCE(U_EXX_AUPP_TCTA,'-1')= '2' ";
                Globals.RunQuery(Globals.Query);
                lLimaPxM = Int32.Parse(Globals.oRec.Fields.Item(0).Value.ToString());
                Globals.Release(Globals.oRec);

                //Validar Pronvincia P.U
                Globals.Query = "SELECT \"ListNum\" FROM OPLN WHERE COALESCE(U_EXX_AUPP_TCTA,'-1')= '3' ";
                Globals.RunQuery(Globals.Query);
                lProvPU = Int32.Parse(Globals.oRec.Fields.Item(0).Value.ToString());
                Globals.Release(Globals.oRec);

                //Validar Provincia PxM
                Globals.Query = "SELECT \"ListNum\" FROM OPLN WHERE COALESCE(U_EXX_AUPP_TCTA,'-1')= '4' ";
                Globals.RunQuery(Globals.Query);
                lProvPxM = Int32.Parse(Globals.oRec.Fields.Item(0).Value.ToString());
                Globals.Release(Globals.oRec);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static void ValidarMaestroListas(SAPbouiCOM.Form oForm)
        {
            try
            {
                SAPbouiCOM.Matrix oMatrix = oForm.Items.Item("3").Specific;
                int iLimaPU = 0, iLimaPxM = 0, iProvPU = 0, iProvPxM = 0;

                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    string sTipo = oMatrix.Columns.Item("U_EXX_AUPP_TCTA").Cells.Item(i).Specific.Value.ToString();
                    switch (sTipo)
                    {
                        case "1":
                            iLimaPU++;
                            break;
                        case "2":
                            iLimaPxM++;
                            break;
                        case "3":
                            iProvPU++;
                            break;
                        case "4":
                            iProvPxM++;
                            break;
                    }
                }

                if (iLimaPU > 1)
                {
                    throw new Exception("No puede existir mas de una lista de precios configurada como 'Lima P.U.'");
                }
                if (iLimaPxM > 1)
                {
                    throw new Exception("No puede existir mas de una lista de precios configurada como 'Lima PxM'.");
                }
                if (iProvPU > 1)
                {
                    throw new Exception("No puede existir mas de una lista de precios configurada como 'Provincia P.U.'");
                }
                if (iProvPxM > 1)
                {
                    throw new Exception("No puede existir mas de una lista de precios configurada como 'Provincia PxM'.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ValidarRegistro(SAPbouiCOM.Form oForm)
        {
            try
            {
                int iCont = 0;
                //Validar Lima P.U
                Globals.Query = "SELECT COUNT(1) FROM OPLN WHERE COALESCE(U_EXX_AUPP_TCTA,'-1')= '1' ";
                Globals.RunQuery(Globals.Query);
                iCont = Int32.Parse(Globals.oRec.Fields.Item(0).Value.ToString());
                Globals.Release(Globals.oRec);

                if (iCont == 0)
                {
                    throw new Exception("Debe existir al menos una lista de precios configurada como 'Lima P.U.'");
                }
                else if (iCont > 1)
                {
                    throw new Exception("No puede existir mas de una lista de precios configurada como 'Lima P.U.'");
                }

                //Validar Lima PxM
                Globals.Query = "SELECT COUNT(1) FROM OPLN WHERE COALESCE(U_EXX_AUPP_TCTA,'-1')= '2' ";
                Globals.RunQuery(Globals.Query);
                iCont = Int32.Parse(Globals.oRec.Fields.Item(0).Value.ToString());
                Globals.Release(Globals.oRec);

                if (iCont == 0)
                {
                    throw new Exception("Debe existir al menos una lista de precios configurada como 'Lima PxM'.");
                }
                else if (iCont > 1)
                {
                    throw new Exception("No puede existir mas de una lista de precios configurada como 'Lima PxM'.");
                }

                //Validar Pronvincia P.U
                Globals.Query = "SELECT COUNT(1) FROM OPLN WHERE COALESCE(U_EXX_AUPP_TCTA,'-1')= '3' ";
                Globals.RunQuery(Globals.Query);
                iCont = Int32.Parse(Globals.oRec.Fields.Item(0).Value.ToString());
                Globals.Release(Globals.oRec);

                if (iCont == 0)
                {
                    throw new Exception("Debe existir al menos una lista de precios configurada como 'Provincia P.U.'");
                }
                else if (iCont > 1)
                {
                    throw new Exception("No puede existir mas de una lista de precios configurada como 'Provincia P.U.'");
                }

                //Validar Provincia PxM
                Globals.Query = "SELECT COUNT(1) FROM OPLN WHERE COALESCE(U_EXX_AUPP_TCTA,'-1')= '4' ";
                Globals.RunQuery(Globals.Query);
                iCont = Int32.Parse(Globals.oRec.Fields.Item(0).Value.ToString());
                Globals.Release(Globals.oRec);

                if (iCont == 0)
                {
                    throw new Exception("Debe existir al menos una lista de precios configurada como 'Provincia PxM'.");
                }
                else if (iCont > 1)
                {
                    throw new Exception("No puede existir mas de una lista de precios configurada como 'Provincia PxM'.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CargarFile(SAPbouiCOM.Form oForm)
        {
            try
            {
                SAPbouiCOM.EditText oPath = oForm.Items.Item("20_U_E").Specific;

                if (oPath.Value.ToString() == "")
                {
                    throw new Exception("Debe ingresar un archivo para la carga.");
                }
                else
                {
                    LoadTXTFile(oPath.Value.ToString(), oForm);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LoadTXTFile(string filename, SAPbouiCOM.Form oForm)
        {
            try
            {
                SAPbouiCOM.Matrix oMatrix = oForm.Items.Item("0_U_G").Specific;
                var lstCodItems = new List<string>();
                var lstDscError = new List<string>();
                var msjLog = string.Empty;

                string line;
                string[] parts;
                int RecordLines = 0;
                var xmlMtx = oMatrix.SerializeAsXML(SAPbouiCOM.BoMatrixXmlSelect.mxs_All);

                isLoadingItems = true;

                using (StreamReader file = new StreamReader(filename))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        RecordLines++;
                    }
                    file.Close();
                }
                //oForm.Freeze(true);

                var lastRowEmpty = string.IsNullOrWhiteSpace(oMatrix.GetCellSpecific("C_0_1", oMatrix.RowCount).Value)
                    || string.IsNullOrWhiteSpace(oMatrix.GetCellSpecific("C_0_2", oMatrix.RowCount).Value);

                using (StreamReader file = new StreamReader(filename))
                {
                    //iRows = 1;
                    line = file.ReadLine();
                    line = file.ReadLine();
                    Globals.iRows = oMatrix.RowCount;
                    var lineNum = 1;
                    //for (int i = 0; i < RecordLines - 2; i++)
                    //{
                    //    //oMatrix.AddRow();
                    //}
                    //oMatrix.Clear();
                    while ((line = file.ReadLine()) != null)
                    {
                        char[] delimiters = new char[] { '\t' };
                        parts = line.Split(delimiters, StringSplitOptions.None);
                        parts.ToString();
                        //capturo el codigo del articulo
                        if (!ArticuloExisteEnSAP(parts[0]))
                        {
                            //lstDscError.Add($"Artículo: {parts[0]}, no existe en SAP");
                            Globals.SBO_Application.StatusBar.SetText($"Artículo: {parts[0]}, no existe en SAP", SAPbouiCOM.BoMessageTime.bmt_Short,
                                SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                            continue;
                        }
                        else if (ArticuloExisteEnGrilla(oMatrix, parts[0]))
                        {
                            //lstDscError.Add($"Artículo: {parts[0]}, se encuentra registrado en una de las lineas");
                            Globals.SBO_Application.StatusBar.SetText($"Artículo: { parts[0]}, se encuentra registrado en una de las lineas"
                                , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                            continue;
                        }
                        lstCodItems.Add(parts[0]);
                        if (!lastRowEmpty) oMatrix.AddRow();
                        AgregarLineaMatrix(parts, oMatrix, lineNum);
                        if (lastRowEmpty)
                        {
                            oMatrix.AddRow();
                            oMatrix.Columns.Item("C_0_1").Cells.Item(oMatrix.RowCount).Specific.Value = null;
                            oMatrix.Columns.Item("C_0_2").Cells.Item(oMatrix.RowCount).Specific.Value = null;
                        }

                        Globals.iRows++;
                        lineNum++;
                    }
                    file.Close();
                    isLoadingItems = false;
                    //((SAPbouiCOM.EditText)oForm.Items.Item("edtCntReg").Specific).Value = (oMatrix.RowCount - 1).ToString();
                    msjLog = $"Proceso culminado \nRegistros cargados correctamente:{lineNum - 1}/{RecordLines - 2}";
                    /* lstDscError.ForEach(m =>
                     {
                         msjLog += "\n" + m;
                     });
                     */
                    MostrarCantidadRegistros(oForm);
                    Globals.SBO_Application.MessageBox(msjLog);
                    Globals.SBO_Application.SetStatusBarMessage("Archivo cargado con éxito.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                isLoadingItems = false;
                oForm.Freeze(false);
            }

        }

        public static void AgregarLineaMatrix(string[] parts, SAPbouiCOM.Matrix oMatrix, int iRows)
        {
            try
            {
                //if (oMatrix.Columns.Item("C_0_1").Cells.Item(iRows).Specific.Value.ToString() != "")
                //{
                //    iRows++;
                //}
                var maxMatrixLineNum = oMatrix.RowCount;
                SAPbouiCOM.CheckBox oChkBox = (SAPbouiCOM.CheckBox)oMatrix.Columns.Item("Col_0").Cells.Item(maxMatrixLineNum).Specific;
                oChkBox.Checked = true;
                oMatrix.Columns.Item("C_0_1").Cells.Item(maxMatrixLineNum).Specific.Value = parts[0].ToString();
                oMatrix.Columns.Item("C_0_2").Cells.Item(maxMatrixLineNum).Specific.Value = parts[1].ToString();
                oMatrix.Columns.Item("C_0_3").Cells.Item(maxMatrixLineNum).Specific.Value = parts[2].ToString();
                oMatrix.Columns.Item("C_0_4").Cells.Item(maxMatrixLineNum).Specific.Value = double.Parse(parts[3].ToString());
                oMatrix.Columns.Item("C_0_5").Cells.Item(maxMatrixLineNum).Specific.Value = parts[4].ToString();
                oMatrix.Columns.Item("C_0_6").Cells.Item(maxMatrixLineNum).Specific.Value = double.Parse(parts[5].ToString());
                oMatrix.Columns.Item("C_0_7").Cells.Item(maxMatrixLineNum).Specific.Value = parts[6].ToString();
                oMatrix.Columns.Item("C_0_8").Cells.Item(maxMatrixLineNum).Specific.Value = double.Parse(parts[7].ToString());
                oMatrix.Columns.Item("C_0_9").Cells.Item(maxMatrixLineNum).Specific.Value = parts[8].ToString();
                oMatrix.Columns.Item("C_0_10").Cells.Item(maxMatrixLineNum).Specific.Value = double.Parse(parts[9].ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static void LoadForm()
        {
            SAPbouiCOM.MenuItem menu = Globals.SBO_Application.Menus.Item("47616");
            try
            {
                if (menu.SubMenus.Count > 0)
                {
                    for (int i = 0; i < menu.SubMenus.Count; i++)
                    {
                        if (menu.SubMenus.Item(i).String.Contains("AUPP_LSPR"))
                        {
                            menu.SubMenus.Item(i).Activate();

                            var frmAux = (SAPbouiCOM.Form)Globals.SBO_Application.Forms.ActiveForm;
                            frmAux.AutoManaged = true;

                            ((SAPbouiCOM.EditText)frmAux.Items.Item("edtCntReg").Specific).Value = "0";

                            frmAux.Items.Item("Item_3").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_All, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                            frmAux.Items.Item("Item_3").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_Add, SAPbouiCOM.BoModeVisualBehavior.mvb_True);

                            frmAux.Items.Item("btnBuscar").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_All, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                            frmAux.Items.Item("btnBuscar").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_Add, SAPbouiCOM.BoModeVisualBehavior.mvb_True);

                            frmAux.Items.Item("btnFile").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_All, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                            frmAux.Items.Item("btnFile").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_Add, SAPbouiCOM.BoModeVisualBehavior.mvb_True);

                            frmAux.Items.Item("0_U_G").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_All, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                            frmAux.Items.Item("0_U_G").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_Add, SAPbouiCOM.BoModeVisualBehavior.mvb_True);

                            frmAux.Items.Item("btnCargar").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_All, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                            frmAux.Items.Item("btnCargar").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_Add, SAPbouiCOM.BoModeVisualBehavior.mvb_True);

                            frmAux.Items.Item("edtCntReg").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, (int)SAPbouiCOM.BoAutoFormMode.afm_All, SAPbouiCOM.BoModeVisualBehavior.mvb_False);

                            SetEditableColumnsMatrix(frmAux, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.SetStatusBarMessage("ERROR: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, true);
            }
        }

        public static void LoadFormEstr()
        {
            try
            {
                SAPbouiCOM.Form oForm = default(SAPbouiCOM.Form);
                try
                {
                    oForm = Globals.SBO_Application.Forms.Item("FormEst_2");
                    Globals.SBO_Application.MessageBox("El formulario ya se encuentra abierto.");
                }
                catch //(Exception ex)
                {
                    SAPbouiCOM.FormCreationParams fcp = default(SAPbouiCOM.FormCreationParams);
                    fcp = Globals.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
                    fcp.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Sizable;
                    fcp.FormType = "FormEst";
                    fcp.UniqueID = "FormEst_2";
                    string FormName = "\\FormEst.srf";
                    fcp.XmlData = Globals.LoadFromXML(ref FormName);
                    oForm = Globals.SBO_Application.Forms.AddEx(fcp);
                }
                oForm.Top = 50;
                oForm.Left = 345;
                oForm.Visible = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void OpenFile(SAPbouiCOM.Form oForm)
        {
            try
            {
                Globals.OpenFile(oForm, "20_U_E");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool ArticuloExisteEnSAP(string itemCode)
        {
            var sqlQry = $"select 'A' from OITM where \"ItemCode\" = '{itemCode}'";
            return !Globals.RunQuery(sqlQry).EoF;
        }

        private static bool MonedaExisteEnSAP(string currCode)
        {
            var sqlQry = $"select 'A' from OCRN where \"CurrCode\" = '{currCode}'";
            return !Globals.RunQuery(sqlQry).EoF;
        }

        private static bool ArticuloExisteEnGrilla(SAPbouiCOM.Matrix matrix, string itemCode)
        {
            for (int i = 0; i < matrix.RowCount; i++)
                if (matrix.GetCellSpecific("C_0_1", i + 1).Value == itemCode) return true;
            return false;
        }


        public static void MostrarCantidadRegistros(SAPbouiCOM.Form oForm)
        {
            var cntReg = 0;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
                var edtCntReg = (SAPbouiCOM.EditText)oForm.Items.Item("edtCntReg").Specific;
                var mtxItems = (SAPbouiCOM.Matrix)oForm.Items.Item("0_U_G").Specific;

                for (int i = 0; i < mtxItems.RowCount; i++)
                    if (!string.IsNullOrWhiteSpace(mtxItems.GetCellSpecific("C_0_1", i + 1).Value)) cntReg++;
                edtCntReg.Value = cntReg.ToString();
                oForm.DataSources.DBDataSources.Item("@EXX_AUPP_LSPR").SetValue("U_EXX_AUPP_CNTREG", 0, cntReg.ToString());
            });
        }

        public static void OnClickAddMenu(SAPbouiCOM.Form oForm)
        {
            SetEditableColumnsMatrix(oForm, true);
        }

        private static void SetEditableColumnsMatrix(SAPbouiCOM.Form oForm, bool editable)
        {
            var matrix = (SAPbouiCOM.Matrix)oForm.Items.Item("0_U_G").Specific;
            matrix.Columns.Item("C_0_1").Editable = editable;
            matrix.Columns.Item("C_0_2").Editable = editable;
            matrix.Columns.Item("C_0_3").Editable = editable;
            matrix.Columns.Item("C_0_4").Editable = editable;
            matrix.Columns.Item("C_0_5").Editable = editable;
            matrix.Columns.Item("C_0_6").Editable = editable;
            matrix.Columns.Item("C_0_7").Editable = editable;
            matrix.Columns.Item("C_0_8").Editable = editable;
            matrix.Columns.Item("C_0_9").Editable = editable;
            matrix.Columns.Item("C_0_10").Editable = editable;
        }
    }
}
