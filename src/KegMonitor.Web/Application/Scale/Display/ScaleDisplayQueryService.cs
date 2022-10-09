using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class ScaleDisplayQueryService : IScaleDisplayQueryService
    {
        private readonly IDbContextFactory<KegMonitorDbContext> _dbContextFactory;

        public ScaleDisplayQueryService(
            IDbContextFactory<KegMonitorDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<ScaleDisplayItem>> GetScalesAsync()
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var scales = await context.Scales.OrderBy(s => s.Id).Include(s => s.Beer).ToListAsync();

            return scales.Select(s => new ScaleDisplayItem()
            {
                Id = s.Id,
                Weight = s.CurrentWeight,
                Percentage = (double)s.Percentage,
                Active = s.Active,
                Beer = s.Beer == null ? null : new BeerDisplayItem()
                {
                    Name = s.Beer.Name,
                    Type = s.Beer.Type,
                    ABV = s.Beer.ABV,
                    Description = s.Beer.Description,
                    ImagePath = s.Beer.ImagePath
                }
            }).ToList();
        }
    }
}
