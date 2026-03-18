# 开发路线图

> 路线图以**功能里程碑**为单位组织，不绑定具体日期。每个阶段完成后进行验收，再推进下一阶段。

---

## 阶段一：核心容器与镜像管理

**目标**：交付可用的 Docker 管理基础功能，无需登录即可使用（单机本地部署场景）。

### 后端

- [ ] 项目结构初始化（`Docker.Manager.Api`）
- [ ] 集成 Docker.DotNet，通过本地 Unix Socket 连接 Docker
- [ ] 容器 API
  - [ ] `POST /api/v1/container/list` — 获取容器列表
  - [ ] `POST /api/v1/container/start` — 启动容器
  - [ ] `POST /api/v1/container/stop` — 停止容器
  - [ ] `POST /api/v1/container/restart` — 重启容器
  - [ ] `POST /api/v1/container/remove` — 删除容器
  - [ ] `POST /api/v1/container/logs` — 获取日志（分页）
  - [ ] `POST /api/v1/container/inspect` — 获取容器详情
- [ ] 镜像 API
  - [ ] `POST /api/v1/image/list` — 获取镜像列表
  - [ ] `POST /api/v1/image/pull` — 拉取镜像（Docker Hub）
  - [ ] `POST /api/v1/image/remove` — 删除镜像
  - [ ] `POST /api/v1/image/inspect` — 获取镜像详情
- [ ] 全局异常处理与统一响应格式
- [ ] Serilog 集成（控制台 + 滚动文件输出）
- [ ] SQLite 初始化（Dapper，启动时自动建表）

### 前端

- [ ] 项目结构初始化（Vue 3 + TypeScript + Vite + Element Plus）
- [ ] 布局框架：左侧导航 + 右侧内容区
- [ ] 国际化（中文 / 英文），主题切换（亮色 / 暗色）
- [ ] 容器列表页（状态标签、操作按钮）
- [ ] 容器日志页
- [ ] 容器详情页
- [ ] 镜像列表页
- [ ] 镜像拉取对话框（实时显示拉取进度）
- [ ] 统一的 API 请求封装（含错误提示）

### 验收标准

- 可通过 Web UI 列出、启停、删除容器和镜像
- 可查看容器日志（历史，非实时流）
- 可从 Docker Hub 拉取镜像
- 支持 Docker 容器化部署（`docker run` 挂载 Socket）

---

## 阶段二：用户认证与资源权限

**目标**：在阶段一功能基础上增加身份认证与按资源粒度的访问控制，使平台可供多人使用。

### 后端

- [ ] SQLite 用户表、资源授权表设计与迁移
- [ ] JWT 登录 / 登出 API
  - [ ] `POST /api/v1/auth/login`
  - [ ] `POST /api/v1/auth/logout`
  - [ ] `POST /api/v1/auth/refresh`（Token 刷新）
- [ ] 用户管理 API（管理员专用）
  - [ ] `POST /api/v1/user/list`
  - [ ] `POST /api/v1/user/create`
  - [ ] `POST /api/v1/user/delete`
  - [ ] `POST /api/v1/user/update-password`
- [ ] 资源授权 API
  - [ ] `POST /api/v1/user/assign-resource` — 为用户分配主机/容器权限
  - [ ] `POST /api/v1/user/revoke-resource`
- [ ] 所有 API 增加 JWT 鉴权中间件
- [ ] 资源权限校验中间件（请求时校验用户是否有目标资源操作权）

### 前端

- [ ] 登录页
- [ ] Token 存储与自动刷新
- [ ] 路由守卫（未登录跳转登录页）
- [ ] 用户管理页（管理员可见）
- [ ] 资源授权管理界面

### 验收标准

- 未登录用户无法访问任何功能页
- 普通用户只能操作被分配的容器/主机
- 管理员可创建用户并分配权限

---

## 阶段三：功能完善

**目标**：补全 Docker 资源管理的剩余模块，并提升操作体验（实时监控、终端、多主机）。

### 功能列表

- [ ] **实时日志流** — SignalR Hub 推送容器日志实时输出
- [ ] **实时监控** — SignalR Hub 推送容器 CPU / 内存 / 网络 I/O
- [ ] **容器终端 exec** — WebSocket + xterm.js 浏览器内交互终端
- [ ] **容器异常退出通知** — SignalR 推送页面内通知（Event 监听）
- [ ] **Volume 管理** — 列表、创建、删除、详情
- [ ] **Network 管理** — 列表、创建、删除、详情
- [ ] **Docker Compose / Stack** — 上传 YAML 部署、更新、删除 Stack
- [ ] **镜像构建** — 上传 Dockerfile 及上下文，实时显示构建进度
- [ ] **私有 Registry** — 配置私有 Registry 地址与认证，支持 pull/push
- [ ] **TCP 远程 Docker 主机** — 配置远程 Docker API（可选 TLS）
- [ ] **Agent 模式** — 目标主机部署轻量 Agent
- [ ] **多主机切换** — 前端支持在多个已配置主机间切换视图

### 验收标准

- 实时日志与监控数据可通过 SignalR 正常推送至前端
- 可通过 Web 终端进入容器执行命令
- Volume / Network / Compose 完整 CRUD 功能可用
- 可添加并管理多个远程 Docker 主机
