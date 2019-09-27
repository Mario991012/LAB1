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
            var diccionarioTemporal = new Dictionary<string, int>();
            var indice = 1;
            var valorDiccionario = 0;

            diccionarioOriginal = ObtenerDiccionarioCaracteresEspeciales(RutaOriginal, ref indice, ref diccionarioTemporal);
            valorDiccionario = indice;

            var cantidadMaximaDiccionario = 5;
            EscribirLZW(diccionarioOriginal, valorDiccionario, RutaOriginal, NombreArchivo, UbicacionAAlmacenarLZW, cantidadMaximaDiccionario);

            return 1;
        }

        static public Dictionary<string, int> ObtenerDiccionarioCaracteresEspeciales(string RutaOriginal, ref int indice, ref Dictionary<string, int> diccionarioTemporal)
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
                                diccionarioTemporal.Add(Convert.ToString(Convert.ToChar(byteBuffer[i])), indice);
                                indice++;
                            }
                        }
                    }
                }
            }
            return DiccionarioAEscribir;
        }

        static public int ObtenerCantidadMax( Dictionary<string, int> caracteres, ref int indice, string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var CaracteresOriginales = new Dictionary<string, int>();
            CaracteresOriginales = caracteres;
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    var PosibleLlave = string.Empty;
                    var posicionesALaDerecha = 0;

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLength);
                        for (int i = 0; i < byteBuffer.Length; i++)
                        {
                            PosibleLlave = Convert.ToString(Convert.ToChar(byteBuffer[i]));

                            while (CaracteresOriginales.ContainsKey(PosibleLlave) && i + posicionesALaDerecha < byteBuffer.Length-1)
                            {
                                posicionesALaDerecha++;
                                PosibleLlave = $"{PosibleLlave}{Convert.ToString(Convert.ToChar(byteBuffer[i + posicionesALaDerecha]))}";
                            }
                            if (i + 1 < byteBuffer.Length)
                            {
                                CaracteresOriginales.Add(PosibleLlave, indice);
                                indice++;
                                i += posicionesALaDerecha - 1;
                                posicionesALaDerecha = 0;

                            }
                        }
                    }
                    return CaracteresOriginales.Count;
                }
            }
        }

        static public void EscribirLZW(Dictionary<string, int> DiccionarioOriginal, int indice, string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW, int cantidadMaximaDiccionario)
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
                            var PosibleLlave = string.Empty;
                            var posicionesALaDerecha = 0;
                            writer.Write(NombreArchivo[1]);
                            foreach (var item in DiccionarioOriginal)
                            {
                                writer.Write($"{item.Key}|{item.Value}");
                            }
                            writer.Write("--");

                            var anterior = string.Empty;
                            var actual = string.Empty;
                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);

                                


                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = Convert.ToString(Convert.ToChar(byteBuffer[i]));

                                    while (DiccionarioOriginal.ContainsKey(PosibleLlave) && i + posicionesALaDerecha <= byteBuffer.Length)
                                    {
                                        posicionesALaDerecha++;
                                        if (i+posicionesALaDerecha < byteBuffer.Length)
                                        {
                                            PosibleLlave = $"{PosibleLlave}{Convert.ToString(Convert.ToChar(byteBuffer[i + posicionesALaDerecha]))}";
                                        }
                                    }
                                    i += posicionesALaDerecha - 1;
                                    posicionesALaDerecha = 0;
                                    if (i + 1 >= byteBuffer.Length)//PARA EL ULTIMO CARACTER
                                    {
                                        var key = DiccionarioOriginal[PosibleLlave];
                                        var claveAEscribir = Convert.ToString(key, 2);
                                        var sizeArray = claveAEscribir.Length / 8 + 1;
                                        if (claveAEscribir.Length <= 8)
                                        {
                                            var byteAEscribirL = Convert.ToByte(DiccionarioOriginal[PosibleLlave]);
                                            writer.Write(Convert.ToByte(1));
                                            writer.Write(byteAEscribirL);
                                            indice++;
                                        }
                                        else
                                        {
                                            var ByteArray = new byte[sizeArray];
                                            var SizeByte = Convert.ToByte(sizeArray);
                                            var contadorPosicion = sizeArray - 1;
                                            var LongitudBinario = claveAEscribir.Length;
                                            for (int m = LongitudBinario; m > 0; m -= 8)
                                            {
                                                if (claveAEscribir.Length > 8)
                                                {
                                                    var ByteAEscribirL = claveAEscribir.Substring(m - 8, 8);
                                                    var ByteAEscribir = Convert.ToByte(Convert.ToInt32(ByteAEscribirL, 2));
                                                    ByteArray[contadorPosicion] = ByteAEscribir;
                                                    claveAEscribir = claveAEscribir.Substring(0, m - 8);
                                                }
                                                else
                                                {
                                                    claveAEscribir = claveAEscribir.PadLeft(8, '0');
                                                    var ByteAEscribir = Convert.ToByte(Convert.ToInt32(claveAEscribir, 2));
                                                    ByteArray[contadorPosicion] = ByteAEscribir;
                                                }
                                                contadorPosicion--;
                                            }
                                            writer.Write(SizeByte);
                                            writer.Write(ByteArray);
                                        }
                                    }
                                    else
                                    {
                                        DiccionarioOriginal.Add(PosibleLlave, indice);

                                        var key = DiccionarioOriginal[PosibleLlave.Substring(0, PosibleLlave.Length - 1)];
                                        var claveAEscribir = Convert.ToString(key, 2).PadLeft(8,'0');

                                        var cantidadDeBytes = claveAEscribir.Length / 8 + 1;
                                        
                                        if (claveAEscribir.Length <= 8)
                                        {
                                            var byteAEscribirL = Convert.ToByte(DiccionarioOriginal[PosibleLlave.Substring(0, PosibleLlave.Length - 1)]);
                                            writer.Write(Convert.ToByte(1));
                                            writer.Write(byteAEscribirL);
                                            indice++;
                                            
                                        }
                                        else
                                        {
                                            var bytesAEscribir = new byte[cantidadDeBytes];
                                            var SizeByte = Convert.ToByte(cantidadDeBytes);
                                            var contadorPosicion = cantidadDeBytes-1;
                                            var LongitudBinario = claveAEscribir.Length;
                                            for (int m = LongitudBinario; m > 0; m -=8)
                                            {
                                                if (claveAEscribir.Length > 8)
                                                {
                                                    var ByteAEscribirL = claveAEscribir.Substring(m-8,8).PadLeft(8,'0');
                                                    var ByteAEscribir = Convert.ToByte(Convert.ToInt32(ByteAEscribirL,2));
                                                    bytesAEscribir[contadorPosicion] = ByteAEscribir;
                                                    claveAEscribir = claveAEscribir.Substring(0, m - 8).PadLeft(8,'0');
                                                }
                                                else
                                                {
                                                    claveAEscribir = claveAEscribir.PadLeft(8, '0');
                                                    var ByteAEscribir = Convert.ToByte(Convert.ToInt32(claveAEscribir,2));
                                                    bytesAEscribir[contadorPosicion] = ByteAEscribir;
                                                }
                                                contadorPosicion--;
                                            }
                                            writer.Write(SizeByte);
                                            writer.Write(bytesAEscribir);
                                        }
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
                            var anterior = string.Empty;
                            var actual = string.Empty;
                            var valorAAgregar = string.Empty;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);
                                var primeraVez = true;
                                for (int i = ultimaPosicion; i < byteBuffer.Length; i++)
                                {
                                    var cantBytesALeer = Convert.ToInt32(byteBuffer[i]);
                                    var binario = string.Empty;

                                    for (int j = 1; j <= cantBytesALeer; j++)
                                    {
                                        binario = $"{binario}{Convert.ToString(Convert.ToInt32(byteBuffer[i + j]),2).PadLeft(8,'0')}";
                                    }

                                    i += cantBytesALeer;

                                    var llaveDecimal = Convert.ToInt32(binario, 2);

                                    if(CaracteresOriginales.ContainsKey(llaveDecimal))
                                    {
                                        actual = CaracteresOriginales[llaveDecimal]; //////////////////////7REVISAR
                                        var escrito = CaracteresOriginales[llaveDecimal].ToCharArray();
                                        foreach (var item in escrito)
                                            writer.Write(Convert.ToByte(item));


                                        if(primeraVez)
                                        {
                                            anterior = actual;
                                            primeraVez = false;
                                        }
                                        else
                                        {
                                            anterior = $"{anterior.Substring(anterior.Length - 1, 1)}{actual}";
                                            CaracteresOriginales.Add(indice, anterior);
                                            indice++;
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
}