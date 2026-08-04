# Unity 技能系统与 Editor 工具

基于 Unity、C# 与 xLua 实现的数据驱动技能系统，并配套开发技能配置与资源生成工具。

项目包含技能运行时框架、Buff 生命周期管理与 Unity Editor 配置工具，覆盖技能配置创建、数据校验、Lua 脚本生成和运行时加载执行等流程。

## 项目架构

### 配置与资源生成流程

* 支持通过 CSV 导入或 Editor 工具创建、修改 SkillSO 配置资产
* 使用 ScriptableObject 保存技能 ID、名称、冷却时间、Lua 路径及关联 Buff 等静态配置
* 提供重复 ID、空名称、非法冷却时间等配置校验
* 支持自动分配技能 ID、生成 Lua 模板及导出 SkillID 枚举
* 集中处理技能资源检索、筛选与排序，供编辑器列表和导出流程复用

### 运行时流程

* `SkillManager` 负责角色运行时的注册、统一更新及施法请求转发
* `CharacterRuntime` 管理单个角色持有的技能实例与 Buff 实例
* `Skill` 封装技能配置、LuaTable、Execute 委托、技能执行与冷却状态
* `LuaManager` 管理 LuaEnv、自定义文件加载器与 Lua 模块缓存，并统一转换 Unity 资产路径和 Lua 模块路径
* `BuffManager` 负责 Buff 的添加、Tick 更新、持续时间管理及到期移除

### 技能执行流程

1. 角色向 `SkillManager` 发起施法请求
2. `SkillManager` 将请求转发给对应的 `CharacterRuntime`
3. `CharacterRuntime` 查找角色持有的 `Skill` 实例
4. `Skill` 检查冷却状态并调用缓存的 Lua Execute 委托
5. 技能执行成功后进入冷却，并根据配置请求添加关联 Buff
6. `BuffManager` 持续更新 Buff，直到 Buff 到期并移除

## 当前功能

* SkillSO 技能静态配置
* 角色技能实例管理
* Lua 技能模块加载与委托缓存
* 技能执行与冷却控制
* Lua 执行异常处理
* Buff 添加、Tick 更新、持续时间管理与到期移除
* 技能搜索、筛选、创建与删除
* 技能 ID 自动分配与重复检测
* Lua 模板生成
* SkillID 枚举导出
* CSV 配置导入
* 运行时资源释放与异常路径处理

## 技术栈

* Unity
* C#
* Lua
* xLua
* ScriptableObject
* Unity Editor 扩展
