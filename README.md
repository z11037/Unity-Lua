# Unity 技能系统与编辑器工具链

基于 Unity + xLua 实现的技能系统与配置工具链。

项目包含运行时技能框架与 Editor 配置工具，支持从技能配置编辑、自动校验、资源生成到运行时加载的流程。

## 项目架构

**配置生产流程：**

- CSV 导入或 Editor 创建/修改 → 生成 SkillSO 资产
- SkillSO 进入 Configuration Pipeline，执行配置校验、SkillID 枚举生成以及 Lua 模板导出的资源生成流程

**运行时流程：**

- SkillManager 加载 SkillSO 配置，管理技能生命周期与冷却
- 技能实例通过 xLua 调用 Lua 脚本执行技能行为逻辑
- BuffManager 独立管理 Buff 生命周期、Tick 更新以及添加移除
