using System;
using Zureo.MigrarImagenes.DataAccess;
using System.Data;
using System.Data.SqlClient;
using System.Security.AccessControl;
using ErpToGoMigrationTool.DataAccess;
using System.Drawing;
using Zureo.MigrarImagenes.Logic;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Zureo.MigrarImagenes
{
    class Program
    {
        /// <summary>
        /// Clase Main del programa.
        /// </summary>
        /// <param name="args">No utilizado.</param>
        static void Main(string[] args)
        { 
            //Get startup path para logs
            DatabaseAccess.GetInstance.ConnectionString = Utils.GetConnectionString();
            DatabaseAccess.GetInstance.InitConnection();
            Int16[] Empresas = Queries.GetInstance.GetEmpresas();
            DataTable ArticleView = new DataTable();
            ArticleView = Queries.GetInstance.GetArticleView();
            List<ZArticle> ArticleList = new List<ZArticle>();
            foreach (DataRow row in ArticleView.Rows)
            {
                int a = (int)Queries.ArticleColumns.ArtEmpresa;
                Console.WriteLine(row.Field<string>(1).ToString());
                ArticleList.Add(new ZArticle(row.Field<int>((int) Queries.ArticleColumns.ArtId), row.Field<Int16>((int) Queries.ArticleColumns.ArtEmpresa), null));
            }
            Console.ReadLine();







            //FilesystemAccess.GetInstance.SetPath = String.Concat(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "\\test.txt");
            Testing testA = new Testing();

        }

    }








    class Testing
    {
        public Testing()
        {
            DatabaseAccess.GetInstance.ConnectionString = Utils.GetConnectionString();
            DatabaseAccess.GetInstance.InitConnection();
            DataTable a = Queries.GetInstance.GetArticleEmpView(25);
            Console.WriteLine("inicio articulos");
            foreach (DataRow tupla in a.Rows)
            {
                Console.WriteLine(tupla.Field<int>(0) + tupla.Field<string>(1));
            }
            Console.WriteLine("Fin articulos");

            Console.WriteLine("inicio getempresas");
            Int16[] b= Queries.GetInstance.GetEmpresas();
            for (int i=0; i<b.Length; i++)
            {
                Console.WriteLine(b[i]);
            }
            Console.WriteLine("fin getempresas");
            Console.WriteLine("Inicio stringpath\n" + Queries.GetInstance.GetImgBasePath(b[0]) + "\nFin stringpath");
            Console.ReadLine();

            ZImage testimg = new ZImage(null);
            Console.ReadLine();


        }
    }
}
