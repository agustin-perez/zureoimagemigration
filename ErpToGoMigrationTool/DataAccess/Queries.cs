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
        /// <summary>
        /// Función encargada de devolver una tabla estilo "vista" con los 
        /// datos necesarios para poder instanciar los objetos de Article.
        /// </summary>
        /// <param name="ArtEmpresa">Empresa a la cual se van a extraer dichos Artículos.</param>
        /// <returns>Tabla con todos los Artículos de dicha empresa, 
        /// lista para instanciar los objetos de Artículo.</returns>
        public DataTable GetArticleData(int ArtEmpresa)
        {
            DatabaseAccess.GetInstance.InitConnection();
            SqlCommand query = new SqlCommand("select ArtId, ArtFoto from Articulo, Imagenes where articulo.ArtID != Imagenes.ImgIdDato and Articulo.ArtEmpresa = "+ ArtEmpresa +";", DatabaseAccess.GetInstance.GetConnection);
            SqlDataAdapter result = new SqlDataAdapter(query);
            DataTable articles = new DataTable();
            result.Fill(articles);
            return articles;
        }
        public enum ArticleColumns { ArtId, ArtEmpresa, ArtFoto }
        /// <summary>
        /// Función encargada de devolver un conjunto de identificadores de empresas.
        /// </summary>
        /// <returns>Un único valor de identificación para cada empresa del sistema.</returns>
        public int[] GetEmpresas()
        {
            DatabaseAccess.GetInstance.InitConnection();
            SqlCommand query = new SqlCommand("select distinct ArtEmpresa from Articulo;", DatabaseAccess.GetInstance.GetConnection);
            SqlDataAdapter result = new SqlDataAdapter(query);
            DataTable articles = new DataTable();
            result.Fill(articles);
            return null;
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
