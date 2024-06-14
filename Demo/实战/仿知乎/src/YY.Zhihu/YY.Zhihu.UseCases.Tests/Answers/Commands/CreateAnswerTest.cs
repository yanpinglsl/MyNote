using FluentAssertions;
using Xunit;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Commands;
using YY.Zhihu.UseCases.AppUsers.Commands;
using YY.Zhihu.UseCases.Common.Exceptions;
using YY.Zhihu.UseCases.Tests;

namespace YY.Zhihu.UseCases.Tests.Answers.Commands
{
    public class CreateAnswerTest(IServiceProvider serviceProvider) : TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new CreateAnswerCommand(1,"王五认为这不是一个问题"));
            result.GetValue().Should().NotBe(0);
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task ShouldAnswerIsEmpty()
        {
            var action = async () =>
            {
                await Sender.Send(new CreateAnswerCommand(1, ""));
            };
            await action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task ShouldQuestionNoExists()
        {
            var result = await Sender.Send(new CreateAnswerCommand(9999, "王五认为这不是一个问题"));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Equal("问题不存在");
        }
    }
}
