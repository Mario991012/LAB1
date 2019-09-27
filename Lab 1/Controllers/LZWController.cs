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

        public ActionResult CargaParaComprimirL()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargaParaComprimirL(HttpPostedFileBase file)
        {
            var nombreDocumento = file.FileName;
            var NombreArray = nombreDocumento.Split('.');
            var OriginalPeso = Convert.ToDouble(file.ContentLength);
            //try
            //{
                if (file != null && file.ContentLength > 0)
                {
                    var model = string.Empty;
                    model = Server.MapPath($"~/Archivos Originales/{NombreArray[0]}.{NombreArray[1]}");
                    var UbicacionAAlmacenarLZW = Server.MapPath("~//Archivos Comprimidos LZW");
                    file.SaveAs(model);
                    if (LogicaLZW.Comprimir(model, NombreArray, UbicacionAAlmacenarLZW) == 1)
                    {
                        var RutaArchivoCompreso = Server.MapPath($"~/Archivos Comprimidos LZW/{NombreArray[0]}.lzw");

                        var ArchivoCompreso = new FileInfo(RutaArchivoCompreso);
                        var PesoCompreso = Convert.ToDouble(ArchivoCompreso.Length);

                        var Archivo = new Archivos();
                        Archivo.NombreArchivo = nombreDocumento;
                        Archivo.Factor = Math.Round(OriginalPeso / PesoCompreso, 3);
                        Archivo.Razon = Math.Round(PesoCompreso / OriginalPeso, 3);
                        Archivo.Porcentaje = Math.Round(100.00 * (1 - Convert.ToDouble(Archivo.Razon)), 3);

                        Huffman.Instancia.DatosDeArchivos.Add(Archivo.NombreArchivo, Archivo);

                        ViewBag.Msg = "Carga del archivo correcta";
                        ViewBag.Mensaje = "Carga del archivo correcta";
                        ViewBag.MensajeDescarga = "Archivo comprimido descargable en apartado de descargas.";
                        return RedirectToAction("ListaArchivos", "Home");
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

        public ActionResult CargaParaDescomprimirL()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CargaParaDescomprimirL(HttpPostedFileBase file)
        {
            var nombreArchivo = file.FileName;
            var nombre = nombreArchivo.Split('.');
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var model = Server.MapPath($"~/Archivos Comprimidos LZW/{nombreArchivo}");

                    var UbicacionDescomprimidos = Server.MapPath("~//Archivos Descomprimidos");
                    file.SaveAs(model);

                    if (LogicaLZW.Descomprimir(model, nombre, UbicacionDescomprimidos) == 1)
                    {
                        ViewBag.Msg = "Carga del archivo correcta";
                        ViewBag.Mensaje = "Carga del archivo correcta";
                        return MostrarListaEscogida(2);
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

        public ActionResult Download()
        {
            var UbicacionHD = Server.MapPath("~//Archivos Descomprimidos");
            var ubicacionHC = Server.MapPath("~//Archivos Comprimidos");
            var dirInfo = new DirectoryInfo(UbicacionHD);
            var dirCom = new DirectoryInfo(ubicacionHC);
            var filesD = dirInfo.GetFiles("*.*");
            var filesC = dirCom.GetFiles("*.*");

            List<string> ListC = new List<string>(filesC.Length);
            List<string> list = new List<string>(filesD.Length);
            foreach (var item in filesD)
            {
                list.Add(item.Name);
            }
            foreach (var item in filesC)
            {
                ListC.Add(item.Name);
            }

            return View(list);
        }

        public ActionResult MostrarOpciones()
        {
            return View("MostrarListaEscogida");
        }
        static int x = 0;
        public ActionResult MostrarListaEscogida(int lista)
        {
            if (lista == 1)
            {
                x = lista;
                var ubicacionHC = Server.MapPath("~//Archivos Comprimidos");
                var dirCom = new DirectoryInfo(ubicacionHC);
                var filesC = dirCom.GetFiles("*.*");
                List<string> ListC = new List<string>(filesC.Length);
                foreach (var item in filesC)
                {
                    ListC.Add(item.Name);
                }
                return View("Download", ListC);
            }
            else if (lista == 2)
            {
                x = lista;
                var UbicacionHD = Server.MapPath("~//Archivos Descomprimidos");
                var dirInfo = new DirectoryInfo(UbicacionHD);
                var filesD = dirInfo.GetFiles("*.*");
                List<string> list = new List<string>(filesD.Length);
                foreach (var item in filesD)
                {
                    list.Add(item.Name);
                }
                return View("Download", list);
            }
            return View();
        }

        public ActionResult DownloadFile(string filename)
        {
            var nombre = filename.Split('.');
            try
            {
                if (x == 2)
                {
                    var fullpath = Path.Combine(Server.MapPath("~//Archivos Descomprimidos"), filename);
                    return File(fullpath, "Archivos Desomprimidos/lzw", $"{nombre[0]}.{nombre[1]}");
                }
                else if (x == 1)
                {
                    var fullpath = Path.Combine(Server.MapPath("~//Archivos Comprimidos"), filename);
                    return File(fullpath, "Archivos Comprimidos/huff", $"{nombre[0]}.{nombre[1]}");
                }
                return View("Download");
            }
            catch
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
        }
    }
}
