using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    /// <summary>
    /// Exposes the different calls to the database that we want.
    /// The reason that we are implementing an interface is because 
    /// we might want to return a mock object that contains static data
    /// or data from an an in-memory data store, while testing.
    /// </summary>
    public class DutchRepository : IDutchRepository
    {
        private readonly DutchContext _ctx;
        private readonly ILogger<DutchRepository> _logger;

        /// <summary>
        /// The class takes an instance of itself (the generic). The reason
        /// for this is the logger will then be tied to this type, so that 
        /// when it emits data we'll be actually able to see where the logging
        /// came from!
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="logger"></param>
        public DutchRepository(DutchContext ctx, ILogger<DutchRepository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public void AddEntity(object model)
        {
            _ctx.Add(model);
        }

        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            if (includeItems)
            {
                return _ctx.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();
            }
            else
            {
                return _ctx.Orders.ToList();
            }
        }

        /// <summary>
        /// Gets all the products, ordered by title
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                // use the logger to log information to the console.
                _logger.LogInformation("GetAllProducts was called");

                return _ctx.Products
                    .OrderBy(p => p.Title)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all products: {ex}");
                return null;
            }
        }

        public Order GetOrderById(int id)
        {
            return _ctx.Orders
                        .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                        .Where(o => o.Id == id)
                        .FirstOrDefault();
        }

        /// <summary>
        /// Gets all the products for a particular category
        /// </summary>
        /// <param name="Category"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetProductsByCategory(string Category)
        {
            return _ctx.Products
                .Where(p => p.Category == Category)
                .ToList();
        }

        /// <summary>
        /// Saves all changes
        /// </summary>
        /// <returns></returns>
        public bool SaveAll()
        {
            // SaveChanges returns the number of rows affected, so a 
            // save actually worked if the number of rows affected was 
            // more than 0
            return _ctx.SaveChanges() > 0;
        }
    }
}
