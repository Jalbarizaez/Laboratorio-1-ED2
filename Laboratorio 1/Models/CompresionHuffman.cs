using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laboratorio_1.Models
{
    public class CompresionHuffman
    {
        private static NodoHuff Raiz { get; set; }
        public static Dictionary<string,string> Tabla_Caracteres { get; set; }
        public static int Tamaño_Datos { get; set; }
        public static double Cantidad_Datos;

        public static byte[] Compresion (string Dato)
        {
            
            Tabla_Caracteres = new Dictionary<string, string>();
            ArbolHuffman(Dato);

        }
        public static NodoHuff Unir_Nodos(NodoHuff Mayor, NodoHuff Menor)
        {
            NodoHuff Padre = new NodoHuff('`',Mayor.Probabilidad+Menor.Probabilidad);
            Padre.Izquierda = Mayor;
            Padre.Derecha = Menor;
            return Padre;
        }
        static void ArbolHuffman(string Dato)
        {
            Dictionary<char, int> Tabla_Frecuencias = new Dictionary<char, int>();
            char[] Caracteres = Dato.ToCharArray();
            Cantidad_Datos = Caracteres.Length;
            Tamaño_Datos = Caracteres.Length;
            foreach (char Caracter in Caracteres)
            {
                if (Tabla_Frecuencias.Keys.Contains(Caracter))
                {
                    Tabla_Frecuencias[Caracter] = Tabla_Frecuencias[Caracter] + 1;
                }
                else Tabla_Frecuencias.Add(Caracter, 1);
            }
            List<NodoHuff> Lista_Frecuencias = new List<NodoHuff>();
            foreach(KeyValuePair<char,int> Nodos in Tabla_Frecuencias)
            {
                Lista_Frecuencias.Add(new NodoHuff(Nodos.Key, (Convert.ToDouble(Nodos.Value) / Cantidad_Datos)));
            }
            while (Lista_Frecuencias.Count >1)
            {
                Lista_Frecuencias = Lista_Frecuencias.OrderBy(x => x.Probabilidad).ToList();
                NodoHuff Union = Unir_Nodos(Lista_Frecuencias[0], Lista_Frecuencias[1]);
                Lista_Frecuencias.RemoveRange(0, 2);
                Lista_Frecuencias.Add(Union);
            }
            Raiz = Lista_Frecuencias[0];

        }
    }
}