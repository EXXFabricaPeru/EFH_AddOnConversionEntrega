using System;
using System.Windows.Forms;
using AddonConEntrega.commons;
using AddonConEntrega.conexion;

namespace AddonConEntrega
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Conexion oConexion = new Conexion();

            if ((oConexion != null) && (Conexion.company.Connected))
            {
                EstructuraDatos oEstructuraDatos = new EstructuraDatos();

                GC.KeepAlive(oConexion);
                FormCommon.StatusMessageSuccess("Inicio satisfactorio."); 
                Application.Run();
            }
            else
                Application.Exit();

            Application.Run();
        }
    }
}
