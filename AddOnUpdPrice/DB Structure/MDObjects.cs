using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using AddOnUpdPrice.App;

namespace AddOnUpdPrice.DB_Structure
{
    public class MDObjects
    {
        public MDObjects() { }
        public void CreateUDO()
        {
            try
            {
                int iret = -1;
                string ErrMsg = null;
                SAPbobsCOM.UserObjectsMD oUDO;
                #region UDO SYP_CONDUC
                oUDO = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                if (oUDO.GetByKey("SPECDB") == false)
                {
                    #region step 1
                    oUDO.Code = "SPECDB";//Code;
                    oUDO.Name = "Databases";//Name;
                    oUDO.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData;//ObjectType;
                    oUDO.TableName = "SYP_SPEC_DB";//TableName;
                    #endregion
                    #region step 2
                    oUDO.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;//CanFind;
                    oUDO.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;//CanDelete;
                    oUDO.CanCancel = SAPbobsCOM.BoYesNoEnum.tNO;//CanCancel;
                    oUDO.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                    #endregion
                    #region step 3
                    oUDO.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES;//CanCreateDefaultForm;
                    oUDO.EnableEnhancedForm = SAPbobsCOM.BoYesNoEnum.tNO;//NO es Grilla, Si es linea de cabecera
                    oUDO.MenuItem = SAPbobsCOM.BoYesNoEnum.tNO;//MenuItem;
                    #endregion
                    #region step 4
                    oUDO.FindColumns.ColumnAlias = "Code";
                    oUDO.FindColumns.ColumnDescription = "Code";

                    oUDO.FindColumns.Add();
                    oUDO.FindColumns.ColumnAlias = "Name";
                    oUDO.FindColumns.ColumnDescription = "Name";
                                     

                    #endregion
                    #region step 5
                    oUDO.FormColumns.FormColumnAlias = "Code";
                    oUDO.FormColumns.FormColumnDescription = "Code";
                    oUDO.FormColumns.Add();

                    oUDO.FormColumns.FormColumnAlias = "Name";
                    oUDO.FormColumns.FormColumnDescription = "Name";
                    oUDO.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUDO.FormColumns.Add();
                    #endregion
                    iret = oUDO.Add();
                    if (iret != 0)
                    {
                        Globals.oCompany.GetLastError(out iret, out ErrMsg);
                        Globals.WriteLogTxt("CREATE UDO\t" + "SPECDB" + "\t" + "Databases" + "\t" + ErrMsg, Globals.LogFile);
                    }
                    else
                    {
                        Globals.SBO_Application.StatusBar.SetText("UDO SPECDB : Creado con éxito.", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    }
                    Globals.Release(oUDO);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("UDO SPECDB : Ya existe. No es necesario crearlo.", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oUDO);
                #endregion
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
            }
        }
    }
}
