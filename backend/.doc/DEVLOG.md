# DEVLOG - 开发日志

> **记录开发过程中的决策、讨论结论、踩过的坑。**
>
> 这是你的小本本，随时更新。

---

## 2026-01-01 ISSUE_02 Review 与测试覆盖补充

### 变更内容

**ISSUE_02_ComplianceEngine.md 文档更新**：

1. **新增 Pre-Validation 章节**
   - 位置：Orchestrator 层（不在 ComplianceEngine 中）
   - 逻辑：检查必填字段完整性，缺失则输出 WARNING 并跳过所有规则

2. **修正 SuperannuationRule 逻辑**
   - 添加 `AnyWorkHours` 检查
   - 当 Gross Pay = 0 但有工时时，输出 WARNING

3. **添加 PenaltyRateRule 重要注释**
   - 强调即使是 Casual 员工，计算基数也必须使用 Permanent Rate

4. **添加 INFO 级别输出规则**
   - 规则通过时不输出 PayrollIssue
   - 统计摘要记录在 PayrollValidation 层

5. **更新测试用例表**
   - 细化到行级别的测试场景
   - 添加预期 Severity

### 测试覆盖补充

发现并填补了 3 个测试覆盖缺口：

| 缺口 | 新增测试数据 | 预期 Severity |
|------|-------------|---------------|
| GAP-1 | `TEST_05_BaseRate_Violations.csv` 行6 (WARN001) | WARNING |
| GAP-2 | `TEST_12_Super_Violations.csv` 行6 (SUPWARN001) | WARNING |
| GAP-3 | `TEST_17_PreValidation.csv` (新文件) | WARNING + Skip |

### 关键决策

| 问题 | 决策 | 理由 |
|------|------|------|
| Pre-Validation 位置 | Orchestrator 层 | 职责分离：数据完整性 vs 业务合规 |
| INFO 级别输出 | 不输出 | PayrollIssue 语义是"问题"，通过不是问题 |
| AnyWorkHours 检查 | 保留 | 边界保护：有工时但无 Gross Pay 是数据异常 |

### 与人类的讨论

人类同意所有建议，并在 `Payroll_Engine_Logic.md` 的 Severity 定义中明确：
> "规则通过时不输出 PayrollIssue，仅在 PayrollValidation 层记录统计摘要。"

---

## 2026-01-01 文档工程全面更新

### 变更内容

**文档权限体系建立**：

人类要求全面更新文档，强调 `.raw_materials/` 的宪法级地位：

| 文档类型 | 权限 | 说明 |
|----------|------|------|
| `.raw_materials/BUSINESS_RULES/` | 🔴 绝对只读 | 费率表、API 契约 |
| `.raw_materials/TECH_CONSTRAINTS/` | 🟡 只读可异议 | 技术约束 |
| `.raw_materials/REFERENCE/` | 🟢 只读参考 | 参考资料 |
| `**/README.md` | 🔴 只读 | 人类文档 |
| `.doc/*` | ✅ 可读写 | AI 工作文档 |
| `**/AI_GUIDE.md` | ✅ 可读写 | AI 导航文档 |

**更新的文件清单**：

1. `CLAUDE.md` - 添加 Constitutional Documents 章节
2. `.doc/AI_GUIDE.md` - 全面重写，反映当前项目状态
3. `.doc/CODING_RULES.md` - 添加文档权限层级说明
4. `.doc/issues/ISSUE_01_*.md` - 标记为已完成
5. `.doc/issues/ISSUE_02_*.md` - 标记为当前任务
6. `.doc/issues/ISSUE_03_*.md` - 更新前置依赖状态
7. `src/FairWorkly.Application/AI_GUIDE.md` - 添加宪法提醒
8. `src/FairWorkly.Application/Payroll/AI_GUIDE.md` - 添加宪法提醒
9. `src/FairWorkly.Infrastructure/AI_GUIDE.md` - 添加宪法提醒
10. `src/FairWorkly.Infrastructure/Persistence/AI_GUIDE.md` - 添加宪法提醒

