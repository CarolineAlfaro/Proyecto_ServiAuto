using ServiAuto.Entidades;
using ServiAuto.Negocio;
using ServiAuto.Utilidades;

namespace ServiAuto.Presentacion
{
    /// <summary>Menús de consola: login, menú principal por rol y submenús de cada módulo.</summary>
    public class Menu
    {
        private readonly Almacen _a;
        private readonly ServicioOrdenes _srv;
        private Usuario _sesion;

        public Menu(Almacen almacen, ServicioOrdenes servicio) { _a = almacen; _srv = servicio; }

        // ------------------------------------------------------------ LOGIN
        /// <summary>Pide credenciales (3 intentos) y abre el menú principal.</summary>
        public void Iniciar()
        {
            Consola.Titulo("SERVIAUTO - Gestión de Taller (avance consola)");
            Console.WriteLine("Usuarios de prueba: admin/admin123 | asesor/asesor123 | mecanico/mec123");
            for (int intento = 1; intento <= 3; intento++)
            {
                Console.Write("Usuario: ");
                string usuario = Console.ReadLine();
                Console.Write("Clave: ");
                string clave = Console.ReadLine();
                _sesion = Autenticacion.Validar(_a.Usuarios, usuario, clave);
                if (_sesion != null)
                {
                    Console.WriteLine($"\nBienvenido/a {_sesion.NombreCompleto} (rol: {_sesion.Rol})");
                    MenuPrincipal();
                    Console.WriteLine("\nSesión cerrada. ¡Hasta pronto!");
                    return;
                }
                Console.WriteLine($"  ✗ Credenciales incorrectas ({intento}/3).");
            }
            Console.WriteLine("Demasiados intentos fallidos. El programa se cierra.");
        }

        private bool EsRol(params Rol[] roles) => roles.Contains(_sesion.Rol);

        // ------------------------------------------------------ MENÚ GENÉRICO
        /// <summary>
        /// Muestra una lista numerada de opciones, lee la elegida y ejecuta su acción.
        /// Se reutiliza en TODOS los menús para no repetir código.
        /// </summary>
        private void Submenu(string titulo, List<(string Texto, Action Accion)> opciones,
                             string textoSalida = "Volver", bool pausar = true)
        {
            while (true)
            {
                Consola.Titulo(titulo);
                for (int i = 0; i < opciones.Count; i++)
                    Console.WriteLine($"  {i + 1}. {opciones[i].Texto}");
                Console.WriteLine($"  0. {textoSalida}");
                int op = Consola.LeerEntero("Opción: ", 0, opciones.Count);
                if (op == 0) return;
                Consola.Ejecutar(opciones[op - 1].Accion);
                if (pausar) Consola.Pausa();
            }
        }

        /// <summary>Menú principal: las opciones visibles dependen del rol.</summary>
        private void MenuPrincipal()
        {
            var ops = new List<(string Texto, Action Accion)>();
            if (EsRol(Rol.Administrador, Rol.Asesor)) ops.Add(("Clientes", () => MenuClientes()));
            if (EsRol(Rol.Administrador, Rol.Asesor)) ops.Add(("Vehículos", () => MenuVehiculos()));
            if (EsRol(Rol.Administrador)) ops.Add(("Repuestos / Inventario", () => MenuRepuestos()));
            if (EsRol(Rol.Administrador)) ops.Add(("Personal (mecánicos)", () => MenuMecanicos()));
            ops.Add(("Órdenes de servicio", () => MenuOrdenes()));
            ops.Add(("Reportes y consultas", () => MenuReportes()));
            ops.Add(("Ayuda", () => Ayuda()));
            ops.Add(("Quiénes somos / Acerca de", () => QuienesSomos()));
            Submenu($"MENÚ PRINCIPAL - {_sesion.Rol}", ops, "Salir", false);
        }

        // --------------------------------------------------------- AYUDA
        private void Ayuda()
        {
            Consola.Titulo("AYUDA");
            Console.WriteLine("- Escriba el número de la opción y presione Enter.");
            Console.WriteLine("- En las búsquedas, deje un filtro vacío (Enter) para no usarlo.");
            Console.WriteLine("- Administrador: acceso total. Asesor: clientes, vehículos y órdenes.");
            Console.WriteLine("- Mecánico: consulta órdenes, cambia su estado y ve reportes.");
        }

