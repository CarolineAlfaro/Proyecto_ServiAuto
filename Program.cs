using System.Text;
using ServiAuto.Negocio;
using ServiAuto.Presentacion;

// Punto de entrada. "Top-level statements": no hace falta escribir class Program/Main.
Console.OutputEncoding = Encoding.UTF8;   // para mostrar tildes y el símbolo ₡

var almacen = new Almacen();                      // 1. crea los repositorios vacíos
var servicioOrdenes = new ServicioOrdenes(almacen);
DatosPrueba.Cargar(almacen, servicioOrdenes);     // 2. precarga los datos de prueba

try
{
    new Menu(almacen, servicioOrdenes).Iniciar(); // 3. login + menú principal
}
catch (Exception ex)                              // red de seguridad final
{
    Console.WriteLine("Error inesperado: " + ex.Message);
}
