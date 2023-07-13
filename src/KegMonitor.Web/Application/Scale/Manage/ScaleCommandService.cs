using KegMonitor.Core.Entities;
using KegMonitor.Infrastructure.EntityFramework;
using KegMonitor.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace KegMonitor.Web.Application
{
    public class ScaleCommandService : IScaleCommandService
    {
        private readonly KegMonitorDbContext _dbContext;
        private readonly IManagedMqttClient _mqttClient;

        public ScaleCommandService(
            KegMonitorDbContext dbContext,
            IManagedMqttClient mqttClient)
        {
            _dbContext = dbContext;
            _mqttClient = mqttClient;
        }

        public async Task<int> AddAsync(ScaleAddModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var scale = new Scale(model.Id, model.Endpoint);

            scale.LastUpdatedDate = DateTime.UtcNow;

            await _mqttClient.SubscribeAsync(scale.Topic);

            await _dbContext.Scales.AddAsync(scale);
            await _dbContext.SaveChangesAsync();

            return scale.Id;
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
            scale.PourDifferenceThreshold = model.PourDifferenceThreshold;
            scale.Endpoint = model.Endpoint;

            if (scale.Topic != model.Topic)
            {
                string oldTopic = scale.Topic;
                scale.Topic = model.Topic;

                await _mqttClient.RefreshTopicSubscriptionAsync(oldTopic, scale.Topic);
            }
            
            scale.LastUpdatedDate = DateTime.UtcNow;

            scale.ForceSetCurrentWeight(model.CurrentWeight);

            await _dbContext.SaveChangesAsync();

            return scale.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var scale = await _dbContext.Scales.FirstOrDefaultAsync(s => s.Id == id);
            if (scale == null)
                throw new InvalidOperationException("Scale not found.");

            await _dbContext.ScaleWeightChanges.Where(swc => swc.Scale.Id == id)
                                               .ExecuteDeleteAsync();

            await _dbContext.BeerPours.Where(swc => swc.Scale.Id == id)
                                      .ExecuteDeleteAsync();

            _dbContext.Scales.Remove(scale);
            await _dbContext.SaveChangesAsync();
        }
    }
}
