using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab_1.Singleton
{
    public class LogicaLZW
    {
        const int bufferLength = 1000;

        //METODOS PARA COMPRIMIR
        static public int Comprimir(string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var caracteres = new Dictionary<string, int>();
            var indice = 1;
            caracteres = ObtenerDiccionarioCaracteresEspeciales(RutaOriginal, ref indice);
            JuntarCaracteresYEscribir(caracteres, ref indice, RutaOriginal, NombreArchivo, UbicacionAAlmacenarLZW);
            return 1;
        }

        static public Dictionary<string, int> ObtenerDiccionarioCaracteresEspeciales(string RutaOriginal, ref int indice)
        {
            var DiccionarioAEscribir = new Dictionary<string, int>();
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLength);
                        for (int i = 0; i < byteBuffer.Length; i++)
                        {
                            if (!DiccionarioAEscribir.ContainsKey(Convert.ToString(Convert.ToChar(byteBuffer[i]))))
                            {
                                DiccionarioAEscribir.Add(Convert.ToString(Convert.ToChar(byteBuffer[i])), indice);
                                indice++;
                            }
                        }
                    }
                }
            }
            return DiccionarioAEscribir;
        }

        static public void JuntarCaracteresYEscribir(Dictionary<string, int> caracteres, ref int indice, string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamWriter = new FileStream($"{UbicacionAAlmacenarLZW}/{NombreArchivo[0]}.lzw", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamWriter))
                        {
                            writer.Write(NombreArchivo[1]);

                            foreach (var caracter in caracteres)
                            {
                                writer.Write($"{caracter.Value}|{caracter.Key}");
                            }
                            writer.Write("--");
                            var byteBuffer = new byte[bufferLength];
                            var PosibleLlave = string.Empty;
                            var posicionesALaDerecha = 0;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);
                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = Convert.ToString(Convert.ToChar(byteBuffer[i]));

                                    while (caracteres.ContainsKey(PosibleLlave) && i + 1 < byteBuffer.Length)
                                    {
                                        posicionesALaDerecha++;
                                        PosibleLlave = $"{PosibleLlave}{Convert.ToString(Convert.ToChar(byteBuffer[i + posicionesALaDerecha]))}";
                                    }

                                    if (i + 1 >= byteBuffer.Length)
                                    {
                                        writer.Write(Convert.ToByte(caracteres[PosibleLlave]));
                                    }
                                    else
                                    {
                                        caracteres.Add(PosibleLlave, indice);
                                        indice++;
                                        i += posicionesALaDerecha - 1;
                                        posicionesALaDerecha = 0;
                                        var escrito = PosibleLlave.Substring(0, PosibleLlave.Length - 1);
                                        var write = Convert.ToByte(caracteres[escrito]);
                                        writer.Write(write);
                                    }



                                }
                            }
                        }
                    }
                }
            }
        }

        //METODOS PARA DESCOMPRIMIR
        public static int Descomprimir(string RutaOriginal, string[] nombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var extension = "";
            extension = ObtenerExtension(RutaOriginal);

            var CaracteresOriginales = new Dictionary<int, string>();
            CaracteresOriginales = ObtenerDiccionarioOriginal(RutaOriginal);

            CompletarDiccionarioYEscribir(CaracteresOriginales, RutaOriginal, UbicacionAAlmacenarLZW, nombreArchivo, extension);

            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamwriter = new FileStream($"{UbicacionAAlmacenarLZW}//{nombreArchivo[0]}.{extension}", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamwriter))
                        {
                            var byteBuffer = new byte[bufferLength];
                            var ExtensionLeida = false;
                            var caracteres = new Dictionary<int, string>();
                            var ultimaPosicion = 0;
                            var ByteLeido = string.Empty;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                ByteLeido = reader.ReadString();
                                if (ExtensionLeida == false)
                                {
                                    ExtensionLeida = true;
                                }
                                else if (ByteLeido != "--")
                                {
                                    var separador = ByteLeido.Split('|');
                                    caracteres.Add(Convert.ToInt32(separador[0]), separador[1]);
                                }
                                else
                                {
                                    ultimaPosicion = Convert.ToInt32(reader.BaseStream.Position + 1);
                                    break;
                                }
                            }

                            var indice = caracteres.Count() + 1;
                            var PosibleLlave = 0;
                            var anterior = string.Empty;
                            var actual = string.Empty;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);
                                var mensaje = string.Empty;
                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = Convert.ToInt32(byteBuffer[i]);

                                    if (i != 0)
                                    {
                                        actual = caracteres[PosibleLlave];
                                        mensaje = $"{anterior.Substring(anterior.Length - 1, 1)}{caracteres[PosibleLlave]}";
                                        caracteres.Add(indice, mensaje);
                                        anterior = caracteres[PosibleLlave];
                                        indice++;
                                    }
                                    else
                                    {
                                        actual = caracteres[PosibleLlave];
                                        //mensaje = caracteres[PosibleLlave];
                                        writer.Write(actual);
                                        anterior = actual;
                                    }


                                }

                            }
                        }
                    }
                }

            }
            return 1;
        }

        public static string ObtenerExtension(string RutaOriginal)
        {
            var ext = string.Empty;
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    ext = reader.ReadString();
                }
            }
            return ext;
        }

        public static Dictionary<int, string> ObtenerDiccionarioOriginal(string RutaOriginal)
        {
            var DiccionarioOriginal = new Dictionary<int, string>();
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    var ExtensionLeida = false;
                    var ultimaPosicion = 0;
                    var ByteLeido = string.Empty;

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        ByteLeido = reader.ReadString();
                        if (ExtensionLeida == false)
                        {
                            ExtensionLeida = true;
                        }
                        else if (ByteLeido != "--")
                        {
                            var separador = ByteLeido.Split('|');
                            DiccionarioOriginal.Add(Convert.ToInt32(separador[0]), separador[1]);
                        }
                        else
                        {
                            ultimaPosicion = Convert.ToInt32(reader.BaseStream.Position + 1);
                            break;
                        }
                    }
                }
            }
            return DiccionarioOriginal;
        }

        public static void CompletarDiccionarioYEscribir(Dictionary<int, string> CaracteresOriginales, string RutaOriginal, string UbicacionAAlmacenarLZW, string[] nombreArchivo, string extension)
        {
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamwriter = new FileStream($"{UbicacionAAlmacenarLZW}//{nombreArchivo[0]}.{extension}", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamwriter))
                        {
                            var byteBuffer = new byte[bufferLength];
                            var indice = CaracteresOriginales.Count() + 1;
                            var PosibleLlave = 0;
                            var anterior = string.Empty;
                            var actual = string.Empty;
                            var KeyAgregada = string.Empty;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);
                                var mensaje = string.Empty;
                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = Convert.ToInt32(byteBuffer[i]);

                                    if (i != 0)
                                    {
                                        actual = CaracteresOriginales[PosibleLlave];
                                        //KeyAgregada = $"{anterior}{actual}";
                                        KeyAgregada = $"{anterior.Substring(anterior.Length - 1, 1)}{CaracteresOriginales[PosibleLlave]}";
                                        CaracteresOriginales.Add(indice, KeyAgregada);
                                        writer.Write(actual);
                                        anterior = CaracteresOriginales[PosibleLlave];
                                        indice++;
                                    }
                                    else
                                    {
                                        actual = CaracteresOriginales[PosibleLlave];
                                        //mensaje = caracteres[PosibleLlave];
                                        writer.Write(actual);
                                        anterior = actual;
                                    }


                                }
                            }
                        }
                    }
                }
            }
        }

    }
}