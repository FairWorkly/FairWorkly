# AI_GUIDE - Payroll 模块开发导航

> **这是你（Claude Code）的项目导航文件。每次开始工作前，先读这个文件。**

---

## 权限声明

### 不能修改的文件（红线）

| 文件/目录 | 原因 |
|-----------|------|
| `FairWorkly.Domain/*/Entities/*.cs` | Entity 已定稿，不可修改 |
| `FairWorkly.Infrastructure/Persistence/FairWorklyDbContext.cs` | DbContext 已配置完成 |
| `.raw_materials/BUSINESS_RULES/*` | 业务规则是法律级约束 |

### 可以修改的文件

| 文件/目录 | 说明 |
|-----------|------|
| `FairWorkly.Application/Payroll/*` | Payroll 模块的业务逻辑 |
| `FairWorkly.Infrastructure/Persistence/Repositories/*` | Repository 实现 |
| `FairWorkly.API/Controllers/*` | API Controller |
| `.doc/*` | 开发文档 |

---

## 项目背景

**FairWorkly** 是一个澳大利亚中小企业薪资合规审计系统。

**Payroll 模块**负责：
1. 解析用户上传的薪资 CSV 文件
2. 同步员工数据到数据库
3. 执行 4 个合规规则检查
4. 输出违规报告

**核心业务流程**：
```
CSV上传 → 解析数据 → 员工Upsert → 4规则检查 → 输出违规报告
```

---

## 技术栈

| 组件 | 技术 |
|------|------|
| 框架 | .NET 8 / ASP.NET Core |
| 数据库 | PostgreSQL |
| ORM | Entity Framework Core |
| CQRS | MediatR |
| 验证 | FluentValidation |
| CSV 解析 | CsvHelper |
| 测试 | xUnit + FluentAssertions + Moq |
| 日志 | Serilog |

---

## 项目结构

```
backend/
├── src/
│   ├── FairWorkly.API/          ← 表现层：Controller
│   ├── FairWorkly.Application/  ← 应用层：用例、服务、编排
│   ├── FairWorkly.Domain/       ← 领域层：Entity、枚举（不可改）
│   └── FairWorkly.Infrastructure/  ← 基础设施：数据库、外部服务
├── tests/
│   └── FairWorkly.UnitTests/    ← 单元测试 + 集成测试
├── .doc/                        ← 开发文档（你维护的）
└── .raw_materials/              ← 原始需求（只读参考）
```

---

## 开发任务（当前状态）

| Issue | 名称 | 状态 | 详情 |
|-------|------|------|------|
| ISSUE_01 | CSV 解析 + 员工同步 | ✅ 联调测试通过 (16/16 tests) | [.doc/issues/ISSUE_01_CsvParser_EmployeeSync.md](./issues/ISSUE_01_CsvParser_EmployeeSync.md) |
| ISSUE_02 | 合规规则引擎 | 待开发 | [.doc/issues/ISSUE_02_ComplianceEngine.md](./issues/ISSUE_02_ComplianceEngine.md) |
| ISSUE_03 | Handler 集成 + API | 待开发 | [.doc/issues/ISSUE_03_Handler_API.md](./issues/ISSUE_03_Handler_API.md) |

### ISSUE_01 已完成的交付物

```
src/FairWorkly.Application/Payroll/
├── Interfaces/
│   ├── ICsvParserService.cs
│   └── IEmployeeSyncService.cs
├── Services/
│   ├── CsvParserService.cs
│   ├── EmployeeSyncService.cs
│   └── Models/
│       └── PayrollCsvRow.cs

src/FairWorkly.Infrastructure/Persistence/
├── Repositories/Employees/
│   └── EmployeeRepository.cs
├── Configurations/
│   ├── Auth/
│   │   ├── OrganizationConfiguration.cs
│   │   ├── UserConfiguration.cs
│   │   └── OrganizationAwardConfiguration.cs
│   ├── Employees/
│   │   └── EmployeeConfiguration.cs
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
└── AI_GUIDE.md (EF Core 配置知识点)

tests/FairWorkly.UnitTests/
├── Unit/
│   ├── CsvParserServiceTests.cs (7 tests ✅)
│   └── EmployeeSyncServiceTests.cs (6 tests ✅)
└── Integration/
    └── EmployeeSyncIntegrationTests.cs (3 tests ✅)
```

---

## 文档导航

| 文档 | 用途 |
|------|------|
| [CODING_RULES.md](./CODING_RULES.md) | 编码规范和红线 |
| [SPEC_Payroll.md](./SPEC_Payroll.md) | Payroll 模块技术规格 |
| [TEST_PLAN.md](./TEST_PLAN.md) | 测试方案 |
| [DEVLOG.md](./DEVLOG.md) | 开发日志 |
| [INTEGRATION_TEST_LOG.md](./INTEGRATION_TEST_LOG.md) | 联调测试日志 |

---

## 快速开始

### 1. 了解业务规则

先读这两个文件（只读，不能改）：
- [Payroll_Engine_Logic.md](../.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md) - 费率表、计算逻辑
- [API_Contract.md](../.raw_materials/BUSINESS_RULES/API_Contract.md) - API 契约

### 2. 了解技术约束

- [CODING_STANDARDS.md](../.raw_materials/TECH_CONSTRAINTS/CODING_STANDARDS.md) - 编码规范

### 3. 查看当前任务

- 打开 `.doc/issues/` 目录，找到当前要做的 ISSUE

### 4. 开发流程

```
1. 阅读 ISSUE 文档
2. 编写代码
3. 编写测试
4. 运行测试通过
5. 更新相关的 AI_GUIDE.md
6. 通知人类 Review
```

---

## 已确认的技术决策

| 决策项 | 结果 | 说明 |
|--------|------|------|
| OrganizationId | 硬编码固定 GUID | MVP 阶段使用固定值 |
| 费率表存储 | 代码中静态配置 | 在 RateTableProvider 类中维护 |
| CSV 文件存储 | 持久化保存 | 保存到 `wwwroot/uploads/` |
| 测试数据库 | InMemory + PostgreSQL | 单元测试用 InMemory |

---

## 数据库连接

| 配置项 | 值 |
|--------|-----|
| Host | localhost |
| Port | 5433 |
| Database | FairWorklyDb |
| Username | postgres |
| Password | fairworkly123 |
| Docker 容器 | fairworkly-db |

**注意**: Docker 端口映射为 `5433:5432`（宿主机:容器内部）

---

## 常用命令

```bash
# 运行后端
dotnet run --project src/FairWorkly.API

# 运行测试
dotnet test

# 清库重建（AI Agent 已授权，可随时执行）
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

# 检查数据库连接
docker exec fairworkly-db psql -U postgres -d FairWorklyDb -c "SELECT version();"

# 查看数据库表
docker exec fairworkly-db psql -U postgres -d FairWorklyDb -c "\dt"
```
