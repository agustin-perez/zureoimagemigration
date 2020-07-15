using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace Zureo.MigrarImagenes.DataAccess
{
    /// <summary>
    /// Clase encargada de la comunicación y conexión hacia la BD.
    /// </summary>
    class DatabaseAccess
    {
        private static DatabaseAccess instance;
        private static string connectionString;
        private static SqlConnection Connection;

        /// <summary>
        /// Función encargada de instanciar y devolver una única instancia Singleton de la clase.
        /// </summary>
        public static DatabaseAccess GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseAccess();
                }
                return instance;
            }
        }

        /// <summary>
        /// Constructor por defecto de la clase.
        /// </summary>
        private DatabaseAccess(){}

        /// <summary>
        /// Clase encargada de inicializar la conexión en la instancia Singleton a la BD.
        /// </summary>
        public void InitConnection()
        {
           try
           {
                if (Connection == null){ Connection = new SqlConnection(connectionString); }
                if (Connection.State != ConnectionState.Open) { Connection.Open(); }
           }
            catch (Exception e)
            {
                FilesystemAccess.GetInstance.LogError("Error al conectar a la base de datos.", MethodBase.GetCurrentMethod());
                Console.WriteLine(e.StackTrace);
                Console.Read();
            }
        }

        /// <summary>
        /// Función encargada de cerrar la conexión provista por la instancia Singleton a la BD.
        /// </summary>
        public void CloseConnection()
        {
            //Se chequea la conexión para evitar excepción en caso de que el objeto haya sido desechado.
            if ( Connection.State != ConnectionState.Open ) { Connection.Close(); }
        }

        /// <summary>
        /// Setter de ConnectionString.
        /// </summary>
        public String ConnectionString
        {
            set
            {
                connectionString = value;
            }
        }

        /// <summary>
        /// Getter de Connection.
        /// </summary>
        public SqlConnection GetConnection
        {
            get
            {
                return Connection;
            }
        }
    }
}
