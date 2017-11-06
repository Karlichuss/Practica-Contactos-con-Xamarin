namespace Practica_Contactos_Xamarin.Modelos
{
    public class Contacto
    {
        /*DECLARACION DE VARIABLES*/
        private string NOMBRE, EDAD, DNI;

        /*CONSTRUCTORES*/
        public Contacto(string NOMBRE, string EDAD, string DNI)
        {
            this.NOMBRE = NOMBRE;
            this.EDAD = EDAD;
            this.DNI = DNI;
        }

        public Contacto()
        {

        }

        /*GETTERS Y SETTERS*/
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

    }
}