        private void QuienesSomos()
        {
            Consola.Titulo("QUIÉNES SOMOS / ACERCA DE");
            Console.WriteLine("Grupo de desarrollo: [Nombre 1], [Nombre 2], [Nombre 3]   <-- complete aquí");
            Console.WriteLine("Sistema ServiAuto - Versión 0.1 (avance en consola)");
        }

        // ======================================================== CLIENTES
        private void MenuClientes()
        {
            Submenu("CLIENTES", new List<(string, Action)>
            {
                ("Registrar cliente particular", () => RegistrarCliente(false)),
                ("Registrar cliente empresa",    () => RegistrarCliente(true)),
                ("Listar clientes",              ListarClientes),
                ("Buscar clientes",              BuscarClientes),
                ("Modificar cliente",            ModificarCliente),
                ("Eliminar cliente",             EliminarCliente),
            });
        }

        private void RegistrarCliente(bool esEmpresa)
        {
            string ident = Consola.Leer("Identificación: ", Validador.Identificacion);
            if (_a.Clientes.Buscar(c => c.Identificacion == ident).Any())
                throw new InvalidOperationException("Ya existe un cliente con esa identificación.");
            string nombre = Consola.Leer("Nombre: ", v => Validador.Texto(v, "Nombre"));
            string correo = Consola.Leer("Correo: ", Validador.Correo);
            string tel = Consola.Leer("Teléfono (8 dígitos): ", Validador.Telefono);

            Cliente nuevo;   // variable de tipo padre: puede guardar cualquiera de los hijos
            if (esEmpresa)
            {
                decimal desc = Consola.LeerDecimal("Descuento por contrato (0 a 30 %): ", 0, 30);
                nuevo = new ClienteEmpresa(ident, nombre, correo, tel, desc);
            }
            else
            {
                nuevo = new ClienteParticular(ident, nombre, correo, tel);
            }
            _a.Clientes.Agregar(nuevo);
            Console.WriteLine("  ✓ Cliente registrado: " + nuevo.ObtenerDescripcion());
        }

        private void ListarClientes()
        {
            foreach (var c in _a.Clientes.ObtenerTodos()) Console.WriteLine(c.ObtenerDescripcion());
        }

        private void BuscarClientes()
        {
            string texto = Consola.LeerLibre("Nombre o identificación contiene");
            var lista = _a.Clientes.Buscar(c =>
                c.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) || c.Identificacion.Contains(texto));
            MostrarLista(lista.Select(c => c.ObtenerDescripcion()));
        }

        private Cliente PedirCliente()
        {
            int id = Consola.LeerEntero("ID del cliente: ", 1, int.MaxValue);
            return _a.Clientes.BuscarPorId(id) ?? throw new KeyNotFoundException("No existe un cliente con ese ID.");
        }

        private void ModificarCliente()
        {
            Cliente c = PedirCliente();
            Console.WriteLine("Actual: " + c.ObtenerDescripcion());
            string nombre = Consola.LeerOpcional("Nuevo nombre", v => Validador.Texto(v, "Nombre"));
            string correo = Consola.LeerOpcional("Nuevo correo", Validador.Correo);
            string tel = Consola.LeerOpcional("Nuevo teléfono", Validador.Telefono);
            if (nombre != null) c.Nombre = nombre;   // se usa el set de la propiedad
            if (correo != null) c.Correo = correo;
            if (tel != null) c.Telefono = tel;
            _a.Clientes.Modificar(c);
            Console.WriteLine("  ✓ Cliente actualizado.");
        }

        private void EliminarCliente()
        {
            Cliente c = PedirCliente();
            if (_a.Vehiculos.Buscar(v => v.IdCliente == c.Id).Any())
                throw new InvalidOperationException("No se puede eliminar: el cliente tiene vehículos registrados.");
            _a.Clientes.Eliminar(c.Id);
            Console.WriteLine("  ✓ Cliente eliminado.");
        }

