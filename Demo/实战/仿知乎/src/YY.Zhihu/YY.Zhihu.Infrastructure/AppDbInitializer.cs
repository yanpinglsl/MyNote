using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Entites;

namespace YY.Zhihu.Infrastructure
{
    public class AppDbInitializer(
        ILogger<AppDbInitializer> logger,
        AppDbContext context,
        UserManager<IdentityUser> userManager)
    {
        public async Task InitializeAsync()
        {
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "初始化数据库出错");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "创建种子数据出错");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            // 初始认证用户
            var authUser = new IdentityUser { UserName = "Zilor@zhaoxi.com", Email = "zilor@zhaoxi.com" };

            if (!userManager.Users.Any(u => u.UserName == authUser.UserName))
            {
                await userManager.CreateAsync(authUser, "1234aA!");

                // 创建业务用户
                var appUser = new AppUser(authUser.Id) { Nickname = "zilor", Bio = "这个家伙很懒没有留下任何信息" };
                context.AppUsers.Add(appUser);
            }

            // 随机生成一些文本作为虚拟数据
            if (!context.Questions.Any())
            {
                var answerFaker = new Faker<Answer>("zh_CN")
                    .RuleFor(a => a.Content, f => f.Lorem.Paragraphs())
                    .RuleFor(a => a.CreatedBy, authUser.Id);

                var quesionsFaker = new Faker<Question>("zh_CN")
                    .RuleFor(q => q.Title, f => f.Lorem.Sentence(10))
                    .RuleFor(q => q.Description, f => f.Lorem.Paragraphs())
                    .RuleFor(q => q.ViewCount, f => f.Random.Number(99999))
                    .RuleFor(q => q.Answers, f => answerFaker.GenerateBetween(1, 30))
                    .RuleFor(q => q.CreatedBy, authUser.Id);

                var questions = quesionsFaker.Generate(50);

                context.Questions.AddRange(questions);
                await context.SaveChangesAsync();
            }
        }
    }
}
