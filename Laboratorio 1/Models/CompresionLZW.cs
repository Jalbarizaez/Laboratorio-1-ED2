using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Laboratorio_1.Models
{
	public class CompresionLZW
	{
		private static Dictionary<string, int> Tabla_Caracteres { get; set; }
		private const int bufferLenght = 500;

		public void Compresion(string pathLectura, string pathEscritura)
		{
			Tabla_Caracteres = new Dictionary<string, int>();
			Diccionario_Inicial(pathLectura, pathEscritura);
			Diccionario_Completo(pathLectura, pathEscritura);
		}
		private void Diccionario_Inicial(string pathLectura, string pathEscritura)
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
							//13 /r  10 /n
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
			//Escribir aqui el diccionario al archivo
		}
		private void Diccionario_Completo(string pathLectura, string pathEscritura)
		{
			var buffer = new byte[bufferLenght];
			var resultado = new List<int>();
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
							if (!Tabla_Caracteres.ContainsKey((UltimoCaracter + Convert.ToString(Convert.ToChar(item)))))
							{
								Tabla_Caracteres.Add((UltimoCaracter + Convert.ToString(Convert.ToChar(item))), (Tabla_Caracteres.Count + 1));
								resultado.Add(Tabla_Caracteres[(UltimoCaracter + Convert.ToString(Convert.ToChar(item))).Substring(0, ((UltimoCaracter + Convert.ToString(Convert.ToChar(item))).Length - 1))]);
								UltimoCaracter = (UltimoCaracter + Convert.ToString(Convert.ToChar(item))).Substring(((UltimoCaracter + Convert.ToString(Convert.ToChar(item))).Length - 1), 1);
							}
							else
							{
								UltimoCaracter = (UltimoCaracter + Convert.ToString(Convert.ToChar(item)));
							}
						}
						resultado.Add(Tabla_Caracteres[UltimoCaracter]);
					}
				}
			}
			//Escribir aca todo los numero en bytes al archivo de escritura
		}
	}
}