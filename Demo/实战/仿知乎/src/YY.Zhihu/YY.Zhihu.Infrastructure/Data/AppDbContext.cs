using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Entites;

namespace YY.Zhihu.Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityUserContext<IdentityUser, int>(options)
    {
        public DbSet<Question> Questions => Set<Question>();

        public DbSet<Answer> Answers => Set<Answer>();

        public DbSet<AnswerLike> AnswerLikes => Set<AnswerLike>();

        public DbSet<AppUser> AppUsers => Set<AppUser>();

        public DbSet<FollowUser> FollowUsers => Set<FollowUser>();

        public DbSet<FollowQuestion> FollowQuestions => Set<FollowQuestion>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
