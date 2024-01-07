using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace ModelValidatorDemo.Sample03.Validators
{
    public class ProductValidator : AbstractValidator<Product> 
    {
        public ProductValidator()
        {
            RuleFor(dto => dto.Name).NotNull().NotEmpty();
            RuleFor(dto => dto.Description).NotNull().NotEmpty();
            RuleFor(dto => dto.Description)
                .Must(desc => !desc.IsNullOrEmpty() && desc.StartsWith("说明"))
                .WithMessage("必须以'说明'开头");

            RuleFor(dto => dto.Price).InclusiveBetween(0.01, 999.99);
        }
    }
}
