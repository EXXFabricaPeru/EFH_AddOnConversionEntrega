using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddonConEntrega.data_schema
{
    public class SchemaAddon
    {
        #region TABLAS_GENERICAS
        public static Dictionary<string, string> tablesGeneric()
        {
            var tables = new Dictionary<string, string>();
            //tables.Add(SCConfigLoteSerie.TABLE_CABE, SCConfigLoteSerie.TABLE_CABE_DES);

            return tables;
        }
        #endregion
        #region TABLAS_DATOS_MAESTROS
        //Cabeceras
        public static Dictionary<string, string> tablesMasterH()
        {
            var tables = new Dictionary<string, string>();
            //tables.Add(SCConfigLoteSerie.TABLE_CABE, SCConfigLoteSerie.TABLE_CABE_DES);
            return tables;
        }

        //Detalles
        public static Dictionary<string, string> tablesMasterD()
        {
            var tables = new Dictionary<string, string>();

            return tables;
        }
        #endregion
        #region TABLAS_DOCUMENTOS
        //Cabeceras
        public static Dictionary<string, string> tablesDocsH()
        {
            var tables = new Dictionary<string, string>();
            return tables;
        }

        //Detalles
        public static Dictionary<string, string> tablesDocsD()
        {
            var tables = new Dictionary<string, string>();

            return tables;
        }
        #endregion
        public static List<CampoBean> camposTB()
        {
            var campos = new List<CampoBean>();
            //campos.AddRange(SCConfigLoteSerie.getCamposTabla());
            campos.AddRange(SCUserFields.getCamposUsuario());
            return campos;
        }

        public static List<ObjetoBean> objetosADDON()
        {
            var objects = new List<ObjetoBean>();
            //objects.Add(SCConfigLoteSerie.getObjeto());
            return objects;
        }
    }
}
