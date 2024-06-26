using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System.Runtime.InteropServices;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Marca"),
                PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Producto")
            };

            if (id == null)
            {
                productoVM.Producto.Estado = true;
                return View(productoVM);
            }
            else
            {
                productoVM.Producto = await _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());
                if (productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files; // Capturalos archivos que se le estan enviando desde el formulario
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productoVM.Producto.Id == 0)
                {
                    // Crear
                    string upload = webRootPath + DS.ImagenRuta; // Crea la ruta de la imagen
                    string fileName = Guid.NewGuid().ToString(); // Crea un Id para la imagen
                    string extension = Path.GetExtension(files[0].FileName); // Captura la extensión del archivo que se le pasa

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream); // La imagen la guardo en una variable en memoria
                    }
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    await _unidadTrabajo.Producto.Agregar(productoVM.Producto); // Guarda el Producto en la base de datos
                }
                else
                {
                    // Actualizar
                    var objProducto = await _unidadTrabajo.Producto.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTraking: false); // Obtiene la imagen del mismo Id de producto
                    if (files.Count > 0) // Mira si estan enviando una imagen desde el formulario
                    {
                        string upload = webRootPath + DS.ImagenRuta; // Crea la ruta de la imagen
                        string fileName = Guid.NewGuid().ToString(); // Crea un Id para la imagen
                        string extension = Path.GetExtension(files[0].FileName); // Captura la extensión del archivo que se le pasa

                        // Borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl); // Recupera la imagen anterior
                        if (System.IO.File.Exists(anteriorFile)) // Comprueba si existe la imagen anterior (si hay)
                        {
                            System.IO.File.Delete(anteriorFile); // Elimina la anterior imagen
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productoVM.Producto.ImagenUrl = fileName + extension; // Pone la nueva imagen en su lugar
                    } // En caso contrario no se carga una nueva imagen
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl; // Asigna la url de la nueva imagen
                    }
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }
                TempData[DS.Existosa] = "Transacción Exitosa!"; // Avisar de que el proceso ha sido exitoso
                await _unidadTrabajo.Guardar(); // Guardar los cambios
                return RedirectToAction("Index"); // Vuelve a la vista principal

            } // Si la transacción no es valida
            productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Categoria");
            productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Marca");
            productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Producto");
            return View("Upsert", productoVM); // Rellena los datos de "Categorias" y "Marcas" y devuelve la vista formulario
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria;Marca");
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productoDb = await _unidadTrabajo.Producto.Obtener(id);
            if (productoDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Producto" });
            }

            // Eliminar imgagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload, productoDb.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }

            _unidadTrabajo.Producto.Eliminar(productoDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrado correctamente" });
        }

        [ActionName("ValidarSerie")]
        public async Task<IActionResult> ValidarNombre(string numeroSerie, int id=0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Producto.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == numeroSerie.ToLower().Trim());

            }
            else
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == numeroSerie.ToLower().Trim() && b.Id != id);
            }

            if (valor)
            {
                return Json(new { data = true });
            }
            return Json(new { success = false });
        }
        #endregion
    }
}
