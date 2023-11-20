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


        // Definimos un diccionario para mapear los valores de la enumeración 'Cuadricula' a las correspondientes imágenes.
        private readonly Dictionary<Cuadricula, ImageSource> gridValToImage = new()
        {
            {Cuadricula.Empty, Imagenes.Empty },
            {Cuadricula.Snake, Imagenes.Body },
            {Cuadricula.Food, Imagenes.Food }
        };


        // Mapea las direcciones a los ángulos de rotación en grados.
        private readonly Dictionary<Direccion, int> rotacionImagenes = new()
        {
            {Direccion.arriba, 0 },
            {Direccion.abajo, 180 },
            {Direccion.derecha, 90 },
            {Direccion.izquierda, 270 }
        };

        private readonly int filas = 15, columnas = 15;                                 // Cantidad de filas y columnas del juego
        private readonly Image[,] gridImages;
        private EstadoDeJuego estadoDeJuego;
        private bool ejecutandoJuego;
        private int velocidad = 350;                                                    // Velocidad inicial de la culbera
        private bool aumentoDeVelocidadRealizado = false;                               // Determina si la velocidad aumentó

        public MainWindow()
        {
            InitializeComponent();
            gridImages = cuadriculaDelJuego();
            estadoDeJuego = new EstadoDeJuego(filas, columnas);
        }


        // Se encarga de iniciar y ejecutar el juego, bajo los parámetros establecidos por mi
        private async Task ejecutarJuego()
        {
            dibujar();
            await mostrarCuentaRegresiva();
            Overlay.Visibility = Visibility.Hidden;
            await cicloDeJuego();
            await interfazJuegoTerminado();
            estadoDeJuego = new EstadoDeJuego(filas, columnas);
        }


        // Maneja la entrada de teclado para iniciar el juego, para este juego, lo establecimos para que inicie o reinicie cuando se presione la tecla 'espacio'
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

        // Establecemos que las flechas sean las teclas para cambiar de dirección en el juego.
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (estadoDeJuego.GameOver)                                                 // Si el juego ha terminado, no porcesar más entradas de teclado.
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


        // Funcion que determina cuando el juego no haya terminado.
        private async Task cicloDeJuego()
        {
            while (!estadoDeJuego.GameOver)
            {
                await Task.Delay(velocidad);
                estadoDeJuego.movimiento();
                dibujar();

                // Si el puntaje es múltiplo de 3 la velocidad aumenta
                if (estadoDeJuego.Puntaje != 0 && estadoDeJuego.Puntaje % 3 == 0 && !aumentoDeVelocidadRealizado)
                {
                    AumentarVelocidad();                                    // Aumenta la velocidad
                    aumentoDeVelocidadRealizado = true;                     // Marca que la velocidad ha aumentado para evitar que aumente infinitamente
                }

                if (estadoDeJuego.Puntaje % 3 != 0)
                {
                    aumentoDeVelocidadRealizado = false;
                }
            }
        }

        // Método para aumentar la velocidad de la culebra
        private void AumentarVelocidad()
        {
            velocidad -= 25; // Ajusta el valor de decremento de la velocidad.
            if (velocidad < 10) // Asegúrate de que la velocidad no sea menor que cierto límite.
            {
                velocidad = 10;
            }
        }

        // Método para reiniciar la velocidad
        private void RestaurarVelocidadInicial()
        {
            velocidad = 350;
        }

        // Realizamos la configuración inicial de la cuadrícula visual del juego.
        private Image[,] cuadriculaDelJuego()
        {
            Image[,] images = new Image[filas, columnas];                       // Inicializa una matriz de imágenes que representa la cuadrícula del juego.
            Gamegrid.Rows = filas;                                              // Configura el número de filas y columnas en la cuadrícula.
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

        // Actualiza la representación visual del juego.
        private void dibujar()
        {
            dibujarCuadricula();                                            // Dibuja la cuadrícula.
            dibujarCabezaCulebra();                                         // Dibuja la cabeza de la serpiente en la cuadrícula correspondiente.
            ScoreText.Text = $"SCORE {estadoDeJuego.Puntaje}";              // Acumula el puntaje del jugador.
        }

        //  Este método se encarga de actualizar visualmente la cuadrícula del juego en la interfaz de usuario
        private void dibujarCuadricula()
        {
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    Cuadricula gridVal = estadoDeJuego.Cuadriculita[i, j];              // Obtiene el valor de la cuadrícula en la posición actual
                    gridImages[i, j].Source = gridValToImage[gridVal];                  // Asignación de la imagen correspondiente
                    gridImages[i, j].RenderTransform = Transform.Identity;              // Restablece la transformación de la imagen
                }
            }
        }

        // Se encarga de actualizar visualmente la cabeza de la serpiente en la interfaz del juego.
        private void dibujarCabezaCulebra()
        {
            Posicion posicionCabeza = estadoDeJuego.cabezaPosicion();
            Image image = gridImages[posicionCabeza.Filas, posicionCabeza.Columnas];
            image.Source = Imagenes.Head;

            int rotation = rotacionImagenes[estadoDeJuego.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }


        // Se encarga de actualizar la cabeza y el cuerpo cuando la serpiente ha muerto en el juego.
        private async Task dibujarCabezaMuerta()
        {
            List<Posicion> posiciones = new List<Posicion>(estadoDeJuego.SnakePosicion());

            for (int i = 0; i < posiciones.Count; i++)
            {
                Posicion pos = posiciones[i];
                ImageSource source = (i == 0) ? Imagenes.DeadHead : Imagenes.DeadBody;
                gridImages[pos.Filas, pos.Columnas].Source = source;
                await Task.Delay(100);
            }
        }

        // Funcion para mostrar una cuenta regresiva antes de comenzar una partida
        private async Task mostrarCuentaRegresiva()
        {
            for (int i = 5; i >= 1; i--)                                 // El valor de i determina cuantas iteraciones demorará el ciclo.
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);                                  // El valor dentro de 'Delay' determina los milisegundo que demora cada iteración.                   
            }
        }

        private async Task interfazJuegoTerminado()
        {
            await dibujarCabezaMuerta();
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
