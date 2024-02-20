namespace MaguiProyecto;

public partial class SeleccionarSesion : ContentPage
{
	public SeleccionarSesion()
	{
		InitializeComponent();
	}

    private async void OnRespuestaCrearUsuario(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearUsuario());

    }

    private async void OnRespuestaIniciarSesion(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new IniciarSesion());
    }

}