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
			//Aqui guarda lo que debe ser escrito en Bytes
			var resultado = new List<int>();
			string UltimoCaracter = "";
			var buffer = new char[bufferLenght];
			using (var file = new FileStream(pathLectura, FileMode.Open))
			{
				using (var reader = new BinaryReader(file))
				{
					while (reader.BaseStream.Position != reader.BaseStream.Length)
					{
						buffer = reader.ReadChars(1);
						string LineaTmp = "";
						foreach (var item in buffer)
						{
							LineaTmp = LineaTmp + Convert.ToString(item);
						}
						LineaTmp = UltimoCaracter + LineaTmp;
						if (!Tabla_Caracteres.ContainsKey(LineaTmp))
						{
							Tabla_Caracteres.Add(LineaTmp, (Tabla_Caracteres.Count + 1));
							//Solo entra a este IF si es el primer caracter del txt, creado ya que sin esto da error en *
							//al no encontrar la llave solicitada
							if (!Tabla_Caracteres.ContainsKey(LineaTmp.Substring(0, (LineaTmp.Length - 1))))
							{ }
							else
							{
								//*
								resultado.Add(Tabla_Caracteres[LineaTmp.Substring(0, (LineaTmp.Length - 1))]);
								UltimoCaracter = LineaTmp.Substring((LineaTmp.Length - 1), 1);
							}
						}
						else
							UltimoCaracter = LineaTmp;
					}
				}
				resultado.Add(Tabla_Caracteres[UltimoCaracter]);
			}

			//Escribir aca todo los numero en bytes al archivo de escritura
		}
	}
}