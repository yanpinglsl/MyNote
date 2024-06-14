using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Commands;

namespace YY.Zhihu.UseCases.Tests.Answers.Commands
{
    public class DeleteAnswerLikeTest(IServiceProvider serviceProvider) : TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new CreateAnswerLikeCommand(1, true));
            result = await Sender.Send(new DeleteAnswerLikeCommand(1));
            result.Status.Should().Be(ResultStatus.Ok);            
            var like = DbContext.AnswerLikes.FirstOrDefault(like => like.AnswerId == 1);
            like.Should().BeNull();
        }

        [Fact]
        public async Task ShouldAnswerNoExist()
        {
            var result = await Sender.Send(new DeleteAnswerLikeCommand(9999));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Equal("回答不存在");
        }
        [Fact]
        public async Task ShouldLikeNoExists()
        {
            var result = await Sender.Send(new DeleteAnswerLikeCommand(1));
            result.Status.Should().Be(ResultStatus.Ok);
        }
    }
}
