using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeJuegoWPF
{
    internal class EstadoDeJuego
    {

        public int Filas { get; }
        public int Columnas { get; }
        public Cuadricula[,] Cuadriculita { get; }
        public Direccion Dir { get; private set; }
        public int Puntaje { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Posicion> snakePosicion = new LinkedList<Posicion>();
        private readonly Random random = new Random();

        public EstadoDeJuego(int filas, int columnas)
        {
            Filas = filas;
            Columnas = columnas;
            Cuadriculita = new Cuadricula[filas, columnas];
            Dir = Direccion.derecha;

            agregarSnake();
            agregarFood();
        }

        private void agregarSnake()
        {
            int r = Filas / 2;

            for (int i = 1; i <= 3; i++)
            {
                Cuadriculita[r, i] = Cuadricula.Snake;
                snakePosicion.AddFirst(new Posicion(r, i));
            }
        }

        private IEnumerable<Posicion> posicionesVacias()
        {
            for(int i = 0; i < Filas; i++)
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

        public Posicion headPosicion()
        {
            return snakePosicion.First.Value;
        }

        public Posicion tailPosicion()
        {
            return snakePosicion.Last.Value;
        }

        public IEnumerable<Posicion> SnakePosicion()
        {
            return snakePosicion;
        }

        private void AddHead(Posicion pos)
        {
            snakePosicion.AddFirst(pos);
            Cuadriculita[pos.Filas, pos.Columnas] = Cuadricula.Snake;
        }

        private void RemoveTail()
        {
            Posicion tail = snakePosicion.Last.Value;
            Cuadriculita[tail.Filas, tail.Columnas] = Cuadricula.Empty;
            snakePosicion.RemoveLast();
        }

        public void ChangeDirection(Direccion dir)
        {
            Dir = dir;
        }

        private bool OutsideGrid(Posicion pos)
        {
            return pos.Filas < 0 || pos.Filas >= Filas || pos.Columnas < 0 || pos.Columnas >= Columnas;
        }

        private Cuadricula WillHit(Posicion newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return Cuadricula.Outside;
            }

            if (newHeadPos == tailPosicion())
            {
                return Cuadricula.Empty;
            }

            return Cuadriculita[newHeadPos.Filas, newHeadPos.Columnas];
        }

        public void Move()
        {
            Posicion newHeadPos = headPosicion().Translate(Dir);
            Cuadricula hit = WillHit(newHeadPos);

            if (hit == Cuadricula.Outside || hit == Cuadricula.Snake)
            {
                GameOver = true;
            }

            else if (hit == Cuadricula.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }

            else if (hit == Cuadricula.Food)
            {
                AddHead(newHeadPos);
                Puntaje++;
                agregarFood();
            }
        }
    }
}
