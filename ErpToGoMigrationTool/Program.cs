﻿using ErpToGoMigrationTool.DataAccess;
using ErpToGoMigrationTool.Logic;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ErpToGoMigrationTool
{
    /// <summary>
    /// Clase principal del programa.
    /// </summary>
    class Program
    {
        private static bool IsFenicio;
        /// <summary>
        /// Método Main del programa.
        /// </summary>
        /// <param name="args">No se utilizan parámetros de inicialización.</param>
        static void Main(string[] args)
        {
            FilesystemAccess.GetInstance.SetExecutionPath = "C:\\Zureo Software\\Imagenes Exportadas GO\\";
            FilesystemAccess.GetInstance.ExportPath = "C:\\Zureo Software\\Imagenes Exportadas GO\\";
            FilesystemAccess.GetInstance.CreateExportDir(FilesystemAccess.GetInstance.ExportPath);
            ParseArgs(args);
            
            //INPUT Y LOGS
            FilesystemAccess.GetInstance.LogToDisk("-------------- Inicio de ErpToGoMigrationTool -------------- ", FilesystemAccess.Logtype.Info);

            if(ConsoleQuery("¿Quiere optimizar las imágenes al momento de exportarlas?"))
            {
                FilesystemAccess.GetInstance.LogToDisk("Se eligió optimizar las imágenes.", FilesystemAccess.Logtype.Info);
                FilesystemAccess.GetInstance.optmizeImages = true;
            }

            if (!ConsoleQuery("¿Confirma que desea continuar con la migración?"))
            {
                FilesystemAccess.GetInstance.LogToDisk("Eligió no continuar, saliendo...", FilesystemAccess.Logtype.Info);
                Environment.Exit(0);
            }
            //FIN INPUT Y LOGS

            try
            {
                DatabaseAccess.GetInstance.ConnectionString = Utils.GetConnectionString();
                DatabaseAccess.GetInstance.InitConnection();
                if (!IsFenicio)
                {
                    Queries.GetInstance.CheckImagenesTable();
                }
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
                    LastEmpresa = Empresas[i];
                    FilesystemAccess.GetInstance.LogToDisk("Inicio de migración, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);
                    Migration migration = new Migration(Empresas[i], IsFenicio);
                    FilesystemAccess.GetInstance.LogToDisk("Fin de migración, empresa: " + Empresas[i], FilesystemAccess.Logtype.Info);
                }
            }
            catch (Exception)
            {
               FilesystemAccess.GetInstance.LogToDisk("Error desconocido al intentar procesar los artículos de la empresa: " + LastEmpresa + " Revisar conexión con BD y permisos de ejecución. Se intentará continuar con el resto de empresas.", FilesystemAccess.Logtype.Error, MethodBase.GetCurrentMethod());
            }

            FilesystemAccess.GetInstance.LogToDisk("-------------- Fin de migración --------------", FilesystemAccess.Logtype.Info);
            
            Console.WriteLine("\nAbriendo carpeta con las imágenes exportadas.");
            Process.Start("explorer.exe", FilesystemAccess.GetInstance.ExportPath);
        }

        /// <summary>
        /// Función encargada de realizar preguntas en consola.
        /// </summary>
        /// <param name="message">Mensaje a preguntar.</param>
        /// <returns>Respuesta en boolean.</returns>
        private static bool ConsoleQuery(string message)
        {
            Console.WriteLine(message + "(Y/n)");
            string input = Console.ReadLine().ToLower();
            while (input != "y")
            {
                if (input == "n")
                {
                    return false;
                }
                Console.WriteLine("Opción inválida. " + message + "(Y/n)");
                input = Console.ReadLine().ToLower();
            }
            return true;
        }

        /// <summary>
        /// Subrutina encargada de parsear los argumentos de ejecución, para lidiar con el caso particular de Fenicio.
        /// </summary>
        /// <param name="args">Argumentos de ejecución</param>
        private static void ParseArgs(String[] args)
        {
            try
            {
                switch (short.Parse(args[0]))
                {
                    case 1:
                        break;
                    case 2:
                        IsFenicio = true;
                        FilesystemAccess.GetInstance.LogToDisk("Se exportarán las imágenes en base a la integración con Fenicio.", FilesystemAccess.Logtype.Info);
                        break;
                    default:
                        throw new FormatException();
                }
            }
            catch (IndexOutOfRangeException)
            {
                FilesystemAccess.GetInstance.LogToDisk("No se ingresó parámetro de ejecución, no se continuará con la ejecución, saliendo...", FilesystemAccess.Logtype.Info);
                Environment.Exit(0);
            }
            catch (FormatException)
            {
                FilesystemAccess.GetInstance.LogToDisk("El argumento ingresado no es válido o no es un número admitido.\nSaliendo...", FilesystemAccess.Logtype.Error);
                Environment.Exit(0);
            }
            catch (Exception)
            {
                FilesystemAccess.GetInstance.LogToDisk("El argumento ingresado es un número fuera de rango, o no es válido.\nSaliendo...", FilesystemAccess.Logtype.Error);
                Environment.Exit(0);
            }
        }
    }
}

