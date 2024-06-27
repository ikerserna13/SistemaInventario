﻿using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos.Especificaiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repositorio(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task Agregar(T entidad)
        {
            await dbSet.AddAsync(entidad); // Insert into Table
        }

        public async Task<T> Obtener(int id)
        {
            return await dbSet.FindAsync(id); // Select * from (Solo por Id)
        }

        public async Task<IEnumerable<T>> ObtenerTodos(Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTraking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro); // Selct * from --- where ....
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); // ejemplo: "Categoria.Marca"                    
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (!isTraking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public async Task<T> ObtenerPrimero(Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTraking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro); // Selct * from --- where 
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); // ejemplo: "Categoria.Marca"                    
                }
            }
            if (!isTraking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public void Eliminar(T entidad)
        {
            dbSet.Remove(entidad);
        }

        public void EliminarRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }

        public PagedList<T> ObtenerTodosPaginado(Parametros parametros, 
            Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, 
            IOrderedQueryable<T>> orderBy = null, 
            string incluirPropiedades = null, bool isTraking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro); // Selct * from --- where ....
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); // ejemplo: "Categoria.Marca"                    
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (!isTraking)
            {
                query = query.AsNoTracking();
            }
            return PagedList<T>.ToPagedList(query, parametros.PageNumber, parametros.PageSize);
        }
    }
}
