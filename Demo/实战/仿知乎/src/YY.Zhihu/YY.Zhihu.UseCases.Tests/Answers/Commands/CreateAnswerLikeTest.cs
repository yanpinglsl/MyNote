﻿using FluentAssertions;
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
    public class CreateAnswerLikeTest(IServiceProvider serviceProvider) : TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new CreateAnswerLikeCommand(1, true));
            result.Status.Should().Be(ResultStatus.Ok);

            var like = DbContext.AnswerLikes.FirstOrDefault(like => like.AnswerId == 1);
            like.Should().NotBeNull();
            like!.IsLike.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldAnswerNoExist()
        {
            var result = await Sender.Send(new CreateAnswerLikeCommand(9999, true));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Equal("回答不存在");
        }

        [Fact]
        public async Task ShouldAnswerLiked()
        {
            var result = await Sender.Send(new CreateAnswerLikeCommand(1, true));
            result = await Sender.Send(new CreateAnswerLikeCommand(1, true));
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Equal("已赞或已踩");
        }
    }
}
