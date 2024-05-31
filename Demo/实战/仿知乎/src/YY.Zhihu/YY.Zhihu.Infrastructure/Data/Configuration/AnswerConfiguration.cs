using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;

namespace YY.Zhihu.Infrastructure.Configuration
{
    public class AnswerConfiguration:IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.Property(p => p.Content)
                .HasColumnType("text")
                .IsRequired();

            // 设置问题与回答列表之间的一对多关系
            builder.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
