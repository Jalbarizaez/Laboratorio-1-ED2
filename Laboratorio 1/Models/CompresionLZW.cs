using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace Laboratorio_1.Models
{
	public class CompresionLZW
	{
		private Dictionary<string, int> Tabla_Caracteres = new Dictionary<string, int>();
		private Dictionary<string, int> Tabla_Escritura = new Dictionary<string, int>();
        private Dictionary<byte, int> Tabla_bytes = new Dictionary<byte, int>();
        private const int bufferLenght = 500;
		int cantidad_bits { get; set; }

		public void Compresion(string pathLectura, string pathEscritura)
        { 
            Diccionario_Inicial(pathLectura);
            Cantidad_Bits(pathLectura);
            Escribir_Diccionario(pathEscritura);
            Diccionario_Completo(pathLectura, pathEscritura);
        }

        private void Cantidad_Bits(string pathLectura)
        {
            var buffer = new byte[bufferLenght];
            string UltimoCaracter = "";
            using (var file = new FileStream(pathLectura, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        foreach (var item in buffer)
                        {
                            var bytes = Convert.ToString(Convert.ToChar(item));
                            if (!Tabla_Caracteres.ContainsKey((UltimoCaracter + bytes)))
                            {
                                Tabla_Caracteres.Add(UltimoCaracter + bytes, (Tabla_Caracteres.Count + 1));
                                UltimoCaracter = bytes;
                            }
                            else
                            {
                                UltimoCaracter += bytes;
                            }
                        }
                    }
                    var numero = Tabla_Caracteres.Values.Max();
                    var bits = Convert.ToString(numero, 2);
                    cantidad_bits = bits.Length;
                }
            }
        }

        private void Diccionario_Inicial(string pathLectura, string pathEscritura)
        {

            var buffer = new byte[bufferLenght];
            //Se llena el diccionario con los valores iniciales
            using (var file = new FileStream(pathLectura, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        foreach (var item in buffer)
                        {
                            var bytes = Convert.ToString(Convert.ToChar(item));
                            //13 /r  10 /n
                            if (!Tabla_Caracteres.ContainsKey(bytes) /*&& Convert.ToInt16(Convert.ToChar(item)) != 13*/)
                            {

                                if (Tabla_Caracteres.Count() == 0)
                                {
                                    Tabla_Caracteres.Add(bytes, 1);
                                    Tabla_bytes.Add(item, 1);
                                    Tabla_Escritura.Add(bytes, 1);
                                }
                                else
                                {
                                    Tabla_Caracteres.Add(bytes, (Tabla_Caracteres.Count + 1));
                                    Tabla_bytes.Add(item, (Tabla_bytes.Count + 1));
                                    Tabla_Escritura.Add(bytes, (Tabla_Escritura.Count + 1));
                                }
                            }
                        }
                    }
                }
            }
            //Escribir aqui el diccionario al archivo
        }

        private void Escribir_Diccionario(string pathEscritura)
        {
            char separador = '|';
            var escritura = new byte[bufferLenght];

            if (Tabla_Caracteres.Keys.Contains("|"))
            {
                separador = 'ÿ';
                if (Tabla_Caracteres.Keys.Contains("ÿ"))
                {
                    separador = 'ß';
                }
            }

            using (var file = new FileStream(pathEscritura, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(file))
                {
                    escritura = Encoding.UTF8.GetBytes(cantidad_bits.ToString().ToArray());
                    writer.Write(escritura);
                    writer.Write(Convert.ToByte(separador));
                    //Escribe el caracter junto con su Frecuencia
                    foreach (KeyValuePair<byte, int> Valores in Tabla_bytes)
                    {
                        //+ Valores.Value.ToString() + "|");
                        writer.Write(Valores.Key);
                        escritura = Encoding.UTF8.GetBytes(Valores.Value.ToString().ToArray());//+ Valores.Value.ToString() + "|");
                        writer.Write(escritura);
                        writer.Write(Convert.ToByte(separador));
                    }
                    writer.Write(Convert.ToByte(separador));
                }

            }

        }

        private void Diccionario_Completo(string pathLectura, string pathEscritura)
        {
            var escritura = new List<byte>();
            string recorrido = "";
            var buffer = new byte[bufferLenght];
            var resultado = new List<int>();
            string UltimoCaracter = "";
            int i = 0;
            using (var writer = new FileStream(pathEscritura, FileMode.Append))
            {
                using (var file = new FileStream(pathLectura, FileMode.Open))
                {
                    using (var reader = new BinaryReader(file))
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            buffer = reader.ReadBytes(bufferLenght);
                            foreach (var item in buffer)
                            {
                                var bytes = Convert.ToString(Convert.ToChar(item));
                                if (!Tabla_Escritura.ContainsKey((UltimoCaracter + bytes)))
                                {
                                    Tabla_Escritura.Add(UltimoCaracter + bytes, (Tabla_Escritura.Count + 1));
                                    //resultado.Add(Tabla_Escritura[UltimoCaracter]);
                                    var bitss = Convert.ToString(Tabla_Escritura[UltimoCaracter], 2);
                                    var completos = bitss.PadLeft(cantidad_bits, '0');
                                    recorrido += completos;
                                    if (recorrido.Length >= 8)
                                    {
                                        while (recorrido.Length > 8)
                                        {
                                            escritura.Add((Convert.ToByte(recorrido.Substring(0, 8), 2)));
                                            //i++;
                                            //Junta el codigo prefico en grupos de 8 en 8
                                            recorrido = recorrido.Remove(0, 8);
                                        }
                                    }
                                    UltimoCaracter = bytes;
                                    // agrega el numero                                         
                                }
                                else
                                {
                                    UltimoCaracter += bytes;
                                }
                            }
                            //i = 0;
                            writer.Write(escritura.ToArray(), 0, escritura.ToArray().Length);
                            escritura.Clear();
                            //resultado.Add(Tabla_Escritura[UltimoCaracter]);
                        }
                        var bit = Convert.ToString(Tabla_Escritura[UltimoCaracter], 2);
                        var completo = bit.PadLeft(cantidad_bits, '0');
                        recorrido += completo;
                        List<byte> escribir = new List<byte>();
                        while (recorrido.Length > 8)
                        {
                            escribir.Add(Convert.ToByte(recorrido.Substring(0, 8), 2));

                            //Junta el codigo prefico en grupos de 8 en 8
                            recorrido = recorrido.Remove(0, 8);
                        }
                        if (recorrido.Length != 0)
                        {
                            for (int j = 0; recorrido.Length < 8; j++)
                            {
                                recorrido += "0";
                            }
                            escribir.Add(Convert.ToByte(recorrido, 2));
                        }
                        writer.Write(escribir.ToArray(), 0, escribir.ToArray().Length);
                    }
                }
            }
        }
    }