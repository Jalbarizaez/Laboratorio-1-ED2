using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Laboratorio_1.Models
{
    public class CompresionHuffman
    {
        private static char separador = new char();
        private const int bufferLenght = 500;
        private static NodoHuff Raiz { get; set; }
        private static Dictionary<byte, int> Tabla_Frecuencias { get; set; }
        private static Dictionary<byte,string> Tabla_Caracteres { get; set; }
        private static decimal Cantidad_Datos;

        public void Compresion (string path_Lectura,string path_Escritura)
        {
            Tabla_Frecuencias = new Dictionary<byte, int>();
            Tabla_Caracteres = new Dictionary<byte, string>();
            ArbolHuffman(path_Lectura);
            Obtener_Codigos_Prefijo();
            Escribir_Valor_y_Frecuencia(path_Escritura);
            Recorrido(path_Lectura, path_Escritura);
        }

        private static NodoHuff Unir_Nodos(NodoHuff Mayor, NodoHuff Menor)
        {
            NodoHuff Padre = new NodoHuff(Mayor.Probabilidad+Menor.Probabilidad);
            Padre.Izquierda = Mayor;
            Padre.Derecha = Menor;
            return Padre;
        }

        private static void ArbolHuffman(string path)
        {
            using (var File = new FileStream(path, FileMode.Open))
            {
                var buffer = new byte[bufferLenght];
                using (var reader = new BinaryReader(File))
                {
                    Cantidad_Datos = reader.BaseStream.Length;
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        foreach (var item in buffer)
                        {
							if (Tabla_Frecuencias.Keys.Contains((item)))
							{
								Tabla_Frecuencias[(item)]++;
							}
							else Tabla_Frecuencias.Add(item, 1);

                        }
                    }
                }
            }

            List<NodoHuff> Lista_Frecuencias = new List<NodoHuff>();
            foreach(KeyValuePair<byte,int> Nodos in Tabla_Frecuencias)
            {
				//(Dato,Probabilidad)
                Lista_Frecuencias.Add(new NodoHuff(Nodos.Key, (Convert.ToDecimal(Nodos.Value) / Cantidad_Datos)));
            }
            //Uniendo Nodos
            while (Lista_Frecuencias.Count > 1)
            {
                if (Lista_Frecuencias.Count == 1)

                {
                    break;
                }
                else
                {
                    Lista_Frecuencias = Lista_Frecuencias.OrderBy(x => x.Probabilidad).ToList();
                    NodoHuff Union = Unir_Nodos(Lista_Frecuencias[1], Lista_Frecuencias[0]);
                    Lista_Frecuencias.RemoveRange(0, 2);
                    Lista_Frecuencias.Add(Union);
                }
            }
            Raiz = Lista_Frecuencias[0];
           //Aqui el arbol ya esta terminado
        }

		private static void Codigos_Prefijo(NodoHuff Nodo, string recorrido)
        {
            if (Nodo.Hoja()) Tabla_Caracteres.Add(Nodo.Dato, recorrido);
            else
            {
                if (Nodo.Izquierda != null) Codigos_Prefijo(Nodo.Izquierda, recorrido + "0");
                if (Nodo.Derecha != null) Codigos_Prefijo(Nodo.Derecha, recorrido + "1");
            }
        }
		
		//LLena la tabla de caracteres con su ascii y su prefijo
        private static void Obtener_Codigos_Prefijo()
        {
            if (Raiz.Hoja()) Tabla_Caracteres.Add(Raiz.Dato, "1");
            else Codigos_Prefijo(Raiz, "");
        }

		private static void Recorrido(string path_Lectura, string path_Escritura)
        {
            string recorrido = "";
            using (var writer = new FileStream(path_Escritura, FileMode.Append))
            {
                using (var File = new FileStream(path_Lectura, FileMode.Open))
                {

                    var buffer = new byte[bufferLenght];
                    var Bytes = new List<byte>();
                    using (var reader = new BinaryReader(File))
                    {

                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            buffer = reader.ReadBytes(bufferLenght);
                            //Lee el archivo letra por letra
                            foreach (var item in buffer)
                            {
                                recorrido += Tabla_Caracteres[item];
                                if (recorrido.Length >= 8)
                                {
                                    while (recorrido.Length > 8)
                                    {
                                        Bytes.Add(Convert.ToByte(recorrido.Substring(0, 8), 2));
                                        //Junta el codigo prefico en grupos de 8 en 8
                                        recorrido = recorrido.Remove(0, 8);
                                    }
                                }
                            }
                            writer.Write(Bytes.ToArray(), 0, Bytes.ToArray().Length);
                            Bytes.Clear();
                            //Escribe la lista de Bytes y se imprimen como ascci
                        }
                        for (int i = recorrido.Length; i < 8; i++)
                        {
                            recorrido += "0";
                        }
                        Bytes.Add(Convert.ToByte(recorrido, 2));
                        writer.Write(Bytes.ToArray(), 0, Bytes.ToArray().Length);
                    }
                
                }
            }
        }

        private static void Escribir_Valor_y_Frecuencia(string path)
        {
            var escritura = new byte[bufferLenght];
            separador = '|';
            if (Tabla_Caracteres.Keys.Contains(Convert.ToByte('|')))
            {
                separador = 'ÿ';
                if (Tabla_Caracteres.Keys.Contains(Convert.ToByte('ÿ')))
                {
                    separador = 'ß';
                }
            }

            using (var file = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(file))
                {
                    escritura = Encoding.UTF8.GetBytes(Cantidad_Datos.ToString().ToArray());
                    //writer.Write(Cantidad_Datos.ToString() + "|");
                    writer.Write(escritura);
                    writer.Write(Convert.ToByte(separador));
                    //Escribe el caracter junto con su Frecuencia
                    foreach (KeyValuePair<byte, int> Valores in Tabla_Frecuencias)
                    {
                        writer.Write(Valores.Key);
                        escritura = Encoding.UTF8.GetBytes(Valores.Value.ToString().ToArray());//+ Valores.Value.ToString() + "|");
                        writer.Write(escritura);
                        writer.Write(Convert.ToByte(separador));
                    }
                    writer.Write(Convert.ToByte(separador));
                }
            
            }
        }

		public void SetMisCompresiones(string pathLectura, string pathEscritura, string pathMiFichero)
		{
			FileInfo ArchivoOriginal = new FileInfo(pathLectura);
			string nombre = ArchivoOriginal.Name;
			double pesoArchivo = ArchivoOriginal.Length;

			FileInfo ArchivoComprimido = new FileInfo(pathEscritura);
			double pesoArchivo2 = ArchivoComprimido.Length;

			double RazonDeCompresion = pesoArchivo2 / pesoArchivo;
			double FactorDeCompresion = pesoArchivo / pesoArchivo2;
			double PorcentajeDeReduccion = 100- (RazonDeCompresion * 100);

			using (StreamWriter Writer = File.AppendText(pathMiFichero))
			{
				Writer.WriteLine(nombre + "," + RazonDeCompresion + "," + FactorDeCompresion + "," + PorcentajeDeReduccion + "," + "Huffman");
			}
		}

		public List<string> GetMisCompresiones(string pathMiFichero)
		{
			List<string> ListaDeCompresiones = new List<string>();
			using (StreamReader Reader = new StreamReader(pathMiFichero))
			{
				while (!Reader.EndOfStream)
				{
					ListaDeCompresiones.Add(Reader.ReadLine());
				}
			}
			return ListaDeCompresiones;
		}

    }
}