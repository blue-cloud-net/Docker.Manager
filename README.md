# Docker.Manager

一个类 Portainer 的轻量化 Docker 管理平台，提供基于 Web 的可视化界面，用于管理本地及远程 Docker 主机上的容器、镜像、Volume、Network 等资源。

后端使用 .NET 10 + Dapper，前端使用 Vue 3 + TypeScript + Element Plus，支持 SignalR 实时推送、JWT 身份认证与按资源粒度的权限控制。

## 文档

| 文档 | 说明 |
|------|------|
| [概述](docs/overview.md) | 项目背景、定位、目标与非目标 |
| [技术约束](docs/constraints.md) | 技术栈选型、版本约束与边界条件 |
| [功能清单](docs/features.md) | 按模块分组的完整功能列表 |
| [开发路线图](docs/roadmap.md) | 三阶段开发计划与里程碑 |
| [架构设计](docs/architecture.md) | 系统分层、通信机制与数据层设计 |
| [API 规范](docs/api-spec.md) | RPC 风格 API 约定与 SignalR Hub 说明 |

## 许可证

[MIT](LICENSE) © 2026 Huansky Co., Ltd.
