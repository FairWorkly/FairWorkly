# Infrastructure Layer AI_GUIDE

> **Infrastructure 层导航。包含数据库访问、外部服务集成。**

---

## 概述

Infrastructure 层负责与外部系统的交互，包括数据库、文件存储、AI 服务等。

---

## 目录结构

```
FairWorkly.Infrastructure/
├── Persistence/
│   ├── FairWorklyDbContext.cs      ← DbContext（不可修改）
│   ├── Configurations/             ← Entity 配置
│   │   └── Payroll/
│   └── Repositories/               ← Repository 实现
│       └── Payroll/
│           ├── PayslipRepository.cs
│           ├── PayrollValidationRepository.cs
│           └── PayrollIssueRepository.cs
├── Services/
│   ├── DateTimeProvider.cs         ← IDateTimeProvider 实现
│   ├── FileStorageService.cs       ← IFileStorageService 实现
│   └── AiClient.cs                 ← IAiClient 实现
└── DependencyInjection.cs          ← DI 注册
```

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
├── IPayslipRepository.cs
├── IPayrollValidationRepository.cs
└── IPayrollIssueRepository.cs
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
    services.AddScoped<IPayslipRepository, PayslipRepository>();
    services.AddScoped<IPayrollValidationRepository, PayrollValidationRepository>();
    // ...

    // Services
    services.AddScoped<IDateTimeProvider, DateTimeProvider>();
    services.AddScoped<IFileStorageService, FileStorageService>();
    // ...
}
```

---

## 数据库操作

```bash
# 添加 Migration
dotnet ef migrations add MigrationName --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

# 更新数据库
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

# 清库重建
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

---

## 文件存储

CSV 文件保存位置：`FairWorkly.API/wwwroot/uploads/`

命名格式：`{timestamp}_{originalFilename}`
