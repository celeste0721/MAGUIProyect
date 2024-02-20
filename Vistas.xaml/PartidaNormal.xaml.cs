using Newtonsoft.Json;
using MaguiProyecto.Entidades;
using System.Text;

namespace MaguiProyecto;

public partial class PartidaNormal : ContentPage
{
    public string CorreoElectronico { get; set; }
    string laURL = "https://localhost:44319/api/proyecto/ingresarDatosH";
    public List<ObtenerPreguntasAPI> listaDePreguntasAPI;
    private string ApiUrl = "https://localhost:44319/api/ObtenerPreguntasApi/ObtenerPreguntasAPI";
    private int preguntasCorrectas = 0;
    private int preguntasIncorrectas = 0;
    private int preguntasRealizadas = 0;

    public PartidaNormal(string correoElectronico)
	{
		InitializeComponent();
        CorreoElectronico = correoElectronico;
        BindingContext = this;
        _ = CargarPreguntasYRespuestasAsync();
    }

    private async void OnRespuestaClicked(object sender, EventArgs e)
    {
        Button clickedButton = (Button)sender;
        string selectedAnswer = clickedButton.Text;

        // lista de preguntas está obteniendo el primer elemento de la lista, es decir, el objeto que está en la posición cero de la lista

        bool esCorrecta = selectedAnswer == listaDePreguntasAPI[0].correctAnswer;

        if (esCorrecta)
        {
            clickedButton.BackgroundColor = Color.FromRgb(0, 255, 0); 
            preguntasCorrectas++;
            txtEnunciado.BackgroundColor = Color.FromRgb(0, 255, 0); 
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
           
            else if (btnAnswer2.Text == listaDePreguntasAPI[0].correctAnswer)
            {
               
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


            // Verifica si se han completado las 15 preguntas después de procesar la respuesta actual
            if (preguntasRealizadas >= 15)
            {
                await DisplayAlert("Juego Finalizado", "¡Completaste las 15 preguntas!", "Aceptar");
                await MostrarEstadisticas();
                ReiniciarEstadisticas();
                await Navigation.PushAsync(new Finalizar(CorreoElectronico));
                return;
            }
        }

        
        await CargarPreguntasYRespuestasAsync();
        RestablecerColoresBotones();
    }

    private async Task EnviarEstadisticasALaAPI(Dato estadisticas)
    {
        try
        {
            // Crea una instancia de la clase ReqIngresarDato y la inicializa con las estadísticas y una sesión.
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
            }
            else
            {
                // Manejar errores de conexión o del backend
                await DisplayAlert("Error", "Hubo un error al enviar los datos", "Aceptar");

            }
        }
        catch (Exception)
        {
            // Manejar errores internos no controlados
            await DisplayAlert("Error interno", "Hubo un error interno al enviar los datos", "Aceptar");
        }

    }

    private async Task CargarPreguntasYRespuestasAsync()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    listaDePreguntasAPI = JsonConvert.DeserializeObject<List<ObtenerPreguntasAPI>>(content);
                    MostrarPreguntaEnUI(listaDePreguntasAPI[0]);
                    preguntasRealizadas++;
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
        MainThread.BeginInvokeOnMainThread(() =>
        {
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
            respuestas = respuestas.OrderBy(x => rand.Next()).ToList();// Aleatoriza el orden de las respuestas.


            // Asigna las respuestas a los botones
            btnAnswer1.Text = respuestas[0];
            btnAnswer2.Text = respuestas[1];
            btnAnswer3.Text = respuestas[2];
        });
    }

    private async Task MostrarEstadisticas()
    {
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


}


