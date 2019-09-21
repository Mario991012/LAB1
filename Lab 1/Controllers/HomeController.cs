using System.IO;
using System.Web;
using System.Web.Mvc;
using Lab_1.Singleton;
using Lab_1.Models;
using System;
using System.Collections.Generic;

namespace Lab_1.Controllers
{
    public class HomeController : Controller
    {
        public string GetContentTyoe { get; private set; }

        public ViewResult MenuPrincipal()
        {
            return View();
        }

        public ActionResult EscogerCompresion(int metodo)
        {
            if (metodo == 1)
            {
                return View("Index");
            }
            else if (metodo == 2)
            {
                return RedirectToAction("MenuLZW", "LZW");
            }
            
            return View("MenuPrincipal");
            
        }

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
                    var model = "";
                    model = Server.MapPath($"~/Archivos Originales/{nombre[0]}.{nombre[1]}");
                    var UbicacionHuffman = Server.MapPath("~//Archivos Comprimidos");
                    file.SaveAs(model);
                    if (Data.Instancia.CompresiónHuffman(model, nombre, UbicacionHuffman) == 1)
                    {
                        var RutaArchivoCompreso = Server.MapPath($"~/Archivos Comprimidos/{nombreArchivo}.huff");

                        var ArchivoCompreso = new FileInfo(RutaArchivoCompreso);
                        var PesoCompreso = Convert.ToDouble(ArchivoCompreso.Length);

                        var Archivo = new Archivos();
                        Archivo.NombreArchivo = nombreArchivo;
                        Archivo.Factor = ((double)((int)((PesoOriginal / PesoCompreso) * 1000.0))) / 1000.0;
                        Archivo.Razon = ((double)((int)((PesoCompreso / PesoOriginal) * 1000.0))) / 1000.0;
                        Archivo.Porcentaje = (((double)((int)((1 - Convert.ToDouble(Archivo.Razon)) * 1000.0))) / 1000.0) * 100;

                        Data.Instancia.DatosDeArchivos.Add(Archivo.NombreArchivo, Archivo);

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

                    if (Data.Instancia.Descompresion(model, nombre, UbicacionDescomprimidos) == 1)
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
                    return File(fullpath, "Archivos Desomprimidos/huff", $"Descargable{nombre[0]}.{nombre[1]}");
                }
                else if (x == 1)
                {
                    var fullpath = Path.Combine(Server.MapPath("~//Archivos Comprimidos"), filename);
                    return File(fullpath, "Archivos Comprimidos/huff", $"Descargable{nombre[0]}.{nombre[1]}");
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