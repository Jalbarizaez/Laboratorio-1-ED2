using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laboratorio_1.Models;
using System.IO;

namespace Laboratorio_1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

		[HttpGet]
		public ActionResult Compresion_Descompresion()
		{
			string path = Server.MapPath("~/ArchivosTmp/");
			string[] pathsTmp = Directory.GetFiles(path);
			foreach (var item in pathsTmp)
				System.IO.File.Delete(item);

			return View();
		}
		[HttpPost]
		public ActionResult Compresion_Descompresion(HttpPostedFileBase ArchivoEntrada)
		{
			if (ArchivoEntrada != null)
			{
				string[] nombreArchivo = ArchivoEntrada.FileName.Split('.');

				if (nombreArchivo[1] == "txt")
				{
					try
					{
						CompresionHuffman H = new CompresionHuffman();
						
						string path = Server.MapPath("~/ArchivosTmp/");
						string pathPrueba = path + nombreArchivo[0] + ".huff";
						path = path + ArchivoEntrada.FileName;
						ArchivoEntrada.SaveAs(path);

						ViewBag.Ok = "Archivo Comprimido :)";

						H.Compresion(path, (pathPrueba));
						return File(pathPrueba, "huff", (nombreArchivo[0] + ".huff"));
					}
					catch
					{
						ViewBag.Error02 = "Ha ocurrido un error con su archivo";
					}
				}
				else if (nombreArchivo[1] == "txt")
				{
				}
			}
			else
			{
				ViewBag.Error01 = "No ha ingresado un archivo";
			}
			return View();
		}
	}
}