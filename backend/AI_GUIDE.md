# Backend AI_GUIDE

> **仓库级导航文件。这是进入后端代码库的第一个入口。**

---

## ⚠️ 宪法级文档声明

> **在开始任何工作之前，必须先阅读 `.raw_materials/AI_README_FIRST.md`**

| 文档类型 | 权限 | 位置 |
|----------|------|------|
| 🔴 业务规则 | **绝对只读** | [.raw_materials/BUSINESS_RULES/](./.raw_materials/BUSINESS_RULES/) |
| 🟡 技术约束 | 只读可异议 | [.raw_materials/TECH_CONSTRAINTS/](./.raw_materials/TECH_CONSTRAINTS/) |
| 🟢 参考资料 | 只读参考 | [.raw_materials/REFERENCE/](./.raw_materials/REFERENCE/) |
| 🔴 README.md | **只读** | 人类文档，不能修改 |

**如有异议，向人类报告，不要擅自修改。**

---

## 概述

FairWorkly 后端服务，提供澳大利亚中小企业薪资合规审计功能。

**当前项目进度: 33%**

| Issue | 名称 | 状态 |
|-------|------|------|
| ISSUE_01 | CSV 解析 + 员工同步 | ✅ 完成 |
| ISSUE_02 | 合规规则引擎 | ⏳ **当前任务** |
| ISSUE_03 | Handler + API 集成 | ⏳ 待开发 |

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
│   ├── FairWorkly.API/              ← 表现层：Controller
│   ├── FairWorkly.Application/      ← 应用层：用例、服务 (主要开发区)
│   ├── FairWorkly.Domain/           ← 领域层：Entity（🔒 不可改）
│   └── FairWorkly.Infrastructure/   ← 基础设施：数据库
├── tests/
│   └── FairWorkly.UnitTests/        ← 测试
├── .doc/                            ← 开发文档 (AI 可读写)
├── .raw_materials/                  ← 原始需求 (🔒 只读)
├── CLAUDE.md                        ← Claude Code 入口配置
└── AI_GUIDE.md                      ← 本文件
```

---

## 层级职责

| 层 | 职责 | 可修改 | AI_GUIDE |
|----|------|--------|----------|
| API | Controller，HTTP 端点 | ✅ | [→ 导航](./src/FairWorkly.API/AI_GUIDE.md) |
| Application | 用例、服务、DTO | ✅ 主要开发区 | [→ 导航](./src/FairWorkly.Application/AI_GUIDE.md) |
| Domain | Entity、Enum | 🔒 不可改 | - |
| Infrastructure | Repository、DbContext | 部分可改 | [→ 导航](./src/FairWorkly.Infrastructure/AI_GUIDE.md) |
| Tests | 单元测试、集成测试 | ✅ | [→ 导航](./tests/FairWorkly.UnitTests/AI_GUIDE.md) |

---

## 文档矩阵导航

### 代码目录 AI_GUIDE

| 层级 | 位置 | 说明 |
|------|------|------|
| **仓库级** | 📍 **本文件** | 后端总览 |
| API 层 | [src/FairWorkly.API/AI_GUIDE.md](./src/FairWorkly.API/AI_GUIDE.md) | Controller 和端点 |
| Application 层 | [src/FairWorkly.Application/AI_GUIDE.md](./src/FairWorkly.Application/AI_GUIDE.md) | 服务和用例 |
| ├─ Payroll 模块 | [src/.../Payroll/AI_GUIDE.md](./src/FairWorkly.Application/Payroll/AI_GUIDE.md) | **当前开发重点** |
| Infrastructure 层 | [src/FairWorkly.Infrastructure/AI_GUIDE.md](./src/FairWorkly.Infrastructure/AI_GUIDE.md) | Repository 和数据库 |
| ├─ Persistence | [src/.../Persistence/AI_GUIDE.md](./src/FairWorkly.Infrastructure/Persistence/AI_GUIDE.md) | EF Core 配置 |
| Tests | [tests/FairWorkly.UnitTests/AI_GUIDE.md](./tests/FairWorkly.UnitTests/AI_GUIDE.md) | 测试组织 |

### 开发文档 (.doc/)

| 文档 | 用途 | 链接 |
|------|------|------|
| **项目导航** | 详细的项目状态和进度 | [.doc/AI_GUIDE.md](./.doc/AI_GUIDE.md) |
| 编码规范 | 红线和代码规范 | [.doc/CODING_RULES.md](./.doc/CODING_RULES.md) |
| Payroll 规格 | 模块技术规格 | [.doc/SPEC_Payroll.md](./.doc/SPEC_Payroll.md) |
| 测试方案 | 测试策略和用例 | [.doc/TEST_PLAN.md](./.doc/TEST_PLAN.md) |
| 开发日志 | 决策和讨论记录 | [.doc/DEVLOG.md](./.doc/DEVLOG.md) |

### Issue 文档

| Issue | 文档 | 状态 |
|-------|------|------|
| ISSUE_01 | [.doc/issues/ISSUE_01_CsvParser_EmployeeSync.md](./.doc/issues/ISSUE_01_CsvParser_EmployeeSync.md) | ✅ 完成 |
| ISSUE_02 | [.doc/issues/ISSUE_02_ComplianceEngine.md](./.doc/issues/ISSUE_02_ComplianceEngine.md) | ⏳ 当前 |
| ISSUE_03 | [.doc/issues/ISSUE_03_Handler_API.md](./.doc/issues/ISSUE_03_Handler_API.md) | ⏳ 待开发 |

### 宪法文档 (.raw_materials/) - 只读

| 文档 | 内容 | 链接 |
|------|------|------|
| 入口文档 | AI 必读的第一个文档 | [AI_README_FIRST.md](./.raw_materials/AI_README_FIRST.md) |
| 费率表 | Payroll 计算逻辑 | [Payroll_Engine_Logic.md](./.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md) |
| API 契约 | 前后端接口定义 | [API_Contract.md](./.raw_materials/BUSINESS_RULES/API_Contract.md) |
| 编码标准 | 技术约束 | [CODING_STANDARDS.md](./.raw_materials/TECH_CONSTRAINTS/CODING_STANDARDS.md) |

---

## 快速开始

### 1. 新会话入口

```
1. 阅读 .raw_materials/AI_README_FIRST.md (理解边界)
2. 阅读 .doc/AI_GUIDE.md (了解当前状态)
3. 查看当前 Issue 文档
4. 开始工作
```

### 2. 常用命令

```bash
# 运行后端
dotnet run --project src/FairWorkly.API

