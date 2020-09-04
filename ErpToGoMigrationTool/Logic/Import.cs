using System;
using System.Reflection;
using ErpToGoMigrationTool.DataAccess;

namespace ErpToGoMigrationTool.Logic
{
    class Import
    {
        /// <summary>
        /// Subrutina encargada de derivar cada Artículo instanciado a la clase necesaria para su escritura en disco y en BD.
        /// </summary>
        /// <param name="articulo">Artículo a realizar la escritura.</param>
        public void ArticleImport(ZArticle articulo)
        { 
            FilesystemAccess.GetInstance.WriteJPEG(articulo.artImg.GetImage, articulo.TPVartImg.GetImage, articulo.artImg.GetGuid.ToString());
            Queries.GetInstance.ImageInsert(articulo.artImg.GetGuid, articulo.artID);
        }

        /// <summary>
        /// Subrutina encargada de administrar la inserción de las imágenes correspondientes al caso de Fenicio.
        /// </summary>
        /// <param name="articulo">Artículo a realizar la escritura.</param>
        /// <param name="savePath">Ruta a guardar la imagen en base al ArtID.</param>
        public void FenicioImport(ZArticle articulo)
        {
            FilesystemAccess.GetInstance.WriteJPEG(articulo.artImg.GetImage, articulo.TPVartImg.GetImage, articulo.artImg.GetGuid.ToString(), articulo.artID.ToString());
        }
    }
}
