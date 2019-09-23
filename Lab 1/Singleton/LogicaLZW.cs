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
                                var cantidadBytes = (caracter.Value / 256) + 1;
                                byte[] bytes = new byte[cantidadBytes];
                                var contador = 0;
                                var binario = Convert.ToString(caracter.Value, 2);

                                for (int i = 0; i < binario.Length; i = i + 8)
                                {
                                    var bin = binario.Substring(i, 8);
                                    if (bin.Length < 8)
                                        bin = Convert.ToString(bin).PadLeft(8, '0');

                                    bytes[contador] = Convert.ToByte(Convert.ToInt16(bin));
                                    contador++;
                                }

                                writer.Write(cantidadBytes);
                                writer.Write(caracter.Key);
                                foreach(var bytesAEscribir in bytes)
                                {
                                    writer.Write(bytesAEscribir);
                                }
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

                                        if(Convert.ToInt64(caracteres[escrito]) > 255)
                                        {

                                        }
                                        
                                        //writer.Write(write);
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
            var extension = ObtenerExtension(RutaOriginal);

            var CaracteresOriginales = ObtenerDiccionarioOriginal(RutaOriginal);

            CompletarDiccionarioYEscribir(CaracteresOriginales, RutaOriginal, UbicacionAAlmacenarLZW, nombreArchivo, extension);

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

        static int ultimaPosicion = 0;
        public static Dictionary<int, string> ObtenerDiccionarioOriginal(string RutaOriginal)
        {
            var DiccionarioOriginal = new Dictionary<int, string>();
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    var ExtensionLeida = false;
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
                            ultimaPosicion = Convert.ToInt32(reader.BaseStream.Position);
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
                            var valorAAgregar = string.Empty;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);
                                var mensaje = string.Empty;
                                for (int i = ultimaPosicion; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = Convert.ToInt32(byteBuffer[i]);

                                    if (i != ultimaPosicion)
                                    {
                                        valorAAgregar = $"{anterior}{CaracteresOriginales[PosibleLlave].Substring(0, 1)}";
                                        var bytes = Encoding.Default.GetBytes(CaracteresOriginales[PosibleLlave]);
                                        foreach (var byteC in bytes)
                                            writer.Write(byteC);
                                        CaracteresOriginales.Add(indice, valorAAgregar);
                                        anterior = CaracteresOriginales[PosibleLlave]; ;
                                        indice++;
                                    }
                                    else
                                    {
                                        valorAAgregar = CaracteresOriginales[PosibleLlave];
                                        writer.Write(Convert.ToByte(Convert.ToChar(valorAAgregar)));
                                        anterior = valorAAgregar;
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