# 运行测试
dotnet test

# 清库重建 (AI 已授权)
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```

### 3. 数据库连接

| 配置项 | 值 |
|--------|-----|
| Host | localhost |
| Port | 5433 |
| Database | FairWorklyDb |
| Username | postgres |
| Password | fairworkly123 |

---

## 文档层级关系图

```
                    ┌─────────────────────┐
                    │   CLAUDE.md         │ ← Claude Code 入口
                    │   (工具配置)         │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │  backend/AI_GUIDE   │ ← 📍 你在这里
                    │   (仓库级导航)       │
                    └──────────┬──────────┘
                               │
          ┌────────────────────┼────────────────────┐
          │                    │                    │
          ▼                    ▼                    ▼
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│ .doc/AI_GUIDE   │  │  src/ 各层      │  │ .raw_materials/ │
│ (开发文档入口)   │  │  AI_GUIDE.md    │  │ (宪法文档-只读) │
└────────┬────────┘  └────────┬────────┘  └─────────────────┘
         │                    │
         ▼                    ▼
┌─────────────────┐  ┌─────────────────────────────────┐
│ issues/         │  │ API → Application → Infra       │
│ SPEC_*.md       │  │       └── Payroll (当前重点)    │
│ TEST_PLAN.md    │  │           └── Services          │
│ DEVLOG.md       │  │               └── ComplianceEngine│
└─────────────────┘  └─────────────────────────────────┘
```

---

*最后更新: 2026-01-01*
