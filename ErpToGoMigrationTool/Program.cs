using ErpToGoMigrationTool.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using Zureo.MigrarImagenes.DataAccess;
using Zureo.MigrarImagenes.Logic;

namespace Zureo.MigrarImagenes
{
    /// <summary>
    /// Clase principal del programa.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Método Main del programa.
        /// </summary>
        /// <param name="args">No se utilizan parámetros de inicialización.</param>
        static void Main(string[] args)
        {
            FilesystemAccess.GetInstance.SetExecutionPath = "C:\\Zureo Software\\Imagenes Exportadas GO\\";
            FilesystemAccess.GetInstance.CreateExportDir("C:\\Zureo Software\\Imagenes Exportadas GO\\");

            FilesystemAccess.GetInstance.LogToDisk("Inicio de ErpToGoMigrationTool", FilesystemAccess.Logtype.Info);
            try
            { 
                DatabaseAccess.GetInstance.ConnectionString = Utils.GetConnectionString();
                DatabaseAccess.GetInstance.InitConnection();
                Queries.GetInstance.CheckImagenesTable();
            }
            catch (Exception) 
            { 
                FilesystemAccess.GetInstance.LogToDisk("No se pudo conectar a la base de datos. Revisar configuración en Zureo.", FilesystemAccess.Logtype.Error); 
                Environment.Exit(1);
            }

            Int16[] Empresas = Queries.GetInstance.GetEmpresas();
            Int16 LastEmpresa = 0;
            try
            {
                for (int i = 0; i < Empresas.Length; i++)
                {
                    LastEmpresa = Empresas[i];
                    List<ZArticle> ArticleList = new List<ZArticle>();

                    FilesystemAccess.GetInstance.LogToDisk("Inicio de Exportación, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);
                    //Se utiliza reference para evitar referencias estáticas a los métodos en main.
                    Migration(Empresas[i], ArticleList);
                    FilesystemAccess.GetInstance.LogToDisk("Fin de Exportación, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);

                    FilesystemAccess.GetInstance.LogToDisk("Inicio de Importación, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);
                    foreach (ZArticle articulo in ArticleList)
                    {
                        //Se utiliza reference para evitar referencias estáticas a los métodos en main.
                        if (!Queries.GetInstance.CheckImgDuplicate(articulo.artID))
                        {
                            ArticleImport(articulo, FilesystemAccess.GetInstance.GetExportPath);
                        }
                        else
                        {
                            FilesystemAccess.GetInstance.LogToDisk("No se guardó la imagen del artículo: " + articulo.artID + " ya que la misma fue migrada anteriormente. ", FilesystemAccess.Logtype.Warning);
                        }
                    }
                    FilesystemAccess.GetInstance.LogToDisk("Fin de Importación, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);
                }
            }
            catch (Exception)
            { 
               FilesystemAccess.GetInstance.LogToDisk("Error desconocido al intentar procesar los artículos de la empresa: "+ LastEmpresa +" Revisar conexión con BD y permisos de ejecución. Se intentará continuar con el resto de empresas.", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod());
            }

            FilesystemAccess.GetInstance.LogToDisk("Fin de migración a GO", FilesystemAccess.Logtype.Info);
            Console.WriteLine("\nPresione enter para salir y abrir la carpeta con las imágenes exportadas.");
            Console.ReadLine();
            Process.Start("explorer.exe", FilesystemAccess.GetInstance.GetExportPath);
        }

        /// <summary>
        /// Función encargada de extraer todas las imágenes y cargarlas en memoria, para luego procesarlas.
        /// </summary>
        /// <param name="ArtEmpresa">Id de la empresa a realizar la extracción de imágenes.</param>
        private static void Migration(int ArtEmpresa, List<ZArticle> ArticleList)
        {
            string EmpPathImg = Queries.GetInstance.GetImgBasePath(ArtEmpresa);
            DataTable ArticleView = new DataTable();
            ArticleView = Queries.GetInstance.GetArticleEmpView(ArtEmpresa);
            foreach (DataRow row in ArticleView.Rows)
            {
                try
                {
                    Console.WriteLine("\nMigrando imagen de artículo: " + row.Field<int>((int)Queries.ArticleColumns.ArtId));
                    string imgpath = row.Field<string>((int)Queries.ArticleColumns.ArtFoto);

                    if (imgpath == "")
                    {
                        //Se lanza excepción en caso de que la imagen no se encuentre en su ruta correspondiente.
                        throw new FileNotFoundException();
                    }

                    if (!System.IO.Path.IsPathRooted(row.Field<string>((int)Queries.ArticleColumns.ArtFoto)))
                    {
                        imgpath = EmpPathImg + row.Field<string>((int)Queries.ArticleColumns.ArtFoto);
                    }
                    ZImage newImage = new ZImage(new Bitmap(imgpath));
                    ArticleList.Add(new ZArticle(row.Field<int>((int)Queries.ArticleColumns.ArtId), row.Field<Int16>((int)Queries.ArticleColumns.ArtEmpresa), newImage));
                    Console.WriteLine("Ruta de la imagen procesándose: " + imgpath);
                    FilesystemAccess.GetInstance.LogToDisk("Se procesó correctamente la imagen con Guid: " + newImage.GetGuid + " del artículo: " + row.Field<int>((int)Queries.ArticleColumns.ArtId) + " Procesada correctamente.", FilesystemAccess.Logtype.Info);
                }
                catch (FileNotFoundException) { FilesystemAccess.GetInstance.LogToDisk("Error de lectura al procesar imagen del artículo: " + row.Field<string>((int)Queries.ArticleColumns.ArtId), FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
                //Este catch es tomado en base a un ArgumentException cuando no se puede crear el Bitmap por falta de la imagen.
                catch (ArgumentException) { FilesystemAccess.GetInstance.LogToDisk("Ruta de imagen inaccesible, ¿Fue movida de su ruta original? artículo: " + row.Field<int>((int)Queries.ArticleColumns.ArtId), FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
                catch (SqlException) { FilesystemAccess.GetInstance.LogToDisk("Error al procesar imagen, ¿Está correctamente cargado el artículo: "+ row.Field<string>((int)Queries.ArticleColumns.ArtId) + " a la base? ", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
            }       
        }

        /// <summary>
        /// Subrutina encargada de derivar cada Artículo instanciado a la clase necesaria para su escritura en disco y en BD.
        /// </summary>
        /// <param name="articulo">Artículo a realizar la escritura.</param>
        /// <param name="savePath">Ruta a guardar la imagen en base al Guid.</param>
        private static void ArticleImport(ZArticle articulo, string savePath)
        {
            try
            {
                FilesystemAccess.GetInstance.WriteJPEG(articulo.artImg.GetImagen ,savePath + articulo.artImg.GetGuid + ".jpg", articulo.artImg.GetJPEGCodec, articulo.artImg.GetJPEGEncoderParams);
                Queries.GetInstance.ImageInsert(articulo.artImg.GetGuid, articulo.artID);
                FilesystemAccess.GetInstance.LogToDisk("Se ha migrado correctamente la imagen con Guid: "+ articulo.artImg.GetGuid, FilesystemAccess.Logtype.Info);
            }
            catch (Exception)
            {
                FilesystemAccess.GetInstance.LogToDisk("Error al escribir imagen a disco, perteneciente al artículo:  " + articulo.artID + " ¿La imagen es muy pesada o estará mal encodeada? ¿Guid mal generado? Prueba correr el programa nuevamente", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod());
            }
        }
    }
}

