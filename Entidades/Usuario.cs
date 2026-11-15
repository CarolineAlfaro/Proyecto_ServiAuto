namespace ServiAuto.Entidades
{
    /// <summary>Usuario del sistema (autenticación simulada en memoria).</summary>
    public class Usuario
    {
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public Rol Rol { get; set; }
        public string NombreCompleto { get; set; }

        public Usuario(string nombreUsuario, string clave, Rol rol, string nombreCompleto)
        {
            NombreUsuario = nombreUsuario; Clave = clave; Rol = rol; NombreCompleto = nombreCompleto;
        }
    }
}
