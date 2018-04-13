﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle. @04/13/2018 09:45:58
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MMPModel.Repository
{
    using Mehdime.Entity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    
    /// <summary>
    /// Classe de base pour tous les repositories
    /// </summary>
    abstract class BaseRepository<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        private readonly AmbientDbContextLocator _dbContextLocator;
            
        internal BaseRepository()
        {
            _dbContextLocator = new AmbientDbContextLocator();
        }
            
        protected TDbContext DbContext
        {
            get
            {
                //Pas de référence directe vers le DbContext, on doit query le locator à chaque fois
                //car il est le seul à savoir si le scope en cours contient bien un DbContext valide
                var _dbContext = _dbContextLocator.Get<TDbContext>();
                if (_dbContext == null)
                    //throw new InvalidOperationException("Pas de DbContext ambiant de type " + typeof(TDbContext).ToString());
    				throw new InvalidOperationException("No ambient DbContext of type " + typeof(TDbContext).ToString() + " found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");
                return _dbContext;
            }
        }
            
        /// <summary>
        /// Retourne un DbSet pour accéder aux données
        /// </summary>
        internal virtual DbSet<TEntity> DbSet
        {
            get
            {
                return DbContext.Set<TEntity>();
            }
        }
            
        /// <summary>
        /// Retourne un objet permettant de tracker les modifications faite sur l'objet model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal DbEntityEntry<TEntity> Entry(TEntity entity)
        {
            return DbContext.Entry(entity);
        }
            
        internal TEntity Add(TEntity entity)
        {
            return DbSet.Add(entity);
        }
        
        internal TEntity Delete(TEntity entity)
        {
            return DbSet.Remove(entity);
        }
    
        /// <summary>
        /// Marque l'entité comme étant modifiée
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal TEntity Update(TEntity entity)
        {
            this.Entry(entity).State = EntityState.Modified;
            return entity;
        }
    
        internal TEntity Find(object key)
        {
            return DbSet.Find(key);
        }
    
        internal System.Threading.Tasks.Task<TEntity> FindAsync(object key)
        {
            return DbSet.FindAsync(key);
        }
        
        internal TEntity FindSingle(Expression<Func<TEntity, bool>> predicate = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var dbSet = FindIncluding(includes);
            return (predicate == null) ?
                dbSet.FirstOrDefault() :
                dbSet.FirstOrDefault(predicate);
        }
        
        internal IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate = null, 
            params Expression<Func<TEntity, object>>[] includes)
        {
            var dbSet = FindIncluding(includes);
            return (predicate == null) ? 
    			dbSet : 
    			dbSet.Where(predicate);
        }
        
        internal IQueryable<TEntity> FindIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var dbSet = DbContext.Set<TEntity>();    
            if (includeProperties != null)
            {
                foreach (var include in includeProperties)
                {
                    dbSet.Include(include);
                }
            }
            return dbSet.AsQueryable();
        }
        
        internal int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            var dbSet = DbContext.Set<TEntity>();
            return (predicate == null) ?
                    dbSet.Count() :
                    dbSet.Count(predicate);
        }
        
        internal bool Exist(Expression<Func<TEntity, bool>> predicate = null)
        {
            var dbSet = DbContext.Set<TEntity>();
            return (predicate == null) ? 
    			dbSet.Any() : 
    			dbSet.Any(predicate);
        }
        
    }
}
