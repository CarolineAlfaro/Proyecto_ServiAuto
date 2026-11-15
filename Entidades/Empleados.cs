using ServiAuto.Utilidades;

namespace ServiAuto.Entidades
{
    /// <summary>NIVEL 2: Empleado hereda de Persona.</summary>
    public class Empleado : Persona
    {
        public string Sucursal { get; set; } = "Central";
        public DateTime FechaIngreso { get; set; } = DateTime.Today;

        public Empleado() { }
        public Empleado(string identificacion, string nombre) : base(identificacion, nombre) { }
        public Empleado(string identificacion, string nombre, string correo, string telefono, string sucursal)
            : base(identificacion, nombre, correo, telefono)
        {
            Sucursal = sucursal;
        }

        /// <summary>Sobrescritura (override) del método abstracto de Persona.</summary>
        public override string ObtenerDescripcion()
            => $"[{Id}] {Nombre} ({Identificacion}) - Empleado, sucursal {Sucursal}";
    }

    /// <summary>NIVEL 3: Mecánico hereda de Empleado (y por tanto de Persona).</summary>
    public class Mecanico : Empleado
    {
        private string _especialidad = "Mecánica general";

        public string Especialidad
        {
            get { return _especialidad; }
            set { _especialidad = Validador.Texto(value, "Especialidad"); }
        }

        public Mecanico() { }
        public Mecanico(string identificacion, string nombre) : base(identificacion, nombre) { }
        public Mecanico(string identificacion, string nombre, string correo, string telefono,
                        string sucursal, string especialidad)
            : base(identificacion, nombre, correo, telefono, sucursal)
        {
            Especialidad = especialidad;
        }

        public override string ObtenerDescripcion()
            => $"[{Id}] Mecánico {Nombre} ({Identificacion}) - {Especialidad} - {Sucursal}";
    }

    /// <summary>NIVEL 3: Asesor de servicio.</summary>
    public class AsesorServicio : Empleado
    {
        public AsesorServicio() { }
        public AsesorServicio(string identificacion, string nombre, string correo, string telefono, string sucursal)
            : base(identificacion, nombre, correo, telefono, sucursal) { }

        public override string ObtenerDescripcion()
            => $"[{Id}] Asesor {Nombre} ({Identificacion}) - {Sucursal}";
    }

    /// <summary>NIVEL 3: Personal administrativo.</summary>
    public class Administrador : Empleado
    {
        public Administrador() { }
        public Administrador(string identificacion, string nombre, string correo, string telefono, string sucursal)
            : base(identificacion, nombre, correo, telefono, sucursal) { }

        public override string ObtenerDescripcion()
            => $"[{Id}] Administrador {Nombre} ({Identificacion}) - {Sucursal}";
    }
}
