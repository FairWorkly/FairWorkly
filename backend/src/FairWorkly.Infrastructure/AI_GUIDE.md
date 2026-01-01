# Infrastructure Layer AI_GUIDE

> **Infrastructure 层导航。包含数据库访问、外部服务集成。**

> ⚠️ **宪法文档提醒**：开始任何工作前，先阅读 [.raw_materials/AI_README_FIRST.md](../../.raw_materials/AI_README_FIRST.md)。
> `.raw_materials/` 和 `README.md` 是只读的。详见 [.doc/AI_GUIDE.md](../../.doc/AI_GUIDE.md)。

---

## 概述

Infrastructure 层负责与外部系统的交互，包括数据库、文件存储、AI 服务等。

---

## 目录结构

```
FairWorkly.Infrastructure/
├── Persistence/
│   ├── FairWorklyDbContext.cs        ← 🔒 不可修改（红线）
│   ├── UnitOfWork.cs
│   ├── Configurations/               ← Entity 配置
│   │   ├── Auth/
│   │   ├── Employees/
│   │   ├── Payroll/
│   │   ├── Compliance/
│   │   ├── Documents/
│   │   └── Awards/
│   ├── Repositories/                 ← Repository 实现
│   │   └── Employees/
│   │       └── EmployeeRepository.cs  ✅ 已实现
│   └── AI_GUIDE.md                   ← EF Core 配置指南
├── Services/
│   ├── DateTimeProvider.cs           ✅
│   ├── FileStorageService.cs         ✅
│   └── AiClient.cs                   ✅
└── DependencyInjection.cs            ← DI 注册
```

---

## 开发状态

### 已实现

| 组件 | 状态 |
|------|------|
| FairWorklyDbContext | ✅ 已存在（不可修改）|
| EmployeeRepository | ✅ ISSUE_01 完成 |
| DateTimeProvider | ✅ |
| FileStorageService | ✅ |
| Entity Configurations | ✅ 所有配置已完成 |

### 待实现 (ISSUE_03)

| 组件 | 状态 |
|------|------|
| PayslipRepository | ⏳ |
| PayrollValidationRepository | ⏳ |
| PayrollIssueRepository | ⏳ |

---

## 不可修改的文件

| 文件 | 原因 |
|------|------|
| `FairWorklyDbContext.cs` | SaveChangesAsync 审计逻辑已配置 |

---

## Repository 接口位置

Repository 接口定义在 **Application 层**：

```
FairWorkly.Application/Payroll/Interfaces/
├── IPayslipRepository.cs           ⚠️ 空壳
├── IPayrollValidationRepository.cs ⚠️ 空壳
└── IPayrollIssueRepository.cs      ⚠️ 空壳

FairWorkly.Application/Employees/Interfaces/
└── IEmployeeRepository.cs          ✅ 已定义
```

---

## DI 注册规则

所有 Infrastructure 层服务在 `DependencyInjection.cs` 中注册：

```csharp
public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
{
    // DbContext
    services.AddDbContext<FairWorklyDbContext>(...);

    // Repositories
    services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // ✅ 已注册
    // PayslipRepository - 待 ISSUE_03
    // PayrollValidationRepository - 待 ISSUE_03
    // PayrollIssueRepository - 待 ISSUE_03

    // Services
    services.AddScoped<IDateTimeProvider, DateTimeProvider>(); // ✅
    services.AddScoped<IFileStorageService, FileStorageService>(); // ✅
}
```

---

## 数据库操作

```bash
# 添加 Migration
dotnet ef migrations add MigrationName --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

# 更新数据库
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

# 清库重建（AI Agent 已授权）
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

---

## 文件存储

CSV 文件保存位置：`FairWorkly.API/wwwroot/uploads/`

命名格式：`{timestamp}_{originalFilename}`

---

## 相关文档

- [Persistence/AI_GUIDE.md](./Persistence/AI_GUIDE.md) - EF Core 配置详细指南

---

## 文档矩阵链接

### 上级导航
- [← 返回仓库级 AI_GUIDE](../../AI_GUIDE.md)
- [← .doc/AI_GUIDE.md](../../.doc/AI_GUIDE.md) - 项目状态

### 下级导航
- [→ Persistence 层](./Persistence/AI_GUIDE.md) - EF Core 配置详解

### 同级导航
- [API 层](../FairWorkly.API/AI_GUIDE.md)
- [Application 层](../FairWorkly.Application/AI_GUIDE.md)
- [Tests](../../tests/FairWorkly.UnitTests/AI_GUIDE.md)

### 依赖的 Application 接口
- [Payroll 模块](../FairWorkly.Application/Payroll/AI_GUIDE.md) - Repository 接口定义

---

*最后更新: 2026-01-01*
