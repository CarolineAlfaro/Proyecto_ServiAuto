using ServiAuto.Interfaces;

namespace ServiAuto.Negocio
{
    /// <summary>
    /// CLASE GENÉRICA. "T" es un tipo que se define al usarla: Repositorio&lt;Cliente&gt;, Repositorio&lt;Repuesto&gt;...
    /// La restricción "where T : class, IEntidad" garantiza que T tenga Id y ObtenerDescripcion().
    /// Guarda los datos en una List&lt;T&gt; en memoria (en la entrega final se cambiará por Dapper + SQL Server).
    /// </summary>
    public class Repositorio<T> where T : class, IEntidad
    {
        private readonly List<T> _items = new List<T>();
        private int _siguienteId = 1;

        /// <summary>Cantidad de elementos guardados.</summary>
        public int Cantidad => _items.Count;

        /// <summary>CREAR: asigna el Id automático y agrega el elemento.</summary>
        public void Agregar(T entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));
            entidad.Id = _siguienteId++;
            _items.Add(entidad);
        }

        /// <summary>LISTAR: devuelve una copia de la lista (así nadie la altera desde afuera).</summary>
        public List<T> ObtenerTodos() => new List<T>(_items);

        /// <summary>BUSCAR por Id. Retorna null si no existe.</summary>
        public T BuscarPorId(int id) => _items.FirstOrDefault(x => x.Id == id);

        /// <summary>BUSCAR con condición (Func = método que recibe T y devuelve bool).</summary>
        public List<T> Buscar(Func<T, bool> condicion) => _items.Where(condicion).ToList();

        /// <summary>MODIFICAR: reemplaza el elemento que tenga el mismo Id. Retorna false si no existe.</summary>
        public bool Modificar(T entidad)
        {
            int pos = _items.FindIndex(x => x.Id == entidad.Id);
            if (pos < 0) return false;
            _items[pos] = entidad;
            return true;
        }

        /// <summary>ELIMINAR por Id. Retorna false si no existe.</summary>
        public bool Eliminar(int id)
        {
            T encontrado = BuscarPorId(id);
            return encontrado != null && _items.Remove(encontrado);
        }
    }
}
