# AI_GUIDE - Infrastructure/Persistence 层导航

> **EF Core 配置和数据库访问相关的开发指南**

---

## 核心机制

### Configuration 自动加载

`FairWorklyDbContext.cs` 中使用了自动配置加载：

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // 自动加载当前程序集下所有 IEntityTypeConfiguration<T> 实现
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
```

**这意味着**：
- 所有 `Configurations/` 目录下的配置类会被自动发现和应用
- 新增 Entity 时，只需创建对应的 Configuration 文件，无需修改 DbContext
- Configuration 文件必须实现 `IEntityTypeConfiguration<TEntity>` 接口

---

## 目录结构

```
Persistence/
├── FairWorklyDbContext.cs          ← 🔒 不可修改（CODING_RULES 红线）
├── UnitOfWork.cs
├── AI_GUIDE.md                     ← 本文件
├── Configurations/
│   ├── Auth/
│   │   ├── OrganizationConfiguration.cs
│   │   ├── UserConfiguration.cs
│   │   └── OrganizationAwardConfiguration.cs
│   ├── Employees/
│   │   └── EmployeeConfiguration.cs
│   ├── Payroll/
│   │   ├── PayslipConfiguration.cs
│   │   ├── PayrollValidationConfiguration.cs
│   │   └── PayrollIssueConfiguration.cs
│   ├── Compliance/
│   │   ├── RosterConfiguration.cs
│   │   ├── RosterValidationConfiguration.cs
│   │   ├── ShiftConfiguration.cs
│   │   └── RosterIssueConfiguration.cs
│   ├── Documents/
│   │   └── DocumentConfiguration.cs
│   └── Awards/
│       ├── AwardConfiguration.cs
│       └── AwardLevelConfiguration.cs
└── Repositories/
    └── Employees/
        └── EmployeeRepository.cs
```

---

## 常见问题

### 问题：EF Core 无法推断导航属性关系

**错误示例**：
```
Unable to determine the relationship represented by navigation 'Organization.CreatedByUser' of type 'User'
```

**原因**：
1. Entity 有导航属性（如 `CreatedByUser`）
2. 但没有对应的 Configuration 文件来配置关系
3. EF Core 无法自动推断复杂的双向关系

**解决方案**：
在 `Configurations/` 对应目录下创建 Configuration 文件，显式配置关系：

```csharp
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        // 显式配置导航属性关系
        builder.HasOne(o => o.CreatedByUser)
               .WithMany()
               .HasForeignKey(o => o.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

---

## 新增 Entity 检查清单

当需要让 EF Core 管理一个新 Entity 时：

- [ ] Entity 是否继承自 `BaseEntity` 或 `AuditableEntity`？
- [ ] 如果继承 `AuditableEntity`，是否需要配置 `CreatedByUser`/`UpdatedByUser` 关系？
- [ ] 是否需要在 `Configurations/` 下创建配置文件？
- [ ] 是否需要在 `DbContext` 中添加 `DbSet`？（如果直接查询需要）
- [ ] 导航属性的关系是否已正确配置？

---

## 审计字段说明

`AuditableEntity` 包含以下审计字段：

| 字段 | 类型 | 说明 |
|------|------|------|
| CreatedByUserId | Guid? | 创建者用户 ID |
| CreatedByUser | User? | 创建者导航属性 |
| UpdatedByUserId | Guid? | 更新者用户 ID |
| UpdatedByUser | User? | 更新者导航属性 |
| UpdatedAt | DateTimeOffset? | 更新时间 |

这些字段由 `SaveChangesAsync` 自动处理（TODO: JWT 认证后完善）。

---

## PostgreSQL 命名约定

本项目使用 `.UseSnakeCaseNamingConvention()` 来配置 PostgreSQL 的命名约定：

```csharp
// DependencyInjection.cs
services.AddDbContext<FairWorklyDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
);
```

**重要提示**：在测试中创建 DbContext 时也需要添加此配置，否则表名会不匹配：

```csharp
var options = new DbContextOptionsBuilder<FairWorklyDbContext>()
    .UseNpgsql(_connectionString)
    .UseSnakeCaseNamingConvention()  // 必须添加!
    .Options;
```

---

## DateTime 处理

PostgreSQL 的 `timestamp with time zone` 类型要求 DateTime 必须有明确的 Kind：

```csharp
// ❌ 错误: DateTime.Kind = Unspecified
StartDate = _dateTimeProvider.UtcNow.DateTime

// ✅ 正确: DateTime.Kind = Utc
StartDate = _dateTimeProvider.UtcNow.UtcDateTime
```

---

## 相关文档

- [CODING_RULES.md](../../../.doc/CODING_RULES.md) - 编码规范
- [AI_GUIDE.md](../../../.doc/AI_GUIDE.md) - 项目主导航
- [INTEGRATION_TEST_LOG.md](../../../.doc/INTEGRATION_TEST_LOG.md) - 联调测试日志
