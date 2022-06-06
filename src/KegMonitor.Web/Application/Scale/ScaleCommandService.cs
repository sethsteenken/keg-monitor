using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class ScaleCommandService : IScaleCommandService
    {
        private readonly KegMonitorDbContext _dbContext;

        public ScaleCommandService(KegMonitorDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task UpdateActiveStateAsync(int scaleId, bool active)
        {
            var scale = await _dbContext.Scales.FirstOrDefaultAsync(s => s.Id == scaleId);
            if (scale == null)
                throw new InvalidOperationException("Scale not found.");

            scale.Active = active;
            scale.LastUpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> SaveAsync(ScaleEditModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var scale = await _dbContext.Scales.Include(s => s.Beer).FirstOrDefaultAsync(s => s.Id == model.Id);
            if (scale == null)
                throw new InvalidOperationException("Scale not found.");

            if (scale.Active)
                throw new InvalidOperationException("Scale must be inactive to make edits.");

            if (model.BeerId != null)
            {
                var beer = await _dbContext.Beers.FirstOrDefaultAsync(b => b.Id == model.BeerId);
                scale.Beer = beer ?? throw new InvalidOperationException("Beer not found.");
            }
            else
                scale.Beer = null;

            scale.EmptyWeight = model.EmptyWeight;
            scale.FullWeight = model.FullWeight;

            scale.RecordingDifferenceThreshold = model.RecordingDifferenceThreshold;
            scale.PourDifferenceThreshold = model.PourDifferenceThreshold;
            scale.MaxThreshold = model.MaxThreshold;

            scale.UpdateWeight(model.CurrentWeight);
            scale.LastUpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return scale.Id;
        }
    }
}
