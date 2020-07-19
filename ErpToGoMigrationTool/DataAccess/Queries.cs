using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Zureo.MigrarImagenes.DataAccess;

namespace ErpToGoMigrationTool.DataAccess
{
    /// <summary>
    /// Clase encargada de todas las consultas SQL realizadas mediante la clase DatabaseAccess.
    /// </summary>
    class Queries
    {
        private static Queries instance;
        /// <summary>
        /// Función encargada de instanciar y devolver una única instancia Singleton de la clase.
        /// </summary>
        public static Queries GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Queries();
                }
                return instance;
            }
        }
        /// <summary>
        /// Función encargada de devolver un conjunto de identificadores de empresas.
        /// </summary>
        /// <returns>Un único valor de identificación para cada empresa del sistema.</returns>
        public Int16[] GetEmpresas()
        {
            DataTable queryEmpresas = TableQuery ("select distinct ArtEmpresa from Articulo;");
            List<Int16> empList = new List<Int16>();
            for (int i=0; i < queryEmpresas.Rows.Count; i++)
            {
                empList.Add(queryEmpresas.Rows[i].Field<Int16>(0));
            }
            return empList.ToArray();
        }

        /// <summary>
        /// Función engargada de devolver la ruta base de imágenes de una empresa dada.
        /// </summary>
        /// <param name="EmpId">Identificador de dicha empresa.</param>
        /// <returns>Ruta absoluta de las imágenes de artículos de dicha empresa.</returns>
        public string GetImgBasePath(int EmpId)
        {
            DatabaseAccess.GetInstance.InitConnection();
            SqlCommand query = new SqlCommand("select EmpPathImg from cceEmpresas where EmpId = '" + EmpId + "';", DatabaseAccess.GetInstance.GetConnection);
            SqlDataReader result = query.ExecuteReader();
            return result.GetString(0);
        }
       
        /// <summary>
        /// Función encargada de devolver una tabla estilo "vista" con los 
        /// datos necesarios para poder instanciar los objetos de Article.
        /// </summary>
        /// <param name="ArtEmpresa">Empresa a la cual se van a extraer dichos Artículos.</param>
        /// <returns>Tabla con todos los Artículos de dicha empresa, 
        /// lista para instanciar los objetos de Artículo.</returns>
        public DataTable GetArticleView(int EmpId)
        {
            return TableQuery("select ArtId, ArtFoto from Articulo, Imagenes where articulo.ArtID != Imagenes.ImgIdDato and Articulo.ArtEmpresa = '" + EmpId + "' AND ArtFoto is not nul;");
        }

        /// <summary>
        /// Enumerador encargado de identificar las diferentes columnas de la DataTable devuelta en GetArticleData.
        /// </summary>
        public enum ArticleColumns { EmpId = 0, ArtId = 0, ArtEmpresa, ArtFoto }

        /// <summary>
        /// Función encargada de devolver tablas provenientes de consultas SQL.
        /// </summary>
        /// <param name="sqlquery">Consulta SQL</param>
        /// <returns>Tabla con los datos solicitados en la consulta.</returns>
        private DataTable TableQuery(string sqlquery)
        {
            DatabaseAccess.GetInstance.InitConnection();
            SqlCommand query = new SqlCommand(sqlquery, DatabaseAccess.GetInstance.GetConnection);
            SqlDataAdapter result = new SqlDataAdapter(query);
            DataTable toBeReturned = new DataTable();
            result.Fill(toBeReturned);
            return toBeReturned;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlquery"></param>
        /// <returns></returns>
        private T GenericFieldQuery<T>(string sqlquery)
        {
            DatabaseAccess.GetInstance.InitConnection();
            SqlCommand query = new SqlCommand(sqlquery, DatabaseAccess.GetInstance.GetConnection);
            SqlDataReader result = query.ExecuteReader();
            return (T)Convert.ChangeType(result.GetValue(0), typeof(T));
        }

    }



}
/*SqlDataReader result = query.ExecuteReader();
           List<String> paths = new List<string>();
           while (result.Read())
           {
               paths.Add(result.GetString(0));
           }
           return paths;*/
