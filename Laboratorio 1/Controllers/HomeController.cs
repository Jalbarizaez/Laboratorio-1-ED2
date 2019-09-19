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
		private void BorrarArchivosTemporales()
		{
			string pathABorrar = Server.MapPath("~/ArchivosTmp/");
			string[] pathsTmp = Directory.GetFiles(pathABorrar);
			foreach (var item in pathsTmp)
				System.IO.File.Delete(item);
		}

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
			BorrarArchivosTemporales();
			return View();
		}
		[HttpPost]
		public ActionResult Compresion_Descompresion(HttpPostedFileBase ArchivoEntrada)
		{
			BorrarArchivosTemporales();
			if (ArchivoEntrada != null)
			{
				string[] nombreArchivo = ArchivoEntrada.FileName.Split('.');

				try
				{
					if (nombreArchivo[1] == "txt")
					{
						CompresionHuffman H = new CompresionHuffman();

						string path = Server.MapPath("~/ArchivosTmp/");
						string pathPrueba = path + nombreArchivo[0];
						path = path + ArchivoEntrada.FileName;
						ArchivoEntrada.SaveAs(path);

						string pathMiFichero = Server.MapPath("~/ArchivoMisCompresiones/");
						pathMiFichero = pathMiFichero + "FicheroMisCompresiones.txt";


						H.Compresion(path, pathPrueba);
						H.SetMisCompresiones(path, pathPrueba, pathMiFichero);
						ViewBag.Ok = "Proceso completado :)";
						return File(pathPrueba, "huff", (nombreArchivo[0] + ".huff"));
					}
					else if (nombreArchivo[1] == "huff")
					{
						DescompresionHuffman H = new DescompresionHuffman();
						string path = Server.MapPath("~/ArchivosTmp/");
						string pathPrueba = path + nombreArchivo[0];
						path = path + ArchivoEntrada.FileName;
						ArchivoEntrada.SaveAs(path);

						H.Descompresion(pathPrueba, path);
						ViewBag.ok = "Proceso completado :)";
						return File(pathPrueba, "txt", (nombreArchivo[0] + ".txt"));
					}
				}
				catch
				{
					ViewBag.Error02 = "Ha ocurrido un error con su archivo";
				}
			}
			else
			{
				ViewBag.Error01 = "No ha ingresado un archivo";
			}
			return View();
		}

		[HttpGet]
		public ActionResult Compresion_Descompresion_LZW()
		{
			BorrarArchivosTemporales();
			return View();
		}
		[HttpPost]
		public ActionResult Compresion_Descompresion_LZW(HttpPostedFileBase ArchivoEntrada)
		{
			BorrarArchivosTemporales();
			if (ArchivoEntrada != null)
			{
				string[] nombreArchivo = ArchivoEntrada.FileName.Split('.');

				try
				{
					if (nombreArchivo[1] == "txt")
					{
						CompresionLZW H = new CompresionLZW();

						string path = Server.MapPath("~/ArchivosTmp/");
						string pathPrueba = path + nombreArchivo[0];
						path = path + ArchivoEntrada.FileName;
						ArchivoEntrada.SaveAs(path);

						string pathMiFichero = Server.MapPath("~/ArchivoMisCompresiones/");
						pathMiFichero = pathMiFichero + "FicheroMisCompresiones.txt";


						H.Compresion(path, pathPrueba);
						//H.SetMisCompresiones(path, pathPrueba, pathMiFichero);
						ViewBag.Ok = "Proceso completado :)";
						return File(pathPrueba, "lzw", (nombreArchivo[0] + ".lzw"));
					}
					else if (nombreArchivo[1] == "lzw")
					{
						//DescompresionHuffman H = new DescompresionHuffman();
						//string path = Server.MapPath("~/ArchivosTmp/");
						//string pathPrueba = path + nombreArchivo[0];
						//path = path + ArchivoEntrada.FileName;
						//ArchivoEntrada.SaveAs(path);

						//H.Descompresion(pathPrueba, path);
						//ViewBag.ok = "Proceso completado :)";
						//return File(pathPrueba, "txt", (nombreArchivo[0] + ".txt"));
					}
				}
				catch
				{
					ViewBag.Error02 = "Ha ocurrido un error con su archivo";
				}
			}
			else
			{
				ViewBag.Error01 = "No ha ingresado un archivo";
			}

			return View();
		}

		public ActionResult MisCompresiones()
		{
			CompresionHuffman H = new CompresionHuffman();
			string pathMiFichero = Server.MapPath("~/ArchivoMisCompresiones/");
			pathMiFichero = pathMiFichero + "FicheroMisCompresiones.txt";
			List<string> Campos = H.GetMisCompresiones(pathMiFichero);
			return View(Campos);
		}
	}
}