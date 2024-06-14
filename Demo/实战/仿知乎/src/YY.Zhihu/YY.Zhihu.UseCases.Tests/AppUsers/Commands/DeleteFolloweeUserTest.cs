using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.AppUsers.Commands;

namespace YY.Zhihu.UseCases.Tests.AppUsers.Commands
{
    public class DeleteFolloweeUserTest(IServiceProvider serviceProvider) : TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            await Sender.Send(new CreateFolloweeUserCommand(2));
            var result = await Sender.Send(new DeleteFolloweeUserCommand(2));
            result.Status.Should().Be(ResultStatus.Ok);
            DbContext.FollowUsers.Count().Should().Be(0);
        }


        [Fact]
        public async Task ShouldQuestionNoExists()
        {
            var result = await Sender.Send(new DeleteFolloweeUserCommand(99));
            result.Status.Should().Be(ResultStatus.Ok);
        }
    }
}
