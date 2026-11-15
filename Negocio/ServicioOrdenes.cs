using ServiAuto.Entidades;

namespace ServiAuto.Negocio
{
    /// <summary>Lógica de negocio de las órdenes (reglas que involucran varias entidades).</summary>
    public class ServicioOrdenes
    {
        /// <summary>Máximo de órdenes abiertas que puede tener un mecánico a la vez.</summary>
        public const int MaxOrdenesAbiertas = 3;

        private readonly Almacen _almacen;

        public ServicioOrdenes(Almacen almacen) { _almacen = almacen; }

        /// <summary>Cuenta las órdenes abiertas de un mecánico.</summary>
        public int OrdenesAbiertasDe(Mecanico mecanico)
            => _almacen.Ordenes.Buscar(o => o.EstaAbierta && o.Mecanicos.Any(m => m.Id == mecanico.Id)).Count;

        /// <summary>Asigna un mecánico a la orden validando su disponibilidad y que no esté repetido.</summary>
        public void AsignarMecanico(OrdenServicio orden, Mecanico mecanico)
        {
            if (orden.Mecanicos.Any(m => m.Id == mecanico.Id))
                throw new InvalidOperationException("Ese mecánico ya está asignado a la orden.");
            if (OrdenesAbiertasDe(mecanico) >= MaxOrdenesAbiertas)
                throw new InvalidOperationException(
                    $"{mecanico.Nombre} no está disponible: ya tiene {MaxOrdenesAbiertas} órdenes abiertas.");
            orden.Mecanicos.Add(mecanico);
            orden.Registrar($"Mecánico asignado: {mecanico.Nombre}");
        }

        /// <summary>Abre una orden nueva con su primer mecánico y la guarda en el repositorio.</summary>
        public OrdenServicio AbrirOrden(Vehiculo vehiculo, Cliente cliente, TipoServicio tipo,
                                        string diagnostico, Mecanico mecanico)
        {
            var orden = new OrdenServicio(vehiculo, cliente, tipo, diagnostico);
            AsignarMecanico(orden, mecanico);   // si falla, la orden NO se guarda
            _almacen.Ordenes.Agregar(orden);
            return orden;
        }
    }
}
