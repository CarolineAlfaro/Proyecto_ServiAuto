using ServiAuto.Interfaces;
using ServiAuto.Utilidades;

namespace ServiAuto.Entidades
{
    /// <summary>Vehículo de un cliente.</summary>
    public class Vehiculo : IEntidad
    {
        private string _placa = string.Empty;
        private string _marca = string.Empty;
        private string _modelo = string.Empty;
        private int _anio;
        private int _kilometraje;

        public int Id { get; set; }
        public int IdCliente { get; set; }   // a qué cliente pertenece

        public string Placa { get { return _placa; } set { _placa = Validador.Placa(value); } }
        public string Marca { get { return _marca; } set { _marca = Validador.Texto(value, "Marca"); } }
        public string Modelo { get { return _modelo; } set { _modelo = Validador.Texto(value, "Modelo"); } }

        public int Anio
        {
            get { return _anio; }
            set { _anio = Validador.Rango(value, 1950, DateTime.Now.Year + 1, "Año"); }
        }

        public int Kilometraje
        {
            get { return _kilometraje; }
            set { _kilometraje = Validador.Rango(value, 0, 2_000_000, "Kilometraje"); }
        }

        public Vehiculo() { }

        public Vehiculo(string placa, string marca, string modelo, int anio, int kilometraje, int idCliente)
        {
            Placa = placa; Marca = marca; Modelo = modelo;
            Anio = anio; Kilometraje = kilometraje; IdCliente = idCliente;
        }

        public string ObtenerDescripcion()
            => $"[{Id}] {Placa} - {Marca} {Modelo} {Anio} - {Kilometraje:N0} km (Cliente #{IdCliente})";

        public override string ToString() => ObtenerDescripcion();
    }
}