        // ======================================================= VEHÍCULOS
        private void MenuVehiculos()
        {
            Submenu("VEHÍCULOS", new List<(string, Action)>
            {
                ("Registrar vehículo",              RegistrarVehiculo),
                ("Listar vehículos",                ListarVehiculos),
                ("Buscar (placa, marca, año, cliente)", BuscarVehiculos),
                ("Modificar vehículo",              ModificarVehiculo),
                ("Eliminar vehículo",               EliminarVehiculo),
            });
        }

        private void RegistrarVehiculo()
        {
            Cliente dueno = PedirCliente();
            string placa = Consola.Leer("Placa: ", Validador.Placa);
            if (_a.Vehiculos.Buscar(v => v.Placa == placa).Any())
                throw new InvalidOperationException("Ya existe un vehículo con esa placa.");
            string marca = Consola.Leer("Marca: ", v => Validador.Texto(v, "Marca"));
            string modelo = Consola.Leer("Modelo: ", v => Validador.Texto(v, "Modelo"));
            int anio = Consola.LeerEntero("Año: ", 1950, DateTime.Now.Year + 1);
            int km = Consola.LeerEntero("Kilometraje: ", 0, 2_000_000);
            var veh = new Vehiculo(placa, marca, modelo, anio, km, dueno.Id);
            _a.Vehiculos.Agregar(veh);
            Console.WriteLine("  ✓ Vehículo registrado: " + veh.ObtenerDescripcion());
        }

        private void ListarVehiculos()
        {
            foreach (var v in _a.Vehiculos.ObtenerTodos()) Console.WriteLine(v.ObtenerDescripcion());
        }

        private void BuscarVehiculos()
        {
            string placa = Consola.LeerLibre("Placa contiene").ToUpper();
            string marca = Consola.LeerLibre("Marca contiene");
            string anio = Consola.LeerLibre("Año exacto");
            string cliente = Consola.LeerLibre("ID de cliente");
            // Los filtros se combinan con &&; el filtro vacío siempre cumple.
            var lista = _a.Vehiculos.Buscar(v =>
                (placa == "" || v.Placa.Contains(placa)) &&
                (marca == "" || v.Marca.Contains(marca, StringComparison.OrdinalIgnoreCase)) &&
                (anio == "" || v.Anio.ToString() == anio) &&
                (cliente == "" || v.IdCliente.ToString() == cliente));
            MostrarLista(lista.Select(v => v.ObtenerDescripcion()));
        }

        private Vehiculo PedirVehiculo()
        {
            int id = Consola.LeerEntero("ID del vehículo: ", 1, int.MaxValue);
            return _a.Vehiculos.BuscarPorId(id) ?? throw new KeyNotFoundException("No existe un vehículo con ese ID.");
        }

        private void ModificarVehiculo()
        {
            Vehiculo v = PedirVehiculo();
            Console.WriteLine("Actual: " + v.ObtenerDescripcion());
            string marca = Consola.LeerOpcional("Nueva marca", x => Validador.Texto(x, "Marca"));
            string modelo = Consola.LeerOpcional("Nuevo modelo", x => Validador.Texto(x, "Modelo"));
            string km = Consola.LeerOpcional("Nuevo kilometraje", x =>
            {
                if (!int.TryParse(x, out int n)) throw new ArgumentException("El kilometraje debe ser un número entero.");
                Validador.Rango(n, 0, 2_000_000, "Kilometraje");
                return x;
            });
            if (marca != null) v.Marca = marca;
            if (modelo != null) v.Modelo = modelo;
            if (km != null) v.Kilometraje = int.Parse(km);
            _a.Vehiculos.Modificar(v);
            Console.WriteLine("  ✓ Vehículo actualizado.");
        }

        private void EliminarVehiculo()
        {
            Vehiculo v = PedirVehiculo();
            if (_a.Ordenes.Buscar(o => o.Vehiculo.Id == v.Id).Any())
                throw new InvalidOperationException("No se puede eliminar: el vehículo tiene órdenes de servicio.");
            _a.Vehiculos.Eliminar(v.Id);
            Console.WriteLine("  ✓ Vehículo eliminado.");
        }

