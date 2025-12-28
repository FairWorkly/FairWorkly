# DEVLOG - 开发日志

> **记录开发过程中的决策、讨论结论、踩过的坑。**
>
> 这是你的小本本，随时更新。

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
