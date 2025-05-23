using System.Collections.Generic;

namespace AddonConEntrega.data_schema
{
    public class SCUserFields
    {
        #region _CABECERA_TABLA
        public const string TABLE_PAGO_RECIBIDO = "ORCT";
        public const string TABLE_DETPAGO_RECIBIDO = "RCT2";
        public const string TABLE_ARTICULOS = "OITM";
        public const string TABLE_LOTESHEADER = "SBDR";
        public const string TABLE_UNIDADES = "EXX_TIPOUMED";

        #endregion

        #region _COLUMNAS
        public static List<CampoBean> getCamposUsuario()
        {
            var myList = new List<CampoBean>();
            myList.Add(new CampoBean()
            {
                nombre_tabla = TABLE_UNIDADES,
                nombre_campo = "EXD_MAIN",
                descrp_campo = "Unidad Principal",
                tipo_campo = SAPbobsCOM.BoFieldTypes.db_Alpha,
                tamano = 1,
                validValues = new string[] { "Y", "N" },
                validDescription = new string[] { "Si", "No" },
            });
            return myList;
        }
        #endregion
    }
}
