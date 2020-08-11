using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Zureo.MigrarImagenes.DataAccess;

namespace ErpToGoMigrationTool.DataAccess
{
    /// <summary>
    /// Clase encargada de todas las consultas SQL realizadas mediante la clase DatabaseAccess.
    /// </summary>
    class Queries
    {
        private static Queries instance;
        /// <summary>
        /// Función encargada de instanciar y devolver una única instancia Singleton de la clase.
        /// </summary>
        public static Queries GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Queries();
                }
                return instance;
            }
        }

        /// <summary>
        /// Subrutina encargada de chequear si existe la tabla "Imagenes" para proceder a su creación.
        /// Esta subrutina es útil si la base no fue actualizada.
        /// </summary>
        public void CheckImagenesTable()
        {
            if (DatabaseAccess.GetInstance.GenericFieldQuery<string>("select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'Imagenes'") == null)
            {
                Console.WriteLine(DatabaseAccess.GetInstance.GenericFieldQuery<string>("CREATE TABLE [dbo].[Imagenes]("
                    + "[ImgId][uniqueidentifier] NOT NULL,"
                    + "[ImgTipoDato][tinyint] NULL,"
                    + "[ImgIdDato][int] NULL,"
                    + "[ImgIdVarianteDato][int] NULL,"
                    + "[ImgTipoImagen][tinyint] NULL,"
                    + "[ImgDescripcion][nvarchar](60) NULL,"
                    + "[ImgFechaModificacion][smalldatetime] NULL,"
                    + "CONSTRAINT[PK_Imagenes] PRIMARY KEY CLUSTERED"
                    + "("
                    + "    [ImgId] ASC"
                    + ") WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON[PRIMARY]) ON[PRIMARY]"));
            }
        }

        /// <summary>
        /// Función encargada de devolver un conjunto de identificadores de empresas.
        /// </summary>
        /// <returns>Un único valor de identificación para cada empresa del sistema.</returns>
        public Int16[] GetEmpresas()
        {
            DataTable queryEmpresas = DatabaseAccess.GetInstance.TableQuery("select distinct ArtEmpresa from Articulo;");
            List<Int16> empList = new List<Int16>();
            for (int i = 0; i < queryEmpresas.Rows.Count; i++)
            {
                empList.Add(queryEmpresas.Rows[i].Field<Int16>(0));
            }
            return empList.ToArray();
        }

        /// <summary>
        /// Función engargada de devolver la ruta base de imágenes de una empresa dada.
        /// </summary>
        /// <param name="EmpId">Identificador de dicha empresa.</param>
        /// <returns>Ruta absoluta de las imágenes de artículos de dicha empresa.</returns>
        public string GetImgBasePath(int EmpId)
        {
            return DatabaseAccess.GetInstance.GenericFieldQuery<String>("select EmpPathImg from cceEmpresas where EmpId = '" + EmpId + "';");
        }

        /// <summary>
        /// Función encargada de devolver una tabla estilo "vista" con los 
        /// datos necesarios para poder instanciar los objetos de Article.
        /// </summary>
        /// <param name="ArtEmpresa">Empresa a la cual se van a extraer dichos Artículos.</param>
        /// <returns>Tabla con todos los Artículos de dicha empresa, 
        /// lista para instanciar los objetos de Artículo.</returns>
        public DataTable GetArticleView()
        {
            return DatabaseAccess.GetInstance.TableQuery("select ArtId, ArtEmpresa, ArtFoto from Articulo where ArtFoto is not null;");
        }

        /// <summary>
        /// Función encargada de devolver una tabla estilo "vista" con los 
        /// datos necesarios para poder instanciar los objetos de Article basados en una empresa particular.
        /// </summary>
        /// <param name="ArtEmpresa">Empresa a la cual se van a extraer dichos Artículos.</param>
        /// <returns>Tabla con todos los Artículos de dicha empresa, 
        /// lista para instanciar los objetos de Artículo.</returns>
        public DataTable GetArticleEmpView(int EmpId)
        {
            return DatabaseAccess.GetInstance.TableQuery("select ArtId, ArtEmpresa, ArtFoto from Articulo where Articulo.ArtEmpresa = '" + EmpId + "' and ArtFoto is not null;");
        }

        /// <summary>
        /// Subrutina encargada de insertar una imagen a la BD.
        /// </summary>
        /// <param name="ImgID">GUID de la imagen.</param>
        /// <param name="ImgIdDato">ArtId de la imagen.</param>
        public void ImageInsert(Guid ImgID, int ImgIdDato)
        {
            List<SqlParameter> InsertList = new List<SqlParameter>();
            InsertList.Add(new SqlParameter("@ImgID", ImgID));
            InsertList.Add(new SqlParameter("@ImgIdDato", ImgIdDato));
            DatabaseAccess.GetInstance.InsertByParams("insert into Imagenes (ImgId, ImgTipoDato, ImgIdDato, ImgIdVarianteDato, ImgTipoImagen, ImgDescripcion, ImgFechaModificacion) values (@ImgID, 1, @ImgIdDato, 1, 1, 'Principal', GETDATE());", InsertList);
        }

        /// <summary>
        /// Función encargada de chequear que no se dupliquen valores de Guid en la tabla Imagenes.
        /// </summary>
        /// <returns>¿Dicha imagen está en la tabla Imagenes?</returns>
        public Boolean CheckImgDuplicate(int ImgIdDato)
        {
            switch (DatabaseAccess.GetInstance.GenericFieldQuery<int>("select count(*) from Imagenes where ImgTipoDato = 1 and ImgIdDato = '" + ImgIdDato + "';"))
            { 
                case 0:
                    return false;
                    break;
                default:
                    return true;
                    break;
            }
        }

        /// <summary>
        /// Enumerador encargado de identificar las diferentes columnas de la DataTable devuelta en GetArticleData.
        /// </summary>
        public enum ArticleColumns { EmpId = 0, ArtId = 0, ArtEmpresa, ArtFoto }
    }
}