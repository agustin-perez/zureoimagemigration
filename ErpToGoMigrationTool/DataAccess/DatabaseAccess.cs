using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ErpToGoMigrationTool.DataAccess
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
        private DatabaseAccess() { }

        /// <summary>
        /// Clase encargada de inicializar la conexión en la instancia Singleton a la BD.
        /// </summary>
        public void InitConnection()
        {
            try
            {
                if (Connection == null) { Connection = new SqlConnection(connectionString); }
                if (Connection.State != ConnectionState.Open) { Connection.Open(); }
            }
            catch (Exception e)
            {
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
            if (Connection.State != ConnectionState.Open) { Connection.Close(); }
        }


        /// <summary>
        /// Función encargada de devolver tablas provenientes de consultas SQL.
        /// </summary>
        /// <param name="sqlquery">Consulta SQL.</param>
        /// <returns>Tabla con los datos solicitados en la consulta.</returns>
        public DataTable TableQuery(string sqlquery)
        {
            DatabaseAccess.GetInstance.InitConnection();
            SqlCommand query = new SqlCommand(sqlquery, DatabaseAccess.GetInstance.GetConnection);
            SqlDataAdapter result = new SqlDataAdapter(query);
            DataTable toBeReturned = new DataTable();
            result.Fill(toBeReturned);
            return toBeReturned;
        }

        /// <summary>
        /// Función genérica para obtener consultas de un solo campo.
        /// </summary>
        /// <typeparam name="T">Tipo de dato a obtener.</typeparam>
        /// <param name="sqlquery">Consulta SQL.</param>
        /// <returns>Dato devuelto, convertido al tipo especificado en la función.</returns>
        public T GenericFieldQuery<T>(string sqlquery)
        {
            DatabaseAccess.GetInstance.InitConnection();
            SqlCommand query = new SqlCommand(sqlquery, DatabaseAccess.GetInstance.GetConnection);
            object result = query.ExecuteScalar();
            if (result != null)
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            return default(T);
        }

        /// <summary>
        /// Función encargada de realizar un instert a la BD en base a una lista de Parámetros SQL creada en la clase Queries.
        /// </summary>
        /// <param name="sqlquery">Consulta SQL.</param>
        /// <param name="paramList">Lista de parámetros SQL.</param>
        public void InsertByParams(string sqlquery, List<SqlParameter> paramList)
        {
            DatabaseAccess.GetInstance.InitConnection();
            SqlCommand query = new SqlCommand(sqlquery, DatabaseAccess.GetInstance.GetConnection);
            foreach (SqlParameter param in paramList)
            {
                query.Parameters.Add(param);
            }
            query.ExecuteNonQuery();
        }

        /// <summary>
        /// Setter de ConnectionString.
        /// </summary>
        public String ConnectionString { set => connectionString = value; }

        /// <summary>
        /// Getter de Connection.
        /// </summary>
        public SqlConnection GetConnection { get => Connection; } 
    }
}
