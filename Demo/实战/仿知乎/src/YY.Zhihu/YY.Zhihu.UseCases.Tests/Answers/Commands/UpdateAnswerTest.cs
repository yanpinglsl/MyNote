using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Commands;
using YY.Zhihu.UseCases.AppUsers.Commands;
using YY.Zhihu.UseCases.Common.Exceptions;
using YY.Zhihu.UseCases.Tests;

namespace YY.Zhihu.UseCases.Tests.Answers.Commands
{
    public class UpdateAnswerTest(IServiceProvider serviceProvider) : TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new UpdateAnswerCommand(1, 1,"王五回答了这个问题"));
            var answer = await DbContext.Answers.FirstAsync(q => q.Id == 1);
            answer.Content.Should().Be("王五回答了这个问题");
            answer.LastModifiedBy.Should().Be(1);
            answer.LastModifiedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromMilliseconds(50));
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task ShouldQuestionNoExists()
        {
            var result = await Sender.Send(new UpdateAnswerCommand(9999, 1, "王五回答了这个问题"));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain("问题不存在");
        }

        [Fact]
        public async Task ShouldAnswerNoExists()
        {
            var result = await Sender.Send(new UpdateAnswerCommand(1, 9999, "王五回答了这个问题"));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain("回答不存在");
        }

        [Fact]
        public async Task ShouldAnswerIsEmpty()
        {
            var action = async () =>
            {
                await Sender.Send(new UpdateAnswerCommand(1, 1, ""));
            };
            await action.Should().ThrowAsync<ValidationException>();
        }
    }
}
