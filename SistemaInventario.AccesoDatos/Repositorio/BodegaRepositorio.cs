using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class BodegaRepositorio : Repositorio<Bodega>, IBodegaRepositorio
    {
        private readonly ApplicationDbContext _db;

        public BodegaRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(Bodega bodega)
        {
            var storeBD = _db.Bodegas.FirstOrDefault(s => s.Id == bodega.Id);
            if (storeBD != null)
            {
                storeBD.Nombre = bodega.Nombre;
                storeBD.Descripcion = bodega.Descripcion;
                storeBD.Estado = bodega.Estado;
                _db.SaveChanges();
            }
        }
    }
}
