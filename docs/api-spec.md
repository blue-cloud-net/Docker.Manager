# API 规范

## RPC 风格约定

本项目 API 采用 **RPC 风格**（非 RESTful）：

- 所有请求使用 `POST` 方法
- URL 表达动作，格式为 `/api/v1/{resource}/{action}`
- 参数统一放在请求 Body（JSON）中，不使用路径参数与查询字符串（分页除外）
- 响应体统一使用 `ApiResult<T>` 格式（见「统一响应格式」）

**示例对比**

| RESTful（不采用） | RPC 风格（本项目） |
|---|---|
| `PUT /api/v1/containers/{id}` | `POST /api/v1/container/start` |
| `DELETE /api/v1/containers/{id}` | `POST /api/v1/container/remove` |
| `GET /api/v1/containers` | `POST /api/v1/container/list` |

---

## 统一响应格式

```json
{
  "success": true,
  "code": 0,
  "message": "ok",
  "data": null
}
```

### 业务错误码

| code | 含义 |
|------|------|
| 0 | 成功 |
| 1000 | 参数校验失败 |
| 1001 | 资源不存在 |
| 1002 | Docker 操作失败 |
| 2000 | 未认证（Token 缺失或无效） |
| 2001 | 无权限操作此资源 |
| 9999 | 服务器内部错误 |

---

## 分页约定

需要分页的列表接口，请求和响应格式如下：

**请求 Body**（追加字段）：
```json
{
  "pageIndex": 1,
  "pageSize": 20
}
```

**响应 `data` 字段**：
```json
{
  "total": 100,
  "items": [ ]
}
```

---

## 认证

- 登录成功后返回 `accessToken`（JWT）和 `refreshToken`
- 后续所有请求需在 Header 中携带：`Authorization: Bearer <accessToken>`
- Token 过期后通过 `POST /api/v1/auth/refresh` 刷新

---

## API 接口列表

### 认证（阶段二）

| 操作 | 路径 | 说明 |
|------|------|------|
| 登录 | `POST /api/v1/auth/login` | 返回 accessToken + refreshToken |
| 登出 | `POST /api/v1/auth/logout` | 使当前 Token 失效 |
| 刷新 Token | `POST /api/v1/auth/refresh` | 用 refreshToken 换取新 accessToken |

---

### 容器管理（阶段一）

| 操作 | 路径 | 说明 |
|------|------|------|
| 获取列表 | `POST /api/v1/container/list` | 支持按状态过滤、分页 |
| 启动 | `POST /api/v1/container/start` | Body: `{ "containerId": "..." }` |
| 停止 | `POST /api/v1/container/stop` | Body: `{ "containerId": "...", "waitSeconds": 10 }` |
| 重启 | `POST /api/v1/container/restart` | Body: `{ "containerId": "..." }` |
| 删除 | `POST /api/v1/container/remove` | Body: `{ "containerId": "...", "force": false }` |
| 获取日志 | `POST /api/v1/container/logs` | Body: `{ "containerId": "...", "tail": 100, "since": "..." }` |
| 获取详情 | `POST /api/v1/container/inspect` | Body: `{ "containerId": "..." }` |

---

### 镜像管理（阶段一）

| 操作 | 路径 | 说明 |
|------|------|------|
| 获取列表 | `POST /api/v1/image/list` | 支持分页 |
| 拉取 | `POST /api/v1/image/pull` | Body: `{ "image": "nginx", "tag": "latest" }` |
| 删除 | `POST /api/v1/image/remove` | Body: `{ "imageId": "...", "force": false }` |
| 获取详情 | `POST /api/v1/image/inspect` | Body: `{ "imageId": "..." }` |

---

### 用户管理（阶段二）

