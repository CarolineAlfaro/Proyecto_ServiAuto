namespace ServiAuto.Interfaces
{
    /// <summary>
    /// Interfaz propia del sistema. Toda entidad que se guarde en un Repositorio
    /// debe tener un Id y saber describirse a sí misma.
    /// </summary>
    public interface IEntidad
    {
        /// <summary>Identificador único asignado por el repositorio.</summary>
        int Id { get; set; }

        /// <summary>Devuelve una línea de texto que resume la entidad.</summary>
        string ObtenerDescripcion();
    }
}
