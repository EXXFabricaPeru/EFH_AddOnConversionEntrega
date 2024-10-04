using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOnUpdPrice.App;

namespace AddOnUpdPrice.DB_Structure
{
    class CreateStructure
    {
        public static void CreateStruct()
        {

            MDFields oMDFields = new MDFields();
            MDTables oMDTables = new MDTables();

            //oMDFields.CreateRegularField("OBPL", "EXX_ABPC_CEMP", "Codigo Empresa BPC", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50,
            // SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");

            //Tablas Nuevas 
            oMDTables.CreateTableMD("EXX_AUPP_LSPR", "EXX - Lista de Precios", SAPbobsCOM.BoUTBTableType.bott_Document);
            oMDTables.CreateTableMD("EXX_AUPP_SPR1", "EXX - Lista de Precios Detalle", SAPbobsCOM.BoUTBTableType.bott_DocumentLines);
            
            #region CamposTabla1
            oMDFields.CreateRegularField("EXX_AUPP_LSPR", "EXX_AUPP_ARCH", "Archivo", SAPbobsCOM.BoFieldTypes.db_Memo, SAPbobsCOM.BoFldSubTypes.st_None, 0,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_CDIT", "Código Item", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 20,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_DSIT", "Descripción Item", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 100,
                 SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_MNPU", "Moneda Lima P.U", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 3,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_LMPU", "Lima P.U", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Price, 0,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_MNPM", "Moneda Lima PxM", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 3,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_LMPM", "Lima PxM", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Price, 100,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_MNRU", "Moneda Provincia P.U", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 3,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_PRPU", "Provincia P.U", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Price, 0,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_MNRM", "Moneda Pronvincia PxM", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 3,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            oMDFields.CreateRegularField("EXX_AUPP_SPR1", "EXX_AUPP_PRPM", "Pronvincia PxM", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Price, 0,
                SAPbobsCOM.BoYesNoEnum.tNO, null, null, null, null, "");
            #endregion          

            #region CamposOPLN
            oMDFields.CreateRegularField("OPLN", "EXX_AUPP_TCTA", "Tipo de Lista Precios", SAPbobsCOM.BoFieldTypes.db_Alpha,
              SAPbobsCOM.BoFldSubTypes.st_None, 1, SAPbobsCOM.BoYesNoEnum.tNO, null, null,
              new string[] { "0", "1","2","3","4" },
              new string[] { "Ninguno","Lima P.U", "Lima PxM", "Provincia P.U", "Provincia PxM" }, "0");
            #endregion

            if (!Globals.ExisteUDO("EXX_AUPP_LSPR"))
            {
                RegisterEXXLSPR();
            }

        }

