using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeJuegoWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HistorialDePuntajes historial = new HistorialDePuntajes();

        private readonly Dictionary<Cuadricula, ImageSource> gridValToImage = new()
        {
            {Cuadricula.Empty, Imagenes.Empty },
            {Cuadricula.Snake, Imagenes.Body },
            {Cuadricula.Food, Imagenes.Food }
        };

        private readonly Dictionary<Direccion, int> dirToRotation = new()
        {
            {Direccion.arriba, 0 },
            {Direccion.abajo, 180 },
            {Direccion.derecha, 90 },
            {Direccion.izquierda, 270 }
        };

        private readonly int filas = 15, columnas = 15;
        private readonly Image[,] gridImages;
        private EstadoDeJuego estadoDeJuego;
        private bool ejecutandoJuego;
        private int velocidad = 350;
        private bool aumentoDeVelocidadRealizado = false;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            estadoDeJuego = new EstadoDeJuego(filas, columnas);
        }

        private async Task ejecutarJuego()
        {
            dibujar();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            estadoDeJuego = new EstadoDeJuego(filas, columnas);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            // Verifica si la tecla presionada es la tecla de espacio
            if (e.Key == Key.Space)
            {
                // Reinicia el juego solo si no está en ejecución
                if (!ejecutandoJuego)
                {
                    ejecutandoJuego = true;
                    await ejecutarJuego();
                    ejecutandoJuego = false;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (estadoDeJuego.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    estadoDeJuego.cambiaDireccion(Direccion.izquierda);
                    break;
                case Key.Right:
                    estadoDeJuego.cambiaDireccion(Direccion.derecha);
                    break;
                case Key.Up:
                    estadoDeJuego.cambiaDireccion(Direccion.arriba);
                    break;
                case Key.Down:
                    estadoDeJuego.cambiaDireccion(Direccion.abajo);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!estadoDeJuego.GameOver)
            {
                await Task.Delay(velocidad);
                estadoDeJuego.movimiento();
                dibujar();

                if (estadoDeJuego.Puntaje != 0 && estadoDeJuego.Puntaje % 4 == 0 && !aumentoDeVelocidadRealizado)
                {
                    AumentarVelocidad();
                    aumentoDeVelocidadRealizado = true;
                }

                if (estadoDeJuego.Puntaje % 4 != 0)
                {
                    aumentoDeVelocidadRealizado = false;
                }
            }
        }

        private void AumentarVelocidad()
        {
            velocidad -= 25; // Puedes ajustar el valor de decremento según tus necesidades.
            if (velocidad < 10) // Asegúrate de que la velocidad no sea menor que cierto límite.
            {
                velocidad = 10;
            }
        }

        private void RestaurarVelocidadInicial()
        {
            velocidad = 800;
        }



        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[filas, columnas];
            Gamegrid.Rows = filas;
            Gamegrid.Columns = columnas;

            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    Image image = new Image
                    {
                        Source = Imagenes.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[i, j] = image;
                    Gamegrid.Children.Add(image);
                }
            }

            return images;

        }

        private void dibujar()
        {
            dibujarCuadricula();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {estadoDeJuego.Puntaje}";
        }

        private void dibujarCuadricula()
        {
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    Cuadricula gridVal = estadoDeJuego.Cuadriculita[i, j];
                    gridImages[i, j].Source = gridValToImage[gridVal];
                    gridImages[i, j].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Posicion headPos = estadoDeJuego.cabezaPosicion();
            Image image = gridImages[headPos.Filas, headPos.Columnas];
            image.Source = Imagenes.Head;

            int rotation = dirToRotation[estadoDeJuego.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Posicion> positions = new List<Posicion>(estadoDeJuego.SnakePosicion());

            for (int i = 0; i < positions.Count; i++)
            {
                Posicion pos = positions[i];
                ImageSource source = (i == 0) ? Imagenes.DeadHead : Imagenes.DeadBody;
                gridImages[pos.Filas, pos.Columnas].Source = source;
                await Task.Delay(100);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 5; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(1000);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Pulsa una tecla para volver a jugar";

            RestaurarVelocidadInicial();

            // Agrega el puntaje al historial
            Puntaje nuevoPuntaje = new Puntaje
            {
                Nickname = "NombreJugador", // Reemplaza esto con el nombre real del jugador
                Puntuacion = estadoDeJuego.Puntaje,
                Fecha = DateTime.Now
            };
            historial.AgregarPuntaje(nuevoPuntaje);

            txtNickname.Visibility = Visibility.Visible;
            (buttonGuardarPuntaje as Button).Visibility = Visibility.Visible;
        }

        private void GuardarPuntaje_Click(object sender, RoutedEventArgs e)
        {
            string nickname = txtNickname.Text;
            if (!string.IsNullOrEmpty(nickname))
            {
                Puntaje nuevoPuntaje = new Puntaje
                {
                    Nickname = nickname,
                    Puntuacion = estadoDeJuego.Puntaje,
                    Fecha = DateTime.Now
                };
                historial.AgregarPuntaje(nuevoPuntaje);

                // Restablecer juego o realizar otras acciones según sea necesario.
                // ...

                // Ocultar la interfaz de ingreso de puntaje.
                txtNickname.Visibility = Visibility.Collapsed;
                (sender as Button).Visibility = Visibility.Collapsed;

                // Mostrar la interfaz de juego nuevamente o realizar otras acciones según sea necesario.
                // ...
            }
        }


    }
}
