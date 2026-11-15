using ServiAuto.Interfaces;
using ServiAuto.Utilidades;

namespace ServiAuto.Entidades
{
    /// <summary>
    /// NIVEL 1 de la herencia. Clase ABSTRACTA: no se puede hacer "new Persona()".
    /// Contiene los datos comunes de clientes y empleados.
    /// </summary>
    public abstract class Persona : IEntidad
    {
        // Campos privados = ENCAPSULAMIENTO: nadie de afuera los toca directamente.
        private string _identificacion = string.Empty;
        private string _nombre = string.Empty;
        private string _correo = string.Empty;
        private string _telefono = string.Empty;

        public int Id { get; set; }   // propiedad automática

        // Propiedades con get/set. El set valida antes de guardar.
        public string Identificacion
        {
            get { return _identificacion; }
            set { _identificacion = Validador.Identificacion(value); }
        }

        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = Validador.Texto(value, "Nombre"); }
        }

        public string Correo
        {
            get { return _correo; }
            set { _correo = Validador.Correo(value); }
        }

        public string Telefono
        {
            get { return _telefono; }
            set { _telefono = Validador.Telefono(value); }
        }

        // CONSTRUCTORES SOBRECARGADOS (mismo nombre, distintos parámetros).
        /// <summary>Constructor vacío.</summary>
        protected Persona() { }

        /// <summary>Constructor con datos mínimos: identificación y nombre.</summary>
        protected Persona(string identificacion, string nombre)
        {
            Identificacion = identificacion;
            Nombre = nombre;
        }

        /// <summary>Constructor completo. Reutiliza el anterior con ": this(...)".</summary>
        protected Persona(string identificacion, string nombre, string correo, string telefono)
            : this(identificacion, nombre)
        {
            Correo = correo;
            Telefono = telefono;
        }

        /// <summary>MÉTODO ABSTRACTO: las clases hijas están obligadas a sobrescribirlo.</summary>
        public abstract string ObtenerDescripcion();

        public override string ToString() => ObtenerDescripcion();
    }
}
