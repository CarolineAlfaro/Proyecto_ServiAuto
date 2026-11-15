using System.Collections;
using System.Text;
using ServiAuto.Interfaces;
using ServiAuto.Utilidades;

namespace ServiAuto.Entidades
{
    /// <summary>Línea de repuesto dentro de una orden.</summary>
    public class DetalleRepuesto
    {
        public Repuesto Repuesto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }   // se guarda el precio del momento
        public decimal Subtotal => Cantidad * PrecioUnitario;

        public DetalleRepuesto(Repuesto repuesto, int cantidad)
        {
            Repuesto = repuesto;
            Cantidad = cantidad;
            PrecioUnitario = repuesto.PrecioVenta;
        }
    }

    /// <summary>Orden de servicio: une vehículo, cliente, servicio, mecánicos y repuestos.</summary>
    public class OrdenServicio : IEntidad
    {
        public const decimal TasaImpuesto = 0.13m;   // impuesto de ventas 13 %

        private string _diagnostico = string.Empty;

        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public Vehiculo Vehiculo { get; set; }
        public Cliente Cliente { get; set; }
        public TipoServicio TipoServicio { get; set; }
        public string Diagnostico { get { return _diagnostico; } set { _diagnostico = Validador.Texto(value, "Diagnóstico"); } }
        public EstadoOrden Estado { get; private set; } = EstadoOrden.Recibida;

        public List<Mecanico> Mecanicos { get; } = new List<Mecanico>();
        public List<DetalleRepuesto> Repuestos { get; } = new List<DetalleRepuesto>();

        /// <summary>ArrayList (colección NO genérica): bitácora de eventos de la orden.</summary>
        public ArrayList Historial { get; } = new ArrayList();

        public bool EstaAbierta => Estado != EstadoOrden.Terminada && Estado != EstadoOrden.Entregada;

        public OrdenServicio() { }

        public OrdenServicio(Vehiculo vehiculo, Cliente cliente, TipoServicio tipo, string diagnostico)
        {
            Vehiculo = vehiculo ?? throw new ArgumentNullException(nameof(vehiculo));
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            TipoServicio = tipo ?? throw new ArgumentNullException(nameof(tipo));
            Diagnostico = diagnostico;
            Registrar("Orden recibida");
        }

        /// <summary>Agrega un texto con fecha a la bitácora.</summary>
        public void Registrar(string evento) => Historial.Add($"{DateTime.Now:dd/MM/yyyy HH:mm} - {evento}");

        /// <summary>
        /// Despacha un repuesto a la orden: rebaja inventario (lanza excepción si no alcanza)
        /// y acumula la cantidad en el detalle.
        /// </summary>
        public void AgregarRepuesto(Repuesto repuesto, int cantidad)
        {
            if (!EstaAbierta) throw new InvalidOperationException("La orden ya está cerrada; no admite repuestos.");
            repuesto.Rebajar(cantidad);   // si falla, no se agrega nada
            var linea = Repuestos.FirstOrDefault(d => d.Repuesto.Id == repuesto.Id);
            if (linea != null) linea.Cantidad += cantidad;
            else Repuestos.Add(new DetalleRepuesto(repuesto, cantidad));
            Registrar($"Repuesto despachado: {repuesto.Nombre} x{cantidad}");
        }

        /// <summary>Cambia el estado respetando reglas básicas.</summary>
        public void CambiarEstado(EstadoOrden nuevo)
        {
            if (Estado == EstadoOrden.Entregada) throw new InvalidOperationException("La orden ya fue entregada.");
            if (nuevo == EstadoOrden.Entregada && Estado != EstadoOrden.Terminada)
                throw new InvalidOperationException("Solo se puede entregar una orden que esté Terminada.");
            if (nuevo == EstadoOrden.Terminada && Mecanicos.Count == 0)
                throw new InvalidOperationException("No se puede terminar una orden sin mecánico asignado.");
            Estado = nuevo;
            Registrar($"Estado cambiado a {nuevo}");
        }

        // ---- Cálculos (propiedades de solo lectura, se calculan al pedirlas) ----
        public decimal ManoObra => TipoServicio?.TarifaManoObra ?? 0m;
        public decimal TotalRepuestos => Repuestos.Sum(r => r.Subtotal);
        public decimal Subtotal => ManoObra + TotalRepuestos;
        public decimal Descuento => Subtotal * (Cliente?.PorcentajeDescuento ?? 0m) / 100m;
        public decimal Impuesto => (Subtotal - Descuento) * TasaImpuesto;
        public decimal Total => Subtotal - Descuento + Impuesto;

        /// <summary>Genera el texto de la factura de la orden.</summary>
        public string GenerarFactura()
        {
            var sb = new StringBuilder();
            sb.AppendLine("================ FACTURA SERVIAUTO ================");
            sb.AppendLine($"Orden #{Id}   Fecha: {Fecha:dd/MM/yyyy}   Estado: {Estado}");
            sb.AppendLine($"Cliente : {Cliente.Nombre} ({Cliente.TipoCliente})");
            sb.AppendLine($"Vehículo: {Vehiculo.Placa} {Vehiculo.Marca} {Vehiculo.Modelo}");
            sb.AppendLine($"Mecánicos: {(Mecanicos.Count == 0 ? "(sin asignar)" : string.Join(", ", Mecanicos.Select(m => m.Nombre)))}");
            sb.AppendLine($"Diagnóstico: {Diagnostico}");
            sb.AppendLine("---------------------------------------------------");
            sb.AppendLine($"Mano de obra ({TipoServicio.Nombre}): ₡{ManoObra:N2}");
            foreach (var d in Repuestos)
                sb.AppendLine($"  {d.Repuesto.Nombre} x{d.Cantidad} @ ₡{d.PrecioUnitario:N2} = ₡{d.Subtotal:N2}");
            sb.AppendLine("---------------------------------------------------");
            sb.AppendLine($"Subtotal           : ₡{Subtotal:N2}");
            sb.AppendLine($"Descuento ({Cliente.PorcentajeDescuento}%)   : -₡{Descuento:N2}");
            sb.AppendLine($"Impuesto ventas 13%: ₡{Impuesto:N2}");
            sb.AppendLine($"TOTAL              : ₡{Total:N2}");
            sb.AppendLine("===================================================");
            return sb.ToString();
        }

        public string ObtenerDescripcion()
            => $"[{Id}] {Fecha:dd/MM/yyyy} | {Vehiculo.Placa} | {Cliente.Nombre} | {TipoServicio.Nombre} | {Estado} | Total ₡{Total:N2}";

        public override string ToString() => ObtenerDescripcion();
    }
}
