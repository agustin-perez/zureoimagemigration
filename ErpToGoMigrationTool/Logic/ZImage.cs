using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private string guid;
        private Bitmap Imagen;
        private ImageFormat FinalEncoder = ImageFormat.Jpeg;
        private EncoderParameters JPEGEncoderParams;
        private ImageCodecInfo JPEGCodec;

        /// <summary>
        /// Constructor por defecto de la clase.
        /// </summary>
        /// <param name="Imagen">Imagen correspondiente a la instancia del objeto.</param>
        public ZImage(Bitmap Imagen)
        {
            guid = guidObj.ToString();
            this.Imagen = Imagen;
            CompressionParams(Imagen);
            Console.WriteLine(Guid); 
        }

        /// <summary>
        /// Función la cual obtiene los parámetros de guardado de cualquier imagen soportada, para que la misma sea encodeada a JPEG y con una compresión del 50%.
        /// </summary>
        /// <param name="Imagen">Imagen a obtener los parámetros.</param>
        private void CompressionParams(Bitmap Imagen)
        {
            ImageCodecInfo[] codecArr = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecArr)
            {
                if (codec.FormatID == FinalEncoder.Guid)
                {
                    JPEGCodec = codec;
                }
            }
            System.Drawing.Imaging.Encoder JPEGEncoder = System.Drawing.Imaging.Encoder.Quality;
            JPEGEncoderParams = new EncoderParameters(1);
            EncoderParameter JPEGEncoderParam = new EncoderParameter(JPEGEncoder, 50L);
            JPEGEncoderParams.Param[0] = JPEGEncoderParam;
        }

        /// <summary>
        /// Getter de guid.
        /// </summary>
        public string Guid { get => guid; }
    }
}