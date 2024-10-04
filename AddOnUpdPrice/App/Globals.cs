using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Xml;
using System.Security.Cryptography;
using System.Data.OleDb;
using System.Runtime.InteropServices;
//using Excel = Microsoft.Office.Interop.Excel;
using SAPbobsCOM;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace AddOnUpdPrice.App
{
    public static class Globals
    {
         public static bool bLoadInputEvents = false;
         //public static cUserInput oUserInput = new cUserInput();


        //globales sociedades
        //public static string TXTPath = null;
        public static string BD1, BD2, BD3, BD4, BD5, SAPUSER, SAPPASS, LICSERVER, BDSERVER, BDUSER, BDPASS, BDTYPE;

        public static SAPbobsCOM.Recordset oRec = default(SAPbobsCOM.Recordset);
        public static SAPbobsCOM.Recordset oRec2 = default(SAPbobsCOM.Recordset);
        public static SAPbobsCOM.Recordset oRec3 = default(SAPbobsCOM.Recordset);
        public static SAPbobsCOM.Recordset oRec4 = default(SAPbobsCOM.Recordset);
        public static SAPbobsCOM.Recordset oRecMirror = default(SAPbobsCOM.Recordset);
        public static SAPbobsCOM.Recordset oRec5 = default(SAPbobsCOM.Recordset);
        public static SAPbouiCOM.Application SBO_Application;
        public static SAPbobsCOM.CompanyService oCmpSrv;
        public static SAPbouiCOM.EventFilters oFilters;
        public static SAPbouiCOM.EventFilter oFilter;
        public static SAPbobsCOM.Company oCompany;
        public static SAPbobsCOM.Company oCompanyMirror;
        public static int SAPVersion;
        public static bool licensed = false;
        public static SAPbouiCOM.Form DialogForm;
        public static string filename = "";
        private static string pathItem;
        private static string strFileName = "";
        public static int lErrCode = 0;
        public static int lRetCode;
        public static string sErrMsg = null;

        public static string Addon = null;
        public static string version = null;
        public static string oldversion = "";
        public static bool Actual = false;
        public static string Query = null;
        public static string Query2 = null;
        public static string Query3 = null;
        public static string Query4 = null;
        public static string Query5 = null;
        public static string QueryMirror = null;
        public static Dictionary<string, string> CancelDictionary = new Dictionary<string, string> { };

        public static string TXTPath = null;
        public static string SentToDataFormEventAction = "";
        public static string[] AuxForFormDataEvent = new string[10] { "", "", "", "", "", "", "", "", "", "" };
        public static string[] parts;
        public static int iRows = 0;
        public static bool RetButtonExists = false;
        public static string Conexion = "";

        //Validation Variables
        public static string fmrUdoEstructuras = "FormEst";
        public static string fmrUdoListaPrecios = "UDO_FT_EXX_AUPP_LSPR";
        public static string Action = null;
        public static string Error = null;
        public static string ServerError = null;
        public static int continuar = -1;
        public static string sGenerado = "G";
        public static string sCancelado = "C";

        public static string LogFile = null;

        public static string TituloArchivoPlano = "ZENTITY;OFISCYEAR;OFISCPER;ZWAERS;ZSCR_CEBE;ZSCR_CECO;ZSCR_ACC;ZSCR_AUX;ZINTERCO;OCURTYPE;OVERSION;OVTYPE;ZBALANCE;ZSALES";

        public class Proyecto
        {
            public string Marcar { get; set; }
            public string CodigoProyecto { get; set; }
            public string RUCMedico { get; set; }
            public string NombreMedico { get; set; }
            public string FechaInicio { get; set; }
            public string Estado { get; set; }
            public string Saldo { get; set; }
        }

        public static List<Proyecto> oLstProyectoFinal = new List<Proyecto>();
        public static List<Proyecto> oLstProyectoTemporal;// = new List<Proyecto>();
        //public static List<Proyecto> oLstProyecto;// = new List<Proyecto>();
        //public static List<Proyecto> oLstProyecto = new List<Proyecto>();

        public static object Release(object objeto)
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(objeto);
            Query = null;
            GC.Collect();
            return null;
        }
        public static SAPbobsCOM.Recordset RunQuery(string Query)
        {
            try
            {
                oRec = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRec.DoQuery(Query);
                return oRec;
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
                return null;
            }
        }
        public static SAPbobsCOM.Recordset RunQuery2(string Query2)
        {
            try
            {
                oRec2 = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRec2.DoQuery(Query2);
                return oRec2;
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
                return null;
            }
        }
        public static SAPbobsCOM.Recordset RunQuery3(string Query3)
        {
            try
            {
                oRec3 = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRec3.DoQuery(Query3);
                return oRec3;
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
                return null;
            }
        }
        public static SAPbobsCOM.Recordset RunQuery4(string Query4)
        {
            try
            {
                oRec4 = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRec4.DoQuery(Query4);
                return oRec4;
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
                return null;
            }
        }
        public static SAPbobsCOM.Recordset RunQuery5(string Query5)
        {
            try
            {
                oRec5 = Globals.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRec5.DoQuery(Query5);
                return oRec5;
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
                return null;
            }
        }
        public static SAPbobsCOM.Recordset RunQueryMirror(string QueryMirror)
        {
            try
            {
                oRecMirror = Globals.oCompanyMirror.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRecMirror.DoQuery(QueryMirror);
                return oRecMirror;
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
                return null;
            }
        }
        public static bool IsHana()
        {
            try
            {
                if (Globals.oCompany.DbServerType == (SAPbobsCOM.BoDataServerTypes)9)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.Message);
                return false;
            }
        }
        public static string GetResourceValue(string name, string ResourceName)
        {
            ResourceManager rm = new ResourceManager(ResourceName, Assembly.GetExecutingAssembly());
            string value = rm.GetString(name);
            return value;
        }
        public static string LoadFromXML(ref string FileName)
        {
            System.Xml.XmlDocument oXmlDoc = null;
            string sPath = null;
            oXmlDoc = new System.Xml.XmlDocument();
            sPath = System.Windows.Forms.Application.StartupPath;
            oXmlDoc.Load(sPath + FileName);
            return (oXmlDoc.InnerXml);
        }


        public static void WriteTxt(string x, string filename, string destPath)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() + destPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string FILE_NAME = path + "\\" + filename + ".txt";
            if (System.IO.File.Exists(FILE_NAME) == false)
            {
                System.IO.File.Create(FILE_NAME).Dispose();
            }
            System.IO.StreamWriter objWriter = new System.IO.StreamWriter(FILE_NAME, true, Encoding.Default);
            objWriter.WriteLine(x);
            objWriter.Close();
        }
        public static void WriteLogTxt(string x, string filename)
        {
            //System.Windows.Forms.Application.StartupPath;
            string path = System.Windows.Forms.Application.StartupPath + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string FILE_NAME = path + "\\" + filename + ".txt";
            if (System.IO.File.Exists(FILE_NAME) == false)
            {
                System.IO.File.Create(FILE_NAME).Dispose();
            }
            System.IO.StreamWriter objWriter = new System.IO.StreamWriter(FILE_NAME, true, Encoding.Default);
            objWriter.WriteLine(x);
            objWriter.Close();
        }
        public static DateTime ConvertDate(string date)
        {
            if (date.Length == 8)
            {
                date = date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
                return Convert.ToDateTime(date);
            }
            else
            {
                Globals.Error = "(SYP)BPS : Invalid Date Format";
                throw new Exception(Globals.Error);
            }
        }

        public static void OpenFile(SAPbouiCOM.Form oForm, string path)
        {
            try
            {
                pathItem = path;
                DialogForm = oForm;
                System.Threading.Thread ShowFolderBrowserThread;
                ShowFolderBrowserThread = new System.Threading.Thread(ShowFolderBrowser);
                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                }
                else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();
                }
                //ShowFolderBrowserThread.Abort();
            }
            catch (Exception ex)
            {
            }
        }

        public static void ShowFolderBrowser()
        {
            try
            {
                NativeWindow nws = new NativeWindow();
                OpenFileDialog MyTest = new OpenFileDialog();
                MyTest.Multiselect = false;
                MyTest.Filter = "Text Files (.txt)|*.txt";
                Process[] MyProcs = null;
                //string filename = null;
                MyProcs = Process.GetProcessesByName("SAP Business One");
                nws.AssignHandle(System.Diagnostics.Process.GetProcessesByName("SAP Business One")[0].MainWindowHandle);
                if (MyTest.ShowDialog(nws) == System.Windows.Forms.DialogResult.OK)
                {
                    filename = MyTest.FileName;
                    DialogForm.Items.Item(pathItem).Specific.Value = filename;
                    DialogForm = null;
                    pathItem = null;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void InitializeCompany()
        {
            oCompanyMirror = new SAPbobsCOM.Company();
        }

        public static void ConnectToOtherCompany(string DB, string userSAP, string passSAP, string userDB, string passDB)
        {
            try
            {
                Globals.InitializeCompany();
                Globals.oCompanyMirror.Server = Globals.oCompany.Server;
                Globals.oCompanyMirror.LicenseServer = Globals.oCompany.LicenseServer;
                Globals.oCompanyMirror.UseTrusted = false;
                Globals.oCompanyMirror.DbUserName = userDB;
                Globals.oCompanyMirror.DbPassword = passDB;
                Globals.oCompanyMirror.DbServerType = Globals.oCompany.DbServerType;
                Globals.oCompanyMirror.CompanyDB = DB;
                Globals.oCompanyMirror.UserName = userSAP;
                Globals.oCompanyMirror.Password = passSAP;
                Globals.lRetCode = Globals.oCompanyMirror.Connect();
                if (Globals.lRetCode != 0)
                {
                    Globals.oCompanyMirror.GetLastError(out Globals.lErrCode, out Globals.sErrMsg);
                }
                //else
                //    Globals.SBO_Application.SetStatusBarMessage("Conectado a: " + DB + " con usuario: " + userSAP + ". Iniciando copia.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.ToString());
            }
        }

        public static void ReadTextFileForFix(string txtfile)
        {
            string line;
            TXTPath = System.Windows.Forms.Application.StartupPath;
            using (StreamReader file = new StreamReader(@TXTPath + "\\" + txtfile + ".txt"))
            {
                iRows = 1;
                line = file.ReadLine();
                while ((line = file.ReadLine()) != null)
                {
                    char[] delimiters = new char[] { '\t' };
                    parts = line.Split(delimiters, StringSplitOptions.None);
                    parts.ToString();
                    iRows++;
                }
                file.Close();
            }
            //Console.ReadLine();
        }

        public static void ReadTextFile()
        {
            string line;
            using (StreamReader file = new StreamReader(filename))
            {
                iRows = 1;
                line = file.ReadLine();
                while ((line = file.ReadLine()) != null)
                {
                    char[] delimiters = new char[] { '\t' };
                    parts = line.Split(delimiters, StringSplitOptions.None);
                    parts.ToString();
                    iRows++;
                }
                file.Close();
            }
            //Console.ReadLine();
        }

        public static void OpenFileExcel(SAPbouiCOM.Form oForm, string path)
        {
            try
            {
                pathItem = path;
                DialogForm = oForm;

                DialogForm.Visible = true;
                System.Threading.Thread ShowFolderBrowserThread;
                ShowFolderBrowserThread = new System.Threading.Thread(ShowFolderBrowserExcel);
                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                }
                else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void ShowFolderBrowserExcel()
        {
            try
            {
                NativeWindow nws = new NativeWindow();
                OpenFileDialog MyTest = new OpenFileDialog();
                MyTest.Multiselect = false;
                //MyTest.Filter = "Excel 2003 files(*.xls)|*.xls|Excel 2007 Files(*.xlsx)|*.xlsx|Excel 2012 Files(*.xlsx)|*.xlsx";
                MyTest.Filter = "Excel 2012 files(*.xlsx)|*.xlsx|Excel 2007 Files(*.xlsx)|*.xlsx|Excel 2003 Files(*.xls)|*.xls";
                Process[] MyProcs = null;
                MyProcs = Process.GetProcessesByName("SAP Business One");
                nws.AssignHandle(System.Diagnostics.Process.GetProcessesByName("SAP Business One")[0].MainWindowHandle);
                if (MyTest.ShowDialog(nws) == System.Windows.Forms.DialogResult.OK)
                {
                    filename = MyTest.FileName;
                    DialogForm.Items.Item(pathItem).Specific.Value = filename;
                    DialogForm = null;
                    pathItem = null;
                }
                else
                {
                    filename = "";
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static List<DataTable> ImportExcel(string strFileName)
        {
            List<DataTable> _dataTables = new List<DataTable>();
            string _ConnectionString = string.Empty;
            string _Extension = Path.GetExtension(strFileName);
            //Checking for the extentions, if XLS connect using Jet OleDB
            if (_Extension.Equals(".xls", StringComparison.CurrentCultureIgnoreCase))
            {
                _ConnectionString =
                    "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0};Extended Properties=Excel 8.0";
            }
            //Use ACE OleDb
            else if (_Extension.Equals(".xlsx", StringComparison.CurrentCultureIgnoreCase))
            {
                _ConnectionString =
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0";
            }

            DataTable dataTable = null;

            using (OleDbConnection oleDbConnection =
                new OleDbConnection(string.Format(_ConnectionString, strFileName)))
            {
                oleDbConnection.Open();
                //Getting the meta data information.
                //This DataTable will return the details of Sheets in the Excel File.
                DataTable dbSchema = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables_Info, null);
                foreach (DataRow item in dbSchema.Rows)
                {
                    //reading data from excel to Data Table
                    using (OleDbCommand oleDbCommand = new OleDbCommand())
                    {
                        oleDbCommand.Connection = oleDbConnection;
                        oleDbCommand.CommandText = string.Format("SELECT * FROM [{0}]",
                            item["TABLE_NAME"].ToString());
                        using (OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter())
                        {
                            oleDbDataAdapter.SelectCommand = oleDbCommand;
                            dataTable = new DataTable(item["TABLE_NAME"].ToString());
                            oleDbDataAdapter.Fill(dataTable);
                            _dataTables.Add(dataTable);
                        }
                    }
                }
            }
            return _dataTables;
        }

        public static double ConvertDouble(string ImporteTexto, int tipo)
        {
            double ImporteConvertido = 0;
            if (tipo == 1)
            {
                string[] tokens = ImporteTexto.Replace("-", "").Split('(');
                ImporteTexto = tokens[1];
                string[] tokens2 = ImporteTexto.Split(')');
                ImporteConvertido = Convert.ToDouble(tokens2[0]);
                bool result = Double.TryParse(tokens2[0], out ImporteConvertido);
                if (!(result))
                {
                    throw new Exception("El monto a convertir no tiene el formato correcto: " + tokens2[0]);
                }
            }
            else if (tipo == 2)
            {
                string[] tokens = ImporteTexto.Replace("-", "").Split(' ');
                bool result, result2 = false;
                ImporteTexto = tokens[1];
                result = Double.TryParse(tokens[0], out ImporteConvertido);
                if (!result)
                {
                    result2 = Double.TryParse(tokens[1], out ImporteConvertido);
                }
                if (!(result) && !(result2))
                {
                    throw new Exception("El monto a convertir no tiene el formato correcto: " + tokens[0]);
                }
            }
            else if (tipo == 3)
            {
                string[] tokens = ImporteTexto.Replace("-", "").Split(' ');
                bool result, result2 = false;
                //ImporteTexto = tokens[1];
                result = Double.TryParse(tokens[0], out ImporteConvertido);
                if (!result)
                {
                    result2 = Double.TryParse(tokens[1], out ImporteConvertido);
                }
                if (!(result) && !(result2))
                {
                    throw new Exception("El monto a convertir no tiene el formato correcto: " + tokens[0]);
                }
            }
            return ImporteConvertido;
        }

        public static void ReadTextFile(string txtfile)
        {
            string line;
            TXTPath = System.Windows.Forms.Application.StartupPath;
            using (StreamReader file = new StreamReader(@TXTPath + "\\" + txtfile + ".txt"))
            {
                line = file.ReadLine();
                //line = file.ReadLine();
                while ((line = file.ReadLine()) != null)
                {
                    char[] delimiters = new char[] { '\t' };
                    string[] parts = line.Split(delimiters, StringSplitOptions.None);
                    parts.ToString();
                    //BDECU,BDFASDER,BDTRAIN,BDTLOG,BDOCEAN,BDTIM,BDTMAR,BDTAF,SAPUSER,SAPPASS,LICSERVER,BDSERVER,BDUSER,BDPASS
                    Globals.BD1 = parts[0];
                    //Globals.BD2 = parts[1];
                    //Globals.BD3 = parts[2];
                    //Globals.BD4 = parts[3];
                    //Globals.BD5 = parts[4];
                    //Globals.SAPUSER = parts[1];
                    //Globals.SAPPASS = parts[2];
                    //Globals.LICSERVER = parts[3];
                    Globals.BDSERVER = parts[1];
                    Globals.BDUSER = parts[2];
                    Globals.BDPASS = parts[3];
                    Globals.BDTYPE = parts[4];

                    if (Globals.BDTYPE == "SQL")
                    {
                        Conexion = "data source = " + Globals.BDSERVER + "; " + "initial catalog = " + Globals.BD1 + "; " + "user id = " + Globals.BDUSER + "; " + "password = " + Globals.BDPASS;
                    }
                    else if (Globals.BDTYPE == "HANA")
                    {
                        if (IntPtr.Size == 8)
                            // Para 64-bit
                            Conexion = string.Concat(Conexion, "Driver={HDBODBC};");
                        else
                            // Para 32-bit
                            Conexion = string.Concat(Conexion, "Driver={HDBODBC32};");
                        Conexion = Conexion + "UID=" + Globals.BDUSER + ";PWD=" + Globals.BDPASS + ";SERVERNODE=" + Globals.BDSERVER + ";CS=" + Globals.BD1;
                    }
                    //Armo conexion con los parametros ingresados
                    //Conexion = "data source = " + UtilCS.goBeSYP_LTTCFG.SYP_LTDbServidor + "; " + "initial catalog = " + UtilCS.goBeSYP_LTTCFG.SYP_LTDbNombre + "; " + "user id = " + UtilCS.goBeSYP_LTTCFG.SYP_LTDbUsuario + "; " + "password = " + UtilCS.goBeSYP_LTTCFG.SYP_LTDbContrasena;

                }
                file.Close();
            }
            Console.ReadLine();
        }


        public static void ReadTextFile2()
        {
            try
            {
                Globals.BD1 = Globals.oCompany.CompanyDB;
                Globals.BDSERVER = Globals.oCompany.Server;
                Globals.BDUSER = Globals.oCompany.DbUserName;
                Globals.BDPASS = Globals.oCompany.DbPassword;
                if (Globals.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    Globals.BDTYPE = "HANA";
                }
                else
                {
                    Globals.BDTYPE = "SQL";
                }

                if (Globals.BDTYPE == "SQL")
                {
                    Conexion = "data source = " + Globals.BDSERVER + "; " + "initial catalog = " + Globals.BD1 + "; " + "user id = " + Globals.BDUSER + "; " + "password = " + Globals.BDPASS;
                }
                else if (Globals.BDTYPE == "HANA")
                {
                    if (IntPtr.Size == 8)
                        // Para 64-bit
                        Conexion = string.Concat(Conexion, "Driver={HDBODBC};");
                    else
                        // Para 32-bit
                        Conexion = string.Concat(Conexion, "Driver={HDBODBC32};");
                    Conexion = Conexion + "UID=" + Globals.BDUSER + ";PWD=" + Globals.BDPASS + ";SERVERNODE=" + Globals.BDSERVER + ";CS=" + Globals.BD1;
                }
                //Armo conexion con los parametros ingresados
                //Conexion = "data source = " + UtilCS.goBeSYP_LTTCFG.SYP_LTDbServidor + "; " + "initial catalog = " + UtilCS.goBeSYP_LTTCFG.SYP_LTDbNombre + "; " + "user id = " + UtilCS.goBeSYP_LTTCFG.SYP_LTDbUsuario + "; " + "password = " + UtilCS.goBeSYP_LTTCFG.SYP_LTDbContrasena;

            }
            catch (Exception ex)
            {
                Globals.SBO_Application.MessageBox(ex.ToString());
            }
        }

        //Query1
        public static SqlCommand cmd1;
        public static SqlConnection con1;
        public static SqlDataReader sdr1;

        //public static SqlCommand cmd2;
        //public static SqlConnection con2 = new SqlConnection(Conexion);
        //public static SqlDataReader sdr2;

        public static SqlDataReader TSQLQuery1(string Query)
        {
            try
            {

                SqlCommand cmd1;
                con1 = new SqlConnection(Conexion);
                //SqlDataReader sdr1;

                cmd1 = new SqlCommand(Query, con1);
                if (con1.State != System.Data.ConnectionState.Open)
                {
                    con1.Close();
                    con1.Open();
                }
                sdr1 = cmd1.ExecuteReader();
                return sdr1;
            }
            catch
            {
                return null;
            }
        }

        public static SqlDataReader ReleaseCon(SqlDataReader objeto)
        {
            Query = null;
            sdr1.Close();
            con1.Close();
            GC.Collect();
            return null;
        }

        public static int InsertSQL(string Query)
        {
            try
            {
                SqlCommand cmd2;
                //SqlDataReader sdr2;
                SqlConnection con2 = new SqlConnection(Conexion);

                cmd2 = new SqlCommand(Query, con2);
                cmd2.CommandTimeout = 6000;
                if (con2.State != System.Data.ConnectionState.Open)
                {
                    con2.Close();
                    con2.Open();
                }

                int rows = cmd2.ExecuteNonQuery();
                return rows;
            }
            catch (Exception ex)
            {
                throw new Exception("No se inserto la información en la tabla intermedia. ");
            }
        }
        public static int InsertHana(string Query)
        {
            OdbcConnection Cnn = new OdbcConnection(Conexion);
            //OdbcCommand cmd;
            //OdbcTransaction trs = null;
            try
            {
                //Cnn.Open();
                OdbcCommand cmd = new OdbcCommand(Query, Cnn);
                if (Cnn.State != System.Data.ConnectionState.Open)
                {
                    Cnn.Close();
                    Cnn.Open();
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("No se inserto la información en la tabla intermedia. ");
            }
            return 1;
        }


        public static OdbcDataReader myReader;
        public static OdbcConnection myConnection;
        public static OdbcDataReader THanaQuery1(string query)
        {
            myConnection = new OdbcConnection(Conexion);
            OdbcCommand myCommand;
            myCommand = new OdbcCommand(query, myConnection);

            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Close();
                myConnection.Open();
            }
            myReader = myCommand.ExecuteReader();
            return myReader;

            //myConnection.Open();
            //myReader = myCommand.ExecuteReader();
            //try
            //{
            //    //while (myReader.Read())
            //    //{
            //    //    Console.WriteLine(myReader.GetString(0));
            //    //}
            //    return myReader;
            //}
            //finally
            //{
            //    myConnection.Close();
            //}
        }
        public static OdbcDataReader Release(OdbcDataReader objeto)
        {
            myReader.Close();
            Query = null;
            GC.Collect();
            return null;
        }
        public static String ConvertDate2(string date)
        {
            string datS = "";
            if (date.Length == 8)
            {
                //datS = date.Substring(0, 2) + "/" + date.Substring(3, 4) + "/" + date.Substring(5, 6);
                //datS = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);
                datS = date.Substring(6, 2) + "/" + date.Substring(4, 2) + "/" + date.Substring(0, 4);
                //return Convert.ToDateTime(datS);
                return datS;
            }
            else
            {
                Globals.Error = "(SYP)BPS : Invalid Date Format";
                throw new Exception(Globals.Error);
            }
        }

        public static void ObtenerRuc(String CardCode, ref String Ruc)
        {
            try
            {
                Globals.Query = "SELECT COALESCE(\"LicTradNum\",'') FROM \"OCRD\" WHERE \"CardCode\" = '" + CardCode + "' \n";
                Globals.RunQuery(Globals.Query);
                Ruc = Globals.oRec.Fields.Item(0).Value.ToString();
                Globals.Release(Globals.oRec);

                if (String.IsNullOrEmpty(Ruc))
                {
                    throw new Exception("Favor de registrar el RUC del proveedor.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        public static bool ExisteUDO(string UDOname)
        {
            try
            {
                Globals.Query = "SELECT COUNT(1) FROM OUDO WHERE \"Code\" ='"+UDOname+"' ";
                Globals.RunQuery(Globals.Query);
                int iExUDO = Int32.Parse(Globals.oRec.Fields.Item(0).Value.ToString());
                Globals.Release(Globals.oRec);

                if (iExUDO == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
