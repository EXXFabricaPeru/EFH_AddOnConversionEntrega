//using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using AddOnUpdPrice.App;

namespace AddOnUpdPrice.DB_Structure
{
    public class MDQueries
    {
        public MDQueries() { }
        public void CreateCategories(string CatName)
        {
            SAPbobsCOM.QueryCategories oQueryCat = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oQueryCategories);
            int iret;
            string ErrMsg = "";
            int CatId;
            Globals.Query = "select \"CategoryId\" from OQCN where \"CatName\" = '" + CatName + "'";
            Globals.RunQuery(Globals.Query);
            CatId = Globals.oRec.Fields.Item(0).Value;
            if (oQueryCat.GetByKey(CatId) == true)
            #region Update
            {
                oQueryCat.Name = CatName;
                oQueryCat.Permissions = "YYYYYYYYYYYYYYY";
                iret = oQueryCat.Update();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    //Globals.WriteLogTxt("UPDATE QCAT\t" + CatName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("Categoría " + CatName + " Actualizada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oQueryCat);
            }
            #endregion
            else
            #region Create
            {
                oQueryCat.Name = CatName;
                oQueryCat.Permissions = "YYYYYYYYYYYYYYY";
                iret = oQueryCat.Add();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    //Globals.WriteLogTxt("CREATE QCAT\t" + CatName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("Categoría " + CatName + " Creada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oQueryCat);
            }
            #endregion
        }
        public void CreateQueries(string CatName, string QName, string Query)
        {
            SAPbobsCOM.UserQueries oUQ = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserQueries);
            string ErrMsg = "";
            int iret;

