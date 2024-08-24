using Amazon.S3;
using Amazon.S3.Transfer;
using ecommerceLab.Models;
using ecommerceLab.Models.ProductoModels;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;

namespace ecommerceLab.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> GetProductosPaginadosAsync(int pagina, int tamanoPagina);
        Task<Producto> GetProductoByIdAsync(int id);
        Task<Producto> AddProductoAsync(Producto producto);
        Task<string?> UploadImageAsync(Stream stream, string fileName, string contentType);
    }
    public class ProductoService : IProductoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public ProductoService(ApplicationDbContext context)
        {
            _context = context;           
        }

        public async Task<IEnumerable<Producto>> GetProductosPaginadosAsync(int pagina, int tamanoPagina)
        {
            return await _context.Producto
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();
        }

        public async Task<Producto> GetProductoByIdAsync(int id)
        {
            // Buscar el producto en la base de datos por su ID
            var producto = await _context.Producto.FindAsync(id);

            // Retornar el producto encontrado (o null si no se encuentra)
            return producto;
        }

        public async Task<Producto> AddProductoAsync(Producto producto)
        {
            _context.Producto.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<string> UploadImageAsync(Stream blobStream, string blobName, string contentType)
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = blobStream,
                Key = $"{Guid.NewGuid()}_{blobName}",
                BucketName = _bucketName,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return $"https://{_bucketName}.s3.amazonaws.com/{uploadRequest.Key}";
        }
    }
}
