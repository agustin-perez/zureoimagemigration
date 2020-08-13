using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace Zureo.MigrarImagenes.DataAccess
{
    /// <summary>
    /// Clase encargada de la comunicación y conexión hacia el sistema de archivos.
    /// </summary>
    class FilesystemAccess
    {
        private static FilesystemAccess instance;
        private static string logpath;
        private static string exportPath;
        private ImageCodecInfo codec;

        /// <summary>
        /// Función encargada de instanciar y devolver una única instancia Singleton de la clase.
        /// </summary>
        public static FilesystemAccess GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FilesystemAccess();
                }
                return instance;
            }
        }

        /// <summary>
        /// Función encargada de devolver un nuevo Bitmap basado en una ruta del sistema de archivos.
        /// </summary>
        /// <param name="path">Ruta absoluta al archivo de imagen.</param>
        /// <returns>Bitmap con la imagen cargada.</returns>
        public Bitmap GetDiskImage(string path)
        {
            return new Bitmap(path);
        }

        /// <summary>
        /// Subrutina encargada de crear directorio donde se van a exportar las imágenes.
        /// </summary>
        /// <param name="path">Ruta absoluta al directorio.</param>
        public void CreateExportDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            exportPath = path;
        }

        /// <summary>
        /// Subrutina encargada de escribir la imagen optimizada en disco.
        /// </summary>
        /// <param name="path">Ruta absoluta de la imagen a guardar.</param>
        public void WriteJPEG(Bitmap imagen, string path)
        {
            if (codec == null)
            {
                GetJPEGCodec();
            }
            EncoderParameter JPEGEncoderParam = new EncoderParameter(Encoder.Quality, 65L);
            EncoderParameters JPEGEncoderParams = new EncoderParameters(1);
            JPEGEncoderParams.Param[0] = JPEGEncoderParam;
            imagen.Save(path, codec, JPEGEncoderParams);
        }

        /// <summary>
        /// Función encargada de obtener el códec JPEG de la clase ImageCodecInfo.
        /// </summary>
        private void GetJPEGCodec()
        {
            ImageCodecInfo[] codecArr = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecArr)
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    this.codec = codec;
                    break;
                }
            }
        }

        /// <summary>
        /// Subrutina encargada de loggear la información solicitada de un evento a disco.
        /// </summary>
        /// <param name="msj">Mensaje del evento a loggear.</param>
        /// <param name="type">Tipo (enumerado) del evento.</param>
        /// <param name="trace">Información del objeto afectado (parámetro opcional).</param>
        public void LogToDisk(string msj, Logtype type , MethodBase trace=null)
        {
            string logformat;
            if (trace != null) 
            { 
                logformat = (System.DateTime.Now + " [" + type.ToString() + "]: \"" + msj + "\" En: \"" + trace.ToString() + "\""+"\r\n");
                Console.WriteLine("[" + type.ToString() + "]: \"" + msj + "\" En: \"" + trace.ToString() + "\"" + "\r\n");
            }
            else 
            { 
                logformat = (System.DateTime.Now + " [" + type.ToString() + "]: \"" + msj + "\""+"\r\n");
                Console.WriteLine("[" + type.ToString() + "]: \"" + msj + "\"" + "\r\n");
            }
            System.IO.File.AppendAllText(logpath + "\\logs.txt", logformat);
        }

        /// <summary>
        /// Enumerador de los tipos de información para loggear.
        /// </summary>
        public enum Logtype
        {
            Error,
            Warning,
            Info
        }

        /// <summary>
        /// Setter de logpath.
        /// </summary>
        public string SetExecutionPath { set => logpath = value; }

        /// <summary>
        /// Getter de ExportPath
        /// </summary>
        public string GetExportPath { get => exportPath; }
    }
}
