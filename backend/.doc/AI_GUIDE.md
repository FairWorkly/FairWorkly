# AI_GUIDE - Payroll 模块开发导航

> **这是你（Claude Code）的项目导航文件。每次开始工作前，先读这个文件。**

---

## ⚠️ 宪法级文档声明

> **在阅读任何其他内容之前，必须先阅读 `.raw_materials/` 文件夹。**

### `.raw_materials/` - 项目的"宪法"

此文件夹包含项目的**最高权威文档**，对 AI Agent 具有法律级约束力：

| 子目录 | 权限级别 | 说明 |
|--------|----------|------|
| `BUSINESS_RULES/` | 🔴 **绝对只读** | 费率表、API 契约。任何修改都是违规。 |
| `TECH_CONSTRAINTS/` | 🟡 **只读，可异议** | 技术约束。如有问题可提出，但不能擅自修改。 |
| `REFERENCE/` | 🟢 **只读参考** | 参考资料，可在 `.doc/` 中重新设计。 |

**AI 必须遵守的规则**：
1. **必须先读** - 开始任何工作前，先通读 `.raw_materials/AI_README_FIRST.md`
2. **只能读取** - AI 对 `.raw_materials/` 只有读取权限，没有写入权限
3. **异议机制** - 发现问题可以向人类提出，但**绝不能擅自修改**
4. **业务规则是红线** - `BUSINESS_RULES/` 中的数值、公式、接口结构不可更改

### `README.md` - 人类文档

**任何位置的 `README.md` 文件都是只读的**。

| 规则 | 说明 |
|------|------|
| 不能修改 | `README.md` 是人写给人看的文档 |
| 不能删除 | 即使内容看起来过时 |
| 不能创建 | AI 不应创建新的 README.md |

如果发现 `README.md` 内容与实际代码不符，**向人类报告**，不要自行修改。

---

## 权限声明

### 不能修改的文件（红线）

| 文件/目录 | 原因 |
|-----------|------|
| `FairWorkly.Domain/*/Entities/*.cs` | Entity 已定稿，不可修改 |
| `FairWorkly.Infrastructure/Persistence/FairWorklyDbContext.cs` | DbContext 已配置完成 |
| `.raw_materials/*` | 宪法级文档 |
| `**/README.md` | 人类文档 |

### 可以修改的文件

| 文件/目录 | 说明 |
|-----------|------|
| `FairWorkly.Application/Payroll/*` | Payroll 模块的业务逻辑 |
| `FairWorkly.Infrastructure/Persistence/Repositories/*` | Repository 实现 |
| `FairWorkly.API/Controllers/*` | API Controller |
| `.doc/*` | 开发文档 |
| `**/AI_GUIDE.md` | AI 导航文档 |

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

## 项目进度总览

| 组件 | Issue | 状态 | 完成度 |
|------|-------|------|--------|
| CSV 解析 + 员工同步 | ISSUE_01 | ✅ 完成 | 100% |
| 合规规则引擎 (4规则) | ISSUE_02 | ⏳ 待开发 | 0% |
| Handler + API 集成 | ISSUE_03 | ⏳ 待开发 | 0% |
| **总体进度** | - | - | **33%** |

### 当前任务

**→ ISSUE_02: 合规规则引擎**

详见: [.doc/issues/ISSUE_02_ComplianceEngine.md](./issues/ISSUE_02_ComplianceEngine.md)

---

## 技术栈

| 组件 | 技术 |
|------|------|
| 框架 | .NET 8 / ASP.NET Core |
| 数据库 | PostgreSQL |
| ORM | Entity Framework Core |
| CQRS | MediatR |
| 验证 | FluentValidation |
| CSV 解析 | CsvHelper 33.1.0 |
| 测试 | xUnit + FluentAssertions + Moq |
| 日志 | Serilog |

---

## 项目结构