        // ======================================================= REPUESTOS
        private void MenuRepuestos()
        {
            Submenu("REPUESTOS E INVENTARIO", new List<(string, Action)>
            {
                ("Registrar repuesto",               RegistrarRepuesto),
                ("Listar repuestos",                 ListarRepuestos),
                ("Buscar repuesto por nombre/código", BuscarRepuestos),
                ("Entrada de inventario",            EntradaInventario),
                ("Modificar repuesto",               ModificarRepuesto),
                ("Eliminar repuesto",                EliminarRepuesto),
            });
        }

        private void RegistrarRepuesto()
        {
            string codigo = Consola.Leer("Código: ", v => Validador.Texto(v, "Código"));
            if (_a.Repuestos.Buscar(r => r.Codigo == codigo.ToUpper()).Any())
                throw new InvalidOperationException("Ya existe un repuesto con ese código.");
            string nombre = Consola.Leer("Nombre: ", v => Validador.Texto(v, "Nombre"));
            int existencia = Consola.LeerEntero("Existencia inicial: ", 0, 100000);
            int minima = Consola.LeerEntero("Existencia mínima: ", 0, 100000);
            decimal costo = Consola.LeerDecimal("Costo: ", 0, 100_000_000);
            decimal precio = Consola.LeerDecimal("Precio de venta: ", 0, 100_000_000);
            var r = new Repuesto(codigo, nombre, existencia, minima, costo, precio);
            _a.Repuestos.Agregar(r);
            Console.WriteLine("  ✓ Repuesto registrado: " + r.ObtenerDescripcion());
        }

        private void ListarRepuestos()
        {
            foreach (var r in _a.Repuestos.ObtenerTodos()) Console.WriteLine(r.ObtenerDescripcion());
        }

        private void BuscarRepuestos()
        {
            string t = Consola.LeerLibre("Nombre o código contiene");
            var lista = _a.Repuestos.Buscar(r =>
                r.Nombre.Contains(t, StringComparison.OrdinalIgnoreCase) || r.Codigo.Contains(t, StringComparison.OrdinalIgnoreCase));
            MostrarLista(lista.Select(r => r.ObtenerDescripcion()));
        }

        private Repuesto PedirRepuesto()
        {
            int id = Consola.LeerEntero("ID del repuesto: ", 1, int.MaxValue);
            return _a.Repuestos.BuscarPorId(id) ?? throw new KeyNotFoundException("No existe un repuesto con ese ID.");
        }

        private void EntradaInventario()
        {
            Repuesto r = PedirRepuesto();
            int cant = Consola.LeerEntero("Cantidad que ingresa: ", 1, 100000);
            r.Ingresar(cant);
            Console.WriteLine("  ✓ Nueva existencia: " + r.Existencia);
        }

        private void ModificarRepuesto()
        {
            Repuesto r = PedirRepuesto();
            Console.WriteLine("Actual: " + r.ObtenerDescripcion());
            string nombre = Consola.LeerOpcional("Nuevo nombre", v => Validador.Texto(v, "Nombre"));
            string precio = Consola.LeerOpcional("Nuevo precio de venta", v =>
            {
                if (!decimal.TryParse(v.Replace(',', '.'), System.Globalization.NumberStyles.Number,
                                      System.Globalization.CultureInfo.InvariantCulture, out decimal p) || p < 0)
                    throw new ArgumentException("El precio debe ser un número mayor o igual a cero.");
                return p.ToString(System.Globalization.CultureInfo.InvariantCulture);
            });
            if (nombre != null) r.Nombre = nombre;
            if (precio != null) r.PrecioVenta = decimal.Parse(precio, System.Globalization.CultureInfo.InvariantCulture);
            _a.Repuestos.Modificar(r);
            Console.WriteLine("  ✓ Repuesto actualizado.");
        }

