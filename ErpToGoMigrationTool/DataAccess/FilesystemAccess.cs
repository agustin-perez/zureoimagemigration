using System;
using System.Drawing;
using System.Reflection;

namespace Zureo.MigrarImagenes.DataAccess
{
    /// <summary>
    /// Clase encargada de la comunicación y conexión hacia el sistema de archivos.
    /// </summary>
    class FilesystemAccess
    {
        private static FilesystemAccess instance;
        private static String logpath;

        /// <summary>
        /// Función encargada de instanciar y devolver una única instancia Singleton de la clase.
        /// </summary>
        public static FilesystemAccess GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FilesystemAccess();
                }
                return instance;
            }
        }

        /// <summary>
        /// Función encargada de devolver un nuevo Bitmap basado en una ruta del sistema de archivos.
        /// </summary>
        /// <param name="path">Ruta absoluta al archivo de imagen.</param>
        /// <returns>Bitmap con la imagen cargada.</returns>
        public Bitmap GetDiskImage(string path)
        {
            return new Bitmap(path);
        }

        /// <summary>
        /// Subrutina encargada de loggear la información solicitada de un evento a disco.
        /// </summary>
        /// <param name="msj">Mensaje del evento a loggear.</param>
        /// <param name="type">Tipo (enumerado) del evento.</param>
        /// <param name="trace">Información del objeto afectado (parámetro opcional).</param>
        public void LogToDisk(string msj, Logtype type , MethodBase trace=null)
        {
            string logformat;
            if (trace != null) 
            { 
                logformat = ("\r\n" + System.DateTime.Now + " [" + type.ToString() + "]: \"" + msj + "\" En: \"" + trace.ToString() + "\""); 
            }
            else 
            { 
                logformat = ("\r\n" + System.DateTime.Now + " [" + type.ToString() + "]: \"" + msj + "\""); 
            }
            System.IO.File.AppendAllText(logpath + "\\ErpToGoLog.txt", logformat);
        }

        /// <summary>
        /// Enumerador de los tipos de información para loggear.
        /// </summary>
        public enum Logtype
        {
            Error,
            Warning,
            Info
        }

        /// <summary>
        /// Setter de logpath.
        /// </summary>
        public string SetPath { set => logpath = value; }
    }
}
