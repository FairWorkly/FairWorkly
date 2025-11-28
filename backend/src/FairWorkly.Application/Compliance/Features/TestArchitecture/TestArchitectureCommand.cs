using MediatR;

namespace FairWorkly.Application.Compliance.Features.TestArchitecture;

// IRequest<T> 表示这个命令会有返回值
public class TestArchitectureCommand : IRequest<TestArchitectureResultDto>
{
    // 测试输入：如果是 "fail"，Validator 应该拦截
    public string InputMessage { get; set; } = string.Empty;

    // 测试输入：用于写入数据库的员工名字
    public string TestEmployeeName { get; set; } = string.Empty;
}