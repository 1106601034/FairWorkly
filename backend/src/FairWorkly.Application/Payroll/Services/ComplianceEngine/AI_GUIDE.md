# ComplianceEngine AI_GUIDE

> **合规规则引擎。包含 4 个薪资合规检查规则。**

> 宪法文档提醒：费率表和计算逻辑定义在 [Payroll_Engine_Logic.md](../../../../../../.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md)，这是只读的。

---

## 概述

ComplianceEngine 是 Payroll 模块的核心逻辑，负责检查薪资数据是否符合澳大利亚劳动法规（General Retail Industry Award 2020 - MA000004）。

**特点**：
- 纯规则计算，不涉及 AI 推理
- 所有规则实现 `IComplianceRule` 接口
- 费率数据来自 `RateTableProvider` 静态类

---

## 文件清单

| 文件 | 职责 | 测试数 | 状态 |
|------|------|--------|------|
| IComplianceRule.cs | 规则接口定义 | - | ✅ |
| RateTableProvider.cs | 静态费率表 (MA000004) | - | ✅ |
| BaseRateRule.cs | 基础时薪 >= Permanent Rate | 17 | ✅ |
| PenaltyRateRule.cs | 周末/公休罚金费率检查 | 13 | ✅ |
| CasualLoadingRule.cs | Casual 员工 25% Loading | 17 | ✅ |
| SuperannuationRule.cs | 12% 养老金检查 | 22 | ✅ |

**测试总计**: 69 tests (ISSUE_02)

---

## 核心接口

```csharp
public interface IComplianceRule
{
    string RuleName { get; }
    List<PayrollIssue> Evaluate(Payslip payslip, Guid validationId);
}
```

每个规则：
1. 从 `RateTableProvider` 获取预期费率
2. 计算实际支付与预期值的差异
3. 如果差异超过容差，返回 `PayrollIssue`

---

## 费率表 (RateTableProvider)

### Permanent Rates (Level 1-8)

| Level | Rate |
|-------|------|
| 1 | $26.55 |
| 2 | $27.16 |
| 3 | $27.58 |
| 4 | $28.12 |
| 5 | $29.27 |
| 6 | $29.70 |
| 7 | $31.19 |
| 8 | $32.45 |

### Casual Rates (含 25% Loading)

| Level | Rate |
|-------|------|
| 1 | $33.19 |
| 2 | $33.95 |
| 3 | $34.48 |
| 4 | $35.15 |
| 5 | $36.59 |
| 6 | $37.13 |
| 7 | $38.99 |
| 8 | $40.56 |

### 罚金倍率

| 日期类型 | Permanent | Casual |
|----------|-----------|--------|
| Saturday | 1.25x | 1.50x |
| Sunday | 1.50x | 1.75x |
| Public Holiday | 2.25x | 2.50x |

---

## 容差值

| 类型 | 容差 | 说明 |
|------|------|------|
| 时薪 | $0.01 | `RateTolerance` |
| 金额 | $0.05 | `PayTolerance` |

---

## Severity 级别

| 级别 | 值 | 含义 |
|------|-----|------|
| CRITICAL | 4 | 实际欠薪 |
| ERROR | 3 | 罚金欠薪 |
| WARNING | 2 | 配置错误或数据异常 |
| INFO | 1 | 信息提示 |

---

## 文档矩阵链接

### 上级导航
- [← 返回 Payroll 模块](../../AI_GUIDE.md)
- [← 返回 Application 层](../../../AI_GUIDE.md)

### 规格文档
- [SPEC_Payroll.md](../../../../../../.doc/SPEC_Payroll.md) - 技术规格
- [TEST_PLAN.md](../../../../../../.doc/TEST_PLAN.md) - 测试方案

### 宪法文档 (只读)
- [Payroll_Engine_Logic.md](../../../../../../.raw_materials/BUSINESS_RULES/Payroll_Engine_Logic.md) - 费率表和计算逻辑

---

*最后更新: 2026-01-07 (ISSUE_02 完成)*
