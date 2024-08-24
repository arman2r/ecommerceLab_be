using ecommerceLab.Models;
using ecommerceLab.Models.CarritoModels;
using ecommerceLab.Models.ProductoModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ecommerceLab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CarritoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarritoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Carrito
        [HttpPost]
        public async Task<ActionResult<Carrito>> PostCarrito(Carrito carrito)
        {
            try
            {
                // Verificar si el total se proporcionó en la solicitud
                if (carrito.Total == 0)
                {
                    return BadRequest("El total del carrito es obligatorio.");
                }

                // Establecer el estado del carrito como "Pendiente"
                carrito.Estado = "Pendiente";

                // Agregar el carrito a la base de datos
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCarrito", new { id = carrito.ID }, carrito);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar el carrito: {ex.Message}");
            }
        }

        // PUT: api/carrito/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarrito(int id, Carrito carrito)
        {
            try
            {
                // Verificar si el ID del carrito en la solicitud coincide con el ID en la ruta
                if (id != carrito.ID)
                {
                    return BadRequest("IDs de carrito no coinciden.");
                }

                // Obtener el carrito existente de la base de datos
                var carritoExistente = await _context.Carritos.FindAsync(id);
                if (carritoExistente == null)
                {
                    return NotFound("No se encontró el carrito.");
                }

                // Actualizar los campos del carrito existente con los valores de la solicitud
                carritoExistente.UsuarioID = carrito.UsuarioID;
                carritoExistente.Total = carrito.Total;
                carritoExistente.Estado = carrito.Estado;

                // Guardar los cambios en la base de datos
                _context.Entry(carritoExistente).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el carrito: {ex.Message}");
            }
        }

        // POST: api/Carrito/AgregarProducto
        [HttpPost("AgregarProducto")]
        public async Task<ActionResult<IEnumerable<DetalleCarrito>>> AgregarProducto(IEnumerable<DetalleCarrito> detalles)
        {
            try
            {
                // Agregar todos los detalles de carrito a la base de datos
                _context.DetallesCarrito.AddRange(detalles);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetDetalleCarrito", detalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al agregar productos al carrito: {ex.Message}");
            }
        }

        // PUT: api/carrito/actualizarProducto
        [HttpPut("actualizarProducto")]
        public async Task<IActionResult> ActualizarProductoEnCarrito(DetalleCarrito detalle)
        {
            try
            {
                // Buscar el detalle del carrito en la base de datos
                var detalleExistente = await _context.DetallesCarrito.FirstOrDefaultAsync(d => d.CarritoID == detalle.CarritoID && d.ProductoID == detalle.ProductoID);
                if (detalleExistente == null)
                {
                    return NotFound("El producto no se encuentra en el carrito.");
                }

                // Actualizar la cantidad del producto en el detalle del carrito
                detalleExistente.Cantidad = detalle.Cantidad;

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el producto en el carrito: {ex.Message}");
            }
        }

        // DELETE: api/carrito/eliminarProducto/{productoId}
        [HttpDelete("eliminarProducto/{productoId}")]
        public async Task<IActionResult> EliminarProductoDeCarrito(int productoId)
        {
            try
            {
                // Buscar el detalle del carrito en la base de datos por el ID del producto
                var detalleExistente = await _context.DetallesCarrito.FirstOrDefaultAsync(d => d.ProductoID == productoId);
                if (detalleExistente == null)
                {
                    return NotFound("El producto no se encuentra en el carrito.");
                }

                // Eliminar el detalle del carrito de la base de datos
                _context.DetallesCarrito.Remove(detalleExistente);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el producto del carrito: {ex.Message}");
            }
        }

        // GET: api/carrito/pendiente
        [HttpGet("pendiente")]
        public async Task<ActionResult<Carrito>> GetCarritoPendiente(string estado)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("Usuario no autenticado.");
                }

                var carritoPendiente = await _context.Carritos
                    .FirstOrDefaultAsync(c => c.UsuarioID == int.Parse(userId) && c.Estado == estado);

                if (carritoPendiente == null)
                {
                    return NotFound($"No se encontró ningún carrito con estado '{estado}' para el usuario.");
                }

                return Ok(carritoPendiente);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener el carrito pendiente: {ex.Message}");
            }
        }

        // GET: api/carrito/{carritoId}/productos
        [HttpGet("{carritoId}/productos")]
        public async Task<ActionResult<IEnumerable<GetDetalleCarrito>>> GetProductosDeCarrito(int carritoId)
        {
            try
            {
                // Obtener los detalles del carrito
                var detallesCarrito = await _context.DetallesCarrito
                    .Where(d => d.CarritoID == carritoId)
                    .ToListAsync();

                // Verificar si no se encontraron detalles para el carrito
                if (detallesCarrito == null || !detallesCarrito.Any())
                {
                    return NotFound("No se encontraron productos asociados a este carrito.");
                }

                // Obtener los IDs de los productos asociados al carrito
                var idsProductos = detallesCarrito.Select(d => d.ProductoID).ToList();

                // Obtener los productos correspondientes a los IDs
                var productos = await _context.Producto
                    .Where(p => idsProductos.Contains(p.Id))
                    .ToListAsync();

                // Mapear los detalles y productos a un nuevo objeto con la información requerida
                var productosConDetalle = detallesCarrito.Select(detalle =>
                {
                    var producto = productos.FirstOrDefault(p => p.Id == detalle.ProductoID);
                    return new GetDetalleCarrito
                    {
                        ID = detalle.ID,
                        CarritoID = detalle.CarritoID,
                        ProductoID = detalle.ProductoID,
                        Cantidad = detalle.Cantidad,
                        Nombre = producto != null ? producto.Nombre : null,
                        Precio = producto != null ? producto.Precio : 0
                    };
                });

                return Ok(productosConDetalle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener productos del carrito: {ex.Message}");
            }
        }

    }
}
