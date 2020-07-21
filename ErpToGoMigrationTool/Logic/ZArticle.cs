using System;

namespace Zureo.MigrarImagenes.Logic
{
    class ZArticle
    {
        private int ArtID;
        private Int16 ArtEmpresa;
        private ZImage ArtImg;
        public ZArticle(int ArtID, Int16 ArtEmpresa, ZImage ArtImg)
        {
            this.ArtID = ArtID;
            this.ArtEmpresa = ArtEmpresa;
            this.ArtImg = ArtImg;
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
    }
}
