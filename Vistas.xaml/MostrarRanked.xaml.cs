using MaguiProyecto.Entidades;
using Newtonsoft.Json;


namespace MaguiProyecto
{
    public partial class MostrarRanked : ContentPage
    {
        string ApiUrl = "https://localhost:44319/api/publicacion/obtenerRanked";

        public MostrarRanked()
        {
            InitializeComponent();
            OnAppearing(); // Llama a OnAppearing directamente para cargar datos al inicio
        }

        //método que forma parte del ciclo de vida de las páginas y se llama cuando la página está a punto de aparecer en la pantalla del dispositivo.
        protected override async void OnAppearing()
        {
            // Llamamos al método base de la clase para realizar las operaciones predeterminadas al aparecer la página.
            base.OnAppearing();

            try
            {
                // Utilizamos un bloque 'using' para asegurarnos de que el recurso HttpClient se libere correctamente.
                using (HttpClient client = new HttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync(ApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        ResObtenerDatosH datosResponse = JsonConvert.DeserializeObject<ResObtenerDatosH>(responseContent);

                        // Invocamos en el hilo principal para actualizar la interfaz de usuario.
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            // Llamamos al método para mostrar los datos en la interfaz de usuario
                            MostrarDatosEnInterfaz(datosResponse?.listaDeRanked);
                        });
                    }
                    else
                    {
                        // Invocamos en el hilo principal para actualizar la interfaz de usuario.
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            // Mostramos un mensaje en el caso de que la solicitud no sea exitosa.
                            lblPropiedad1.Text = response.IsSuccessStatusCode ? "No se encontraron datos." : $"Error en la respuesta: {response.ReasonPhrase}";
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error:", "El sistema respondió :"  + ex.Message, "Aceptar" );
        
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
                // Manejar el error de manera apropiada
            }
        }

        private void MostrarDatosEnInterfaz(List<ObtenerDatosH> datos)
        {
            // Limpiamos las etiquetas existentes
            lblPropiedad1.Text = string.Empty;

            // Verificamos si se proporcionaron datos y si hay elementos en la lista.
            if (datos != null && datos.Count > 0)
            {
                // Iteramos a través de cada elemento en la lista de datos.
                foreach (var dato in datos)
                {
                    // Concatenamos información de cada elemento y la agregamos al contenido de la etiqueta.
                    lblPropiedad1.Text += $"\nNombreUsuario: {dato.NombreUsuario}\nPuntuacion: {dato.Puntuacion}\n\n";
                }
            }
            else
            {
                // Manejar el caso en que no hay datos
                lblPropiedad1.Text = "No se encontraron datos.";
            }
        }
    }
}
