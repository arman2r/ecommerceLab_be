namespace ecommerceLab.Models.ProductoModels
{
    public class Producto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public int Precio { get; set; }
        public string? Descripcion { get; set; }
        public int Stock { get; set; }
        public string? UrlImagen { get; set; }
    }
}
