using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Common;
using YY.Zhihu.SharedLibraries.Domain;

namespace YY.Zhihu.Infrastructure.Data.Interceptors
{
    public class DispatchDomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
    {
        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            DispatchDomainEvents(eventData.Context);
            return base.SavedChanges(eventData, result);
        }
        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default(CancellationToken))
        {
            await DispatchDomainEvents(eventData.Context);
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public async Task DispatchDomainEvents(DbContext? dbContext)
        {
            if (dbContext == null) return;
            var entities = dbContext?.ChangeTracker
                  .Entries<BaseEntity>()
                  .Where(l => l.Entity.DomainEvents.Any())
                  .Select(l => l.Entity)
                  .ToList();

            var domainEvets = entities
                .SelectMany(l => l.DomainEvents)
                .ToList();
            entities.ForEach(e => e.ClearDomainEvent());
            foreach (var item in domainEvets)
            {
                await publisher.Publish(item);
            }

        }
    }

}