### 项目状态确认

经代码库探索确认：

| 组件 | 状态 | 完成度 |
|------|------|--------|
| ISSUE_01 (CSV + Sync) | ✅ 完成 | 100% |
| ISSUE_02 (ComplianceEngine) | ⏳ 未开始 | 0% |
| ISSUE_03 (Handler + API) | ⏳ 未开始 | 0% |
| **总体** | - | **33%** |

### 已实现的代码

- `CsvParserService.cs` - CSV 解析
- `EmployeeSyncService.cs` - 员工同步
- `EmployeeRepository.cs` - 员工仓储
- 16 个测试用例全部通过

### 待实现的代码

- ComplianceEngine (4 个规则类 + RateTableProvider)
- Payroll Repositories (Payslip, Validation, Issue)
- ValidatePayroll Handler + API

---

## 2025-12-28 项目初始化

### 已确认的技术决策

| 决策项 | 结果 | 原因 |
|--------|------|------|
| OrganizationId | 硬编码固定 GUID | MVP 阶段暂不实现 JWT 认证 |
| 费率表存储 | 代码中静态配置 | 费率不常变，静态配置更简单 |
| CSV 文件存储 | 持久化保存到 `wwwroot/uploads/` | 便于审计追溯 |
| 测试数据库 | 单元测试 InMemory，集成测试 PostgreSQL | 平衡速度和真实性 |

### 项目状态

- [x] 阅读所有原始文档
- [x] 生成 `.doc/` 目录下的文档
- [x] ISSUE_01: CSV 解析 + 员工同步 (2025-12-28 完成)
- [ ] ISSUE_02: 合规规则引擎
- [ ] ISSUE_03: Handler 集成 + API 暴露

---

## 2025-12-28 ISSUE_01 CSV Parser + Employee Sync - Completed

### 变更内容

**新建文件**:
- `src/FairWorkly.Application/Payroll/Services/Models/PayrollCsvRow.cs` - CSV 行数据模型
- `src/FairWorkly.Application/Payroll/Interfaces/ICsvParserService.cs` - CSV 解析服务接口
- `src/FairWorkly.Application/Payroll/Services/CsvParserService.cs` - CSV 解析服务实现
- `src/FairWorkly.Application/Payroll/Interfaces/IEmployeeSyncService.cs` - 员工同步服务接口
- `src/FairWorkly.Application/Payroll/Services/EmployeeSyncService.cs` - 员工同步服务实现
- `src/FairWorkly.Infrastructure/Persistence/Repositories/Employees/EmployeeRepository.cs` - 员工仓储实现
- `tests/FairWorkly.UnitTests/Unit/CsvParserServiceTests.cs` - CSV 解析服务单元测试 (7 tests)
- `tests/FairWorkly.UnitTests/Unit/EmployeeSyncServiceTests.cs` - 员工同步服务单元测试 (6 tests)

**修改文件**:
- `src/FairWorkly.Application/Employees/Interfaces/IEmployeeRepository.cs` - 添加 GetByEmployeeNumbersAsync, CreateAsync, UpdateAsync 方法
- `src/FairWorkly.Application/DependencyInjection.cs` - 注册 ICsvParserService 和 IEmployeeSyncService
- `src/FairWorkly.Infrastructure/DependencyInjection.cs` - 注册 IEmployeeRepository

**NuGet 包**:
- 添加 CsvHelper 33.1.0 到 Application 层
- 添加 FluentAssertions 8.8.0 到测试项目
- 添加 Moq 4.20.72 到测试项目
- 添加 Microsoft.EntityFrameworkCore.InMemory 8.0.0 到测试项目

### 技术决策

