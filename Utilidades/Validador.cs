using System.Text.RegularExpressions;

namespace ServiAuto.Utilidades
{
    /// <summary>
    /// Validaciones reutilizables. Cada método devuelve el valor ya limpio
    /// o lanza ArgumentException con un mensaje claro para el usuario.
    /// </summary>
    public static class Validador
    {
        /// <summary>Texto obligatorio. Parámetros: valor y nombre del campo. Retorna el texto sin espacios sobrantes.</summary>
        public static string Texto(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException($"El campo '{campo}' es obligatorio.");
            return valor.Trim();
        }

        /// <summary>Valida formato de correo. Retorna el correo en minúscula.</summary>
        public static string Correo(string valor)
        {
            valor = Texto(valor, "Correo");
            if (!Regex.IsMatch(valor, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("El correo no tiene un formato válido (ej: nombre@dominio.com).");
            return valor.ToLower();
        }

        /// <summary>Valida teléfono de 8 dígitos.</summary>
        public static string Telefono(string valor)
        {
            valor = Texto(valor, "Teléfono");
            if (!Regex.IsMatch(valor, @"^\d{8}$"))
                throw new ArgumentException("El teléfono debe tener exactamente 8 dígitos.");
            return valor;
        }

        /// <summary>Valida identificación: 9 a 12 dígitos (se permiten guiones).</summary>
        public static string Identificacion(string valor)
        {
            valor = Texto(valor, "Identificación");
            if (!Regex.IsMatch(valor, @"^[0-9\-]{9,12}$"))
                throw new ArgumentException("La identificación debe tener de 9 a 12 dígitos (puede incluir guiones).");
            return valor;
        }

        /// <summary>Valida placa: 4 a 8 letras, números o guion. Retorna en mayúscula.</summary>
        public static string Placa(string valor)
        {
            valor = Texto(valor, "Placa");
            if (!Regex.IsMatch(valor, @"^[A-Za-z0-9\-]{4,8}$"))
                throw new ArgumentException("La placa debe tener de 4 a 8 caracteres (letras, números o guion).");
            return valor.ToUpper();
        }

        /// <summary>Valida que un número esté dentro de un rango. Retorna el mismo número.</summary>
        public static int Rango(int valor, int min, int max, string campo)
        {
            if (valor < min || valor > max)
                throw new ArgumentException($"'{campo}' debe estar entre {min} y {max}.");
            return valor;
        }

        /// <summary>Valida que un decimal sea mayor o igual que el mínimo indicado.</summary>
        public static decimal Minimo(decimal valor, decimal min, string campo)
        {
            if (valor < min)
                throw new ArgumentException($"'{campo}' no puede ser menor que {min}.");
            return valor;
        }
    }
}
