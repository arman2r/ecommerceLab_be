namespace ecommerceLab.Models.User
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CorreoElectronico { get; set; }
        public string PasswordUser { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
