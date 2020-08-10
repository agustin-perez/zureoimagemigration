using ErpToGoMigrationTool.DataAccess;
using ErpToGoMigrationTool.Logic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Zureo.MigrarImagenes.DataAccess;
using Zureo.MigrarImagenes.Logic;

namespace Zureo.MigrarImagenes
{
    /// <summary>
    /// Clase principal del programa.
    /// </summary>
    class Program
    {
        private static bool IsFenicio = false;
        /// <summary>
        /// Método Main del programa.
        /// </summary>
        /// <param name="args">No se utilizan parámetros de inicialización.</param>
        static void Main(string[] args)
        {
            FilesystemAccess.GetInstance.SetExecutionPath = "C:\\Zureo Software\\Imagenes Exportadas GO\\";
            FilesystemAccess.GetInstance.CreateExportDir("C:\\Zureo Software\\Imagenes Exportadas GO\\");

            ParseArgs(args[0]);

            FilesystemAccess.GetInstance.LogToDisk("Inicio de ErpToGoMigrationTool", FilesystemAccess.Logtype.Info);
            try
            {
                DatabaseAccess.GetInstance.ConnectionString = Utils.GetConnectionString();
                DatabaseAccess.GetInstance.InitConnection();
                Queries.GetInstance.CheckImagenesTable();
            }
            catch (Exception)
            {
                FilesystemAccess.GetInstance.LogToDisk("No se pudo conectar a la base de datos. Revisar configuración en Zureo.", FilesystemAccess.Logtype.Error);
                Environment.Exit(1);
            }

            Int16[] Empresas = Queries.GetInstance.GetEmpresas();
            Int16 LastEmpresa = 0;

            try
            {
                for (int i = 0; i < Empresas.Length; i++)
                {
                    ImportAndExport migration = new ImportAndExport();

                    LastEmpresa = Empresas[i];
                    List<ZArticle> ArticleList = new List<ZArticle>();

                    FilesystemAccess.GetInstance.LogToDisk("Inicio de Exportación, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);

                    migration.Migration(Empresas[i], ArticleList);
                    FilesystemAccess.GetInstance.LogToDisk("Fin de Exportación, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);

                    FilesystemAccess.GetInstance.LogToDisk("Inicio de Importación, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);
                    foreach (ZArticle articulo in ArticleList)
                    {
                        if (!Queries.GetInstance.CheckImgDuplicate(articulo.artID))
                        {
                            migration.ArticleImport(articulo, FilesystemAccess.GetInstance.GetExportPath);
                        }
                        else
                        {
                            FilesystemAccess.GetInstance.LogToDisk("No se guardó la imagen del artículo: " + articulo.artID + " ya que la misma fue migrada anteriormente. ", FilesystemAccess.Logtype.Warning);
                        }
                    }
                    FilesystemAccess.GetInstance.LogToDisk("Fin de Importación, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);
                }
            }
            catch (Exception)
            {
                FilesystemAccess.GetInstance.LogToDisk("Error desconocido al intentar procesar los artículos de la empresa: " + LastEmpresa + " Revisar conexión con BD y permisos de ejecución. Se intentará continuar con el resto de empresas.", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod());
            }

            FilesystemAccess.GetInstance.LogToDisk("Fin de migración a GO", FilesystemAccess.Logtype.Info);
            Console.WriteLine("\nPresione enter para salir y abrir la carpeta con las imágenes exportadas.");
            Console.ReadLine();
            Process.Start("explorer.exe", FilesystemAccess.GetInstance.GetExportPath);
        }

        private static void ParseArgs(String arg)
        {
            try
            {
                if (short.Parse(arg) == 2)
                {
                    IsFenicio = true;
                }
            }
            catch (ArgumentNullException)
            {
                FilesystemAccess.GetInstance.LogToDisk("No se ingresó parámetro de ejecución, se asume que no se utilizará Integración con Fenicio.", FilesystemAccess.Logtype.Error);
            }
            catch (FormatException)
            {
                FilesystemAccess.GetInstance.LogToDisk("El argumento ingresado no es válido.\nSaliendo...", FilesystemAccess.Logtype.Error);
                Environment.Exit(0);
            }
            finally
            {
                FilesystemAccess.GetInstance.LogToDisk("El argumento ingresado es un número fuera de rango, o no es válido.\nSaliendo...", FilesystemAccess.Logtype.Error);
                Environment.Exit(0);
            }
        }
    }
}

