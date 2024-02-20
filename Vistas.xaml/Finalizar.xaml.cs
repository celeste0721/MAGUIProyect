
using static MaguiProyecto.Finalizar;

namespace MaguiProyecto;

public partial class Finalizar : ContentPage
{
    public string CorreoElectronico { get; set; }
    public Finalizar(string correoElectronico)
	{
		InitializeComponent();
        CorreoElectronico = correoElectronico;
        BindingContext = this;
    }

    private async void OnRespuestaInicio(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private async void OnRespuestaAgain(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ModalidadJuego(CorreoElectronico));
    }

    private async void OnRanked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MostrarRanked());
    }


}


 