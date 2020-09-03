using System;

namespace Zureo.MigrarImagenes.Logic
{
    /// <summary>
    /// Clase encargada de manejar lógicamente un artículo de Zureo.
    /// </summary>
    class ZArticle
    {
        private int ArtID;
        private Int16 ArtEmpresa;
        private ZImage ArtImg;
        private ZImage TPVArtImg;

        /// <summary>
        /// Constructor por defecto de la clase ZArticle.
        /// </summary>
        /// <param name="ArtID">ID de artículo.</param>
        /// <param name="ArtEmpresa">Empresa a la que pertenece el artículo.</param>
        /// <param name="ArtImg">Imagen del artículo en cuestión.</param>
        public ZArticle(int ArtID, Int16 ArtEmpresa, ZImage ArtImg, ZImage TPVArtImg)
        {
            this.ArtID = ArtID;
            this.ArtEmpresa = ArtEmpresa;
            this.ArtImg = ArtImg;
            this.TPVArtImg = TPVArtImg;
        }

        /// <summary>
        /// Getter de ArtID.
        /// </summary>
        public int artID { get => ArtID; }

        /// <summary>
        /// Getter de ArtEmpresa.
        /// </summary>
        public short artEmpresa { get => ArtEmpresa;  }

        /// <summary>
        /// Getter de ArtImg.
        /// </summary>
        internal ZImage artImg { get => ArtImg; }

        /// <summary>
        /// Getter de TPVArtImg.
        /// </summary>
        internal ZImage TPVartImg { get => TPVArtImg; }
    }
}
