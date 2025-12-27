# Backend AI_GUIDE

> **仓库级导航文件。快速了解后端项目结构。**

---

## 概述

FairWorkly 后端服务，提供薪资合规审计功能。

---

## 技术栈

| 组件 | 技术 |
|------|------|
| 框架 | .NET 8 / ASP.NET Core |
| 数据库 | PostgreSQL |
| ORM | Entity Framework Core |
| CQRS | MediatR |
| 验证 | FluentValidation |
| 日志 | Serilog |

---

## 项目结构

```
backend/
├── src/
│   ├── FairWorkly.API/          ← 表现层：Controller
│   ├── FairWorkly.Application/  ← 应用层：用例、服务
│   ├── FairWorkly.Domain/       ← 领域层：Entity（不可改）
│   └── FairWorkly.Infrastructure/  ← 基础设施：数据库
├── tests/
│   └── FairWorkly.UnitTests/    ← 测试
├── .doc/                        ← 开发文档
└── .raw_materials/              ← 原始需求（只读）
```

---

## 层级职责

| 层 | 职责 | 可修改 |
|----|------|--------|
| Domain | Entity、Enum | ❌ 不可改 |
| Application | 用例、服务、DTO | ✅ 主要开发区 |
| Infrastructure | Repository、DbContext | 部分可改 |
| API | Controller | ✅ 可改 |

---

## 快速导航

| 目录 | AI_GUIDE |
|------|----------|
| Application 层 | [src/FairWorkly.Application/AI_GUIDE.md](./src/FairWorkly.Application/AI_GUIDE.md) |
| Payroll 模块 | [src/FairWorkly.Application/Payroll/AI_GUIDE.md](./src/FairWorkly.Application/Payroll/AI_GUIDE.md) |
| Infrastructure 层 | [src/FairWorkly.Infrastructure/AI_GUIDE.md](./src/FairWorkly.Infrastructure/AI_GUIDE.md) |

---

## 开发文档

| 文档 | 用途 |
|------|------|
| [.doc/AI_GUIDE.md](./.doc/AI_GUIDE.md) | 项目导航 |
| [.doc/CODING_RULES.md](./.doc/CODING_RULES.md) | 编码规范 |
| [.doc/SPEC_Payroll.md](./.doc/SPEC_Payroll.md) | Payroll 模块规格 |
| [.doc/TEST_PLAN.md](./.doc/TEST_PLAN.md) | 测试方案 |
| [.doc/DEVLOG.md](./.doc/DEVLOG.md) | 开发日志 |

---

## 常用命令

```bash
# 运行后端
dotnet run --project src/FairWorkly.API

# 运行测试
dotnet test

# 清库重建
dotnet ef database drop --force --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
dotnet ef database update --project src/FairWorkly.Infrastructure --startup-project src/FairWorkly.API
```
