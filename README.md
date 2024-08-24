# Gestión de Productos API

Esta aplicación está desarrollada en .NET Core 8 y proporciona una API REST completa para la gestión de productos, autenticación de usuarios y manejo de carritos de compra. La aplicación está diseñada para interactuar con una base de datos SQL Server y utiliza Amazon S3 para almacenar imágenes de productos.

## Funcionalidades

### Gestión de Productos

- **Listar Productos:** Obtén una lista de todos los productos disponibles en la base de datos.
- **Obtener Detalle del Producto:** Obtén información detallada sobre un producto específico.
- **Crear Producto:** Permite la creación de nuevos productos en la base de datos.
- **Almacenar Imagen del Producto:** Sube y almacena imágenes de productos en Amazon S3.

### Autenticación y Autorización

- **Sistema de Autenticación JWT:** Utiliza JSON Web Tokens (JWT) para gestionar la autenticación de usuarios.
- **Endpoint de Login:** Permite a los usuarios iniciar sesión en el sistema.
- **Endpoint de Registro:** Permite a nuevos usuarios registrarse en el sistema.

### Gestión del Carrito de Compra

- **Sistema API para Carrito de Compra:** Gestiona los elementos del carrito de compra de los usuarios, incluyendo agregar, eliminar y listar productos en el carrito.

## Tecnologías Utilizadas

- **.NET Core 8:** Versión del framework utilizado para desarrollar la aplicación.
- **SQL Server:** Sistema de gestión de bases de datos utilizado para almacenar la información de productos y usuarios.
- **Amazon S3:** Servicio de almacenamiento en la nube utilizado para almacenar imágenes de productos.

## Configuración y Ejecución

1. **Clonar el Repositorio:**

   ```bash
   git clone https://github.com/arman2r/ecommerceLab_front.git
2. **Restaurar Nuggets**

-- **En Visual Studio:** Asegurate de restaurar los nuggets necesarios para soportar las dependencias de aws
   
## Contribuciones

Las contribuciones son bienvenidas. Si tienes sugerencias, correcciones o mejoras, por favor, abre un issue o un pull request en el repositorio.

## Licencia

Este proyecto está licenciado bajo la MIT License.