| 决策项 | 结果 | 原因 |
|--------|------|------|
| CSV 解析库 | CsvHelper 33.1.0 | 成熟稳定，支持 ClassMap 映射，容错处理好 |
| 员工姓名拆分 | 按空格拆分，第一部分为 FirstName，其余为 LastName | 简单实用，适用于大多数场景 |
| 新员工 Email | 使用 `{EmployeeNumber}@placeholder.local` | MVP 阶段占位符，未来可由用户管理 |
| 新员工 JobTitle | 默认 "Employee" | MVP 阶段简化处理 |
| Award Type 解析 | 支持 "Retail", "MA000004", "Hospitality", "Clerks" 等多种格式 | 兼容不同 CSV 来源 |
| Classification 解析 | 从 "Level X" 提取数字 | 符合业务规范 |
| EmploymentType 解析 | 支持连字符和空格分隔的变体 | 容错处理，提高兼容性 |
| 测试策略 | 单元测试 + Mock，单独测试每个 Service | 快速反馈，隔离性好 |

### 实现细节

**CsvParserService**:
- 使用 CsvHelper 的 ClassMap 进行字段映射
- 必填字段缺失时记录错误并继续处理其他行
- 可选字段（周末、公休工时）缺失时使用默认值 0
- 返回 (成功解析的行, 错误列表) 元组

**EmployeeSyncService**:
- Upsert 逻辑：根据 EmployeeNumber + OrganizationId 判断是否存在
- 存在则更新：FirstName, LastName, AwardType, AwardLevelNumber, EmploymentType
- 不存在则创建：生成占位符 Email 和默认 JobTitle
- 返回 EmployeeNumber → EmployeeId 映射字典供后续使用

**EmployeeRepository**:
- GetByEmployeeNumbersAsync: 批量查询员工，避免 N+1 问题
- CreateAsync: 创建新员工，DbContext 自动生成 Id
- UpdateAsync: 更新现有员工

### 测试覆盖

**CsvParserServiceTests** (7 tests):
1. ParseAsync_ValidCsv_ReturnsRows - 正常 CSV 解析
2. ParseAsync_MissingRequiredField_ReturnsError - 必填字段缺失
3. ParseAsync_OptionalFieldsMissing_UsesDefaultValues - 可选字段默认值
4. ParseAsync_EmptyStream_ReturnsEmptyList - 空文件处理
5. ParseAsync_InvalidDateFormat_ReturnsError - 日期格式错误
6. ParseAsync_NegativeHourlyRate_ReturnsError - 数值验证
7. ParseAsync_FromTestFile_TEST_01_NewEmployees - 真实测试文件

**EmployeeSyncServiceTests** (6 tests):
1. SyncEmployeesAsync_NewEmployees_CreatesEmployees - 创建新员工
2. SyncEmployeesAsync_ExistingEmployees_UpdatesEmployees - 更新现有员工
3. SyncEmployeesAsync_MixedScenario_CreatesAndUpdates - 混合场景
4. SyncEmployeesAsync_ParsesEmploymentTypeCorrectly - 雇佣类型解析
5. SyncEmployeesAsync_ParsesAwardLevelCorrectly - Award Level 解析
6. SyncEmployeesAsync_ParsesNameCorrectly - 姓名解析

**测试结果**: All 13 tests passed ✅

### 遵守的编码规范

- ✅ 金额字段使用 `decimal`
- ✅ 日期使用 `DateOnly` (PayPeriodStart, PayPeriodEnd)
- ✅ 时间戳使用 `DateTimeOffset`
- ✅ 注入 `IDateTimeProvider` 获取当前时间
- ✅ 服务注册在对应层的 `DependencyInjection.cs`
- ✅ Repository 接口在 Application 层，实现在 Infrastructure 层
- ✅ 代码注释和命名使用 English
- ✅ 不修改 Domain 层 Entity

### 下一步

ISSUE_01 已完成，准备进入 ISSUE_02: 合规规则引擎开发

---

## 模板：后续记录格式

### YYYY-MM-DD 标题

**变更内容**：
- 做了什么

**决策**：
- 为什么这样做

**问题/坑**：
- 遇到什么问题，怎么解决的

**和人类的讨论**：
- 如果有讨论，记录结论