        private void EliminarRepuesto()
        {
            Repuesto r = PedirRepuesto();
            if (_a.Ordenes.ObtenerTodos().Any(o => o.Repuestos.Any(d => d.Repuesto.Id == r.Id)))
                throw new InvalidOperationException("No se puede eliminar: el repuesto ya fue usado en órdenes.");
            _a.Repuestos.Eliminar(r.Id);
            Console.WriteLine("  ✓ Repuesto eliminado.");
        }

        // ======================================================== MECÁNICOS
        private void MenuMecanicos()
        {
            Submenu("PERSONAL - MECÁNICOS", new List<(string, Action)>
            {
                ("Registrar mecánico", RegistrarMecanico),
                ("Listar mecánicos",   ListarMecanicos),
                ("Buscar por especialidad o nombre", BuscarMecanicos),
                ("Modificar mecánico", ModificarMecanico),
                ("Eliminar mecánico",  EliminarMecanico),
            });
        }

        private void RegistrarMecanico()
        {
            string ident = Consola.Leer("Identificación: ", Validador.Identificacion);
            string nombre = Consola.Leer("Nombre: ", v => Validador.Texto(v, "Nombre"));
            string correo = Consola.Leer("Correo: ", Validador.Correo);
            string tel = Consola.Leer("Teléfono (8 dígitos): ", Validador.Telefono);
            string esp = Consola.Leer("Especialidad: ", v => Validador.Texto(v, "Especialidad"));
            var m = new Mecanico(ident, nombre, correo, tel, "Central", esp);
            _a.Empleados.Agregar(m);
            Console.WriteLine("  ✓ Mecánico registrado: " + m.ObtenerDescripcion());
        }

        private void ListarMecanicos()
        {
            foreach (var m in _a.ObtenerMecanicos())
                Console.WriteLine($"{m.ObtenerDescripcion()} | Órdenes abiertas: {_srv.OrdenesAbiertasDe(m)}");
        }

        private void BuscarMecanicos()
        {
            string t = Consola.LeerLibre("Nombre o especialidad contiene");
            var lista = _a.ObtenerMecanicos().Where(m =>
                m.Nombre.Contains(t, StringComparison.OrdinalIgnoreCase) ||
                m.Especialidad.Contains(t, StringComparison.OrdinalIgnoreCase));
            MostrarLista(lista.Select(m => m.ObtenerDescripcion()));
        }

        private Mecanico PedirMecanico()
        {
            int id = Consola.LeerEntero("ID del mecánico: ", 1, int.MaxValue);
            return _a.Empleados.BuscarPorId(id) as Mecanico ?? throw new KeyNotFoundException("No existe un mecánico con ese ID.");
        }

        private void ModificarMecanico()
        {
            Mecanico m = PedirMecanico();
            Console.WriteLine("Actual: " + m.ObtenerDescripcion());
            string nombre = Consola.LeerOpcional("Nuevo nombre", v => Validador.Texto(v, "Nombre"));
            string esp = Consola.LeerOpcional("Nueva especialidad", v => Validador.Texto(v, "Especialidad"));
            string tel = Consola.LeerOpcional("Nuevo teléfono", Validador.Telefono);
            if (nombre != null) m.Nombre = nombre;
            if (esp != null) m.Especialidad = esp;
            if (tel != null) m.Telefono = tel;
            _a.Empleados.Modificar(m);
            Console.WriteLine("  ✓ Mecánico actualizado.");
        }

        private void EliminarMecanico()
        {
            Mecanico m = PedirMecanico();
            if (_srv.OrdenesAbiertasDe(m) > 0)
                throw new InvalidOperationException("No se puede eliminar: el mecánico tiene órdenes abiertas.");
            _a.Empleados.Eliminar(m.Id);
            Console.WriteLine("  ✓ Mecánico eliminado.");
        }

