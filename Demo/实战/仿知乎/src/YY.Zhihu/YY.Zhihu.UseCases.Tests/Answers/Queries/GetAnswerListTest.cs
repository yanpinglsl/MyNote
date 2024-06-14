using FluentAssertions;
using Xunit;
using YY.Zhihu.SharedLibraries.Paging;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Queries;
using YY.Zhihu.UseCases.Questions.Queries;

namespace YY.Zhihu.UseCases.Tests.Answers.Queries
{
    public class GetAnswerListTest(IServiceProvider serviceProvider):TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new GetAnswerList(1, new Pagination(1, 10)));
            result.Value?.Count.Should().BeLessThanOrEqualTo(10);
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task ShouldAnswerNoExists()
        {
            var result = await Sender.Send(new GetAnswerList(99, new Pagination()));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain("回答不存在");
        }

    }
}
