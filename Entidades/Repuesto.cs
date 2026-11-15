using ServiAuto.Interfaces;
using ServiAuto.Utilidades;

namespace ServiAuto.Entidades
{
    /// <summary>Repuesto del inventario.</summary>
    public class Repuesto : IEntidad
    {
        private string _codigo = string.Empty;
        private string _nombre = string.Empty;
        private decimal _costo;
        private decimal _precioVenta;
        private int _existenciaMinima;

        public int Id { get; set; }
        public string Codigo { get { return _codigo; } set { _codigo = Validador.Texto(value, "Código").ToUpper(); } }
        public string Nombre { get { return _nombre; } set { _nombre = Validador.Texto(value, "Nombre"); } }
        public decimal Costo { get { return _costo; } set { _costo = Validador.Minimo(value, 0, "Costo"); } }
        public decimal PrecioVenta { get { return _precioVenta; } set { _precioVenta = Validador.Minimo(value, 0, "Precio de venta"); } }
        public int ExistenciaMinima { get { return _existenciaMinima; } set { _existenciaMinima = Validador.Rango(value, 0, 100000, "Existencia mínima"); } }

        // private set: la existencia SOLO cambia con Ingresar() o Rebajar().
        public int Existencia { get; private set; }

        /// <summary>True cuando la existencia llegó al mínimo (alerta de reorden).</summary>
        public bool BajoMinimo => Existencia <= ExistenciaMinima;

        public Repuesto() { }

        public Repuesto(string codigo, string nombre, int existencia, int existenciaMinima, decimal costo, decimal precioVenta)
        {
            Codigo = codigo; Nombre = nombre; ExistenciaMinima = existenciaMinima;
            Costo = costo; PrecioVenta = precioVenta;
            Existencia = Validador.Rango(existencia, 0, 100000, "Existencia");
        }

        /// <summary>Entrada de inventario. Parámetro: cantidad (>0). Sin retorno.</summary>
        public void Ingresar(int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor que cero.");
            Existencia += cantidad;
        }

        /// <summary>Salida de inventario. Lanza excepción si no hay existencias suficientes.</summary>
        public void Rebajar(int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor que cero.");
            if (cantidad > Existencia)
                throw new InvalidOperationException(
                    $"Existencias insuficientes de '{Nombre}': solicitado {cantidad}, disponible {Existencia}.");
            Existencia -= cantidad;
        }

        public string ObtenerDescripcion()
            => $"[{Id}] {Codigo} - {Nombre} | Exist: {Existencia} (mín {ExistenciaMinima}) | Precio ₡{PrecioVenta:N2}"
               + (BajoMinimo ? "  ⚠ REORDEN" : "");

        public override string ToString() => ObtenerDescripcion();
    }
}
