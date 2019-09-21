﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace Laboratorio_1.Models
{
	public class CompresionLZW
	{
		private static Dictionary<string, int> Tabla_Caracteres { get; set; }
		private const int bufferLenght = 500;

		public void Compresion(string pathLectura, string pathEscritura)
		{
			Tabla_Caracteres = new Dictionary<string, int>();
            Diccionario_Inicial(pathLectura);
            Escribir_Diccionario(pathEscritura);
            Diccionario_Completo(pathLectura, pathEscritura);
        }
		private void Diccionario_Inicial(string pathLectura)
		{
			var buffer = new char[bufferLenght];	
			//Se llena el diccionario con los valores iniciales
			using (var file = new FileStream(pathLectura, FileMode.Open))
			{
				using (var reader = new BinaryReader(file))
				{
					while (reader.BaseStream.Position != reader.BaseStream.Length)
					{
						buffer = reader.ReadChars(bufferLenght);
						foreach (var item in buffer)
						{
							if (!Tabla_Caracteres.ContainsKey(Convert.ToString(item)) && Convert.ToInt16(item) != 13)
							{
								if (Tabla_Caracteres.Count() == 0)
									Tabla_Caracteres.Add(Convert.ToString(item), 1);
								else
									Tabla_Caracteres.Add(Convert.ToString(item), (Tabla_Caracteres.Count + 1));
							}
						}
					}
				}
			}
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

                    //Escribe el caracter junto con su Frecuencia
                    foreach (KeyValuePair<string, int> Valores in Tabla_Caracteres)
                    {
                        escritura = Encoding.UTF8.GetBytes(Valores.Key.ToString().ToArray());//+ Valores.Value.ToString() + "|");
                        writer.Write(escritura);
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
			var buffer = new char[bufferLenght];
			var resultado = new List<int>();
			string UltimoCaracter = "";
			using (var file = new FileStream(pathLectura, FileMode.Open))
			{
				using (var reader = new BinaryReader(file))
				{
					while (reader.BaseStream.Position != reader.BaseStream.Length)
					{
						buffer = reader.ReadChars(bufferLenght);
						foreach (var item in buffer)
						{
							if (!Tabla_Caracteres.ContainsKey((UltimoCaracter + Convert.ToString(Convert.ToChar(item)))))
							{
								Tabla_Caracteres.Add((UltimoCaracter + Convert.ToString(Convert.ToChar(item))), (Tabla_Caracteres.Count + 1));
								resultado.Add(Tabla_Caracteres[UltimoCaracter]);
								UltimoCaracter = (UltimoCaracter + Convert.ToString(Convert.ToChar(item))).Substring(((UltimoCaracter + Convert.ToString(Convert.ToChar(item))).Length - 1), 1);
							}
							else
							{
								UltimoCaracter = (UltimoCaracter + Convert.ToString(Convert.ToChar(item)));
							}
						}
						resultado.Add(Tabla_Caracteres[UltimoCaracter]);
						int mayor = resultado.Max();
					}
				}
			}
		}
	}
}