```
backend/
├── src/
│   ├── FairWorkly.API/              ← 表现层：Controller (骨架)
│   ├── FairWorkly.Application/      ← 应用层：用例、服务、编排
│   │   └── Payroll/
│   │       ├── Services/            ← CsvParser, EmployeeSync (已实现)
│   │       ├── Interfaces/          ← 服务接口
│   │       └── Features/            ← CQRS Handler (待实现)
│   ├── FairWorkly.Domain/           ← 领域层：Entity、枚举（不可改）
│   └── FairWorkly.Infrastructure/   ← 基础设施：数据库、外部服务
│       └── Persistence/
│           └── Repositories/        ← EmployeeRepository (已实现)
├── tests/
│   └── FairWorkly.UnitTests/        ← 单元测试 + 集成测试
├── .doc/                            ← 开发文档（AI 维护）
└── .raw_materials/                  ← 原始需求（只读）
```

---

## 已完成的交付物 (ISSUE_01)

### 代码文件

```
src/FairWorkly.Application/Payroll/
├── Interfaces/
│   ├── ICsvParserService.cs         ✅
│   ├── IEmployeeSyncService.cs      ✅
│   ├── IPayslipRepository.cs        (空壳，待实现)
│   ├── IPayrollValidationRepository.cs (空壳，待实现)
│   └── IPayrollIssueRepository.cs   (空壳，待实现)
├── Services/
│   ├── CsvParserService.cs          ✅
│   ├── EmployeeSyncService.cs       ✅
│   └── Models/
│       └── PayrollCsvRow.cs         ✅
└── Orchestrators/
    └── PayrollAiOrchestrator.cs     (骨架)

src/FairWorkly.Infrastructure/Persistence/Repositories/
└── Employees/
    └── EmployeeRepository.cs        ✅
```

### 测试文件

```
tests/FairWorkly.UnitTests/
├── Unit/
│   ├── CsvParserServiceTests.cs     ✅ 7 tests
│   └── EmployeeSyncServiceTests.cs  ✅ 6 tests
└── Integration/
    └── EmployeeSyncIntegrationTests.cs ✅ 3 tests
```

### DI 注册状态

| 服务 | 状态 |
|------|------|
| ICsvParserService → CsvParserService | ✅ 已注册 |
| IEmployeeSyncService → EmployeeSyncService | ✅ 已注册 |
| IEmployeeRepository → EmployeeRepository | ✅ 已注册 |
| IPayslipRepository | ❌ 未实现 |
| IPayrollValidationRepository | ❌ 未实现 |
| IPayrollIssueRepository | ❌ 未实现 |
| ComplianceEngine Rules | ❌ 未实现 |

---

## 待实现的交付物 (ISSUE_02 + ISSUE_03)

### ISSUE_02: ComplianceEngine

```
src/FairWorkly.Application/Payroll/Services/ComplianceEngine/
├── IComplianceRule.cs               ← 规则接口
├── BaseRateRule.cs                  ← 基础费率检查
├── PenaltyRateRule.cs               ← 罚金费率检查
├── CasualLoadingRule.cs             ← Casual Loading 检查
├── SuperannuationRule.cs            ← 养老金检查
└── RateTableProvider.cs             ← 静态费率表

tests/FairWorkly.UnitTests/Unit/
├── BaseRateRuleTests.cs
├── PenaltyRateRuleTests.cs
├── CasualLoadingRuleTests.cs
└── SuperannuationRuleTests.cs
```

### ISSUE_03: Handler + API

