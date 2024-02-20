using Newtonsoft.Json;
using MaguiProyecto.Entidades;
using System.Text;

namespace MaguiProyecto;

public partial class CrearUsuario : ContentPage
{

    string laURL = "https://localhost:44319/api/proyecto/ingresarUsuario";

    public CrearUsuario()
	{
		InitializeComponent();
	}

    private async void OnRespuestaCrearUsuario(object sender, EventArgs e)
    {
        await CreacionUsuario();

    }


    private async Task CreacionUsuario() {

        try
        {
            ReqIngresarUsuario req = new ReqIngresarUsuario();
            req.elUsuario= new ingresarUsuario();

            req.elUsuario.Nombre=txtNombre.Text;
            req.elUsuario.Apellidos = txtApellidos.Text;
            req.elUsuario.NombreUsuario = txtNombreUsuario.Text;
            req.elUsuario.CorreoElectronico = txtCorreo.Text;
            req.elUsuario.HashContrasena = txtContrasena.Text;
            req.session = "1234";


            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpClient httpClient = new HttpClient();

            var response = await httpClient.PostAsync(laURL, jsonContent);


            if (response.IsSuccessStatusCode)
            {
                ResIngresarUsuario res = new ResIngresarUsuario();

                var responseContent = await response.Content.ReadAsStringAsync();

                res = JsonConvert.DeserializeObject<ResIngresarUsuario>(responseContent);

                if (res.resultado)
                {
                    bool userAccepted = await DisplayAlert("Crear Usuario", "Creación de Usuario exitoso, ¡Bienvenido! ", "Aceptar", "Regresar");

                    if (userAccepted)
                    {
 
                        await Navigation.PushAsync(new ModalidadJuego(txtCorreo.Text));
                    }
       
                }
                else
                {
                    await DisplayAlert("Error:", "Sistema respondió: " + res.errorMensaje, "Aceptar");
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