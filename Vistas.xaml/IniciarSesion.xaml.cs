using MaguiProyecto.Entidades;
using Newtonsoft.Json;
using System.Text;

namespace MaguiProyecto;

public partial class IniciarSesion : ContentPage
{

    string laURL = "https://localhost:44319/api/proyecto/iniciarSesion";


    public IniciarSesion()
	{
		InitializeComponent();

    }

    private async void OnRespuestaIniciarSesion(object sender, EventArgs e)
    {
        await EnviarSesion();

    }

    private async Task EnviarSesion() {

        try
        {

            ReqIniciarSesion req = new ReqIniciarSesion();
            req.lasesion= new iniciarsesion();

            req.lasesion.CorreoElectronico=txtCorreo.Text;
            req.lasesion.HashContrasena =txtContrasena.Text;
            req.session = " ";

            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpClient httpClient = new HttpClient();

            var response = await httpClient.PostAsync(laURL, jsonContent);


            if (response.IsSuccessStatusCode)
            {
                ResIniciarSesion res = new ResIniciarSesion();

                var responseContent = await response.Content.ReadAsStringAsync();

                res = JsonConvert.DeserializeObject<ResIniciarSesion>(responseContent);

                if (res.resultado)
                {
                    bool userAccepted = await DisplayAlert("Inicio de Sesión", "Inicio de Sesión exitoso, ¡Bienvenido! ", "Aceptar", "Regresar");

                    if (userAccepted)
                    {
                        // Si el usuario presiona "Aceptar", navegamos a la página ModalidadJuego
                        await Navigation.PushAsync(new ModalidadJuego(txtCorreo.Text));
                    }
                    // Si el usuario presiona "Cancelar", no hacemos nada especial
                }
                else
                {
                    await DisplayAlert("Error:", "El sistema respondió : " + res.errorMensaje, "Aceptar");
                }
            }
            else
            {
                await DisplayAlert("Error de conexión:", "No se pudo establecer conexión", "Aceptar");
            }


        }
        catch (Exception )
        {

            await DisplayAlert("Error interno no controlado:", "Error en la aplicacion", "Aceptar");


        }




    }






}





