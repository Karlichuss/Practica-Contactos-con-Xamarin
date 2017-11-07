namespace Practica_Contactos_Xamarin.Modelos
{
    public class Contacto
    {
        #region Atributos

        private string NOMBRE, EDAD, DNI;

        #endregion Atributos

        #region Constructores

        public Contacto(string NOMBRE, string EDAD, string DNI)
        {
            this.NOMBRE = NOMBRE;
            this.EDAD = EDAD;
            this.DNI = DNI;
        }

        public Contacto()
        {

        }

        #endregion Constructores

        #region Getters y setters

        public string Nombre
        {
            get
            {
                return NOMBRE;
            }
        }

        public string Edad
        {
            get
            {
                return EDAD;
            }
        }

        public string Dni
        {
            get
            {
                return DNI;
            }
        }

        #endregion Getters y setters

    }
}
