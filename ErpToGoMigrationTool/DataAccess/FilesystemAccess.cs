using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace ErpToGoMigrationTool.DataAccess
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
        private bool TPVDestFolderWasChecked = false;
        private string TPVPath;
        private bool OptimizeImages = false;

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
        }

        /// <summary>
        /// Subrutina encargada de escribir imágenes a disco.
        /// </summary>
        /// <param name="Image">Imagen a escribir.</param>
        /// <param name="TPVImage">Imagen de TPV a escribir (si es que existe).</param>
        /// <param name="GUID">GUID de la imagen a guardar</param>
        /// <param name="FenicioID">ID de fenicio.</param>
        public void WriteJPEG(Bitmap Image, Bitmap TPVImage, string GUID, string FenicioID = null)
        {
            if (codec == null)
            {
                GetJPEGCodec();
            }

            EncoderParameter JPEGEncoderParam;
            if (optmizeImages)
            {
                JPEGEncoderParam = new EncoderParameter(Encoder.Quality, 65L);
            }
            else
            {
                JPEGEncoderParam = new EncoderParameter(Encoder.Quality, 100L);
            }
            
            EncoderParameters JPEGEncoderParams = new EncoderParameters(1);
            JPEGEncoderParams.Param[0] = JPEGEncoderParam;
            Image.Save(ImgSaveDir(GUID, FenicioID), codec, JPEGEncoderParams);
            if(TPVImage != null)
                TPVImage.Save(TPVSaveDir(GUID, FenicioID), codec, JPEGEncoderParams);
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
                logformat = (System.DateTime.Now + " [" + type.ToString() + "]: " + msj + " En: " + trace.ToString() +"\r\n");
                Console.WriteLine("[" + type.ToString() + "]: " + msj + " En: "+ trace.ToString()  + "\r\n");
            }
            else 
            { 
                logformat = (System.DateTime.Now + " [" + type.ToString() + "]: " + msj +"\r\n");
                Console.WriteLine("[" + type.ToString() + "]: " + msj  + "\r\n");
            }
            System.IO.File.AppendAllText(logpath + "\\logs.txt", logformat);
        }

        /// <summary>
        /// Función encargada de chequear al momento de guardado si la imagen existía en el directorio TPV para su correspondiente guardado.
        /// </summary>
        /// <param name="EmpPathImg">Ruta base de la empresa.</param>
        /// <param name="ArtFoto">Ruta de la imagen, la cual puede o no ser absoluta.</param>
        /// <returns>Si la imagen existe o no.</returns>
        public bool CheckTPVImage(string EmpPathImg, string ArtFoto)
        {
            string checkImg = Path.Combine(EmpPathImg, Path.GetDirectoryName(ArtFoto));
            TPVPath = Path.Combine(exportPath, "TPV");

            if (!TPVDestFolderWasChecked && !Directory.Exists(TPVPath) && Directory.Exists(Path.Combine(@checkImg, "TPV")))
            {
                CreateExportDir(TPVPath);
                if(Directory.Exists(TPVPath))
                    TPVDestFolderWasChecked = true;
            }

            if (File.Exists(Path.Combine(@EmpPathImg, "TPV", Path.GetFileName(ArtFoto))))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Función encargada de traer la imagen correspondiente a ruta TPV.
        /// </summary>
        /// <param name="EmpPathImg">Ruta base de la empresa.</param>
        /// <param name="ArtFoto">Ruta de la imagen, la cual puede o no ser absoluta.</param>
        /// <returns>Ruta de la imagen.</returns>
        public string GetTPVImage(string EmpPathImg, string ArtFoto)
        {
            return Path.Combine(@EmpPathImg, "TPV", Path.GetFileName(ArtFoto));
        }

        /// <summary>
        /// Función encargada de manejar las rutas de imágenes dentro de la importación a memoria, antes de ser guardadas en disco.
        /// </summary>
        /// <param name="EmpPathImg">Ruta base de la empresa.</param>
        /// <param name="ArtFoto">Ruta de la imagen, la cual puede o no ser absoluta.</param>
        /// <returns>Ruta de la imagen lista para su guardado a disco.</returns>
        public String ImagePath(string EmpPathImg, string ArtFoto)
        {
            //Se lanza excepción en caso de que la imagen no se encuentre en su ruta correspondiente.
            if (ArtFoto == "")
                throw new FileNotFoundException();

            if (!System.IO.Path.IsPathRooted(ArtFoto))
                return Path.Combine(EmpPathImg, ArtFoto);

            return @ArtFoto;
        }

        /// <summary>
        /// Función interna encargada de crear el path de guardado de cada imagen.
        /// </summary>
        /// <param name="GUID">GUID de la imagen a guardar.</param>
        /// <param name="FenicioID">[Sobrecarga opcional] ID de Fenicio, el cual indica que se utilizo dicha implementación.</param>
        /// <returns>Devuelve el path listo para escribir el Bitmap a disco.</returns>
        private String ImgSaveDir(string GUID, string FenicioID = null)
        {
            if (FenicioID == null)
                return Path.Combine(exportPath, GUID.ToString() + ".jpg");
            else
                return Path.Combine(exportPath, FenicioID.ToString() + "-0.jpg");
        }

        /// <summary>
        /// Función interna encargada de preparar el directorio para TPV.
        /// </summary>
        /// <param name="GUID">GUID de la imagen a guardar.</param>
        /// <param name="FenicioID">ID de Fenicio, el cual indica que se utilizo dicha implementación.</param>
        /// <returns>Devuelve el path listo para escribir el Bitmap a disco.</returns>
        private String TPVSaveDir(string GUID, string FenicioID = null)
        {
            if (FenicioID == null)
                return Path.Combine(TPVPath, GUID.ToString() + ".jpg");
            else
                return Path.Combine(TPVPath, FenicioID.ToString() + "-0.jpg");
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
        /// Propiedad de exportPath.
        /// </summary>
        public string ExportPath { set => exportPath = value; get => exportPath; }


        /// <summary>
        /// Propiedad de OptimizeImages
        /// </summary>
        public bool optmizeImages { set => OptimizeImages = value; get => OptimizeImages; }
    }
}