            int CategoryId = GetCategoryId(CatName);
            int QueryId = GetQueryId(QName, CategoryId);
            if (oUQ.GetByKey(QueryId, CategoryId) == true)
            #region Update
            {
                oUQ.Query = Query;
                oUQ.QueryDescription = QName;
                oUQ.QueryCategory = CategoryId;
                iret = oUQ.Update();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    //Globals.WriteLogTxt("UPDATE QUERY\t" + QName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("Query " + QName + " Actualizado con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oUQ);
            }
            #endregion
            else
            #region Create
            {
                oUQ.Query = Query;
                oUQ.QueryDescription = QName;
                oUQ.QueryCategory = CategoryId;
                iret = oUQ.Add();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    //Globals.WriteLogTxt("CREATE QUERY\t" + QName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("Query " + QName + " Creado con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oUQ);
            }
            #endregion
        }
        public void RemoveQueries(string CatName, string QName)
        {
            SAPbobsCOM.UserQueries oUQ = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserQueries);
            string ErrMsg = "";
            int iret;
            int CategoryId = GetCategoryId(CatName);
            int QueryId = GetQueryId(QName, CategoryId);
            if (oUQ.GetByKey(QueryId, CategoryId) == true)
            {
                iret = oUQ.Remove();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    ////Globals.WriteLogTxt("DELETE QUERY\t" + QName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("Query " + QName + " Eliminado con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oUQ);
            }
        }

        public void CreateFMS(string QName, string FormId, string ItemId, string ColumnId, string FieldId, string ByField)
        {
            SAPbobsCOM.FormattedSearches oFMS = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oFormattedSearches);
            string ErrMsg = "";
            int iret;
            int FMSId = GetFMSId(FormId, ItemId, ColumnId);
            int CategoryId = GetCategoryId("EXX_AddOn_UpdPrice");
            int QueryId = GetQueryId(QName, CategoryId);
            #region Ignore custom FMS
            Globals.Query = "select A.\"IndexID\",B.\"QName\" from CSHS A inner join OUQR B on A.\"QueryId\" = B.\"IntrnalKey\"  where \"FormID\" = '" + FormId + "' and \"ItemID\" = '" + ItemId + "' and \"ColID\" = '" + ColumnId + "'";
            Globals.RunQuery(Globals.Query);
            string Custom = Globals.oRec.Fields.Item(1).Value;
            Globals.Release(Globals.oRec);
            if (Custom.Trim() != QName.Trim() && Custom.Trim().Length > 0)
                return;
            #endregion
            if (oFMS.GetByKey(FMSId) == true)
            #region Update
            {
                oFMS.QueryID = QueryId;
                oFMS.FormID = FormId;
                oFMS.ItemID = ItemId;
                oFMS.ColumnID = ColumnId;
                oFMS.FieldID = FieldId;
                oFMS.Action = SAPbobsCOM.BoFormattedSearchActionEnum.bofsaQuery;
                if (ByField == "Y") { oFMS.ByField = SAPbobsCOM.BoYesNoEnum.tYES; } else { oFMS.ByField = SAPbobsCOM.BoYesNoEnum.tNO; }
                if (FieldId == "") { oFMS.Refresh = SAPbobsCOM.BoYesNoEnum.tNO; } else { oFMS.Refresh = SAPbobsCOM.BoYesNoEnum.tYES; }
                iret = oFMS.Update();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    //Globals.WriteLogTxt("UPDATE FMS\t" + "Form: " + FormId + "\t" + "Field: " + ItemId + "\t" + QName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("FMS " + QName + " Actualizada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oFMS);
            }
            #endregion
            else
            #region Create
            {
                oFMS.QueryID = QueryId;
                oFMS.FormID = FormId;
                oFMS.ItemID = ItemId;
                oFMS.ColumnID = ColumnId;
                oFMS.FieldID = FieldId;
                oFMS.Action = SAPbobsCOM.BoFormattedSearchActionEnum.bofsaQuery;
                if (ByField == "Y") { oFMS.ByField = SAPbobsCOM.BoYesNoEnum.tYES; } else { oFMS.ByField = SAPbobsCOM.BoYesNoEnum.tNO; }
                if (FieldId == "") { oFMS.Refresh = SAPbobsCOM.BoYesNoEnum.tNO; } else { oFMS.Refresh = SAPbobsCOM.BoYesNoEnum.tYES; }
                iret = oFMS.Add();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    //Globals.WriteLogTxt("CREATE FMS\t" + "Form: " + FormId + "\t" + "Field: " + ItemId + "\t" + QName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("FMS " + QName + " Actualizada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oFMS);
            }
            #endregion
        }
        public void CreateFMSWithReplace(string QName, string FormId, string ItemId, string ColumnId, string FieldId, string ByField)
        {
            SAPbobsCOM.FormattedSearches oFMS = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oFormattedSearches);
            string ErrMsg = "";
            int iret;
            int FMSId = GetFMSId(FormId, ItemId, ColumnId);
            int CategoryId = GetCategoryId("00. SYP Conf. General");
            int QueryId = GetQueryId(QName, CategoryId);
            #region Ignore custom FMS
            Globals.Query = "select A.\"IndexID\",B.\"QName\" from CSHS A inner join OUQR B on A.\"QueryId\" = B.\"IntrnalKey\"  where \"FormID\" = '" + FormId + "' and \"ItemID\" = '" + ItemId + "' and \"ColID\" = '" + ColumnId + "'";
            Globals.RunQuery(Globals.Query);
            string Custom = Globals.oRec.Fields.Item(1).Value;
            Globals.Release(Globals.oRec);
            #endregion
            if (oFMS.GetByKey(FMSId) == true)
            #region Update
            {
                oFMS.QueryID = QueryId;
                oFMS.FormID = FormId;
                oFMS.ItemID = ItemId;
                oFMS.ColumnID = ColumnId;
                oFMS.FieldID = FieldId;
                oFMS.Action = SAPbobsCOM.BoFormattedSearchActionEnum.bofsaQuery;
                if (ByField == "Y") { oFMS.ByField = SAPbobsCOM.BoYesNoEnum.tYES; } else { oFMS.ByField = SAPbobsCOM.BoYesNoEnum.tNO; }
                if (FieldId == "") { oFMS.Refresh = SAPbobsCOM.BoYesNoEnum.tNO; } else { oFMS.Refresh = SAPbobsCOM.BoYesNoEnum.tYES; }
                iret = oFMS.Update();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    ////Globals.WriteLogTxt("UPDATE FMS\t" + "Form: " + FormId + "\t" + "Field: " + ItemId + "\t" + QName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("FMS " + QName + " Actualizada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oFMS);
            }
            #endregion
            else
            #region Create
            {
                oFMS.QueryID = QueryId;
                oFMS.FormID = FormId;
                oFMS.ItemID = ItemId;
                oFMS.ColumnID = ColumnId;
                oFMS.FieldID = FieldId;
                oFMS.Action = SAPbobsCOM.BoFormattedSearchActionEnum.bofsaQuery;
                if (ByField == "Y") { oFMS.ByField = SAPbobsCOM.BoYesNoEnum.tYES; } else { oFMS.ByField = SAPbobsCOM.BoYesNoEnum.tNO; }
                if (FieldId == "") { oFMS.Refresh = SAPbobsCOM.BoYesNoEnum.tNO; } else { oFMS.Refresh = SAPbobsCOM.BoYesNoEnum.tYES; }
                iret = oFMS.Add();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    ////Globals.WriteLogTxt("CREATE FMS\t" + "Form: " + FormId + "\t" + "Field: " + ItemId + "\t" + QName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("FMS " + QName + " Actualizada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oFMS);
            }
            #endregion
        }
        public void RemoveFMS(string QName, string FormId, string ItemId, string ColumnId, string FieldId, string ByField,string Category)
        {
            SAPbobsCOM.FormattedSearches oFMS = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oFormattedSearches);
            string ErrMsg = "";
            int iret;
            int FMSId = GetFMSId(FormId, ItemId, ColumnId);
            int CategoryId = GetCategoryId(Category);
            int QueryId = GetQueryId(QName, CategoryId);
            if (oFMS.GetByKey(FMSId) == true)
            #region Remove
            {
                iret = oFMS.Remove();
                if (!(iret == 0))
                {
                    Globals.oCompany.GetLastError(out iret, out ErrMsg);
                    //Globals.SBO_Application.MessageBox(ErrMsg);
                    //Globals.WriteLogTxt("DELETE FMS\t" + "Form: " + FormId + "\t" + "Field: " + ItemId + "\t" + QName + "\t" + ErrMsg, Globals.LogFile);
                }
                else
                {
                    Globals.SBO_Application.StatusBar.SetText("FMS " + QName + " Eliminada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, (SAPbouiCOM.BoStatusBarMessageType)SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                Globals.Release(oFMS);
            }
            #endregion
        }

        private int GetCategoryId(string CatName)
        {
            int iRetVal = -1;
            try
            {
                Globals.Query = ("select \"CategoryId\" from OQCN where \"CatName\" = '" + CatName + "'");
                Globals.RunQuery(Globals.Query);
                if (!Globals.oRec.EoF) iRetVal = Convert.ToInt32(Globals.oRec.Fields.Item("CategoryId").Value.ToString());
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
            }
            finally
            {
                Globals.Release(Globals.oRec);
            }
            return iRetVal;
        }
        private int GetQueryId(string QName, int CategoryId)
        {
            int iRetVal = -1;
            try
            {
                Globals.Query = ("select \"IntrnalKey\" from OUQR where \"QName\" = '" + QName + "' and \"QCategory\" = " + CategoryId);
                Globals.RunQuery(Globals.Query);
                if (!Globals.oRec.EoF) iRetVal = Convert.ToInt32(Globals.oRec.Fields.Item("IntrnalKey").Value.ToString());
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
            }
            finally
            {
                Globals.Release(Globals.oRec);
            }
            return iRetVal;
        }
        private int GetFMSId(string FormID, string ItemID, string ColumnId)
        {
            int iRetVal = -1;
            try
            {
                Globals.Query = ("select \"IndexID\" from CSHS where \"FormID\" = '" + FormID + "' and \"ItemID\" = '" + ItemID + "' and \"ColID\" = '" + ColumnId + "'");
                Globals.RunQuery(Globals.Query);
                if (!Globals.oRec.EoF) iRetVal = Convert.ToInt32(Globals.oRec.Fields.Item("IndexID").Value.ToString());
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
            }
            finally
            {
                Globals.Release(Globals.oRec);
            }
            return iRetVal;
        }
    }

}
