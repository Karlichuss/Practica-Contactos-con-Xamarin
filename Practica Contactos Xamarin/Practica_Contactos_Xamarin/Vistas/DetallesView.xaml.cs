using Practica_Contactos_Xamarin.Modelos;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Practica_Contactos_Xamarin.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetallesView : ContentPage
    {
        #region Constructor

        public DetallesView(Contacto contacto)
        {
            InitializeComponent();

            /// Simplemente mostramos en el lblDetalles los datos del contacto pasado por constructor.
            lblDetalles.Text = String.Format("Nombre: {0}\nEdad: {1}\nDNI: {2}", contacto.Nombre, contacto.Edad, contacto.Dni);

            /// Cuando pulsamos en btnCerrar, volvemos al MainView con los datos recuperados.
            btnCerrar.Clicked += (sender, args) =>
            {
                OnBackButtonPressed();
            };
        }

        #endregion Contructor
    }
}