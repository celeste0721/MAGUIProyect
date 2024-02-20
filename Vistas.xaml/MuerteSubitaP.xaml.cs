using Newtonsoft.Json;
using MaguiProyecto.Entidades;
using System.Text;

namespace MaguiProyecto;

public partial class MuerteSubitaP : ContentPage

{
    string laURL = "https://localhost:44319/api/proyecto/ingresarDatosH";
    public string CorreoElectronico { get; set; }
    public List<ObtenerPreguntasAPI> listaDePreguntasAPI;
    private  string ApiUrl = "https://localhost:44319/api/ObtenerPreguntasApi/ObtenerPreguntasAPI";
    private int preguntasCorrectas = 0;
    private int preguntasIncorrectas = 0;

    private  int TiempoLimite = 10; // 10 segundos
    private bool tiempoCorriendo = false;
    private bool tiempoAgotado = false;
    private bool preguntaRespondida = false;
    private bool mensajeTiempoAgotadoMostrado = false;

    private CancellationTokenSource tokenSource;


    [Obsolete]
    public MuerteSubitaP(string correoElectronico)
	{
        InitializeComponent();
        CorreoElectronico = correoElectronico;
        BindingContext = this;
        _ = CargarPreguntasYRespuestasAsync();
        IniciarTemporizador();
    }

    [Obsolete]
    private async void OnRespuestaClicked(object sender, EventArgs e)
    {
        tiempoAgotado = false;
        Button clickedButton = (Button)sender;
        string selectedAnswer = clickedButton.Text;
        bool esCorrecta = selectedAnswer == listaDePreguntasAPI[0].correctAnswer;

        DetenerTemporizador();

        if (esCorrecta)
        {
            clickedButton.BackgroundColor = Color.FromRgb(0, 255, 0); 
            preguntasCorrectas++;
            txtEnunciado.BackgroundColor = Color.FromRgb(0, 255, 0); 
            await DisplayAlert("¡Correcto!", "¡Has seleccionado la respuesta correcta!", "Aceptar");

            await CargarPreguntasYRespuestasAsync();
            RestablecerColoresBotones();
            IniciarTemporizador();
        }
        else
        {
            clickedButton.BackgroundColor = Color.FromRgb(255, 0, 0); 
            preguntasIncorrectas++;
            txtEnunciado.BackgroundColor = Color.FromRgb(255, 0, 0);


            // Se declara una variable 'correctButton' y se inicializa como null.
            Button correctButton = null;
            // Se verifica si el texto del primer botón de respuesta (btnAnswer1) es igual a la respuesta correcta de la primera pregunta en la lista.
            if (btnAnswer1.Text == listaDePreguntasAPI[0].correctAnswer)
            {
                // Si es igual, se asigna el botón actual a 'correctButton'.
                correctButton = btnAnswer1;
            }
            // Si no es igual, se verifica lo mismo para el segundo botón de respuesta (btnAnswer2).
            else if (btnAnswer2.Text == listaDePreguntasAPI[0].correctAnswer)
            {
                // Si es igual, se asigna el botón actual a 'correctButton'.
                correctButton = btnAnswer2;
            }
            else if (btnAnswer3.Text == listaDePreguntasAPI[0].correctAnswer)
            {
                correctButton = btnAnswer3;
            }

            // Se verifica si 'correctButton' no es null, es decir, si se encontró un botón con la respuesta correcta.
            if (correctButton != null)
            {
                // Si se encontró, se cambia el color de fondo del botón a verde.
                correctButton.BackgroundColor = Color.FromRgb(0, 255, 0);
            }

            await DisplayAlert("Incorrecto", "La respuesta seleccionada es incorrecta.", "Aceptar");


            tiempoAgotado = true;
            preguntaRespondida = true;

            if (!mensajeTiempoAgotadoMostrado)
            {
                await MostrarEstadisticas();
                ReiniciarEstadisticas();
                mensajeTiempoAgotadoMostrado = true;
            }
        }

        RestablecerColoresBotones();

        if (tiempoAgotado || preguntaRespondida)
            return; // Evitar procesar respuestas si el tiempo ya se agotó o la pregunta ya fue respondida

        tiempoAgotado = true;
        preguntaRespondida = true;

        if (tiempoCorriendo)
        {
            // Detener el temporizador antes de cargar la siguiente pregunta
            DetenerTemporizador();
            await CargarPreguntasYRespuestasAsync();
            IniciarTemporizador();
            mensajeTiempoAgotadoMostrado = false; // Restablecer el indicador
        }
    }