| 操作 | 路径 | 说明 |
|------|------|------|
| 获取列表 | `POST /api/v1/user/list` | 管理员专用 |
| 创建用户 | `POST /api/v1/user/create` | 管理员专用 |
| 删除用户 | `POST /api/v1/user/delete` | 管理员专用 |
| 修改密码 | `POST /api/v1/user/update-password` | 用户自身或管理员 |
| 分配资源权限 | `POST /api/v1/user/assign-resource` | 管理员专用 |
| 撤销资源权限 | `POST /api/v1/user/revoke-resource` | 管理员专用 |

---

### Volume 管理（阶段三）

| 操作 | 路径 | 说明 |
|------|------|------|
| 获取列表 | `POST /api/v1/volume/list` | — |
| 创建 | `POST /api/v1/volume/create` | Body: `{ "name": "...", "driver": "local" }` |
| 删除 | `POST /api/v1/volume/remove` | Body: `{ "name": "..." }` |
| 获取详情 | `POST /api/v1/volume/inspect` | Body: `{ "name": "..." }` |

---

### Network 管理（阶段三）

| 操作 | 路径 | 说明 |
|------|------|------|
| 获取列表 | `POST /api/v1/network/list` | — |
| 创建 | `POST /api/v1/network/create` | Body: `{ "name": "...", "driver": "bridge", "subnet": "..." }` |
| 删除 | `POST /api/v1/network/remove` | Body: `{ "networkId": "..." }` |
| 获取详情 | `POST /api/v1/network/inspect` | Body: `{ "networkId": "..." }` |

---

### Docker Compose / Stack（阶段三）

| 操作 | 路径 | 说明 |
|------|------|------|
| 获取列表 | `POST /api/v1/stack/list` | — |
| 部署 | `POST /api/v1/stack/deploy` | Body: `{ "name": "...", "composeContent": "..." }` |
| 更新 | `POST /api/v1/stack/update` | Body: `{ "name": "...", "composeContent": "..." }` |
| 停止 | `POST /api/v1/stack/stop` | Body: `{ "name": "..." }` |
| 删除 | `POST /api/v1/stack/remove` | Body: `{ "name": "..." }` |

---

## SignalR Hub 规范

### Hub 路径

| Hub | 路径 | 说明 |
|-----|------|------|
| `ContainerLogHub` | `/hubs/container-log` | 实时日志流 |
| `ContainerStatsHub` | `/hubs/container-stats` | 实时监控数据 |
| `NotificationHub` | `/hubs/notification` | 系统事件通知 |

---

### ContainerLogHub

**客户端 → 服务端**

| 方法 | 参数 | 说明 |
|------|------|------|
| `SubscribeLogs` | `containerId: string` | 开始订阅指定容器日志 |
| `UnsubscribeLogs` | `containerId: string` | 取消订阅 |

**服务端 → 客户端**

| 事件 | 数据 | 说明 |
|------|------|------|
| `OnLog` | `{ containerId, line, timestamp }` | 推送单行日志 |

---

### ContainerStatsHub

**客户端 → 服务端**

| 方法 | 参数 | 说明 |
|------|------|------|
| `SubscribeStats` | `containerId: string` | 开始订阅指定容器监控数据 |
| `UnsubscribeStats` | `containerId: string` | 取消订阅 |

**服务端 → 客户端**

| 事件 | 数据 | 说明 |
|------|------|------|
| `OnStats` | `{ containerId, cpuPercent, memoryUsage, memoryLimit, networkRx, networkTx }` | 推送一次统计快照 |

---

### NotificationHub

**服务端 → 客户端**（主动推送，无需订阅方法）

| 事件 | 数据 | 说明 |
|------|------|------|
| `OnContainerDied` | `{ containerId, name, exitCode, timestamp }` | 容器异常退出通知 |

---

## WebSocket 终端（阶段三）

容器 exec 终端使用原生 WebSocket，不走 SignalR：

- 连接路径：`/ws/container/exec?containerId={id}&cmd=/bin/sh`
- 协议：原始二进制流，前端使用 `xterm.js` + `AttachAddon` 对接
- 认证：连接时携带 `?token=<accessToken>` 查询参数（WebSocket 不支持自定义 Header）
