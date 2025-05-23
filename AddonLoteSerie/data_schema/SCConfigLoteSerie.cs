using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddonConEntrega.data_schema
{
    class SCConfigLoteSerie
    {
        #region _CABECERA_TABLA
        public const string TABLE_CABE = "EXD_CFG_AULS";
        public const string TABLE_CABE_DES = "EXD - Cfg. Aux. LoteSeries";
        #endregion

        #region _CAMPOS
        public static List<CampoBean> getCamposTabla()
        {
            var myList = new List<CampoBean>();


           
            return myList;
        }

        #endregion
        #region _OBJETO

        public static ObjetoBean getObjeto()
        {
            var myObj = new ObjetoBean
            {
                code = TABLE_CABE,
                name = "CONFIG_AUX_LOTESERIE",
                tableName = TABLE_CABE,
                canCancel = SAPbobsCOM.BoYesNoEnum.tNO,
                canClose = SAPbobsCOM.BoYesNoEnum.tNO,
                canDelete = SAPbobsCOM.BoYesNoEnum.tYES,
                canCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES,
                childFormColumns = new string[] { "U_XLS_SERSAL", "U_XLS_SERENT", "U_XLS_LOTSAL", "U_XLS_LOTENT", "U_FVE_LOTE" },
                canFind = SAPbobsCOM.BoYesNoEnum.tYES,
                canLog = SAPbobsCOM.BoYesNoEnum.tYES,
                objectType = SAPbobsCOM.BoUDOObjType.boud_MasterData,
                manageSeries = SAPbobsCOM.BoYesNoEnum.tNO,
                enableEnhancedForm = SAPbobsCOM.BoYesNoEnum.tNO,
                rebuildEnhancedForm = SAPbobsCOM.BoYesNoEnum.tNO
            };
            return myObj;
        }

        #endregion


    }
}
