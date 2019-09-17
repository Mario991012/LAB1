using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab_1.Singleton;
using Lab_1.Models;
using System.IO;

namespace Lab_1.Controllers
{
    public class LZWController : Controller
    {
     
        public ViewResult MenuLZW()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargaParaComprimirL(HttpPostedFileBase file)
        {
            var nombreDocumento = file.FileName;
            var NombreArray = nombreDocumento.Split('.');
            var OriginalPeso = Convert.ToDouble(file.ContentLength);
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var model = string.Empty;
                    model = Server.MapPath($"~/Archivos Originales/{NombreArray[0]}.{NombreArray[1]}");
                    var UbicacionAAlmacenarLZW = Server.MapPath("~//Archivos Comprimidos LZW");
                    file.SaveAs(model);
                    if (LogicaLZW.Instancia.LecturaArchivo(model, NombreArray, UbicacionAAlmacenarLZW) == 1)
                    {
                        var RutaArchivoCompreso = Server.MapPath($"~/Archivos Comprimidos LZW/{nombreDocumento}.huff");

                        var ArchivoCompreso = new FileInfo(RutaArchivoCompreso);
                        var PesoCompreso = Convert.ToDouble(ArchivoCompreso.Length);

                        var Archivo = new Archivos();
                        Archivo.NombreArchivo = nombreDocumento;
                        Archivo.Factor = ((double)((int)((OriginalPeso / PesoCompreso) * 1000.0))) / 1000.0;
                        Archivo.Razon = ((double)((int)((PesoCompreso / OriginalPeso) * 1000.0))) / 1000.0;
                        Archivo.Porcentaje = (((double)((int)((1 - Convert.ToDouble(Archivo.Razon)) * 1000.0))) / 1000.0) * 100;

                        LogicaLZW.Instancia.DatosDeArchivos.Add(Archivo.NombreArchivo, Archivo);

                        ViewBag.Msg = "Carga del archivo correcta";
                        ViewBag.Mensaje = "Carga del archivo correcta";
                        ViewBag.MensajeDescarga = "Archivo comprimido descargable en apartado de descargas.";
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
            return View();
        }

        [HttpPost]
        public ActionResult CargaParaDescomprimirL(HttpPostedFileBase file)
        {
            return View();
        }

        [HttpPost]
        public ActionResult MostrarOpcionesL()
        {
            return View();
        }
    }
}
