namespace MaguiProyecto
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        //La utilización de métodos asíncronos suele estar relacionada con operaciones que pueden llevar tiempo

        private async void OnRespuestaJugar(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SeleccionarSesion());
        }

        private async void OnRespuestaRanked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MostrarRanked());
        }
    }
}
