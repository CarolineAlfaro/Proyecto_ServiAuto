using ServiAuto.Interfaces;
using ServiAuto.Utilidades;

namespace ServiAuto.Entidades
{
    /// <summary>Tipo de servicio con su tarifa de mano de obra.</summary>
    public class TipoServicio : IEntidad
    {
        private string _nombre = string.Empty;
        private decimal _tarifa;

        public int Id { get; set; }
        public string Nombre { get { return _nombre; } set { _nombre = Validador.Texto(value, "Nombre del servicio"); } }
        public decimal TarifaManoObra { get { return _tarifa; } set { _tarifa = Validador.Minimo(value, 0, "Tarifa"); } }

        public TipoServicio() { }
        public TipoServicio(string nombre, decimal tarifa) { Nombre = nombre; TarifaManoObra = tarifa; }

        public string ObtenerDescripcion() => $"[{Id}] {Nombre} - Mano de obra ₡{TarifaManoObra:N2}";
        public override string ToString() => ObtenerDescripcion();
    }
}
