# Application Layer AI_GUIDE

> **Application 层导航。包含用例、服务、编排器。**

---

## 概述

Application 层是业务逻辑的主要实现区域，采用 CQRS + Vertical Slicing 架构。

---

## 目录结构

```
FairWorkly.Application/
├── Common/
│   ├── Interfaces/          ← 通用接口
│   └── Behaviors/           ← MediatR 管道行为
├── Payroll/                 ← Payroll 模块（当前开发重点）
├── Compliance/              ← 合规模块
├── Documents/               ← 文档模块
├── Employees/               ← 员工模块
└── DependencyInjection.cs   ← DI 注册
```

---

## 通用接口

| 接口 | 用途 |
|------|------|
| `IAiClient` | AI 服务调用 |
| `ICurrentUserService` | 当前用户上下文 |
| `IDateTimeProvider` | 时间抽象（禁止直接用 DateTime.Now）|
| `IFileStorageService` | 文件存储 |
| `IUnitOfWork` | 工作单元 |

---

## 模块导航

| 模块 | AI_GUIDE | 状态 |
|------|----------|------|
| Payroll | [Payroll/AI_GUIDE.md](./Payroll/AI_GUIDE.md) | 开发中 |
| Compliance | - | 待开发 |
| Documents | - | 已有骨架 |
| Employees | - | 已有骨架 |

---

## DI 注册规则

所有 Application 层服务必须在 `DependencyInjection.cs` 中注册：

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // MediatR
    services.AddMediatR(cfg => { ... });

    // FluentValidation
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    // Services
    services.AddScoped<ICsvParserService, CsvParserService>();
    // ...
}
```

---

## Feature 目录结构（CQRS）

```
Payroll/Features/
└── ValidatePayroll/
    ├── ValidatePayrollCommand.cs      # IRequest<TResponse>
    ├── ValidatePayrollValidator.cs    # AbstractValidator
    ├── ValidatePayrollHandler.cs      # IRequestHandler
    └── ValidationResultDto.cs         # 响应 DTO
```