        // ============================================================ ÓRDENES
        private void MenuOrdenes()
        {
            var ops = new List<(string Texto, Action Accion)>();
            if (EsRol(Rol.Administrador, Rol.Asesor)) ops.Add(("Abrir orden de servicio", AbrirOrden));
            ops.Add(("Listar órdenes", ListarOrdenes));
            ops.Add(("Buscar órdenes (estado, placa, cliente)", BuscarOrdenes));
            ops.Add(("Ver detalle y factura", VerFactura));
            if (EsRol(Rol.Administrador, Rol.Asesor)) ops.Add(("Asignar otro mecánico", AsignarMecanico));
            if (EsRol(Rol.Administrador, Rol.Asesor)) ops.Add(("Despachar repuesto a la orden", DespacharRepuesto));
            ops.Add(("Cambiar estado de la orden", CambiarEstado));
            if (EsRol(Rol.Administrador)) ops.Add(("Eliminar orden (solo si está Recibida)", EliminarOrden));
            Submenu("ÓRDENES DE SERVICIO", ops);
        }

        private OrdenServicio PedirOrden()
        {
            int id = Consola.LeerEntero("ID de la orden: ", 1, int.MaxValue);
            return _a.Ordenes.BuscarPorId(id) ?? throw new KeyNotFoundException("No existe una orden con ese ID.");
        }

        private void AbrirOrden()
        {
            Vehiculo veh = PedirVehiculo();
            Cliente cli = _a.Clientes.BuscarPorId(veh.IdCliente);
            foreach (var s in _a.Servicios.ObtenerTodos()) Console.WriteLine("  " + s.ObtenerDescripcion());
            int idServ = Consola.LeerEntero("ID del tipo de servicio: ", 1, int.MaxValue);
            TipoServicio tipo = _a.Servicios.BuscarPorId(idServ) ?? throw new KeyNotFoundException("Tipo de servicio inexistente.");
            string diag = Consola.Leer("Diagnóstico: ", v => Validador.Texto(v, "Diagnóstico"));
            foreach (var m in _a.ObtenerMecanicos())
                Console.WriteLine($"  {m.ObtenerDescripcion()} | abiertas: {_srv.OrdenesAbiertasDe(m)}/{ServicioOrdenes.MaxOrdenesAbiertas}");
            Mecanico mec = PedirMecanico();
            var orden = _srv.AbrirOrden(veh, cli, tipo, diag, mec);
            Console.WriteLine("  ✓ Orden abierta: " + orden.ObtenerDescripcion());
        }

        private void ListarOrdenes()
        {
            foreach (var o in _a.Ordenes.ObtenerTodos()) Console.WriteLine(o.ObtenerDescripcion());
        }

        private void BuscarOrdenes()
        {
            string estado = Consola.LeerLibre("Estado (Recibida, EnProceso, EnEsperaRepuesto, Terminada, Entregada)");
            string placa = Consola.LeerLibre("Placa contiene").ToUpper();
            string cliente = Consola.LeerLibre("Nombre de cliente contiene");
            var lista = _a.Ordenes.Buscar(o =>
                (estado == "" || o.Estado.ToString().Equals(estado, StringComparison.OrdinalIgnoreCase)) &&
                (placa == "" || o.Vehiculo.Placa.Contains(placa)) &&
                (cliente == "" || o.Cliente.Nombre.Contains(cliente, StringComparison.OrdinalIgnoreCase)));
            MostrarLista(lista.Select(o => o.ObtenerDescripcion()));
        }

        private void VerFactura()
        {
            OrdenServicio o = PedirOrden();
            Console.WriteLine(o.GenerarFactura());
            Console.WriteLine("Bitácora:");
            foreach (object evento in o.Historial)   // ArrayList guarda "object"
                Console.WriteLine("  - " + evento);
        }

        private void AsignarMecanico()
        {
            OrdenServicio o = PedirOrden();
            Mecanico m = PedirMecanico();
            _srv.AsignarMecanico(o, m);
            Console.WriteLine("  ✓ Mecánico asignado.");
        }

        private void DespacharRepuesto()
        {
            OrdenServicio o = PedirOrden();
            Repuesto r = PedirRepuesto();
            Console.WriteLine($"Existencia actual: {r.Existencia}");
            int cant = Consola.LeerEntero("Cantidad a despachar: ", 1, 100000);
            o.AgregarRepuesto(r, cant);   // lanza excepción si no hay suficiente
            Console.WriteLine($"  ✓ Despachado. Existencia restante: {r.Existencia}");
            if (r.BajoMinimo) Console.WriteLine("  ⚠ ALERTA: el repuesto llegó a la existencia mínima. Reordenar.");
        }

