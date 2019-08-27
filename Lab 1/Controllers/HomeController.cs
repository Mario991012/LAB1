using System.Web;
using System.Web.Mvc;
using Lab_1.Singleton;

namespace Lab_1.Controllers
{
    public class HomeController : Controller
    {
        static bool SeCargoDiccionario = false;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Carga()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Carga(HttpPostedFileBase file)
        {
            string nombreArchivo = file.FileName;
            string[] nombre = nombreArchivo.Split('.');
            nombreArchivo = nombre[0];
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    string model = "";
                    SeCargoDiccionario = true;
                    model = Server.MapPath("~/Archivo.txt");
                    file.SaveAs(model);
                    if (Data.Instancia.Lectura(model, nombreArchivo) == 1)
                    {
                        ViewBag.Msg = "Carga del archivo correcta";
                        ViewBag.Mensaje = "Carga del archivo correcta";
                        return RedirectToAction("CargaCorrecta");
                    }
                    else
                    {
                        ViewBag.Msg = "Carga del archivo incorrecta";
                        return RedirectToAction("CargaIncorrecta");
                    }
                }
                else
                {
                    ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
                    return View();
                }
            }
            catch
            {
                return ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
            }
        }

        public ActionResult CargaCorrecta()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CargaIncorrecta()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}