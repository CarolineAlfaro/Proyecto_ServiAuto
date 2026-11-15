namespace ServiAuto.Entidades
{
    /// <summary>Roles de acceso del sistema.</summary>
    public enum Rol { Administrador, Asesor, Mecanico }

    /// <summary>Estados posibles de una orden de servicio.</summary>
    public enum EstadoOrden { Recibida, EnProceso, EnEsperaRepuesto, Terminada, Entregada }
}
