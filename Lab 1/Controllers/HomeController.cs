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

        public ActionResult CargaParaComprimir()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargaParaComprimir(HttpPostedFileBase file)
        {
            var nombreArchivo = file.FileName;
            var nombre = nombreArchivo.Split('.');
            nombreArchivo = nombre[0];
            var PesoOriginal = Convert.ToDouble(file.ContentLength);
            //try
            //{
                if (file != null && file.ContentLength > 0)
                {
                    string model = "";
                    model = Server.MapPath($"~/Archivos Originales/{nombre[0]}.{nombre[1]}");
                    string UbicacionHuffman = Server.MapPath("~//Archivos Comprimidos");
                    file.SaveAs(model);
                    if (Data.Instancia.Lectura(model, nombre, UbicacionHuffman) == 1)
                    {
                        var RutaArchivoCompreso = Server.MapPath($"~/Archivos Comprimidos/{nombreArchivo}.huff");

                        var ArchivoCompreso = new FileInfo(RutaArchivoCompreso);
                        var PesoCompreso = Convert.ToDouble(ArchivoCompreso.Length);
                        
                        var Archivo = new Archivos();
                        Archivo.NombreArchivo = nombreArchivo;
                        Archivo.Razon = ((double)((int)((PesoOriginal/PesoCompreso) * 1000.0))) / 1000.0;
                        Archivo.Factor = ((double)((int)((PesoCompreso / PesoOriginal) * 1000.0))) / 1000.0;
                        Archivo.Porcentaje = (((double)((int)((1 - Convert.ToDouble(Archivo.Factor)) * 1000.0))) / 1000.0) * 100;

                        Data.Instancia.DatosDeArchivos.Add(Archivo.NombreArchivo, Archivo);

                        ViewBag.Msg = "Carga del archivo correcta";
                        ViewBag.Mensaje = "Carga del archivo correcta";
                        return RedirectToAction("ListaArchivos");
                    }
                    else
                    {
                        ViewBag.Msg = "Carga del archivo incorrecta";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
                    return View();
                }
            //}
            //catch
            //{
            //    ViewBag.Msg = "ERROR AL CARGAR EL ARCHIVO, INTENTE DE NUEVO";
            //    return View();
            //}
        }

        public ActionResult CargaParaDescomprimir()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargaParaDescomprimir(HttpPostedFileBase file)
        {
            var nombreArchivo = file.FileName;
            var nombre = nombreArchivo.Split('.');
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var model = Server.MapPath($"~/Archivos Comprimidos/{nombreArchivo}");

                    var UbicacionDescomprimidos = Server.MapPath("~//Archivos Descomprimidos");
                    file.SaveAs(model);

                    if (Data.Instancia.Descompresion(model, nombre[0], UbicacionDescomprimidos) == 1)
                    {
                        var RutaArchivoDescompreso = Server.MapPath($"~/Archivos Descompresos/{nombre[0]}.huff");

                        ViewBag.Msg = "Carga del archivo correcta";
                        ViewBag.Mensaje = "Carga del archivo correcta";
                        return RedirectToAction("ListaArchivos");
                    }
                    else
                    {
                        ViewBag.Msg = "Carga del archivo incorrecta";
                        return View();
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
        
    }
}