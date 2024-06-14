using FluentAssertions;
using Xunit;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.AppUsers.Commands;

namespace YY.Zhihu.UseCases.Tests.AppUsers.Commands
{
    public class CreateFollowQuestionTest(IServiceProvider serviceProvider) : TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new CreateFollowQuestionCommand(1));

            result.IsSuccess.Should().Be(true);
            DbContext.FollowQuestions
                .Count(f => f.UserId == CurrentUser.Id).Should().Be(1);
        }

        [Fact]
        public async Task ShouldQuestionNoExists()
        {
            var result = await Sender.Send(new CreateFollowQuestionCommand(99));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain("关注问题不存在");
        }

        [Fact]
        public async Task ShouldQuestionFollowed()
        {
            await Sender.Send(new CreateFollowQuestionCommand(1));
            var result = await Sender.Send(new CreateFollowQuestionCommand(1));
            result.Status.Should().Be(ResultStatus.Invalid);
            result.Errors.Should().Contain("问题已关注");
        }

        [Fact]
        public async Task ShouldFollowerCountOne()
        {
            await Sender.Send(new CreateFollowQuestionCommand(1));
            var question = await DbContext.Questions.FindAsync(1);
            question!.FollowerCount.Should().Be(1);
        }
    }
}