        public static void RegisterEXXLSPR()
        {
            Globals.SBO_Application.ActivateMenuItem("4879");
            SAPbouiCOM.Form oForm = Globals.SBO_Application.Forms.ActiveForm;
            string title = oForm.Title;
            #region Step1
            oForm.Items.Item("4").Click();
            oForm.Items.Item("4").Click();
            SAPbouiCOM.EditText oEditCode = oForm.Items.Item("16").Specific;
            oEditCode.Value = "EXX_AUPP_LSPR";
            SAPbouiCOM.EditText oEditName = oForm.Items.Item("18").Specific;
            oEditName.Value = "EXX - Lista de Precios";
            SAPbouiCOM.ComboBox oComboType = oForm.Items.Item("20").Specific;
            oComboType.Select("3");
            SAPbouiCOM.EditText oEditTableName = oForm.Items.Item("62").Specific;
            oEditTableName.Value = "EXX_AUPP_LSPR";
            oForm.Items.Item("4").Click();
            #endregion
            #region Step2
            SAPbouiCOM.CheckBox oCheckFind = oForm.Items.Item("30").Specific;
            oCheckFind.Checked = true;
            SAPbouiCOM.CheckBox oCheckLog = oForm.Items.Item("35").Specific;
            oCheckLog.Checked = true;
            oForm.Items.Item("4").Click();
            #endregion
            #region Step3
            SAPbouiCOM.CheckBox oCheckDefForm = oForm.Items.Item("37").Specific;
            oCheckDefForm.Checked = true;
            SAPbouiCOM.OptionBtn oOptnNewForm = oForm.Items.Item("1250000092").Specific;
            oOptnNewForm.Selected = true;
            //SAPbouiCOM.CheckBox oCheckMenuItem = oForm.Items.Item("1250000084").Specific;
            //oCheckMenuItem.Checked = true;
            //SAPbouiCOM.EditText oEditMenuCpt = oForm.Items.Item("1250000085").Specific;
            //oEditMenuCpt.Value = "AsientosControl";
            //SAPbouiCOM.EditText oEditFather = oForm.Items.Item("1250000088").Specific;
            //oEditFather.Value = "43526";
            //SAPbouiCOM.EditText oEditPositon = oForm.Items.Item("1250000090").Specific;
            //oEditPositon.Value = "3";
            //SAPbouiCOM.EditText oEditUID = oForm.Items.Item("1250000094").Specific;
            //oEditUID.Value = "TPODOC";
            oForm.Items.Item("4").Click();
            #endregion
            #region Step4
            SAPbouiCOM.Matrix oMatrixFind = oForm.Items.Item("46").Specific;
            SAPbouiCOM.CheckBox oCheckMatFind;
            string[] ArrFind = {  "U_EXX_AUPP_ARCH"};
            for (int j = 1; j < oMatrixFind.RowCount + 1; j++)
            {
                string value = oMatrixFind.Columns.Item("3").Cells.Item(j).Specific.Value;
                oCheckMatFind = oMatrixFind.Columns.Item("2").Cells.Item(j).Specific;
                if (ArrFind.Contains(value))
                    if (oCheckMatFind.Checked == false)
                        oMatrixFind.Columns.Item("2").Cells.Item(j).Click();
            }
            oForm.Items.Item("4").Click();
            #endregion
            #region Step5
            SAPbouiCOM.Matrix oMatrixDefault = oForm.Items.Item("42").Specific;
            SAPbouiCOM.CheckBox oCheckMatDefault;
            string[] ArrDefault = { "U_EXX_AUPP_ARCH" };
            for (int j = 1; j < oMatrixDefault.RowCount + 1; j++)
            {
                string value = oMatrixDefault.Columns.Item("3").Cells.Item(j).Specific.Value;
                oCheckMatDefault = oMatrixDefault.Columns.Item("2").Cells.Item(j).Specific;
                if (ArrDefault.Contains(value))
                    if (oCheckMatDefault.Checked == false)
                        oMatrixDefault.Columns.Item("2").Cells.Item(j).Click();
            }
            oForm.Items.Item("4").Click();
            #endregion
            #region Step6
            SAPbouiCOM.Matrix oMatrixSon = oForm.Items.Item("23").Specific;
            SAPbouiCOM.CheckBox oCheckMatSon;
            string[] ArrSon = { "EXX_AUPP_SPR1" };
            for (int j = 1; j < oMatrixSon.RowCount + 1; j++)
            {
                string value = oMatrixSon.Columns.Item("1").Cells.Item(j).Specific.Value;
                oCheckMatSon = oMatrixSon.Columns.Item("2").Cells.Item(j).Specific;
                if (ArrSon.Contains(value))
                    if (oCheckMatSon.Checked == false)
                        oMatrixSon.Columns.Item("2").Cells.Item(j).Click();
            }
            oForm.Items.Item("4").Click();
            #endregion
            #region Step7
            SAPbouiCOM.ComboBox oComboSon = oForm.Items.Item("65").Specific;
            oComboSon.Select("1");
            SAPbouiCOM.Matrix oMatrixChildren = oForm.Items.Item("63").Specific;
            SAPbouiCOM.CheckBox oCheckChildren;
            string[] ArrChild1 = { "U_EXX_AUPP_CDIT", "U_EXX_AUPP_DSIT", "U_EXX_AUPP_MNPU", "U_EXX_AUPP_LMPU", "U_EXX_AUPP_MNPM","U_EXX_AUPP_LMPM",
                                    "U_EXX_AUPP_MNRU","U_EXX_AUPP_PRPU","U_EXX_AUPP_MNRM","U_EXX_AUPP_PRPM"};
            for (int j = 1; j < oMatrixChildren.RowCount + 1; j++)
            {
                string value = oMatrixChildren.Columns.Item("3").Cells.Item(j).Specific.Value;
                oCheckChildren = oMatrixChildren.Columns.Item("2").Cells.Item(j).Specific;
                if (ArrChild1.Contains(value))
                    if (oCheckChildren.Checked == false)
                        oMatrixChildren.Columns.Item("2").Cells.Item(j).Click();
            }
            oForm.Items.Item("4").Click();
            oForm.Items.Item("5").Click();
            oForm.Items.Item("5").Click();
            #endregion
        }
    }
}
