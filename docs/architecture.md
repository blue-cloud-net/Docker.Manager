# 架构设计

## 系统分层概览

```
┌─────────────────────────────────────────────────────┐
│                    浏览器 / 客户端                     │
│         Vue 3 + TypeScript + Element Plus            │
│   Pinia 状态管理 · vue-i18n · @microsoft/signalr    │
└──────────────┬──────────────────────┬───────────────┘
               │ HTTP (RPC API)       │ WebSocket (SignalR)
               ▼                      ▼
┌─────────────────────────────────────────────────────┐
│               ASP.NET Core 10 宿主进程               │
│                                                     │
│  ┌────────────────┐      ┌────────────────────────┐ │
│  │  API 层        │      │  SignalR Hub 层         │ │
│  │  /api/v1/...   │      │  ContainerLogHub       │ │
│  │  Controllers   │      │  ContainerStatsHub     │ │
│  └───────┬────────┘      └──────────┬─────────────┘ │
│          │                          │               │
│  ┌───────▼──────────────────────────▼─────────────┐ │
│  │                 Service 层                      │ │
│  │  ContainerService  ImageService  UserService   │ │
│  └───────┬──────────────────────────┬─────────────┘ │
│          │                          │               │
│  ┌───────▼────────┐      ┌──────────▼─────────────┐ │
│  │  Docker 接入层  │      │  数据访问层             │ │
│  │  Docker.DotNet │      │  Dapper + SQLite        │ │
│  └───────┬────────┘      └────────────────────────┘ │
└──────────┼──────────────────────────────────────────┘
           │ Unix Socket
           ▼
┌─────────────────┐
│   Docker 守护进程 │
│   /var/run/      │
│   docker.sock    │
└─────────────────┘
```

---

## 各层职责

### API 层（Controllers）

- 接收 HTTP 请求，解析入参，调用 Service 层
- 统一响应格式封装（`ApiResult<T>`）
- JWT 鉴权过滤器在此层生效
- 不包含业务逻辑

### SignalR Hub 层

| Hub | 职责 |
|-----|------|
| `ContainerLogHub` | 客户端订阅指定容器的实时日志流 |
| `ContainerStatsHub` | 客户端订阅指定容器的实时 CPU/内存/网络统计 |
| `NotificationHub` | 推送系统事件（容器异常退出等通知） |

Hub 层通过调用 Service 层获取数据，并通过 SignalR 将数据推送至已订阅的客户端。

### Service 层

- 封装业务逻辑与 Docker.DotNet 调用细节
- 隔离 Controller 与 Docker SDK 的直接耦合
- 封装 Dapper Repository 调用（用户、授权相关数据）

### Docker 接入层

- 通过 `Docker.DotNet` 创建 `DockerClient`
- 阶段一：固定使用本地 Unix Socket（`unix:///var/run/docker.sock`）
- 后续：支持 TCP 连接（`tcp://host:port`）与 TLS 证书，通过主机配置动态创建 `DockerClient`

### 数据访问层

- 使用 `Dapper` 执行手写 SQL
- 数据库：SQLite，文件存储在宿主机可持久化路径
- 应用启动时自动执行建表 SQL（若表不存在）
- 存储内容：用户信息、资源授权关系、Docker 主机配置（阶段三）

---

## 前端架构

### 目录结构（规划）

```
src/
├── api/          # API 请求封装（axios + RPC 风格方法）
├── assets/       # 静态资源
├── components/   # 通用组件（布局、表格、对话框等）
├── composables/  # 可复用的 Composition 函数
├── layouts/      # 页面布局（左侧导航 + 右侧内容区）
├── locales/      # i18n 语言文件（zh-CN, en-US）
├── router/       # Vue Router 路由配置
├── stores/       # Pinia 状态（用户信息、主机选择等）
├── types/        # TypeScript 类型定义
└── views/        # 页面视图（containers/, images/, users/ 等）
```

### 通信方式

| 场景 | 方式 |
|------|------|
| 普通数据请求 | HTTP POST（axios），RPC 风格路径 |
| 实时日志 | SignalR WebSocket，`ContainerLogHub` |
| 实时监控 | SignalR WebSocket，`ContainerStatsHub` |
| 页面内通知 | SignalR WebSocket，`NotificationHub` |
| 容器终端（阶段三） | 原生 WebSocket，后端 `/ws/container/exec` |

---

## 统一响应格式

所有 HTTP API 均返回统一结构：

```json
{
  "success": true,
  "code": 0,
  "message": "ok",
  "data": { }
}
```

| 字段 | 类型 | 说明 |
|------|------|------|
| `success` | bool | 请求是否成功 |
| `code` | int | 业务错误码（0 = 成功） |
| `message` | string | 错误描述（成功时为 "ok"） |
| `data` | object / null | 返回数据主体 |

---

## 部署架构

### Docker 容器部署

```yaml
services:
  docker-manager:
    image: docker-manager:latest
    ports:
      - "8080:8080"
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock  # Docker Socket 挂载
      - ./data:/app/data                            # SQLite 数据持久化
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - JWT__Secret=<your-secret>
```

### 独立进程部署

```
dotnet Docker.Manager.Api.dll --urls http://0.0.0.0:8080
```

要求宿主机当前用户对 `/var/run/docker.sock` 有读写权限（通常需要加入 `docker` 用户组）。
