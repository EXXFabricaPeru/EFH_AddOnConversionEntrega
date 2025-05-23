using System.Text;

namespace AddonConEntrega.commons
{
    public class Consultas
    {
        #region _Attributes_

        private static StringBuilder m_sSQL = new StringBuilder();

        #endregion

        #region _Functions_

        public static string GetCheckCFGAux(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string entry)
        {
            m_sSQL.Length = 0;
            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    m_sSQL.AppendFormat("SELECT TOP 1 IFNULL(\"{0}\",'N') \"Check\" FROM \"@{1}\"", entry, data_schema.SCConfigLoteSerie.TABLE_CABE);
                    break;
                default:
                    m_sSQL.AppendFormat("SELECT TOP 1 ISNULL(\"{0}\",'N') \"Check\" FROM [@{1}]", entry, data_schema.SCConfigLoteSerie.TABLE_CABE);
                    break;
            }
            return m_sSQL.ToString();
        }

        public static string GetMainUM(SAPbobsCOM.BoDataServerTypes bo_ServerTypes)
        {
            m_sSQL.Length = 0;
            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    m_sSQL.AppendFormat("SELECT TOP 1 \"Code\" \"Value\" FROM \"@{0}\" WHERE \"U_EXD_MAIN\"='Y'", data_schema.SCUserFields.TABLE_UNIDADES);
                    break;
                default:
                    m_sSQL.AppendFormat("SELECT TOP 1 \"Code\" \"Value\" FROM [@{0}] WHERE \"U_EXD_MAIN\"='Y'", data_schema.SCUserFields.TABLE_UNIDADES);
                    break;
            }
            return m_sSQL.ToString();
        }

        public static string GetGrupoUM(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string ItemCode)
        {
            m_sSQL.Length = 0;
            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    m_sSQL.AppendFormat("SELECT T1.\"UgpCode\" \"Value\" FROM OITM T0  INNER JOIN OUGP T1 ON T0.\"UgpEntry\" = T1.\"UgpEntry\" WHERE T0.\"ItemCode\" = '{0}'", ItemCode);
                    break;
                default: 
                    m_sSQL.AppendFormat("SELECT T1.\"UgpCode\" \"Value\" FROM OITM T0  INNER JOIN OUGP T1 ON T0.\"UgpEntry\" = T1.\"UgpEntry\" WHERE T0.\"ItemCode\" = ['{0}']", ItemCode);
                    break;
            }
            return m_sSQL.ToString();
        }
        public static string GetTotalLote(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string ItemCode, string Whs)
        {
            m_sSQL.Length = 0;
            m_sSQL.Append("SELECT SUM(CASE T0.\"Direction\" when 0 then 1 else -1 end * T0.\"Quantity\") \"Value\" ");
            m_sSQL.Append("FROM IBT1 T0 ");
            m_sSQL.Append("INNER JOIN OWHS T1 ON T0.\"WhsCode\" = T1.\"WhsCode\"");
            m_sSQL.Append("INNER JOIN (SELECT T0.\"ItemCode\", T1.\"WhsName\",");
            m_sSQL.Append("SUM(CASE T0.\"Direction\" when 0 then 1 else -1 end * T0.\"Quantity\") \"Quantity\"");
            m_sSQL.Append("FROM IBT1 T0 INNER JOIN OWHS T1 ON T0.\"WhsCode\" = T1.\"WhsCode\"");
            m_sSQL.Append("GROUP BY T1.\"WhsName\", T0.\"ItemCode\") V0 ON T0.\"ItemCode\"=V0.\"ItemCode\" and t1.\"WhsName\"=v0.\"WhsName\" ");
            m_sSQL.AppendFormat("WHERE T0.\"ItemCode\" ='{0}' and T0.\"WhsCode\"='{1}'", ItemCode, Whs);

            return m_sSQL.ToString();
        }
        public static string GetChkFV(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string ItemCode)
        {
            m_sSQL.Length = 0;
            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    m_sSQL.AppendFormat("SELECT TOP 1 IFNULL(\"U_EXD_PERFVE\",'N') \"Check\" FROM \"OITM\" WHERE \"ItemCode\"='{0}'", ItemCode);
                    break;
                default:
                    m_sSQL.AppendFormat("SELECT TOP 1 ISNULL(\"U_EXD_PERFVE\",'N') \"Check\" FROM OITM WHERE \"ItemCode\"='{0}'", ItemCode);
                    break;
            }
            return m_sSQL.ToString();
        }
        public static string GetCheckLogicaEvLote(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string ItemCode)
        {
            m_sSQL.Length = 0;
            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    m_sSQL.AppendFormat("SELECT TOP 1 IFNULL(\"U_EXD_LOGLOT\",'FE') \"Log\" FROM \"OITM\" WHERE \"ItemCode\"='{0}'", ItemCode);
                    break;
                default:
                    m_sSQL.AppendFormat("SELECT TOP 1 ISNULL(\"U_EXD_LOGLOT\",'FE') \"Log\" FROM OITM WHERE \"ItemCode\"='{0}'", ItemCode);
                    break;
            }
            return m_sSQL.ToString();
        }
        public static string GetDespachoDays(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string ItemCode)
        {
            m_sSQL.Length = 0;
            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    m_sSQL.AppendFormat("SELECT TOP 1 ABS(IFNULL(\"U_EXD_VIDDES\",0)-IFNULL(\"U_EXD_VIDUTL\",0)) \"Val\" FROM \"OITM\" WHERE \"ItemCode\"='{0}'", ItemCode);
                    break;
                default:
                    m_sSQL.AppendFormat("SELECT TOP 1 ABS(ISNULL(\"U_EXD_VIDDES\",0)-ISNULL(\"U_EXD_VIDUTL\",0)) \"Val\" FROM OITM WHERE \"ItemCode\"='{0}'", ItemCode);
                    break;
            }
            return m_sSQL.ToString();
        }
        public static string ConsultaTablaConfiguracion(SAPbobsCOM.BoDataServerTypes bo_ServerTypes, string NAddon, string Version, bool Ordenamiento)
        {
            m_sSQL.Length = 0;

            switch (bo_ServerTypes)
            {
                case SAPbobsCOM.BoDataServerTypes.dst_HANADB:
                    m_sSQL.AppendFormat("SELECT * FROM \"@{0}\"", NAddon.ToUpper());
                    if (NAddon != "" || Version != "")
                    {
                        m_sSQL.Append(" WHERE ");
                        if (NAddon != "")
                        {
                            m_sSQL.AppendFormat("\"Name\" Like '{0}%'", NAddon);
                            if (Version != "") m_sSQL.AppendFormat(" AND \"Code\" = '{0}'", Version);
                        }
                        else if (Version != "") m_sSQL.AppendFormat("\"Code\" = '{0}'", Version);
                    }
                    if (Ordenamiento) m_sSQL.Append(" ORDER BY LENGTH(\"Code\") DESC, \"Code\" DESC");

                    break;
                default:
                    m_sSQL.AppendFormat("SELECT * FROM [@{0}]", NAddon.ToUpper());
                    if (NAddon != "" || Version != "")
                    {
                        m_sSQL.Append(" WHERE ");
                        if (NAddon != "")
                        {
                            m_sSQL.AppendFormat("Name Like '{0}%'", NAddon);
                            if (Version != "") m_sSQL.AppendFormat(" AND Code = '{0}'", Version);
                        }
                        else if (Version != "") m_sSQL.AppendFormat("Code = '{0}'", Version);
                    }
                    if (Ordenamiento) m_sSQL.Append(" ORDER BY LEN(Code) DESC, Code DESC");
                    break;
            }

            return m_sSQL.ToString();
        }

        #endregion

    }
}
