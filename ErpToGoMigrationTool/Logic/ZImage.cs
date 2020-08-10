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
        private ImageFormat FinalEncoder = ImageFormat.Jpeg;
        private EncoderParameters JPEGEncoderParams;
        private ImageCodecInfo JPEGCodec;

        /// <summary>
        /// Constructor por defecto de la clase.
        /// </summary>
        /// <param name="Imagen">Imagen correspondiente a la instancia del objeto.</param>
        public ZImage(Bitmap Imagen)
        {
            this.Imagen = Imagen;
            CompressionParams();
        }

        /// <summary>
        /// Función la cual obtiene los parámetros de guardado de cualquier imagen soportada, para que la misma sea encodeada a JPEG y con una compresión del 35%.
        /// </summary>
        /// <param name="Imagen">Imagen a obtener los parámetros.</param>
        private void CompressionParams()
        {
            ImageCodecInfo[] codecArr = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecArr)
            {
                if (codec.FormatID == FinalEncoder.Guid)
                {
                    JPEGCodec = codec;
                    break;
                }
            }
            System.Drawing.Imaging.Encoder JPEGEncoder = System.Drawing.Imaging.Encoder.Quality;
            JPEGEncoderParams = new EncoderParameters(1);
            EncoderParameter JPEGEncoderParam = new EncoderParameter(JPEGEncoder, 65L);
            JPEGEncoderParams.Param[0] = JPEGEncoderParam;
        }

        /// <summary>
        /// Getter de guid.
        /// </summary>
        public Guid GetGuid { get => guidObj; }

        /// <summary>
        /// Getter de JPEGEncoderParams
        /// </summary>
        public EncoderParameters GetJPEGEncoderParams { get => JPEGEncoderParams; }

        /// <summary>
        /// Getter de JPEGCodec
        /// </summary>
        public ImageCodecInfo GetJPEGCodec { get => JPEGCodec; }

        /// <summary>
        /// Getter de Imagen
        /// </summary>
        public Bitmap GetImagen { get => Imagen; }
    }
}