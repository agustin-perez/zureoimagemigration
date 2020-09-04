using System;

namespace ErpToGoMigrationTool.DataAccess
{
    public static class Utils
    {
        private static string _connString;

        /// <summary>
        /// Función que obtiene el  string de conexión a la base de datos de Zureo, necesario tener instalado Zureo para que
        /// tome la conexión o configurada la conexión en el ServerConfig
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(_connString)) return _connString;

            try
            {
                ZureoUtilidades.ZUtilities.Inicialize(ZureoUtilidades.ZUtilities.InicializeTypes.UseConnectionFromZureo, "ORG04", iniDlls:true, Logs: false);
            }
            catch
            {
                ZureoUtilidades.ZUtilities.Inicialize(ZureoUtilidades.ZUtilities.InicializeTypes.UseConnectionFromServerConfig, idBaseDatos:"ZFactura1", iniDlls: true, Logs: false);
            }

            _connString = ZureoUtilidades.ZUtilities.ConnectionString ?? throw new Exception("No se pudo obtener el string conexión");

            return _connString;
        }
    }
}
