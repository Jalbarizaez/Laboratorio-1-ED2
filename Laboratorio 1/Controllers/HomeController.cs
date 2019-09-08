using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laboratorio_1.Models;

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

		CompresionHuffman H = new CompresionHuffman();

		public ActionResult CargaArchivo()
		{

			return View();
		}

		[HttpPost]
		public ActionResult CargaArchivo(HttpPostedFileBase ArchivoEntrada)
		{
			if (ArchivoEntrada != null)
			{
				string path = Server.MapPath("~/ArchivosTmp/");
				path = path + ArchivoEntrada.FileName;
				try
				{
					ArchivoEntrada.SaveAs(path);
					ViewBag.Ok = "Iniciando Comprecion";
				}
				catch
				{
					ViewBag.Error = "Ha ocurrido un error con su archivo";
				}
			}
			return View();
		}
	}
}