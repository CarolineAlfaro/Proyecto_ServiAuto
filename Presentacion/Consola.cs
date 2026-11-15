using System.Globalization;

namespace ServiAuto.Presentacion
{
    /// <summary>
    /// Utilidades de entrada/salida. Todas las lecturas se repiten hasta que el
    /// usuario escriba un dato válido, así el programa nunca se cae por una entrada incorrecta.
    /// </summary>
    public static class Consola
    {
        /// <summary>Lee texto y lo valida con la función recibida; repite si lanza ArgumentException.</summary>
        public static string Leer(string mensaje, Func<string, string> validar)
        {
            while (true)
            {
                Console.Write(mensaje);
                try { return validar(Console.ReadLine()); }
                catch (ArgumentException ex) { Console.WriteLine("  ✗ " + ex.Message); }
            }
        }

        /// <summary>Lectura opcional: Enter devuelve null (sin cambio). Si es inválido, avisa y devuelve null.</summary>
        public static string LeerOpcional(string mensaje, Func<string, string> validar)
        {
            Console.Write(mensaje + " [Enter = omitir]: ");
            string entrada = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entrada)) return null;
            try { return validar(entrada); }
            catch (ArgumentException ex) { Console.WriteLine("  ✗ " + ex.Message + " (se omite el cambio)"); return null; }
        }

        /// <summary>Lee texto libre sin validar (para filtros de búsqueda). Nunca devuelve null.</summary>
        public static string LeerLibre(string mensaje)
        {
            Console.Write(mensaje + " [Enter = omitir]: ");
            return (Console.ReadLine() ?? string.Empty).Trim();
        }

        /// <summary>Lee un entero dentro de [min, max]. TryParse evita excepciones por letras.</summary>
        public static int LeerEntero(string mensaje, int min, int max)
        {
            while (true)
            {
                Console.Write(mensaje);
                if (int.TryParse(Console.ReadLine(), out int valor) && valor >= min && valor <= max) return valor;
                Console.WriteLine($"  ✗ Digite un número entero entre {min} y {max}.");
            }
        }

        /// <summary>Lee un decimal dentro de [min, max]. Acepta punto o coma como separador.</summary>
        public static decimal LeerDecimal(string mensaje, decimal min, decimal max)
        {
            while (true)
            {
                Console.Write(mensaje);
                string texto = (Console.ReadLine() ?? "").Replace(',', '.');
                if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valor)
                    && valor >= min && valor <= max) return valor;
                Console.WriteLine($"  ✗ Digite un número entre {min} y {max}.");
            }
        }

        /// <summary>Lee una fecha en formato dd/MM/yyyy.</summary>
        public static DateTime LeerFecha(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje + " (dd/MM/yyyy): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out DateTime f)) return f;
                Console.WriteLine("  ✗ Fecha inválida. Ejemplo: 25/10/2026");
            }
        }

        /// <summary>Ejecuta una acción dentro de try/catch: si algo falla, muestra el mensaje y la app sigue.</summary>
        public static void Ejecutar(Action accion)
        {
            try { accion(); }
            catch (Exception ex) { Console.WriteLine("  ✗ " + ex.Message); }
        }

        public static void Titulo(string texto)
        {
            Console.WriteLine();
            Console.WriteLine("══════════════════════════════════════════");
            Console.WriteLine("  " + texto);
            Console.WriteLine("══════════════════════════════════════════");
        }

        public static void Pausa()
        {
            Console.Write("\nPresione Enter para continuar...");
            Console.ReadLine();
        }
    }
}
