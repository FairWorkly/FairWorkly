using FluentValidation;

namespace FairWorkly.Application.Compliance.Features.TestArchitecture;

public class TestArchitectureValidator : AbstractValidator<TestArchitectureCommand>
{
    public TestArchitectureValidator()
    {
        // 规则 1: 必须有值
        RuleFor(x => x.InputMessage)
            .NotEmpty().WithMessage("Test input message cannot be empty.");

        // 规则 2: 用于测试校验拦截
        // 如果输入是 "error"，则直接报错，测试 GlobalExceptionHandler 是否返回 400
        RuleFor(x => x.InputMessage)
            .NotEqual("error").WithMessage("Triggered validation error for testing purposes!");

        // 规则 3: 员工名字必填
        RuleFor(x => x.TestEmployeeName)
            .NotEmpty().WithMessage("Employee name is required for DB test.");
    }
}