        private void CambiarEstado()
        {
            OrdenServicio o = PedirOrden();
            Console.WriteLine("Estado actual: " + o.Estado);
            var estados = Enum.GetValues<EstadoOrden>();
            for (int i = 0; i < estados.Length; i++) Console.WriteLine($"  {i + 1}. {estados[i]}");
            int op = Consola.LeerEntero("Nuevo estado: ", 1, estados.Length);
            o.CambiarEstado(estados[op - 1]);
            Console.WriteLine("  ✓ Estado actualizado.");
        }

        private void EliminarOrden()
        {
            OrdenServicio o = PedirOrden();
            if (o.Estado != EstadoOrden.Recibida || o.Repuestos.Count > 0)
                throw new InvalidOperationException("Solo se eliminan órdenes Recibidas y sin repuestos despachados.");
            _a.Ordenes.Eliminar(o.Id);
            Console.WriteLine("  ✓ Orden eliminada.");
        }

        // ========================================================== REPORTES
        private void MenuReportes()
        {
            Submenu("REPORTES Y CONSULTAS", new List<(string, Action)>
            {
                ("Órdenes por mecánico",              ReporteMecanico),
                ("Órdenes por rango de fechas",       ReporteFechas),
                ("Órdenes por cliente",               ReporteCliente),
                ("Repuestos más utilizados",          ReporteMasUsados),
                ("Repuestos bajo la existencia mínima", ReporteBajoMinimo),
            });
        }

        private void ReporteMecanico()
        {
            foreach (var m in _a.ObtenerMecanicos())
            {
                var suyas = _a.Ordenes.Buscar(o => o.Mecanicos.Any(x => x.Id == m.Id));
                Console.WriteLine($"{m.Nombre}: {suyas.Count} orden(es)");
                foreach (var o in suyas) Console.WriteLine("    " + o.ObtenerDescripcion());
            }
        }

        private void ReporteFechas()
        {
            DateTime desde = Consola.LeerFecha("Desde");
            DateTime hasta = Consola.LeerFecha("Hasta");
            if (hasta < desde) throw new ArgumentException("La fecha final no puede ser anterior a la inicial.");
            var lista = _a.Ordenes.Buscar(o => o.Fecha.Date >= desde.Date && o.Fecha.Date <= hasta.Date);
            MostrarLista(lista.Select(o => o.ObtenerDescripcion()));
        }

        private void ReporteCliente()
        {
            string t = Consola.LeerLibre("Nombre de cliente o empresa contiene");
            var lista = _a.Ordenes.Buscar(o => o.Cliente.Nombre.Contains(t, StringComparison.OrdinalIgnoreCase));
            MostrarLista(lista.Select(o => o.ObtenerDescripcion()));
        }

        private void ReporteMasUsados()
        {
            var ranking = _a.Ordenes.ObtenerTodos()
                .SelectMany(o => o.Repuestos)
                .GroupBy(d => d.Repuesto.Nombre)
                .Select(g => new { Nombre = g.Key, Total = g.Sum(d => d.Cantidad) })
                .OrderByDescending(x => x.Total).Take(5);
            MostrarLista(ranking.Select(x => $"{x.Nombre}: {x.Total} unidades"));
        }

        private void ReporteBajoMinimo()
        {
            var lista = _a.Repuestos.Buscar(r => r.BajoMinimo);
            MostrarLista(lista.Select(r => r.ObtenerDescripcion()));
        }

        // ============================================================ UTIL
        /// <summary>Imprime las líneas o un aviso si no hay resultados.</summary>
        private void MostrarLista(IEnumerable<string> lineas)
        {
            var datos = lineas.ToList();
            if (datos.Count == 0) { Console.WriteLine("  (sin resultados)"); return; }
            foreach (var l in datos) Console.WriteLine(l);
            Console.WriteLine($"  -- {datos.Count} resultado(s)");
        }
    }
}
