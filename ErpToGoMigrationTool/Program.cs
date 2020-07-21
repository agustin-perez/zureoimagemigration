using System;
using Zureo.MigrarImagenes.DataAccess;
using System.Data;
using System.Data.SqlClient;
using ErpToGoMigrationTool.DataAccess;
using System.Drawing;
using Zureo.MigrarImagenes.Logic;
using System.Collections.Generic;
using System.Reflection;

namespace Zureo.MigrarImagenes
{
    class Program
    {
        /// <summary>
        /// Clase Main del programa.
        /// </summary>
        /// <param name="args">No utilizado.</param>
        static void Main(string[] args)
        {
            //Referencia de la clase para evitar el uso de métodos estáticos.
            Program reference = new Program();
            //Get startup path para logs
            FilesystemAccess.GetInstance.SetPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            FilesystemAccess.GetInstance.LogToDisk("Inicio de migración", FilesystemAccess.Logtype.Info);
            DatabaseAccess.GetInstance.ConnectionString = Utils.GetConnectionString();
            DatabaseAccess.GetInstance.InitConnection();
            Queries.GetInstance.CheckImagenesTable();
            Int16[] Empresas = Queries.GetInstance.GetEmpresas();
            Int16 LastEmpresa=0;
            try
            {
                for (int i = 0; i < Empresas.Length; i++)
                {
                    LastEmpresa = Empresas[i];
                    reference.Migration(Empresas[i]);
                }
            }
            catch (Exception)
            {
                FilesystemAccess.GetInstance.LogToDisk("Error desconocido al intentar procesar los artículos de la empresa: "+LastEmpresa+"Revisar conexión con BD y permisos de ejecución.", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod());
            }
            Console.WriteLine("\nPresione enter para salir.");
            Console.ReadLine();
        }

        public void Migration(int ArtEmpresa)
        {
            FilesystemAccess.GetInstance.LogToDisk("Inicio de artículos para empresa: " + ArtEmpresa, FilesystemAccess.Logtype.Info);
            string EmpPathImg = Queries.GetInstance.GetImgBasePath(ArtEmpresa);
            DataTable ArticleView = new DataTable();
            ArticleView = Queries.GetInstance.GetArticleEmpView(ArtEmpresa);
            List<ZArticle> ArticleList = new List<ZArticle>();

            foreach (DataRow row in ArticleView.Rows)
            {
                try
                {
                    //informaciones
                    Console.WriteLine("\nMigrando imagen de artículo: " + row.Field<int>((int)Queries.ArticleColumns.ArtId));
                    
                    string imgpath = row.Field<string>((int)Queries.ArticleColumns.ArtFoto);
                    if (!System.IO.Path.IsPathRooted(row.Field<string>((int)Queries.ArticleColumns.ArtFoto)))
                    {
                        imgpath = EmpPathImg + new Bitmap(row.Field<string>((int)Queries.ArticleColumns.ArtFoto));
                    }
                    ZImage newImage = new ZImage(new Bitmap(imgpath));
                    ArticleList.Add(new ZArticle(row.Field<int>((int)Queries.ArticleColumns.ArtId), row.Field<Int16>((int)Queries.ArticleColumns.ArtEmpresa), newImage));
                    
                    //Informaciones
                    Console.WriteLine("Ruta de la imagen procesándose: " + imgpath);
                    Console.WriteLine("Se procesó correctamente la imagen con Guid: " + newImage.Guid + " del artículo: " + row.Field<int>((int)Queries.ArticleColumns.ArtId));
                    FilesystemAccess.GetInstance.LogToDisk("Imagen con Guid: " + newImage.Guid + " del artículo: " + row.Field<int>((int)Queries.ArticleColumns.ArtId) + " Procesada correctamente.", FilesystemAccess.Logtype.Info);
                }
                catch (System.IO.FileNotFoundException) { FilesystemAccess.GetInstance.LogToDisk("Error de lectura al procesar imagen del artículo: " + row.Field<string>((int)Queries.ArticleColumns.ArtId), FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
                catch (SqlException) { FilesystemAccess.GetInstance.LogToDisk("Error al procesar imagen, ¿Está correctamente cargado el artículo: "+ row.Field<string>((int)Queries.ArticleColumns.ArtId) + " a la base? ", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
            }
        }
    }
}

