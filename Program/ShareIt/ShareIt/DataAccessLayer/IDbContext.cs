using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DataAccessLayer
{
    /// <summary>
    /// Interface for defining Entity framework contexts
    /// </summary>
    /// <author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// </author>
    public interface IDbContext : IDisposable
    {
        /// <summary>
        /// Method for getting entity sets
        /// </summary>
        /// <typeparam name="T">The type of entity to get</typeparam>
        /// <returns>The IDbSet containing entity data</returns>
        IDbSet<T> Set<T>() where T : class;

        /// <summary>
        /// Retunes the number of entities saved to the context
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Creates an entry in the context. 
        /// The change is not saved until Save changes is called.
        /// </summary>
        /// <param name="o">The object to add to the context</param>
        /// <returns>The DbEntityEntry just added to the contect</returns>
        DbEntityEntry Entry(object o);
    }
}
