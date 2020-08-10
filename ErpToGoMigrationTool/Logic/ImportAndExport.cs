using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using ErpToGoMigrationTool.DataAccess;
using Zureo.MigrarImagenes.DataAccess;
using Zureo.MigrarImagenes.Logic;

namespace ErpToGoMigrationTool.Logic
{
    class ImportAndExport
    {
        /// <summary>
        /// Función encargada de extraer todas las imágenes y cargarlas en memoria, para luego procesarlas.
        /// </summary>
        /// <param name="ArtEmpresa">Id de la empresa a realizar la extracción de imágenes.</param>
        public void Migration(int ArtEmpresa, List<ZArticle> ArticleList)
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
                catch (SqlException) { FilesystemAccess.GetInstance.LogToDisk("Error al procesar imagen, ¿Está correctamente cargado el artículo: " + row.Field<string>((int)Queries.ArticleColumns.ArtId) + " a la base? ", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
            }
        }

        /// <summary>
        /// Subrutina encargada de derivar cada Artículo instanciado a la clase necesaria para su escritura en disco y en BD.
        /// </summary>
        /// <param name="articulo">Artículo a realizar la escritura.</param>
        /// <param name="savePath">Ruta a guardar la imagen en base al Guid.</param>
        public void ArticleImport(ZArticle articulo, string savePath)
        {
            try
            {
                FilesystemAccess.GetInstance.WriteJPEG(articulo.artImg.GetImagen, savePath + articulo.artImg.GetGuid + ".jpg", articulo.artImg.GetJPEGCodec, articulo.artImg.GetJPEGEncoderParams);
                Queries.GetInstance.ImageInsert(articulo.artImg.GetGuid, articulo.artID);
                FilesystemAccess.GetInstance.LogToDisk("Se ha migrado correctamente la imagen con Guid: " + articulo.artImg.GetGuid, FilesystemAccess.Logtype.Info);
            }
            catch (Exception)
            {
                FilesystemAccess.GetInstance.LogToDisk("Error al escribir imagen a disco, perteneciente al artículo:  " + articulo.artID + " ¿La imagen es muy pesada o estará mal encodeada? ¿Guid mal generado? Prueba correr el programa nuevamente", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod());
            }
        }

        private void FenicioImport(ZArticle articulo, string savePath)
        {
            try
            {
                FilesystemAccess.GetInstance.WriteJPEG(articulo.artImg.GetImagen, savePath + articulo.artID + "-0.jpg", articulo.artImg.GetJPEGCodec, articulo.artImg.GetJPEGEncoderParams);
                FilesystemAccess.GetInstance.LogToDisk("Se ha migrado correctamente la imagen con el ID: " + articulo.artID+"-0.jpg", FilesystemAccess.Logtype.Info);
            }
            catch (Exception)
            {
                ///FilesystemAccess.GetInstance.LogToDisk("Error al escribir imagen a disco, perteneciente al artículo:  " + articulo.artID + " ¿La imagen es muy pesada o estará mal encodeada? ¿Guid mal generado? Prueba correr el programa nuevamente", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod());
            }
        }
    }
}
