基于 Unity + xLua 构建的战斗系统框架，实现技能触发、Buff叠加、持续效果（Tick）以及Lua热更新。

## 技术栈
- Unity
- C#
- xLua
- ScriptableObject

## 核心功能
- 技能系统（Lua驱动）
- Buff系统（支持叠加与生命周期管理）
- Tick机制（中毒）
- Lua热更新（require + Reload）

## 技术亮点
- C# / Lua 双向调用（XLua）
- ScriptableObject 数据驱动
- 可扩展 Buff 系统设计
- 统一日志系统（模拟战斗流程）

## 操作说明
- J：基于攻击力攻击自己
- K：给自己添加Buff（中毒）
- R：Lua热更新
- H: 查看当前状态（生命值和攻击力）
