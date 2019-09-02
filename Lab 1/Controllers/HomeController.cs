using System.IO;
using System.Web;
using System.Web.Mvc;
using Lab_1.Singleton;
using Lab_1.Models;
using System;

namespace Lab_1.Controllers
{
    public class HomeController : Controller
    {
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
            var nombreArchivo = file.FileName;
            var nombre = nombreArchivo.Split('.');
            nombreArchivo = nombre[0];
            var PesoOriginal = file.ContentLength;
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    string model = "";
                    model = Server.MapPath($"~/Archivos Originales/{nombreArchivo}");
                    string UbicacionHuffman = Server.MapPath("~//Archivos Huffman");
                    file.SaveAs(model);
                    if (Data.Instancia.Lectura(model, nombreArchivo, UbicacionHuffman) == 1)
                    {
                        var RutaArchivoCompreso = Server.MapPath($"~/Archivos Huffman/{nombreArchivo}.huff");
                        FileInfo ArchivoCompreso = new FileInfo(RutaArchivoCompreso);
                        var PesoCompreso = ArchivoCompreso.Length;
                        
                        var Archivo = new Archivos();
                        Archivo.NombreArchivo = nombreArchivo;
                        Archivo.Razon = ((double)((int)((PesoOriginal/PesoCompreso) * 1000.0))) / 1000.0;
                        Archivo.Factor = Math.Truncate(Convert.ToDouble(PesoCompreso / PesoOriginal)); 
                        Archivo.Porcentaje = Math.Truncate(Convert.ToDouble(1 - Archivo.Factor));
                        Data.Instancia.DatosDeArchivos.Add(Archivo.NombreArchivo, Archivo);

                        ViewBag.Msg = "Carga del archivo correcta";
                        ViewBag.Mensaje = "Carga del archivo correcta";
                        return RedirectToAction("ListaArchivos");
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
                ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
                return View();
            }
        }

        public ActionResult ListaArchivos()
        {
            return View();
        }
        public ActionResult CargaIncorrecta()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}