using FluentAssertions;
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
    public class CreateFolloweeUserTest(IServiceProvider serviceProvider) :TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new CreateFolloweeUserCommand(2));
            result.IsSuccess.Should().Be(true);
            DbContext.FollowUsers
                .Count(f => f.FolloweeId == 2).Should().Be(1);
        }


        [Fact]
        public async Task ShouldFolloweeUserNoExists()
        {
            var result = await Sender.Send(new CreateFolloweeUserCommand(3));
            result.IsSuccess.Should().Be(false);
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Equal("关注用户不存在");
        }

        [Fact]
        public async Task ShouldFolloweeOwn()
        {
            var result = await Sender.Send(new CreateFolloweeUserCommand(1));
            result.IsSuccess.Should().Be(false);
            result.Status.Should().Be(ResultStatus.Invalid);
            result.Errors.Should().Equal("不能关注自己");
        }
        [Fact]
        public async Task ShouldFollowed()
        {
            var result = await Sender.Send(new CreateFolloweeUserCommand(2));
            result = await Sender.Send(new CreateFolloweeUserCommand(2));
            result.IsSuccess.Should().Be(false);
            result.Status.Should().Be(ResultStatus.Invalid);
            result.Errors.Should().Equal("该用户已关注");
        }
    }
}
