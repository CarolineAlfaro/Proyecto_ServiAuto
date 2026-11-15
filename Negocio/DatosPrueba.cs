using ServiAuto.Entidades;

namespace ServiAuto.Negocio
{
    /// <summary>Carga datos de ejemplo al iniciar para poder demostrar todas las opciones.</summary>
    public static class DatosPrueba
    {
        public static void Cargar(Almacen a, ServicioOrdenes srv)
        {
            // Usuarios (autenticación simulada)
            a.Usuarios.Add(new Usuario("admin", "admin123", Rol.Administrador, "Ana Administradora"));
            a.Usuarios.Add(new Usuario("asesor", "asesor123", Rol.Asesor, "Luis Asesor"));
            a.Usuarios.Add(new Usuario("mecanico", "mec123", Rol.Mecanico, "Pedro Mecánico"));

            // Empleados: se guardan en el mismo repositorio (polimorfismo)
            a.Empleados.Agregar(new Administrador("101110111", "Ana Rojas", "ana@serviauto.com", "88880001", "Central"));
            a.Empleados.Agregar(new AsesorServicio("202220222", "Luis Mora", "luis@serviauto.com", "88880002", "Central"));
            a.Empleados.Agregar(new Mecanico("303330333", "Pedro Solano", "pedro@serviauto.com", "88880003", "Central", "Mecánica general"));
            a.Empleados.Agregar(new Mecanico("404440444", "María Vargas", "maria@serviauto.com", "88880004", "Central", "Electromecánica"));
            a.Empleados.Agregar(new Mecanico("505550555", "Jorge Campos", "jorge@serviauto.com", "88880005", "Alajuela", "Enderezado y pintura"));
            a.Empleados.Agregar(new Mecanico("606660666", "Sofía Araya", "sofia@serviauto.com", "88880006", "Alajuela", "Mecánica general"));

            // Clientes (3 particulares y 2 empresas)
            a.Clientes.Agregar(new ClienteParticular("107770777", "Carlos Jiménez", "carlos@mail.com", "70010001"));
            a.Clientes.Agregar(new ClienteParticular("108880888", "Laura Quesada", "laura@mail.com", "70010002"));
            a.Clientes.Agregar(new ClienteParticular("109990999", "Esteban Rojas", "esteban@mail.com", "70010003"));
            a.Clientes.Agregar(new ClienteEmpresa("3101123456", "Transportes del Norte S.A.", "info@tnorte.com", "22001111", 10));
            a.Clientes.Agregar(new ClienteEmpresa("3101654321", "Distribuidora Central S.A.", "flota@dcentral.com", "22002222", 15));

            // Vehículos
            a.Vehiculos.Agregar(new Vehiculo("ABC123", "Toyota", "Corolla", 2018, 62000, 1));
            a.Vehiculos.Agregar(new Vehiculo("BCD234", "Hyundai", "Tucson", 2020, 38000, 2));
            a.Vehiculos.Agregar(new Vehiculo("CDE345", "Nissan", "Sentra", 2016, 95000, 3));
            a.Vehiculos.Agregar(new Vehiculo("TNO001", "Hino", "300", 2019, 140000, 4));
            a.Vehiculos.Agregar(new Vehiculo("TNO002", "Isuzu", "NPR", 2017, 210000, 4));
            a.Vehiculos.Agregar(new Vehiculo("DCE100", "Toyota", "Hilux", 2021, 54000, 5));

            // Repuestos (el último queda bajo el mínimo para probar la alerta)
            a.Repuestos.Agregar(new Repuesto("FIL-ACE", "Filtro de aceite", 25, 5, 2500, 4000));
            a.Repuestos.Agregar(new Repuesto("ACE-5W30", "Aceite 5W30 (litro)", 40, 10, 5000, 7500));
            a.Repuestos.Agregar(new Repuesto("PAS-DEL", "Pastillas de freno delanteras", 12, 4, 18000, 27000));
            a.Repuestos.Agregar(new Repuesto("BAT-12V", "Batería 12V", 8, 3, 45000, 65000));
            a.Repuestos.Agregar(new Repuesto("BUJ-NGK", "Bujía NGK", 30, 8, 2000, 3500));
            a.Repuestos.Agregar(new Repuesto("CORR-DIST", "Correa de distribución", 3, 3, 22000, 33000));

            // Tipos de servicio
            a.Servicios.Agregar(new TipoServicio("Mecánica general", 25000));
            a.Servicios.Agregar(new TipoServicio("Electromecánica", 35000));
            a.Servicios.Agregar(new TipoServicio("Enderezado y pintura", 60000));

            var mec = a.ObtenerMecanicos();   // Pedro, María, Jorge, Sofía

            // Órdenes en distintos estados
            var o1 = srv.AbrirOrden(a.Vehiculos.BuscarPorId(1), a.Clientes.BuscarPorId(1), a.Servicios.BuscarPorId(1), "Cambio de aceite y filtro", mec[0]);
            o1.AgregarRepuesto(a.Repuestos.BuscarPorId(1), 1);
            o1.AgregarRepuesto(a.Repuestos.BuscarPorId(2), 4);
            o1.CambiarEstado(EstadoOrden.EnProceso);
            o1.CambiarEstado(EstadoOrden.Terminada);
            o1.CambiarEstado(EstadoOrden.Entregada);
            o1.Fecha = DateTime.Now.AddDays(-20);

            var o2 = srv.AbrirOrden(a.Vehiculos.BuscarPorId(2), a.Clientes.BuscarPorId(2), a.Servicios.BuscarPorId(2), "Falla en alternador", mec[1]);
            o2.AgregarRepuesto(a.Repuestos.BuscarPorId(4), 1);
            o2.CambiarEstado(EstadoOrden.EnProceso);
            o2.Fecha = DateTime.Now.AddDays(-5);

            var o3 = srv.AbrirOrden(a.Vehiculos.BuscarPorId(4), a.Clientes.BuscarPorId(4), a.Servicios.BuscarPorId(1), "Revisión de frenos", mec[0]);
            o3.AgregarRepuesto(a.Repuestos.BuscarPorId(3), 2);
            o3.CambiarEstado(EstadoOrden.EnEsperaRepuesto);
            o3.Fecha = DateTime.Now.AddDays(-3);

            var o4 = srv.AbrirOrden(a.Vehiculos.BuscarPorId(6), a.Clientes.BuscarPorId(5), a.Servicios.BuscarPorId(3), "Enderezado de puerta y pintura", mec[2]);
            o4.CambiarEstado(EstadoOrden.EnProceso);
            o4.Fecha = DateTime.Now.AddDays(-7);

            var o5 = srv.AbrirOrden(a.Vehiculos.BuscarPorId(3), a.Clientes.BuscarPorId(3), a.Servicios.BuscarPorId(1), "Cambio de correa y bujías", mec[3]);
            o5.AgregarRepuesto(a.Repuestos.BuscarPorId(6), 1);
            o5.AgregarRepuesto(a.Repuestos.BuscarPorId(5), 4);
            o5.CambiarEstado(EstadoOrden.EnProceso);
            o5.CambiarEstado(EstadoOrden.Terminada);
            o5.Fecha = DateTime.Now.AddDays(-1);

            // Orden recién recibida (sin trabajo aún)
            srv.AbrirOrden(a.Vehiculos.BuscarPorId(5), a.Clientes.BuscarPorId(4), a.Servicios.BuscarPorId(2), "Luces del tablero encendidas", mec[1]);
        }
    }
}
