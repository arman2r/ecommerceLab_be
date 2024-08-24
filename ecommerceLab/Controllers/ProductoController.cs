using ecommerceLab.Models;
using ecommerceLab.Models.ProductoModels;
using ecommerceLab.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;
    private readonly ApplicationDbContext _context;

    public ProductosController(IProductoService productoService, ApplicationDbContext context)
    {
        _productoService = productoService;
        _context = context;      
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPaginados([FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 5)
    {
        try
        {
            var productos = await _productoService.GetProductosPaginadosAsync(pagina, tamanoPagina);
            return Ok(productos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrió un error al obtener los productos: {ex.Message}");
        }
        
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetProductoById(int id)
    {
        try
        {
            var producto = await _productoService.GetProductoByIdAsync(id);
            if (producto == null)
            {
                return NotFound($"No se encontró ningún producto con el ID {id}");
            }
            return Ok(producto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrió un error al obtener el producto: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Producto>> AddProducto([FromForm] Producto producto)
    {
        try
        {
            if (producto == null)
            {
                return BadRequest("Los datos del producto son requeridos.");
            }

            var nuevoProducto = await _productoService.AddProductoAsync(producto);
            return CreatedAtAction(nameof(GetProductoById), new { id = nuevoProducto.Id }, nuevoProducto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrió un error al crear el producto: {ex.Message}");
        }
    }

    [HttpPost("{id}/imagen")]
    public async Task<ActionResult> UploadImage(int id, [FromForm] IFormFile imagen)
    {
        try
        {
            if (imagen == null || imagen.Length == 0)
            {
                return BadRequest("La imagen es requerida.");
            }

            var producto = await _productoService.GetProductoByIdAsync(id);
            if (producto == null)
            {
                return NotFound($"No se encontró ningún producto con el ID {id}");
            }

            using (var stream = imagen.OpenReadStream())
            {
                var urlImagen = await _productoService.UploadImageAsync(stream, imagen.FileName, imagen.ContentType);
                producto.UrlImagen = urlImagen;

                _context.Producto.Update(producto);
                await _context.SaveChangesAsync();
            }

            return Ok(producto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocurrió un error al subir la imagen: {ex.Message}");
        }
    }
}