    private async Task EnviarEstadisticasALaAPI(Dato estadisticas)
    {
        DetenerTemporizador(); // Detener el temporizador si ya está en ejecución
        try
        {
            // Crear el objeto de solicitud con las estadísticas
            ReqIngresarDato req = new ReqIngresarDato
            {
                elDato = estadisticas,
                session = "1234"
            };

            // Convertir el objeto a JSON y preparar el contenido para la solicitud
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            // Crear una instancia de HttpClient y realizar la solicitud POST
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.PostAsync(laURL, jsonContent);

            // Procesar la respuesta
            if (response.IsSuccessStatusCode)
            {
                // Manejar la respuesta exitosa
                await DisplayAlert("Actualizado", "¡Ranked Actualizado,gracias por jugar!", "Aceptar");
                await Navigation.PushAsync(new Finalizar(CorreoElectronico));
            }
            else
            {
                // Manejar errores de conexión o del backend
                await DisplayAlert("Error", "Hubo un error al enviar los datos", "Aceptar");
        
            }
        }
        catch (Exception )
        {
            // Manejar errores internos no controlados
            await DisplayAlert("Error interno", "Hubo un error interno al enviar los datos", "Aceptar");

        }




    }


    //Metodos para la jugabilidad

    private async Task CargarPreguntasYRespuestasAsync()
    {
        try
        {
           
            DetenerTemporizador(); // Detener el temporizador si ya está en ejecución

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    listaDePreguntasAPI = JsonConvert.DeserializeObject<List<ObtenerPreguntasAPI>>(content);
                    MostrarPreguntaEnUI(listaDePreguntasAPI[0]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar datos: {ex.Message}");
        }
    }

    private void MostrarPreguntaEnUI(ObtenerPreguntasAPI nuevaPregunta)
    {
        // Restablecer el temporizador cada vez que se carga una nueva pregunta
        DetenerTemporizador(); // Detener el temporizador si ya está en ejecución
        IniciarTemporizador(); // Iniciar el temporizador para la nueva pregunta

        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Habilitar los botones
            btnAnswer1.IsEnabled = true;
            btnAnswer2.IsEnabled = true;
            btnAnswer3.IsEnabled = true;

            txtEnunciado.Text = nuevaPregunta.question;

            // Lista de respuestas con la correcta e incorrectas
            List<string> respuestas = new List<string>
        {
            nuevaPregunta.correctAnswer,
            nuevaPregunta.incorrectAnswers[0],
            nuevaPregunta.incorrectAnswers[1]
        };

            // Aleatoriza el orden de las respuestas
            Random rand = new Random();
            respuestas = respuestas.OrderBy(x => rand.Next()).ToList();

            // Asigna las respuestas a los botones
            btnAnswer1.Text = respuestas[0];
            btnAnswer2.Text = respuestas[1];
            btnAnswer3.Text = respuestas[2];
        });
    }

    private async Task MostrarEstadisticas()
    {
        DetenerTemporizador(); // Detener el temporizador si ya está en ejecución
        int puntajeTotal = preguntasCorrectas * 100; // Asigna 100 puntos por cada pregunta correcta

        // Crear un objeto Dato con las estadísticas
        Dato estadisticas = new Dato
        {
            CorreoElectronico = CorreoElectronico,
            preguntasCorrectas = preguntasCorrectas,
            preguntasIncorrectas = preguntasIncorrectas,
            Puntuacion = puntajeTotal
        };

        // Mostrar estadísticas en el DisplayAlert
        await DisplayAlert("Estadísticas", $"Correo electrónico: {CorreoElectronico}\nPreguntas correctas: {preguntasCorrectas}\nPreguntas incorrectas: {preguntasIncorrectas}\nPuntaje total: {puntajeTotal}", "Aceptar");
        // Enviar estadísticas a la API
        await EnviarEstadisticasALaAPI(estadisticas);
    }

    private void ReiniciarEstadisticas()
    {

        preguntasIncorrectas = 0;
        preguntasCorrectas = 0;
        RestablecerColoresBotones();
    }
    private void RestablecerColoresBotones()
    {
        txtEnunciado.BackgroundColor = Color.FromRgb(173, 216, 230); // Restablece el color de fondo del texto a su valor predeterminado
        btnAnswer1.BackgroundColor = Color.FromRgb(173, 216, 230); ;
        btnAnswer2.BackgroundColor = Color.FromRgb(173, 216, 230);
        btnAnswer3.BackgroundColor = Color.FromRgb(173, 216, 230);
    }

    [Obsolete]
    private async void IniciarTemporizador()
    {
        tiempoCorriendo = true;
        tokenSource = new CancellationTokenSource();

        // Ejecutar el temporizador de forma asíncrona
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(TiempoLimite), tokenSource.Token);
                if (!tokenSource.Token.IsCancellationRequested)
                {
                    // Tiempo agotado
                    tiempoAgotado = true;
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await DisplayAlert("Tiempo agotado", "Se ha agotado el tiempo para responder.", "Aceptar");
                        await MostrarEstadisticas();
                        ReiniciarEstadisticas();
                    });
                }
            }
            catch (TaskCanceledException)
            {
                // La excepción se lanza si se cancela la tarea, lo cual es esperado si se responde antes de que el temporizador se agote
            }
        });
    }

    private void DetenerTemporizador()
    {
        tiempoCorriendo = false;
        tokenSource?.Cancel();
    }

}







