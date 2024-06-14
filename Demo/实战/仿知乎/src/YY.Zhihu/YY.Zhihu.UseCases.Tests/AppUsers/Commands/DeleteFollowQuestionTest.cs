using FluentAssertions;
using Xunit;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.AppUsers.Commands;
using YY.Zhihu.UseCases.Tests;

namespace YY.Zhihu.UseCases.Tests.AppUsers.Commands
{

    public class DeleteFollowQuestionTest(IServiceProvider serviceProvider) : TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new CreateFollowQuestionCommand(1));
            result.Status.Should().Be(ResultStatus.Ok);
            var result2 = await Sender.Send(new DeleteFollowQuestionCommand(1));
            result2.Status.Should().Be(ResultStatus.Ok);
            DbContext.FollowQuestions.Count().Should().Be(0);
        }


        [Fact]
        public async Task ShouldQuestionNoExists()
        {
            var result = await Sender.Send(new DeleteFollowQuestionCommand(99));
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task ShouldFollowerCountZero()
        {
            await Sender.Send(new CreateFollowQuestionCommand(1));
            await Sender.Send(new DeleteFollowQuestionCommand(1));
            var question = await DbContext.Questions.FindAsync(1);
            question!.FollowerCount.Should().Be(0);
        }
    }
}

