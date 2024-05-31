using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Common;
using YY.Zhihu.UseCases.Common.Interfaces;

namespace YY.Zhihu.Infrastructure.Interceptors
{

    /// <summary>
    /// 自动设置审计属性:使用数据拦截器，在数据更新前设置审计属性
    /// 如果系统只是单纯的用户信息，则使用IUser，含有角色信息则需使用IdentityUser
    /// </summary>
    /// <param name="currentUser"></param>
    public class AuditEntityInterceptor(IUser currentUser) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return result;
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateEntities(eventData.Context);
            return new ValueTask<InterceptionResult<int>>(result);
        }
        public void UpdateEntities(DbContext? dbContext)
        {
            if (dbContext == null) return;
            foreach(var entities in dbContext.ChangeTracker.Entries<AuditBaseEntity>())
            {
                var utcNow = DateTimeOffset.UtcNow;
                if (entities.State == EntityState.Added)
                {
                    entities.Entity.CreatedAt = utcNow;
                }
                else if (entities.State == EntityState.Modified)
                {
                    entities.Entity.LastModifiedAt = utcNow;
                }
            }

            if (currentUser is null ||currentUser.Id is null)  return;
            foreach (var entities in dbContext.ChangeTracker.Entries<AuditUserEntity>())
            {
                if (entities.State == EntityState.Added)
                {
                    entities.Entity.CreatedBy = currentUser.Id;
                }
                else if (entities.State == EntityState.Modified)
                {
                    entities.Entity.LastModifiedBy = currentUser.Id;
                }
            }
        }
    }
}
