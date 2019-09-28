using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab_1.Singleton
{
    public class LogicaLZW
    {
        const int bufferLength = 1000000;

        static public int Comprimir(string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var diccionarioOriginal = new Dictionary<string, int>();
            var indice = 1;
            var valorDiccionario = 0;

            diccionarioOriginal = ObtenerDiccionarioCaracteresEspeciales(RutaOriginal, ref indice);
            valorDiccionario = indice;

            EscribirLZW(diccionarioOriginal, valorDiccionario, RutaOriginal, NombreArchivo, UbicacionAAlmacenarLZW);

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
                            var llave = Convert.ToString(Convert.ToChar(byteBuffer[i]));
                            if (!DiccionarioAEscribir.ContainsKey(llave))
                            {
                                DiccionarioAEscribir.Add(llave, indice);
                                indice++;
                            }
                        }
                    }
                }
            }
            return DiccionarioAEscribir;
        }

        
        static public void EscribirLZW(Dictionary<string, int> DiccionarioOriginal, int indice, string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamWriter = new FileStream($"{UbicacionAAlmacenarLZW}/{NombreArchivo[0]}.lzw", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamWriter))
                        {
                            var byteBuffer = new byte[bufferLength];

                            writer.Write(NombreArchivo[1]);

                            foreach (var item in DiccionarioOriginal)
                            {
                                writer.Write($"{item.Key}|{item.Value}");
                            }

                            writer.Write("--");


                            var anterior = string.Empty;
                            var actual = string.Empty;
                            var anteriorYActual = string.Empty;
                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);

                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    actual = Convert.ToString(Convert.ToChar(byteBuffer[i]));

                                    anteriorYActual = $"{anterior}{actual}";

                                    if (DiccionarioOriginal.ContainsKey(anteriorYActual))
                                    {
                                        anterior = anteriorYActual;
                                    }
                                    else
                                    {
                                        var binario = Convert.ToString(DiccionarioOriginal[anterior], 2);
                                        if (binario.Length <= 8)
                                        {
                                            writer.Write(Convert.ToByte(1));
                                            writer.Write(Convert.ToByte(DiccionarioOriginal[anterior]));
                                        }
                                        else
                                        {
                                            var cantDeBytes = binario.Length / 8 + 1;
                                            binario = Convert.ToString(binario).PadLeft(cantDeBytes * 8, '0');
                                            var vectorBytes = new byte[cantDeBytes];
                                            var index = 0;
                                            for (int j = 0; j < binario.Length; j += 8)
                                            {
                                                vectorBytes[index] = Convert.ToByte(Convert.ToInt32(binario.Substring(j, 8), 2));
                                                index++;
                                            }

                                            writer.Write(Convert.ToByte(cantDeBytes));
                                            foreach (var bytes in vectorBytes)
                                                writer.Write(bytes);

                                        }
                                        DiccionarioOriginal.Add(anteriorYActual, indice);
                                        indice++;
                                        anterior = actual;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        

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
                            DiccionarioOriginal.Add(Convert.ToInt32(separador[1]), separador[0]);
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


        static int BinarioADecimal(string input)
        {
            char[] array = input.ToCharArray();
            Array.Reverse(array);
            int sum = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == '1')
                {
                    sum += (int)Math.Pow(2, i);
                }
            }
            return sum;
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

                            var codigoViejo = 0;
                            var codigoNuevo = 0;
                            var cadena = string.Empty;
                            var caracter = string.Empty;
                            var salida = string.Empty;


                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);

                                codigoViejo = byteBuffer[ultimaPosicion + 1];
                                caracter = CaracteresOriginales[codigoViejo];

                                var caracChar = caracter.ToCharArray();
                                foreach(var item in caracChar)
                                {
                                    var caracterAChar = Convert.ToChar(item);
                                    var dec = Convert.ToInt32(caracterAChar);
                                    writer.Write(Convert.ToByte(dec));
                                }
                                

                                for (int i = ultimaPosicion + 2; i < byteBuffer.Length; i++)
                                {
                                    var cantBytesALeer = Convert.ToInt32(byteBuffer[i]);
                                    var binario = string.Empty;

                                    for (int j = 1; j <= cantBytesALeer; j++)
                                    {
                                        if (i + j < byteBuffer.Length)
                                        {
                                            binario = $"{binario}{Convert.ToString(Convert.ToInt32(byteBuffer[i + j]), 2).PadLeft(8, '0')}";
                                        }
                                    }

                                    codigoNuevo = BinarioADecimal(binario);

                                    i += cantBytesALeer;

                                    if (CaracteresOriginales.ContainsKey(codigoNuevo))
                                    {
                                        cadena = CaracteresOriginales[codigoNuevo];

                                        var cadenaBytes = cadena.ToCharArray();
                                        foreach (var byteEnCadena in cadenaBytes)
                                        {
                                            writer.Write(Convert.ToByte(Convert.ToInt32(byteEnCadena)));
                                        }

                                        caracter = cadena.Substring(0, 1);

                                        var agregado = $"{CaracteresOriginales[codigoViejo]}{caracter}";
                                        
                                        CaracteresOriginales.Add(indice, agregado);
                                        indice++;
                                        codigoViejo = codigoNuevo;

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