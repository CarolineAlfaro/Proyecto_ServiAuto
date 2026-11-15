using ServiAuto.Entidades;

namespace ServiAuto.Negocio
{
    /// <summary>Contenedor de todos los repositorios (la "base de datos en memoria").</summary>
    public class Almacen
    {
        public Repositorio<Cliente> Clientes { get; } = new Repositorio<Cliente>();
        public Repositorio<Vehiculo> Vehiculos { get; } = new Repositorio<Vehiculo>();
        public Repositorio<Empleado> Empleados { get; } = new Repositorio<Empleado>();
        public Repositorio<Repuesto> Repuestos { get; } = new Repositorio<Repuesto>();
        public Repositorio<TipoServicio> Servicios { get; } = new Repositorio<TipoServicio>();
        public Repositorio<OrdenServicio> Ordenes { get; } = new Repositorio<OrdenServicio>();

        /// <summary>Lista simple de usuarios para la autenticación simulada.</summary>
        public List<Usuario> Usuarios { get; } = new List<Usuario>();

        /// <summary>Devuelve solo los empleados que son mecánicos (polimorfismo con OfType).</summary>
        public List<Mecanico> ObtenerMecanicos() => Empleados.ObtenerTodos().OfType<Mecanico>().ToList();
    }
}
