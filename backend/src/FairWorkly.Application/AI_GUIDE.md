# Application Layer AI_GUIDE

> **Application 层导航。包含用例、服务、编排器。**

> ⚠️ **宪法文档提醒**：开始任何工作前，先阅读 [.raw_materials/AI_README_FIRST.md](../../.raw_materials/AI_README_FIRST.md)。
> `.raw_materials/` 是只读的，`README.md` 也是只读的。详见 [.doc/AI_GUIDE.md](../../.doc/AI_GUIDE.md)。

> ⚠️ **架构约束**：必读 [ARCHITECTURE.md](../../.raw_materials/TECH_CONSTRAINTS/ARCHITECTURE.md) - Handler/Orchestrator 职责划分。

---

## 概述

Application 层是业务逻辑的主要实现区域，采用 CQRS + Vertical Slicing 架构。

### 核心组件职责

| 组件 | 职责 | 示例 |
|------|------|------|
| **Handler** | 业务流程的"总指挥"，编排整个工作流 | ValidatePayrollHandler |
| **Orchestrator** | 仅封装 AI 调用，不含业务逻辑 | ComplianceAiOrchestrator |
| **Service** | 可复用的业务逻辑 | CsvParserService |
| **Repository** | 数据访问（接口在此层，实现在 Infrastructure） | IPayslipRepository |

**关键原则**：
- 所有业务逻辑判断在 Handler 中
- Orchestrator 只封装 Python AI 服务调用
- 不需要 AI 的功能不创建 Orchestrator

---

## 目录结构

```
FairWorkly.Application/
├── Common/
│   ├── Interfaces/              ← 通用接口
│   │   ├── IAiClient.cs
│   │   ├── ICurrentUserService.cs
│   │   ├── IDateTimeProvider.cs
│   │   ├── IFileStorageService.cs
│   │   └── IUnitOfWork.cs
│   └── Behaviors/               ← MediatR 管道行为
├── Payroll/                     ← Payroll 模块（当前开发重点）
│   ├── Services/                ← 已实现: CsvParser, EmployeeSync
│   ├── Interfaces/              ← 服务接口
│   ├── Features/                ← 待实现: CQRS Handler
│   └── AI_GUIDE.md              ← 模块导航
├── Compliance/                  ← 合规模块
├── Documents/                   ← 文档模块
├── Employees/                   ← 员工模块
│   └── Interfaces/
│       └── IEmployeeRepository.cs
└── DependencyInjection.cs       ← DI 注册
```

---

## 模块开发状态

| 模块 | AI_GUIDE | 状态 |
|------|----------|------|
| **Payroll** | [Payroll/AI_GUIDE.md](./Payroll/AI_GUIDE.md) | ⏳ ISSUE_03 进行中 |
| Compliance | - | 骨架已有 (Orchestrator) |
| Documents | - | 已有骨架 |
| Employees | - | Repository 接口已定义 |

---

## 通用接口

| 接口 | 用途 | 实现位置 |
|------|------|----------|
| `IAiClient` | AI 服务调用 | Infrastructure |
| `ICurrentUserService` | 当前用户上下文 | Infrastructure |
| `IDateTimeProvider` | 时间抽象 | Infrastructure |
| `IFileStorageService` | 文件存储 | Infrastructure |
| `IUnitOfWork` | 工作单元 | Infrastructure |

**重要**: 禁止直接使用 `DateTime.Now`，必须注入 `IDateTimeProvider`。

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

    // Payroll Services
    services.AddScoped<ICsvParserService, CsvParserService>();       // ✅ 已注册
    services.AddScoped<IEmployeeSyncService, EmployeeSyncService>(); // ✅ 已注册
    // ComplianceEngine rules - 待 ISSUE_02 实现后注册
}
```

---

## Feature 目录结构（CQRS）

每个 Feature 遵循以下结构：

```
Payroll/Features/
└── ValidatePayroll/                    ← ISSUE_03 待实现
    ├── ValidatePayrollCommand.cs       # IRequest<TResponse>
    ├── ValidatePayrollValidator.cs     # AbstractValidator
    ├── ValidatePayrollHandler.cs       # IRequestHandler
    └── ValidationResultDto.cs          # 响应 DTO
```

---

## 已注册的服务

| 服务 | 状态 |
|------|------|
| ICsvParserService → CsvParserService | ✅ |
| IEmployeeSyncService → EmployeeSyncService | ✅ |
| IComplianceRule → BaseRateRule | ✅ |
| IComplianceRule → PenaltyRateRule | ✅ |
| IComplianceRule → CasualLoadingRule | ✅ |
| IComplianceRule → SuperannuationRule | ✅ |
| MediatR | ✅ |
| FluentValidation | ✅ |
| ValidationBehavior | ✅ |

## 待注册的服务 (ISSUE_03)

| 服务 | 状态 |
|------|------|
| IPayslipRepository → PayslipRepository | ⏳ |
| IPayrollValidationRepository → PayrollValidationRepository | ⏳ |
| IPayrollIssueRepository → PayrollIssueRepository | ⏳ |

---

## 文档矩阵链接

### 上级导航
- [← 返回仓库级 AI_GUIDE](../../AI_GUIDE.md)
- [← .doc/AI_GUIDE.md](../../.doc/AI_GUIDE.md) - 项目状态

### 下级导航
- [→ Payroll 模块](./Payroll/AI_GUIDE.md) ← **当前开发重点**

### 同级导航
- [API 层](../FairWorkly.API/AI_GUIDE.md)
- [Infrastructure 层](../FairWorkly.Infrastructure/AI_GUIDE.md)
- [Tests](../../tests/FairWorkly.UnitTests/AI_GUIDE.md)

---

*最后更新: 2026-01-02 (添加架构约束，同步 ISSUE_02 完成状态)*
