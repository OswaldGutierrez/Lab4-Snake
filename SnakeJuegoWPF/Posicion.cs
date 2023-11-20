using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeJuegoWPF
{

    // Clase para proprocionar una manera conveniente de trabajar con coordenadas en el espacio bidimensional y ofrece métodos para realizar desplazamientos y comparaciones de igualdad.
    internal class Posicion
    {

        // Propiedades de solo lectura para Filas y Columnas
        public int Filas { get; }
        public int Columnas { get; }


        // Constructor para inicializar una instancia de Posicion
        public Posicion(int filas, int columnas)
        {
            Filas = filas;
            Columnas = columnas;
        }


        // Método para realizar un desplazamiento en la posición según una dirección dada
        public Posicion nuevaPosicion(Direccion dir)
        {
            return new Posicion(Filas + dir.DesplazamientoFilas, Columnas + dir.DesplazamientoColumnas);
        }




        /* Sobrescribir Equals y GetHashCode:
         * Este proceso automatizado utiliza las propiedades de tu clase para generar las comparaciones de igualdad y los códigos hash de manera coherente.
         */
        public override bool Equals(object? obj)
        {
            return obj is Posicion posicion &&
                   Filas == posicion.Filas &&
                   Columnas == posicion.Columnas;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Filas, Columnas);
        }

        public static bool operator ==(Posicion? left, Posicion? right)
        {
            return EqualityComparer<Posicion>.Default.Equals(left, right);
        }

        public static bool operator !=(Posicion? left, Posicion? right)
        {
            return !(left == right);
        }
    }
}
