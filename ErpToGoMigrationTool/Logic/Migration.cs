using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using ErpToGoMigrationTool.DataAccess;

namespace ErpToGoMigrationTool.Logic
{
    class Migration
    {
        private bool isFenicio;
        private DataTable ArticleView;

        /// <summary>
        /// Función encargada de extraer todas las imágenes y cargarlas en memoria, para luego procesarlas.
        /// </summary>
        /// <param name="ArtEmpresa">Id de la empresa a realizar la extracción de imágenes.</param>
        public Migration(int ArtEmpresa, bool isFenicio)
        {
            this.isFenicio = isFenicio;
            string EmpPathImg = Queries.GetInstance.GetImgBasePath(ArtEmpresa);
            if (EmpPathImg == ""){FilesystemAccess.GetInstance.LogToDisk("La empresa: " + ArtEmpresa + " No tiene ruta de imágenes seteada, esto va a dar problemas si hay imágenes para TPV.", FilesystemAccess.Logtype.Info);}

            ArticleView = new DataTable();
            ArticleView = Queries.GetInstance.GetArticleEmpView(ArtEmpresa);
            foreach (DataRow row in ArticleView.Rows)
            {
                try
                { 
                    ArticleProcessing(EmpPathImg, row.Field<String>((int)Queries.ArticleColumns.ArtFoto), row.Field<int>((int)Queries.ArticleColumns.ArtId), row.Field<Int16>((int)Queries.ArticleColumns.ArtEmpresa));
                }
                catch (FileNotFoundException) { FilesystemAccess.GetInstance.LogToDisk("Error de lectura al procesar imagen del artículo: " + row.Field<string>((int)Queries.ArticleColumns.ArtId), FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
                catch (ArgumentException) { FilesystemAccess.GetInstance.LogToDisk("Ruta de imagen inaccesible, ¿Fue movida de su ruta original? artículo: " + row.Field<int>((int)Queries.ArticleColumns.ArtId), FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
                catch (SqlException) { FilesystemAccess.GetInstance.LogToDisk("Error al procesar imagen, ¿Está correctamente cargado el artículo: " + row.Field<string>((int)Queries.ArticleColumns.ArtId) + " a la base? ", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
                catch (Exception) { FilesystemAccess.GetInstance.LogToDisk("Error al escribir la imagen, ¿La misma es muy pesada o estará mal encodeada? ¿Guid mal generado? Prueba correr el programa nuevamente ", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod()); }
            }
        }

        private void ArticleProcessing(string EmpPathImg, string ArtFoto, Int32 ArtId, Int16 ArtEmpresa)
        {
            Console.WriteLine("\nLeyendo imagen de artículo: " + ArtId);
            Import DataImport = new Import();
            ZImage newImage = new ZImage(new Bitmap(FilesystemAccess.GetInstance.ImagePath(EmpPathImg, ArtFoto)));

            if (!isFenicio)
            {
                if (!Queries.GetInstance.CheckImgDuplicate(ArtId))
                {
                    if (FilesystemAccess.GetInstance.CheckTPVImage(EmpPathImg, ArtFoto))
                    {
                        ZImage newTPVImage = new ZImage(new Bitmap(FilesystemAccess.GetInstance.GetTPVImage(EmpPathImg, ArtFoto)));
                        DataImport.ArticleImport(new ZArticle(ArtId, ArtEmpresa, newImage, newTPVImage));
                    }
                    else
                    {
                        ZImage nullImg = new ZImage(null);
                        DataImport.ArticleImport(new ZArticle(ArtId, ArtEmpresa, newImage, nullImg));
                    }
                }
                else
                {
                    FilesystemAccess.GetInstance.LogToDisk("No se guardó la imagen del artículo: " + ArtId.ToString() + " ya que la misma fue migrada anteriormente. ", FilesystemAccess.Logtype.Warning);
                }
            }
            else
            {
                if (FilesystemAccess.GetInstance.CheckTPVImage(EmpPathImg, ArtFoto))
                {
                    ZImage newTPVImage = new ZImage(new Bitmap(FilesystemAccess.GetInstance.GetTPVImage(EmpPathImg, ArtFoto)));
                    DataImport.FenicioImport(new ZArticle(ArtId, ArtEmpresa, newImage, newTPVImage));
                }
                else
                {
                    ZImage nullImg = new ZImage(null);
                    DataImport.FenicioImport(new ZArticle(ArtId, ArtEmpresa, newImage, nullImg));
                }
            }
            FilesystemAccess.GetInstance.LogToDisk("Ruta de la imagen procesándose: " + FilesystemAccess.GetInstance.ImagePath(EmpPathImg, ArtFoto), FilesystemAccess.Logtype.Info);
            FilesystemAccess.GetInstance.LogToDisk("Se procesó correctamente la imagen con Guid: " + newImage.GetGuid + " del artículo: " + ArtId, FilesystemAccess.Logtype.Info);
        }
        
    }
}
