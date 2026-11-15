namespace ServiAuto.Entidades
{
    /// <summary>NIVEL 2: Cliente hereda de Persona.</summary>
    public class Cliente : Persona
    {
        public Cliente() { }
        public Cliente(string identificacion, string nombre) : base(identificacion, nombre) { }
        public Cliente(string identificacion, string nombre, string correo, string telefono)
            : base(identificacion, nombre, correo, telefono) { }

        /// <summary>Descuento (%) que se aplica a la factura. Virtual: los hijos pueden cambiarlo.</summary>
        public virtual decimal PorcentajeDescuento => 0m;

        public virtual string TipoCliente => "Cliente";

        public override string ObtenerDescripcion()
            => $"[{Id}] {TipoCliente}: {Nombre} ({Identificacion}) - {Correo} - Tel {Telefono}";
    }

    /// <summary>NIVEL 3: cliente particular (persona física).</summary>
    public class ClienteParticular : Cliente
    {
        public ClienteParticular() { }
        public ClienteParticular(string identificacion, string nombre, string correo, string telefono)
            : base(identificacion, nombre, correo, telefono) { }

        public override string TipoCliente => "Particular";
    }

    /// <summary>NIVEL 3: empresa con contrato de mantenimiento (flotilla). Tiene descuento.</summary>
    public class ClienteEmpresa : Cliente
    {
        private decimal _descuentoContrato;

        public decimal DescuentoContrato
        {
            get { return _descuentoContrato; }
            set
            {
                if (value < 0 || value > 30)
                    throw new ArgumentException("El descuento de contrato debe estar entre 0 y 30 %.");
                _descuentoContrato = value;
            }
        }

        public ClienteEmpresa() { }
        public ClienteEmpresa(string identificacion, string nombre, string correo, string telefono, decimal descuento)
            : base(identificacion, nombre, correo, telefono)
        {
            DescuentoContrato = descuento;
        }

        public override decimal PorcentajeDescuento => DescuentoContrato;
        public override string TipoCliente => "Empresa";
    }
}
