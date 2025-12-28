# INTEGRATION_TEST_LOG - 联调测试日志

> **记录每次联调测试的计划、执行过程和结果。**
>
> 这是 AI Agent 的测试记忆，帮助新 session 了解测试状态。

---

## 2025-12-28 ISSUE_01 联调测试

### 测试目标

验证 ISSUE_01（CSV 解析 + 员工同步）的代码能否正确与真实 PostgreSQL 数据库交互。

### 前置条件

- [x] Docker 已启动
- [x] PostgreSQL 容器 `fairworkly-db` 运行中
- [x] 端口映射：`5433:5432`
- [x] 数据库：`FairWorklyDb`
- [x] 连接字符串已配置：`Host=localhost;Port=5433;Database=FairWorklyDb;Username=postgres;Password=fairworkly123`

### 测试计划

#### Step 1: 清库重建
```bash
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```
**预期**: 数据库表结构重建成功

#### Step 2: 验证表结构
```bash
docker exec fairworkly-db psql -U postgres -d FairWorklyDb -c "\dt"
```
**预期**: 应包含 `employees` 表

#### Step 3: 编写并运行集成测试

创建真实数据库集成测试，测试场景：
1. **TC-SYNC-001**: 解析 TEST_01_NewEmployees.csv → 创建 5 个新员工 → 验证数据库中有 5 条记录
2. **TC-SYNC-002**: 预插入员工 → 解析 TEST_02_UpdateEmployees.csv → 验证员工信息已更新
3. **TC-SYNC-003**: 混合场景测试

#### Step 4: 验证数据库数据
```bash
docker exec fairworkly-db psql -U postgres -d FairWorklyDb -c "SELECT * FROM employees;"
```

### 测试结果

| Step | 状态 | 备注 |
|------|------|------|
| Step 1: 清库重建 | ✅ 通过 | 删除旧 migration，重新生成干净的 InitialCreate |
| Step 2: 验证表结构 | ✅ 通过 | 15 个表创建成功，包括 employees 表 |
| Step 3: 集成测试 | ✅ 通过 | 3/3 测试用例全部通过 |
| Step 4: 验证数据 | ✅ 通过 | 数据库中正确存储了员工数据 |

### 集成测试详情

测试文件：`tests/FairWorkly.UnitTests/Integration/EmployeeSyncIntegrationTests.cs`

```
测试总数: 3
通过数: 3
总时间: 2.3497 秒

✅ TC_SYNC_001_NewEmployees_CreatesInDatabase [465 ms]
✅ TC_SYNC_002_UpdateEmployees_UpdatesInDatabase [35 ms]
✅ TC_SYNC_003_MixedScenario_CreatesAndUpdates [62 ms]
```

### 问题记录

#### 问题 1: EF Core 关系配置缺失

**错误信息**：
```
Unable to determine the relationship represented by navigation 'Organization.CreatedByUser' of type 'User'
```

**原因**：Entity 继承了 `AuditableEntity`，但没有对应的 Configuration 文件来配置 `CreatedByUser`/`UpdatedByUser` 导航属性关系。

**解决方案**：创建 Configuration 文件（Auth, Compliance, Documents, Awards 模块）显式配置关系。

#### 问题 2: DateTime Kind 未指定

**错误信息**：
```
Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone', only UTC is supported
```

**原因**：`EmployeeSyncService` 中使用 `_dateTimeProvider.UtcNow.DateTime` 返回的 DateTime 没有 Kind 属性。

**解决方案**：改为使用 `_dateTimeProvider.UtcNow.UtcDateTime`，确保 DateTime.Kind = Utc。

#### 问题 3: Snake Case 命名约定

**错误信息**：
```
relation "Organization" does not exist
```

**原因**：测试中创建 DbContext 时没有调用 `.UseSnakeCaseNamingConvention()`，导致查询的表名大小写不匹配。

**解决方案**：在测试的 DbContextOptionsBuilder 中添加 `.UseSnakeCaseNamingConvention()`。

#### 问题 4: Migration 与 Entity 不同步

**原因**：旧的 migration 是基于早期 Entity 定义生成的，与当前 Entity 结构不匹配。

**解决方案**：删除所有 migration，重新生成干净的 InitialCreate migration。

---

## 模板：后续测试记录格式

### YYYY-MM-DD ISSUE_XX 联调测试

**测试目标**:
**前置条件**:
**测试计划**:
**测试结果**:
**问题记录**:
