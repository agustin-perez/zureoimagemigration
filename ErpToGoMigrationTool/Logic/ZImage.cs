using System;
using System.Drawing;

namespace ErpToGoMigrationTool.Logic
{
    /// <summary>
    /// Clase encargada de manejar una imagen dada, correspondiente a un ZArticle.
    /// </summary>
    class ZImage
    {
        private Guid guidObj = System.Guid.NewGuid();
        private Bitmap Image;

        /// <summary>
        /// Constructor por defecto de la clase.
        /// </summary>
        /// <param name="Imagen">Imagen correspondiente a la instancia del objeto.</param>
        public ZImage(Bitmap Image)
        {
            this.Image = Image;
        }

        /// <summary>
        /// Getter de guid.
        /// </summary>
        public Guid GetGuid { get => guidObj; }

        /// <summary>
        /// Getter de Image
        /// </summary>
        public Bitmap GetImage { get => Image; }
    }
}