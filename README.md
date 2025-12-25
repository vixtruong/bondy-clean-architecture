# Bondy Backend – Microservices (.NET)

Bondy là hệ thống backend được xây dựng theo **Microservices Architecture** kết hợp **Clean Architecture**, sử dụng **.NET 8**, **Ocelot API Gateway**, và **JWT Authentication**.

Project được thiết kế để dễ mở rộng, dễ bảo trì, và phù hợp cho hệ thống mạng xã hội / platform lớn.

---

## 🧱 Kiến trúc tổng thể

```
Client
   |
   v
API Gateway (Ocelot)
   |
   +--> Identity Service
   +--> Social Service
   +--> Friendships Service
   +--> Messaging Service
   +--> Notifications Service
   +--> Reels Service
   +--> Upload Service
   +--> Mail Service
```

**Nguyên tắc:**

* Client **chỉ gọi API Gateway**
* Mỗi service **độc lập domain & database**
* Gateway chịu trách nhiệm routing & security
* Service không gọi trực tiếp service khác

---

## 📁 Cấu trúc thư mục

```
Bondy/
├── services/
│   ├── ApiGateway/
│   ├── Identity/
│   ├── Social/
│   ├── Friendships/
│   ├── Messaging/
│   ├── Notifications/
│   ├── Reels/
│   ├── Upload/
│   └── Mail/
│
├── shared/
│   ├── Bondy.SharedKernel/
│   ├── Bondy.Contracts/
│   └── Bondy.ServiceDefaults/
│
└── README.md
```

---

## 🚪 ApiGateway

**Vai trò:**

* Entry-point duy nhất cho toàn bộ hệ thống
* Routing request đến các service phía sau
* Validate JWT Bearer Token
* Aggregate Swagger

**Công nghệ:**

* Ocelot
* JWT Bearer Authentication

---

## 🔐 Identity Service

**Vai trò:**

* Authentication (Login / Register / Refresh Token)
* Quản lý User, Role, Permission
* Phát hành JWT

**Kiến trúc:** Clean Architecture

```
Identity/
├── Identity.Api
├── Identity.Application
├── Identity.Domain
└── Identity.Infrastructure
```

---

## 👥 Social Service

**Vai trò:**

* Quản lý profile người dùng
* Các tương tác xã hội cơ bản

---

## 🤝 Friendships Service

**Vai trò:**

* Kết bạn / huỷ kết bạn
* Theo dõi / huỷ theo dõi
* Quản lý quan hệ giữa người dùng

---

## 💬 Messaging Service

**Vai trò:**

* Chat 1-1 / group
* Gửi & nhận tin nhắn

---

## 🔔 Notifications Service

**Vai trò:**

* Gửi thông báo hệ thống
* In-app / push notification

---

## 🎞️ Reels Service

**Vai trò:**

* Quản lý video ngắn (reels)
* Feed video

---

## 📤 Upload Service

**Vai trò:**

* Upload file (image, video, document)
* Abstraction cho storage

---

## ✉️ Mail Service

**Vai trò:**

* Gửi email (verify account, reset password, notification)
* Được các service khác gọi thông qua HTTP

---

## 🧼 Clean Architecture (áp dụng cho mọi service)

### 1️⃣ Domain

* Entity, Aggregate Root
* Value Object
* Domain rule & invariant
* Không phụ thuộc framework

> Domain **được phép throw exception** khi vi phạm invariant.

---

### 2️⃣ Application

* Use case / Application Service
* Interface repository
* DTO
* Result / Error pattern

> Application **không throw exception nghiệp vụ**, mà trả về `Result`.

---

### 3️⃣ Infrastructure

* EF Core
* Repository implementation
* External integrations (Mail, Clock, HttpClient)

> Không chứa business logic.

---

### 4️⃣ API

* Controller
* Request / Response mapping
* Authentication / Authorization
* Global exception handling

---

## 🔐 Authentication & Authorization

* JWT Bearer Authentication
* Token được phát bởi Identity Service
* API Gateway validate token
* Route public / protected cấu hình tại Ocelot

---

## 🚏 API Gateway – Routing Convention

### Public (không cần token)

```
/identity/auth/*
```

### Protected (cần Bearer token)

```
/identity/*
/social/*
/friendships/*
/messaging/*
/notifications/*
/reels/*
/upload/*
/mail/*
```

> ⚠️ Mỗi route Ocelot phải **unique theo** `UpstreamPathTemplate + HttpMethod`.

---

## 📄 Swagger

* Mỗi service expose Swagger riêng
* Gateway aggregate Swagger bằng `SwaggerKey`
* Swagger hỗ trợ JWT Bearer

---

## 📦 Shared Projects

### Bondy.SharedKernel

* Base Entity / AggregateRoot
* Result / Error
* Clock abstraction
* Constants & helpers

### Bondy.Contracts

* DTO giao tiếp giữa services
* Integration contract
* Không chứa business logic

### Bondy.ServiceDefaults

* Swagger configuration
* Logging (Serilog)
* Common service extensions

---

## ▶️ Chạy project (Development)

### 1️⃣ Chạy service

Ví dụ Identity:

```bash
cd services/Identity/Identity.Api
dotnet run
```

Mail:

```bash
cd services/Mail/Mail.Api
dotnet run
```

Các service khác chạy tương tự.

---

### 2️⃣ Chạy API Gateway

```bash
cd services/ApiGateway
dotnet run
```

Gateway mặc định:

```
http://localhost:8080
```

---

## 🧠 Nguyên tắc thiết kế đã thống nhất

* Domain > Application > Infrastructure
* Không reference Infrastructure từ Application
* Không dùng Entity làm DTO
* Không throw exception nghiệp vụ ở Application
* Gateway chịu trách nhiệm routing & security
* Service độc lập, không coupling lẫn nhau

---

## 🚀 Định hướng mở rộng

* Thêm service mới:

  * Tạo service theo Clean Architecture
  * Thêm route vào ApiGateway
* Có thể mở rộng:

  * Refresh Token
  * Event-driven (RabbitMQ / Kafka)
  * Distributed Cache
  * Rate Limiting tại Gateway
