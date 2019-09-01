using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laboratorio_1.Models
{
    public class CompresionHuffman
    {
        private static NodoHuff Raiz { get; set; }
        private static Dictionary<char,string> Tabla_Caracteres { get; set; }
        private static int Tamaño_Datos { get; set; }
        private static decimal Cantidad_Datos;

        public static void Compresion (string Dato)
        {
            
            Tabla_Caracteres = new Dictionary<char, string>();
            ArbolHuffman(Dato);

        }
        private static NodoHuff Unir_Nodos(NodoHuff Mayor, NodoHuff Menor)
        {
            NodoHuff Padre = new NodoHuff('`',Mayor.Probabilidad+Menor.Probabilidad);
            Padre.Izquierda = Mayor;
            Padre.Derecha = Menor;
            return Padre;
        }
        private static void ArbolHuffman(string Dato)
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
                Lista_Frecuencias.Add(new NodoHuff(Nodos.Key, (Convert.ToDecimal(Nodos.Value) / Cantidad_Datos)));
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
        private static void Codigos_Prefijo(NodoHuff Nodo, string recorrido)
        {
            if (Nodo.Hoja()) Tabla_Caracteres.Add(Nodo.Dato, recorrido);
            else
            {
                if (Nodo.Izquierda != null) Codigos_Prefijo(Nodo.Izquierda, recorrido + "0");
                if (Nodo.Derecha != null) Codigos_Prefijo(Nodo.Derecha, recorrido + "1");
            }
        }
        private static void Obtener_Codigos_Prefijo()
        {
            if (Raiz.Hoja()) Tabla_Caracteres.Add(Raiz.Dato, "1");
            else Codigos_Prefijo(Raiz, "");
        }
        private static void Recorrido(string Dato)
        {
            string recorrdio = "";
            char[] recorrer = Dato.ToCharArray();
            foreach (char Caracter in recorrer)
            {
                recorrdio += Tabla_Caracteres[Caracter];
            }
        }
    }
}