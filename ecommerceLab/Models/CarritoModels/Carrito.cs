using ecommerceLab.Models.ProductoModels;

namespace ecommerceLab.Models.CarritoModels
{
    // Carrito.cs
    public class Carrito
    {
        public int ID { get; set; } // Cambiar Id por ID para que coincida con el nombre de la columna en la base de datos
        public int UsuarioID { get; set; }
        public int Total { get; set; }
        public string Estado { get; set; }
        //public List<DetalleCarrito> Detalles { get; set; }
    }

    public class GetCarrito
    {
        public int ID { get; set; } // Cambiar Id por ID para que coincida con el nombre de la columna en la base de datos
        public int UsuarioID { get; set; }
        public int Total { get; set; }
        public string Estado { get; set; }
        public List<GetDetalleCarrito> Detalles { get; set; }
    }

    // Clase DetalleCarrito
    public class DetalleCarrito
    {
        public int ID { get; set; } // Agregar una clave primaria
        public int CarritoID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
    }

    public class GetDetalleCarrito
    {
        public int ID { get; set; } // Agregar una clave primaria
        public int CarritoID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public string? Nombre { get; set; }
        public int Precio{ get; set; }
    }
}
