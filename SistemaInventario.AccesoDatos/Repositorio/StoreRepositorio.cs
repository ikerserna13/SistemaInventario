using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class StoreRepositorio : Repositorio<Store>, IStoreRepositorio
    {
        private readonly ApplicationDbContext _db;

        public StoreRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(Store store)
        {
            var storeBD = _db.Stores.FirstOrDefault(s => s.Id == store.Id);
            if (storeBD != null)
            {
                storeBD.Nombre = store.Nombre;
                storeBD.Descripcion = store.Descripcion;
                storeBD.Estado = store.Estado;
                _db.SaveChanges();
            }
        }
    }
}
