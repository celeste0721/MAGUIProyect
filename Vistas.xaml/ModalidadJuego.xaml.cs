using System.ComponentModel;

namespace MaguiProyecto;

public partial class ModalidadJuego : ContentPage
{
    public string CorreoElectronico { get; set; }


    public ModalidadJuego(string correoElectronico)
	{
		InitializeComponent();
        CorreoElectronico = correoElectronico;
        BindingContext = this;
    }

    [Obsolete]
    private  void OnRespuestaMuerteSubita(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MuerteSubitaP(CorreoElectronico));

    }

    private  void OnRespuestaPartidaNormal(object sender, EventArgs e)
    {
        Navigation.PushAsync(new PartidaNormal(CorreoElectronico));

    }


}