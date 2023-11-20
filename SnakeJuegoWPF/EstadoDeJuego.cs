using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeJuegoWPF
{
    internal class EstadoDeJuego
    {
        //10:50
        /* Filas, Columnas, Cuadriculita, Dir, Puntaje, y GameOver son propiedades de solo lectura que proporcionan información sobre el estado actual del juego.
         * Filas y Columnas representan las dimensiones del área de juego.
         * Cuadriculita es una matriz que representa el estado de cada cuadrado en el área de juego mediante el uso del enumerado Cuadricula
         * Dir representa la dirección actual en la que se está moviendo la serpiente.
         * Puntaje contiene la puntuación actual del jugador.
         * GameOver indica si el juego ha terminado.
         */
        public int Filas { get; }
        public int Columnas { get; }
        public Cuadricula[,] Cuadriculita { get; }
        public Direccion Dir { get; private set; }
        public int Puntaje { get; private set; }
        public bool GameOver { get; private set; }

        // Listas y Random para el control del juego
        private readonly LinkedList<Direccion> dirCambios = new LinkedList<Direccion>();
        private readonly LinkedList<Posicion> snakePosicion = new LinkedList<Posicion>();
        private readonly Random random = new Random();


        public EstadoDeJuego(int filas, int columnas)
        {
            Filas = filas;
            Columnas = columnas;
            Cuadriculita = new Cuadricula[filas, columnas];
            Dir = Direccion.derecha;

            crearSnake();
            agregarFood();
        }



        // Inicializamos la culebra una vez se ejecute el juego.
        private void crearSnake()
        {
            int r = Filas / 2;

            for (int i = 1; i <= 3; i++)
            {
                Cuadriculita[r, i] = Cuadricula.Snake;
                snakePosicion.AddFirst(new Posicion(r, i));
            }
        }

        // Este metodo se encarga de generar y colocar comina en una posicion vacía de la matriz o área del juego.
        private void agregarFood()
        {
            List<Posicion> empty = new List<Posicion>(posicionesVacias());

            if (empty.Count == 0)
            {
                return;
            }

            Posicion pos = empty[random.Next(empty.Count)];
            Cuadriculita[pos.Filas, pos.Columnas] = Cuadricula.Food;
        }

        // Este método se encarga de representar todas las posiciones vacías en el área de juego.
        private IEnumerable<Posicion> posicionesVacias()
        {
            for (int i = 0; i < Filas; i++)
            {
                for (int j = 0; j < Columnas; j++)
                {
                    if (Cuadriculita[i, j] == Cuadricula.Empty)
                    {
                        yield return new Posicion(i, j);
                    }
                }
            }
        }

        // Devuelve la posición de la cabeza de la culebra en el área de juego.
        public Posicion cabezaPosicion()
        {
            return snakePosicion.First.Value;
        }

        // Se encarga de agregar una nueva cabeza a la serpiente en la posición especificada y actualizar la matriz Cuadriculita para reflejar este cambio.
        private void agregarCabeza(Posicion posicion0)
        {
            snakePosicion.AddFirst(posicion0);
            Cuadriculita[posicion0.Filas, posicion0.Columnas] = Cuadricula.Snake;
        }

        // Devuelve la posición de la cola de la culebra en el área de juego
        public Posicion colaPosicion()
        {
            return snakePosicion.Last.Value;
        }

        // Se encarga de quitar la cola de la serpiente en el área de juego y actualizar la matriz Cuadriculita para reflejar este cambio.
        private void removerCola()
        {
            Posicion cola = snakePosicion.Last.Value;
            Cuadriculita[cola.Filas, cola.Columnas] = Cuadricula.Empty;
            snakePosicion.RemoveLast();
        }

        // Devuelve una enumeración de todas las posiciones de la serpiente en el área de juego. 
        public IEnumerable<Posicion> SnakePosicion()
        {
            return snakePosicion;
        }

        

        





        /* Metodos para manejar los cambios de dirección de la serpiente.
         * obtenerUltimaDireccion' sirve para determinar la direccion actual de la serpiente.
         */
        private Direccion obtenerUltimaDireccion()
        {
            if (dirCambios.Count == 0)
            {
                return Dir;
            }

            return dirCambios.Last.Value;
        }

        // cambieDireccionSiPuede verifica si es posible cambiar la dirección de la serpiente según ciertas condiciones.
        private bool cambieDireccionSiPuede(Direccion nuevaDireccion)
        {
            if (dirCambios.Count == 2)
            {
                return false;
            }

            Direccion ultimaDireccion = obtenerUltimaDireccion();
            return nuevaDireccion != ultimaDireccion && nuevaDireccion != ultimaDireccion.movOpuesto();
        }

        /* 'cambiaDireccion' se utiliza para solicitar un cambio en la dirección de la serpiente. Si es posible realizar el cambio de dirección según las condiciones verificadas por
         * 'cambieDireccionSiPuede', la nueva dirección se agrega a la lista de cambios de dirección (dirChanges).
         */
        public void cambiaDireccion(Direccion direccion)
        {
            if (cambieDireccionSiPuede(direccion))
            {
                dirCambios.AddLast(direccion);
            }
        }





        // Los siguientes métodos son los encargados de la lógica del movimiento de la serpiente y las verificaciones de las colisiones.
        // Verificamos que una posición dada está fuera de los límites de jueg
        private bool limiteCuadricula(Posicion posicion1)
        {
            return posicion1.Filas < 0 || posicion1.Filas >= Filas || posicion1.Columnas < 0 || posicion1.Columnas >= Columnas;
        }

        // Determinamos qué tipo de cuadricula la serpiente golpeará en la próxima posición de la cabeza. Devuelve el tipo de cuadricula (Cuadricula) que se encontrará.
        private Cuadricula colision(Posicion posicionNuevaDeLaCabeza)
        {
            if (limiteCuadricula(posicionNuevaDeLaCabeza))
            {
                return Cuadricula.Outside;
            }

            if (posicionNuevaDeLaCabeza == colaPosicion())
            {
                return Cuadricula.Empty;
            }

            return Cuadriculita[posicionNuevaDeLaCabeza.Filas, posicionNuevaDeLaCabeza.Columnas];
        }

        /* Este método ejecuta un ciclo de movimiento de la serpiente
         * Si hay cambios de dirección pendientes, actualiza la dirección de la serpiente.
         * Calcula la nueva posición de la cabeza utilizando la dirección actual.
         * Determina qué tipo de cuadricula se encuentra en la nueva posición de la cabeza utilizando el método colision.
         * Determina si la culebra golpea los límites del juego.
         * Si la serpiente golpea una casilla vacía, se elimina la cola y se agrega la nueva cabeza.
         * Si la serpiente golpea una comida, se agrega la nueva cabeza, se incrementa el puntaje y se agrega nueva comida.
         */
        public void movimiento()
        {

            if (dirCambios.Count > 0)
            {
                Dir = dirCambios.First.Value;
                dirCambios.RemoveFirst();
            }

            Posicion posicionNuevaDeLaCabeza = cabezaPosicion().nuevaPosicion(Dir);
            Cuadricula golpe = colision(posicionNuevaDeLaCabeza);

            if (golpe == Cuadricula.Outside || golpe == Cuadricula.Snake)
            {
                GameOver = true;
            }

            else if (golpe == Cuadricula.Empty)
            {
                removerCola();
                agregarCabeza(posicionNuevaDeLaCabeza);
            }

            else if (golpe == Cuadricula.Food)
            {
                agregarCabeza(posicionNuevaDeLaCabeza);
                Puntaje++;
                agregarFood();
            }
        }
    }
}
