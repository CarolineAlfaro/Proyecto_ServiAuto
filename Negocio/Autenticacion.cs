using ServiAuto.Entidades;

namespace ServiAuto.Negocio
{
    /// <summary>Autenticación simulada: compara contra la lista de usuarios en memoria.</summary>
    public static class Autenticacion
    {
        /// <summary>Retorna el Usuario si las credenciales son correctas; si no, null.</summary>
        public static Usuario Validar(List<Usuario> usuarios, string nombre, string clave)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(clave)) return null;
            return usuarios.FirstOrDefault(u =>
                u.NombreUsuario.Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase) && u.Clave == clave);
        }
    }
}
