using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Zureo.MigrarImagenes.Logic
{
    /// <summary>
    /// Clase encargada de manejar una imagen dada, correspondiente a un ZArticle.
    /// </summary>
    class ZImage
    {
        private Guid guidObj = System.Guid.NewGuid();
        private Bitmap Imagen;

        /// <summary>
        /// Constructor por defecto de la clase.
        /// </summary>
        /// <param name="Imagen">Imagen correspondiente a la instancia del objeto.</param>
        public ZImage(Bitmap Imagen)
        {
            this.Imagen = Imagen;
        }

        /// <summary>
        /// Getter de guid.
        /// </summary>
        public Guid GetGuid { get => guidObj; }

        /// <summary>
        /// Getter de Imagen
        /// </summary>
        public Bitmap GetImagen { get => Imagen; }
    }
}