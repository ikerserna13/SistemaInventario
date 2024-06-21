using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoreController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public StoreController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Store store = new Store();
            if (id == null) {
                store.Estado = true;
                return View(store);
            }
            store = await _unidadTrabajo.Store.Obtener(id.GetValueOrDefault());
            if (store == null)
            {
                return NotFound();
            }
            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Store store)
        {
            if (ModelState.IsValid)
            {
                if (store.Id == 0)
                {
                    await _unidadTrabajo.Store.Agregar(store);
                    TempData[DS.Existosa] = "Almacen creado existosamente";
                }
                else
                {
                    _unidadTrabajo.Store.Actualizar(store);
                    TempData[DS.Existosa] = "Almacen actualizado existosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al guardar Almacen";
            return View(store);
        }

        

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Store.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var storeDb = await _unidadTrabajo.Store.Obtener(id);
            if (storeDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Almacen" });
            }
            _unidadTrabajo.Store.Eliminar(storeDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Almacen borrado correctamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id=0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Store.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());

            }
            else
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
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
