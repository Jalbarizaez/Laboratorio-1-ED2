using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laboratorio_1.Models
{
    public class NodoHuff
    {
        public string Dato { get; set; }
        public double Probabilidad { get; set; }
        public NodoHuff Izquierda { get; set; }
        public NodoHuff Derecha { get; set; }
        public NodoHuff Padre { get; set; }

        public NodoHuff (string dato, double probabilidad)
        {
            Dato = dato;
            Probabilidad = probabilidad;
            Izquierda = null;
            Derecha = null;
            Padre = null;
        }
    }
}