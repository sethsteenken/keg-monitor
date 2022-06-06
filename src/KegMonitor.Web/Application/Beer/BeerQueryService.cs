using KegMonitor.Core.Entities;
using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class BeerQueryService : IBeerQueryService
    {
        private readonly KegMonitorDbContext _dbContext;

        public BeerQueryService(KegMonitorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BeerEditModel> BuildEditModelAsync(int? id)
        {
            if (id == null)
                return new BeerEditModel();

            var beer = await _dbContext.Beers.FirstOrDefaultAsync(b => b.Id == id.Value);
            if (beer == null)
                throw new InvalidOperationException("Beer not found.");

            return new BeerEditModel()
            {
                Id = beer.Id,
                Name = beer.Name,
                Type = beer.Type,
                ABV = beer.ABV,
                Description = beer.Description,
                ImagePath = beer.ImagePath
            };
        }

        public async Task<IEnumerable<Beer>> GetAllAsync()
        {
            return await _dbContext.Beers.AsNoTracking()
                                         .OrderByDescending(b => b.LastUpdatedDate)
                                         .ToListAsync();                                  
        }
    }
}
