# 技术约束

## 技术栈选型

### 后端

| 组件 | 选型 | 版本 | 说明 |
|------|------|------|------|
| 运行时 | .NET | 10 | 最新 LTS 版本 |
| Web 框架 | ASP.NET Core | 10 | WebAPI + SignalR Hub |
| Docker 交互 | Docker.DotNet | 最新稳定版 | 官方 .NET Docker SDK |
| 数据访问 | Dapper | 最新稳定版 | 轻量 ORM，手写 SQL |
| 数据库 | SQLite | — | 嵌入式，无需额外部署 |
| 认证 | JWT（Bearer Token） | — | 无状态，前端存储 Token |
| 实时通信 | ASP.NET Core SignalR | — | 日志流、监控推送、状态通知 |
| 日志 | Serilog | 最新稳定版 | 结构化日志，输出到控制台 + 滚动文件 |

### 前端

| 组件 | 选型 | 版本 | 说明 |
|------|------|------|------|
| 框架 | Vue | 3 | Composition API |
| 语言 | TypeScript | 5.x | 强类型 |
| UI 组件库 | Element Plus | 最新稳定版 | 与 Vue 3 生态对齐 |
| 构建工具 | Vite | 最新稳定版 | 快速开发构建 |
| 状态管理 | Pinia | — | 推荐的 Vue 3 状态管理 |
| 实时通信 | @microsoft/signalr | — | 与后端 SignalR Hub 对接 |
| 终端组件 | xterm.js | — | 容器 exec 交互终端（阶段三） |
| 国际化 | vue-i18n | — | 中文 / 英文双语 |

## API 约定

- 风格：**RPC 风格**（非 RESTful）
- 路径前缀：`/api/v1/`
- 示例：`POST /api/v1/container/start`，而非 `POST /api/v1/containers/{id}`

## 部署约束

| 部署方式 | 说明 |
|----------|------|
| Docker 容器 | 挂载 `/var/run/docker.sock`，通过 `docker run` 或 `docker-compose` 启动 |
| 独立进程 | 直接运行 `dotnet Docker.Manager.dll`，需要宿主机具有 Docker Socket 访问权限 |

## 多主机连接约束（分阶段）

| 阶段 | 支持的连接方式 |
|------|--------------|
| 阶段一（当前） | 仅本地 Unix Socket（`/var/run/docker.sock`） |
| 后续 | TCP 远程 Docker API（可选 TLS 证书验证） |
| 后续 | Agent 模式（目标主机部署轻量 Agent） |

## 边界条件

- **数据库**：SQLite 适用于单节点部署，不支持多实例水平扩展。
- **监控数据**：实时推送，不持久化历史指标（当前阶段）。
- **镜像构建**：阶段一仅支持从 Docker Hub pull，Dockerfile 构建和私有 Registry 支持在阶段三实现。
- **容器终端**：exec 交互终端在阶段三实现，阶段一仅提供日志查看。
- **浏览器支持**：仅支持现代浏览器（Chrome、Firefox、Edge 最近两个主要版本）。
