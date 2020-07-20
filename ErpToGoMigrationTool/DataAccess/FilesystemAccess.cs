using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZureoUtilidades;

namespace Zureo.MigrarImagenes.DataAccess
{
    class FilesystemAccess
    {
        private static FilesystemAccess instance;
        private static String logpath;
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

        public void LogInfo(string Info)
        {

        }

        public void LogError(string Error, MethodBase Trace)
        {
            /*ZUtilities.LogEnDiscoPath = logpath;
            System.Console.WriteLine(logpath);
            System.Console.WriteLine(ZUtilities.LogEnDiscoPath);
            System.Console.Read();
            ZUtilities.LogEnDisco("ErrorLog.txt", Trace, Error);
            */
        }

        /// <summary>
        /// Setter de logpath.
        /// </summary>
        public string SetPath { set => logpath = value; }
    }
}