```
src/FairWorkly.Application/Payroll/Features/ValidatePayroll/
├── ValidatePayrollCommand.cs
├── ValidatePayrollValidator.cs
├── ValidatePayrollHandler.cs
└── ValidationResultDto.cs

src/FairWorkly.Infrastructure/Persistence/Repositories/Payroll/
├── PayslipRepository.cs
├── PayrollValidationRepository.cs
└── PayrollIssueRepository.cs

src/FairWorkly.API/Controllers/
└── PayrollController.cs             ← 实现 POST /api/payroll/validation

tests/FairWorkly.UnitTests/Integration/
└── PayrollValidationTests.cs
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

### Issue 文档

| Issue | 文档 | 状态 |
|-------|------|------|
| ISSUE_01 | [CSV 解析 + 员工同步](./issues/ISSUE_01_CsvParser_EmployeeSync.md) | ✅ 完成 |
| ISSUE_02 | [合规规则引擎](./issues/ISSUE_02_ComplianceEngine.md) | ⏳ 当前任务 |
| ISSUE_03 | [Handler 集成 + API](./issues/ISSUE_03_Handler_API.md) | ⏳ 待开发 |

---

## 快速开始

### 1. 了解业务规则

先读这两个文件（只读，不能改）：
- [Payroll_Engine_Logic.md](../.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md) - 费率表、计算逻辑
- [API_Contract.md](../.raw_materials/BUSINESS_RULES/API_Contract.md) - API 契约

### 2. 了解技术约束

- [CODING_STANDARDS.md](../.raw_materials/TECH_CONSTRAINTS/CODING_STANDARDS.md) - 编码规范

### 3. 查看当前任务

- 当前任务: ISSUE_02 (合规规则引擎)
- 打开 [.doc/issues/ISSUE_02_ComplianceEngine.md](./issues/ISSUE_02_ComplianceEngine.md)

### 4. 开发流程

```
1. 阅读 ISSUE 文档
2. 编写代码
3. 编写测试
4. 运行测试通过
5. 更新相关的 AI_GUIDE.md
6. 更新 DEVLOG.md
7. 通知人类 Review
```

---

## 已确认的技术决策

| 决策项 | 结果 | 说明 |
|--------|------|------|
| OrganizationId | 硬编码固定 GUID | MVP 阶段使用固定值 |
| 费率表存储 | 代码中静态配置 | 在 RateTableProvider 类中维护 |
| CSV 文件存储 | 持久化保存 | 保存到 `wwwroot/uploads/` |
| 测试数据库 | InMemory + PostgreSQL | 单元测试用 InMemory |
| 员工姓名拆分 | 空格分隔 | 第一部分 FirstName，其余 LastName |
| 新员工 Email | 占位符 | `{EmployeeNumber}@placeholder.local` |
| 数值容差 | $0.01 / $0.05 | 时薪 0.01，金额 0.05 |

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

# 运行特定测试
dotnet test --filter "FullyQualifiedName~CsvParserServiceTests"

# 清库重建（AI Agent 已授权）
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API

# 检查数据库连接
docker exec fairworkly-db psql -U postgres -d FairWorklyDb -c "SELECT version();"

# 查看数据库表
docker exec fairworkly-db psql -U postgres -d FairWorklyDb -c "\dt"
```

---

## Domain Entity 参考（只读）

已存在的 Entity，在 `FairWorkly.Domain/` 中：

| Entity | 用途 |
|--------|------|
| `Payroll/Entities/Payslip` | 薪资快照记录 |
| `Payroll/Entities/PayrollValidation` | 验证批次记录 |
| `Payroll/Entities/PayrollIssue` | 违规问题记录 |
| `Employees/Entities/Employee` | 员工信息 |

**重要**: 这些 Entity 不可修改，只能使用。

---

## 文档矩阵链接

### 上级导航
- [← 返回仓库级 AI_GUIDE](../AI_GUIDE.md)
- [← Claude Code 入口 (CLAUDE.md)](../CLAUDE.md)

### 同级文档
- [CODING_RULES.md](./CODING_RULES.md) - 编码规范
- [SPEC_Payroll.md](./SPEC_Payroll.md) - 模块规格
- [TEST_PLAN.md](./TEST_PLAN.md) - 测试方案
- [DEVLOG.md](./DEVLOG.md) - 开发日志

### 代码目录导航
- [API 层](../src/FairWorkly.API/AI_GUIDE.md)
- [Application 层](../src/FairWorkly.Application/AI_GUIDE.md)
- [Payroll 模块](../src/FairWorkly.Application/Payroll/AI_GUIDE.md) ← **当前开发重点**
- [Infrastructure 层](../src/FairWorkly.Infrastructure/AI_GUIDE.md)
- [Tests](../tests/FairWorkly.UnitTests/AI_GUIDE.md)

### 宪法文档 (只读)
- [AI_README_FIRST.md](../.raw_materials/AI_README_FIRST.md) - 必读入口
- [BUSINESS_RULES/](../.raw_materials/BUSINESS_RULES/) - 业务规则

---

*最后更新: 2026-01-01*
