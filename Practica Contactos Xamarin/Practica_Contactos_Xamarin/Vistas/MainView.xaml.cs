using Practica_Contactos_Xamarin.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Practica_Contactos_Xamarin.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : ContentPage
    {
        #region Variables

        List<Contacto> contactos = new List<Contacto>(); // Lista de contactos que obtendremos del fichero que el usuario debe indicar
        List<Contacto> contactosMostrar = new List<Contacto>(); // Lista de contactos que obtendremos de la busqueda
        MatchCollection matches; // Necesario para analizar cadenas con matches y expresiones regulares.

        #endregion Variables

        #region Constructor

        public MainView()
        {
            InitializeComponent();

            /// Inicializamos los campos de texto para que no den null.
            txtBusqueda.Text = "";
            txtMaxEdad.Text = "";
            txtMinEdad.Text = "";

            #region Eventos

            /// Accion al pulsar en el btnSeleccionarArchivo
            btnSeleccionarArchivo.Clicked += (sender, args) =>
            {
                /// Limpiamos el array de contactos.
                contactos.Clear();

                /// Si esta en modo XML (switch swtXML activado), cargamos el .XML si no, el .TXT
                if (swtXML.IsToggled)
                {
                    CargarXML();
                }
                else
                {
                    CargarTXT();
                }

                /// Una vez cargado los datos al array contactos, lo vinculamos al listview
                lstContactos.ItemsSource = null;
                lstContactos.ItemsSource = contactos;

                /// Y habilitamos el boton de busqueda
                btnRealizarBusqueda.IsEnabled = true;

            };

            /// Accion al pulsar en el btnRealizarBusqueda
            btnRealizarBusqueda.Clicked += (sender, args) =>
            {
                /// Llamamos al metodo Buscar()
                Buscar();
            };

            /// Accion al pulsar en uno de los elementos del listview
            lstContactos.ItemTapped += (sender, args) =>
            {
                /// Obtenemos el indice de la fila seleccionada
                var index = (lstContactos.ItemsSource as List<Contacto>).IndexOf(lstContactos.SelectedItem as Contacto);

                DetallesView nuevaVentana;

                /// Si la lista obtiene los datos del array contactosMostrar...
                if (contactosMostrar.Count != 0)
                {
                    nuevaVentana = new DetallesView(contactosMostrar[index]);
                }
                /// Si la lista obtiene los datos del array contactos...
                else
                {
                    nuevaVentana = new DetallesView(contactos[index]);
                }

                lstContactos.SelectedItem = null;

                /// Y mostramos la nueva ventana pasando al constructor el contacto seleccionado
                Navigation.PushModalAsync(nuevaVentana);
            };

            #endregion Eventos

        }

        #endregion Contructor

        #region Metodos IO

        /// <summary>
        /// Metodo que lee el archivo XML y va construyendo los contactos y los agrega al array contactos
        /// </summary>
        private void CargarXML()
        {
            var assembly = typeof(Contacto).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("Practica_Contactos_Xamarin.Datos.Info.xml");

            lblPath.Text = "Practica_Contactos_Xamarin.Datos.Info.xml";

            // Forma propia segun la API de Xamarin
            /*using (var reader = new StreamReader(stream))
            {
                var serializer = new XmlSerializer(typeof(List<Contacto>));
                contactos = (List<Contacto>)serializer.Deserialize(reader);
            }*/

            // Alternativa propuesta por David
            StreamReader objReader = new StreamReader(stream);

            var doc = XDocument.Load(stream);

            // Forma 1 de crear la lista
            List<Contacto> contactos1 = new List<Contacto>();
            foreach (XElement element in doc.Root.Elements())
            {
                contactos.Add(new Contacto(element.Element("NOMBRE").Value, element.Element("EDAD").Value, element.Element("DNI").Value));
            }
        }

        /// <summary>
        /// Metodo que lee el archivo TXT y va construyendo los contactos y los agrega al array contactos
        /// </summary>
        private void CargarTXT()
        {
            string nombre, edad, dni;

            var assembly = typeof(Contacto).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("Practica_Contactos_Xamarin.Datos.Info.txt");

            lblPath.Text = "Practica_Contactos_Xamarin.Datos.Info.txt";

            using (var reader = new StreamReader(stream))
            {
                do
                {
                    nombre = reader.ReadLine();
                    edad = reader.ReadLine();
                    dni = reader.ReadLine();
                    if (nombre != null && edad != null && dni != null)
                        contactos.Add(new Contacto(nombre, edad, dni));
                } while (nombre != null && edad != null && dni != null);
            }
        }

        #endregion Metodos IO

        #region Metodos busqueda

        /// <summary>
        /// Busca en el array contactos con los valores indicados, sino se indica un error.
        /// </summary>
        void Buscar()
        {
            /// Si no hay campos vacios
            if (txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0 && txtBusqueda.Text.Trim().Length > 0)
            {
                /// Se hace control numerico
                ControlNumerico();
            }

            else if (txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
            {
                /// Se hace control numerico
                ControlNumerico();
            }
            else if (txtMaxEdad.Text.Length > 0)
            {
                /// Se hace control numerico
                if (int.TryParse(txtMaxEdad.Text.Trim(), out int edad))
                {
                    RealizarBusqueda();
                }
                else
                {
                    txtMaxEdad.Text = "";
                    LanzarAdvertencia("Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.");
                }
            }
            else if (!txtMinEdad.Text.Equals(""))
            {
                /// Se hace control numerico
                if (int.TryParse(txtMinEdad.Text.Trim(), out int edad))
                {
                    RealizarBusqueda();
                }
                else
                {
                    txtMinEdad.Text = "";
                    LanzarAdvertencia("Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.");
                }
            }
            else
            {
                RealizarBusqueda();
            }
        }

        /// <summary>
        /// Controla que los campos numericos sean correctos
        /// </summary>
        void ControlNumerico()
        {
            if (int.TryParse(txtMinEdad.Text.Trim(), out int edadMin) && int.TryParse(txtMaxEdad.Text.Trim(), out int edadMax))
            {
                if (edadMin >= edadMax)
                {
                    ///Mostrar advertencia
                    LanzarAdvertencia("La edad mínima no puede ser mayor o igual a la máxima.");
                }
                else
                {
                    RealizarBusqueda();
                }
            }
            else
            {
                txtMaxEdad.Text = txtMinEdad.Text = "";
                LanzarAdvertencia("Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.");
            }
        }

        /// <summary>
        /// Realiza la busqueda de verdad
        /// </summary>
        void RealizarBusqueda()
        {
            lstContactos.ItemsSource = null;

            /// Limpiamos los contactos cargados para volver a cargar los correctos
            contactosMostrar.Clear();
            /// Se obtiene resultado de la busqueda con los valores introducidos y se carga en listView
            BuscarContacto();
            /// Se rellena listView
            lstContactos.ItemsSource = contactosMostrar;
        }

        /// <summary>
        /// Implementa los filtros para obtener una list de contactos a mostrar
        /// </summary>
        /// <param name="filtro"></param>
        void BuscarContacto()
        {
            /// Recorremos el array contactos y si encontramos una coincidencia la añadimos al arraylist resultado
            for (int i = 0; i < contactos.Count; i++)
            {
                if (ComprobarNombre(contactos[i], txtBusqueda.Text))
                {
                    contactosMostrar.Add(contactos[i]);
                }
            }

            /// Si no se encontro ninguna coincidencia se informa
            if (contactosMostrar.Count == 0)
            {
                LanzarAdvertencia("No se encontro ninguna coincidencia, prueba de nuevo.");
            }
        }

        /// <summary>
        /// Metodo que comprueba si el nombre del contacto tiene alguna coincidencia con el filtro de busqueda.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="filtro">La cadena que queremos usar como filtro. Puede estar vacia. En ese caso, devolveremos siempre true.</param>
        /// <returns> Devuelve true si se ha encontrado coincidencia, devuelve false si no la encuentra.</returns>
        Boolean ComprobarNombre(Contacto contacto, string filtro)
        {
            Boolean ok = false;
            Regex rgx = new Regex("%", RegexOptions.IgnoreCase);

            matches = rgx.Matches(filtro);

            /// Primero tenemos que controlar que en el filtro no hemos introducido mas de un %.
            if (matches.Count > 1)
            {
                txtBusqueda.Text = "";
                LanzarAdvertencia("No se pueden poner mas de 2 veces el caracter '%'");
            }
            else
            {
                /// Si no se ha escrito nada en el patron de busqueda, o solo se ha escrito %...
                if (filtro.Trim().Equals("") || filtro.Trim().Equals("%"))
                {
                    if (txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length == 0)
                    {
                        ok = ComprobarEdad(contacto, txtMinEdad.Text.Trim(), true);
                    }
                    else if (txtMinEdad.Text.Trim().Length == 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = ComprobarEdad(contacto, txtMaxEdad.Text.Trim(), false);
                    }
                    else if (txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = ComprobarEdad(contacto, txtMinEdad.Text.Trim(), txtMaxEdad.Text.Trim());
                    }
                    else
                    {
                        ok = true;
                    }
                }
                /// Si el ultimo caracter es %...
                else if (filtro.Substring(filtro.Length - 1).Equals("%"))
                {
                    /// Quitamos el caracter % para poder usarlo como patron de busqueda.
                    filtro = filtro.Replace("%", "");
                    rgx = new Regex(String.Format("^" + filtro + ".*"), RegexOptions.IgnoreCase);
                    matches = rgx.Matches(contacto.Nombre);
                    /// Si encuentra alguna coincidencia, devolvemos true siempre y cuando la edad tambien coincida (Si se ha usado ese filtro tambien).
                    if (matches.Count > 0 && txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length == 0)
                    {
                        ok = ComprobarEdad(contacto, txtMinEdad.Text.Trim(), true);
                    }
                    else if (matches.Count > 0 && txtMinEdad.Text.Trim().Length == 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = ComprobarEdad(contacto, txtMaxEdad.Text.Trim(), false);
                    }
                    else if (matches.Count > 0 && txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = ComprobarEdad(contacto, txtMinEdad.Text.Trim(), txtMaxEdad.Text.Trim());
                    }
                    else if (matches.Count > 0)
                    {
                        ok = true;
                    }
                }
                /// Si en el patron de busqueda no hemos puesto como ultimo caracter un %...
                else
                {
                    rgx = new Regex("^" + filtro + "$", RegexOptions.IgnoreCase);
                    matches = rgx.Matches(contacto.Nombre);
                    /// Si encuentra alguna coincidencia, devolvemos true siempre y cuando la edad tambien coincida (Si se ha usado ese foltro tambien).
                    if (matches.Count > 0 && txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length == 0)
                    {
                        ok = ComprobarEdad(contacto, txtMinEdad.Text.Trim(), true);
                    }
                    else if (matches.Count > 0 && txtMinEdad.Text.Trim().Length == 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = ComprobarEdad(contacto, txtMaxEdad.Text.Trim(), false);
                    }
                    else if (matches.Count > 0 && txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = ComprobarEdad(contacto, txtMinEdad.Text.Trim(), txtMaxEdad.Text.Trim());
                    }
                    else if (matches.Count > 0)
                    {
                        ok = true;
                    }
                }
            }

            return ok;
        }

        /// <summary>
        /// Metodo que compara la edad introducida en el formulario con la edad de un contacto.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="edad">La edad introducida al formulario para comparar.</param>
        /// <param name="modoMayor">Determina si estamos buscando mayores que la edad introducida o menos que la edad introducida</param>
        /// <returns>Devuelve true si el contacto tiene la edad correcta. Devuelve false si no cumple.</returns>
        Boolean ComprobarEdad(Contacto contacto, string edad, Boolean modoMayor)
        {
            Boolean ok = false;

            if ((Int32.Parse(contacto.Edad) < Int32.Parse(edad) && !modoMayor) || (Int32.Parse(contacto.Edad) >= Int32.Parse(edad) && modoMayor))
            {
                ok = true;
            }

            return ok;
        }

        /// <summary>
        ///  Muestra al usuario información sobre un error que él esta cometiendo
        /// </summary>
        /// <param name="mensaje"></param>
        void LanzarAdvertencia(String mensaje)
        {
            DisplayAlert("ERROR", mensaje, "OK");
        }

        /// <summary>
        /// Metodo que compara la edad introducida en el formulario con la edad de un contacto.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="edadMin">La edad minima introducida al formulario para comparar.</param>
        /// <param name="edadMax">La edad maxima introducida al formulario para comparar.</param>
        /// <returns>Devuelve true si el contacto tiene la edad correcta. Devuelve false si no cumple.</returns>
        Boolean ComprobarEdad(Contacto contacto, string edadMin, string edadMax)
        {
            Boolean ok = false;

            if (Int32.Parse(contacto.Edad) <= Int32.Parse(edadMax) && Int32.Parse(contacto.Edad) >= Int32.Parse(edadMin))
            {
                ok = true;
            }

            return ok;
        }

        #endregion Metodos busqueda

    }
}