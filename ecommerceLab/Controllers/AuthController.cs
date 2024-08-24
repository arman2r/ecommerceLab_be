using ecommerceLab.Models;
using ecommerceLab.Models.User;
using ecommerceLab.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecommerceLab.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _dbContext;
        public AuthController(IAuthService authService, ApplicationDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("El correo electrónico y la contraseña son obligatorios.");
            }

            // Verificar si el usuario existe en la base de datos
            var existingUser = await _dbContext.Usuario.FirstOrDefaultAsync(u => u.CorreoElectronico == request.Email);
            if (existingUser == null)
            {
                return NotFound("El usuario no existe.");
            }

            // Autenticar al usuario
            var token = await _authService.Authenticate(request.Email, request.Password);
            if (token == null)
            {
                return Unauthorized("La contraseña proporcionada es incorrecta.");
            }

            return Ok(new { Token = token });
        }


        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] Usuario request)
        {
            // Verificar si el usuario ya existe en la base de datos (por ejemplo, por su correo electrónico)
            var existingUser = await _dbContext.Usuario.FirstOrDefaultAsync(u => u.CorreoElectronico == request.CorreoElectronico);
            if (existingUser != null)
            {
                // Devolver un error indicando que el usuario ya está registrado
                return Conflict("El usuario ya está registrado");
            }

            // Encriptar la contraseña antes de almacenarla
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.PasswordUser);

            // Crear una nueva instancia de Usuario con la información proporcionada en la solicitud
            var newUser = new Usuario
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                CorreoElectronico = request.CorreoElectronico,
                PasswordUser = hashedPassword,
                FechaRegistro = DateTime.UtcNow
            };

            // Agregar el nuevo usuario al contexto y guardar los cambios en la base de datos
            _dbContext.Usuario.Add(newUser);
            await _dbContext.SaveChangesAsync();

            // Devolver un código de estado 201 (Created) indicando que el registro fue exitoso
            return StatusCode(201);
        }
    }
}
