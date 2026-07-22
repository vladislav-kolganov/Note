<div align="center">

# 📝 Note

**Backend-сервис для управления заметками/отчётами на базе ASP.NET Core 8**
с Clean Architecture, JWT-аутентификацией, кэшированием (Redis), очередями
сообщений (RabbitMQ) и наблюдаемостью (Prometheus + Grafana).

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-cache-DC382D?logo=redis&logoColor=white)](https://redis.io/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-broker-FF6600?logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![Prometheus](https://img.shields.io/badge/Prometheus-metrics-E6522C?logo=prometheus&logoColor=white)](https://prometheus.io/)

</div>

---

## 📖 О проекте

**Note** — это REST API для ведения пользовательских отчётов (заметок) с ролевой
моделью доступа. Проект построен по принципам **Clean Architecture / DDD** и
демонстрирует промышленный набор практик: послойная архитектура, паттерны
*Repository* + *Unit of Work*, Result-паттерн для единообразных ответов,
автоматическое валидирование запросов, структурированное логирование, кэш,
асинхронный обмен сообщениями и сбор метрик.

> Проект разрабатывается как пет-/-портфолио и активно дополняется (см. [историю коммитов](#-история-изменений)).

### ✨ Возможности

- 🔐 **Аутентификация и авторизация** — регистрация, вход, JWT access-токены и
  обновление через refresh-токены, ролевая модель (`User`, `Role`, `UserRole`).
- 📋 **CRUD отчётов** — создание, чтение, обновление, удаление; отчёты
  привязаны к пользователю.
- 🛡 **Ролевая модель** — управление ролями и назначение ролей пользователям.
- ⚡ **Кэширование** — Redis (StackExchange) для снижения нагрузки на БД.
- 📨 **Асинхронный обмен сообщениями** — RabbitMQ (Producer/Consumer) с topic-exchange.
- 📊 **Метрики** — `/metrics` для Prometheus, готовые дашборды Grafana.
- 🧾 **Единый формат ответов** — `BaseResult<T>` / `CollectionResult<T>`.
- 🔍 **Swagger / OpenAPI** с версионированием API (`v1`, `v2`).

---

## 🧱 Технологии

| Слой / Назначение | Технология |
|---|---|
| Платформа | .NET 8.0, ASP.NET Core Web API |
| База данных | PostgreSQL + EF Core 8 (провайдер `Npgsql`) |
| Кэш | Redis (`Microsoft.Extensions.Caching.StackExchangeRedis`) |
| Брокер сообщений | RabbitMQ (`RabbitMQ.Client`) |
| Аутентификация | JWT Bearer (`System.IdentityModel.Tokens.Jwt`) |
| Маппинг | AutoMapper |
| Валидация | FluentValidation |
| Логирование | Serilog (Console + File, Compact JSON) |
| Метрики | prometheus-net + Prometheus + Grafana |
| Документация API | Swashbuckle (Swagger), API Versioning |
| Тестирование | xUnit + Moq + MockQueryable |

---

## 🏛 Архитектура

Решение разбито на логические группы (solution folders), отражающие Clean Architecture.
Зависимости направлены **внутрь** — к `Note.Domain`, который не зависит ни от чего.

```
                         ┌─────────────────────────────┐
   Presentation  ──────► │          Note.API           │  Контроллеры, middleware, конфигурация
                         └──────────────┬──────────────┘
                                        │
              ┌─────────────────────────┼──────────────────────────┐
              ▼                         ▼                          ▼
   ┌──────────────────┐      ┌─────────────────────┐    ┌──────────────────────┐
   │ Note.Application │      │      Note.DAL       │    │  Note.Producer       │
   │  (сервисы,       │      │  (EF Core, репо,    │    │  Note.Consumer       │
   │   маппинг,       │      │   UnitOfWork,       │    │  (очереди RabbitMQ)  │
   │   валидация)     │      │   интерсепторы)     │    │                      │
   └────────┬─────────┘      └──────────┬──────────┘    └──────────┬────────────┘
            │                           │                          │
            └───────────────────────────┼──────────────────────────┘
                                        ▼
                         ┌─────────────────────────────┐
   Core          ──────► │        Note.Domain          │  Сущности, DTO, интерфейсы,
                         │                             │  настройки, Result, перечисления
                         └─────────────────────────────┘
```

| Группа | Проект | Ответственность |
|---|---|---|
| **Core** | `Note.Domain` | Домен: сущности, DTO, интерфейсы сервисов/репозиториев, настройки (`JwtSettings`, `RedisSettings`, `RabbitMqSettings`), Result-паттерн |
| **Core** | `Note.Application` | Бизнес-логика: сервисы (`Auth`, `Report`, `Role`, `Token`), AutoMapper-профили, FluentValidation-валидаторы |
| **Infrastructure** | `Note.DAL` | Доступ к данным: `ApplicationDbContext`, `BaseRepository`, `UnitOfWork`, EF-конфигурации, перехватчики аудита |
| **Infrastructure** | `Note.Producer` | Отправка сообщений в RabbitMQ |
| **Infrastructure** | `Note.Consumer` | Приём/обработка сообщений из RabbitMQ |
| **Presentation** | `Note.API` | Точка входа: контроллеры, middleware, DI-регистрация, Swagger, метрики |
| **Tests** | `Note.Tests` | Unit-тесты сервисов (xUnit + Moq) |

### Доменная модель

```
User 1 ──< Report          (пользователь владеет множеством отчётов)
User 1 ──< UserRole >── 1 Role   (пользователь ↔ роли, многие-ко-многим)
User 1 ──1 UserToken      (refresh-токен пользователя)
```

Все сущности реализуют `IEntityId<long>` и `IAuditable`
(`CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`). Поля аудита
заполняются автоматически через `DateInterceptor`.

---

## 📂 Структура проекта

```
Note/
├── Note.sln
├── Note.Domain/            # Core — домен
│   ├── Entity/             # User, Role, Report, UserRole, UserToken
│   ├── Dto/                # DTO для запросов/ответов
│   ├── Interfaces/         # I*Service, IBaseRepository, IUnitOfWork, валидаторы
│   ├── Settings/           # JwtSettings, RedisSettings, RabbitMqSettings
│   ├── Result/             # BaseResult<T>, CollectionResult<T>
│   └── Enum/               # ErrorCodes, RoleCodes
├── Note.Application/       # Core — бизнес-логика
│   ├── Services/           # AuthService, ReportService, RoleService, TokenService
│   ├── Mapping/            # профили AutoMapper
│   ├── Validations/        # FluentValidation + валидаторы сущностей
│   └── Resources/          # локализованные сообщения об ошибках (.resx)
├── Note.DAL/               # Infrastructure — БД
│   ├── ApplicationDbContext.cs
│   ├── Repositories/       # BaseRepository, UnitOfWork
│   ├── Configurations/     # EF Core конфигурации сущностей
│   └── Interceptors/       # DateInterceptor (автозаполнение аудита)
├── Note.Producer/          # Infrastructure — RabbitMQ Producer
├── Note.Consumer/          # Infrastructure — RabbitMQ Consumer
├── Note.API/               # Presentation — Web API
│   ├── Controllers/        # Auth, Report, Role, Token
│   ├── Middlewares/        # ExceptionHandlingMiddleware
│   ├── Program.cs          # точка входа, конфигурация конвейера
│   ├── appsettings.json    # RabbitMq, Serilog, Jwt
│   ├── docker-compose.yml  # Postgres, pgAdmin, Prometheus, Grafana
│   └── prometheus.yml      # конфиг scraping'а метрик
└── Note.Tests/             # Tests — xUnit + Moq
```

> ℹ️ В корне репозитория также присутствуют `Note.App` (отдельный клиент —
> мобильное/десктоп-приложение), `LoggingWithMongo` (экспериментальный
> модуль логирования в MongoDB) и `TemplateEngineHost`. Они **не входят** в
> `Note.sln` и не требуются для запуска backend-части.

---

## 🔌 API

Базовый адрес при локальном запуске: `http://localhost:5062`
В режиме `Development` Swagger UI доступен по корню: `http://localhost:5062/`.

| Метод | Маршрут | Описание | Авторизация |
|---|---|---|---|
| `POST` | `/register` | Регистрация пользователя | — |
| `POST` | `/login` | Вход, возвращает access + refresh токены | — |
| `POST` | `/refresh` | Обновление access-токена | — |
| `GET` | `/api/v1/Report/{id}` | Получить отчёт по идентификатору | ✅ JWT |
| `GET` | `/api/v1/Report/reports/{userId}` | Получить отчёты пользователя | ✅ JWT |
| `POST` | `/api/v1/Report` | Создать отчёт | ✅ JWT |
| `PUT` | `/api/v1/Report` | Обновить отчёт | ✅ JWT |
| `DELETE` | `/api/v1/Report/{reportId}` | Удалить отчёт | ✅ JWT |
| `POST` | `/api/Role` | Создать роль | — |
| `PUT` | `/api/Role` | Обновить роль | — |
| `DELETE` | `/api/Role/{roleId}` | Удалить роль | — |
| `POST` | `/api/Role/addrole` | Назначить роль пользователю | — |

Пример ответа (единый формат через `BaseResult<T>`):

```json
{
  "data": { "id": 1, "name": "Report test", "description": "..." },
  "errorMessage": null,
  "errorCode": 0,
  "isSuccess": true
}
```

---

## 🚀 Быстрый старт

### Предварительные требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (для инфраструктуры) — либо локальные PostgreSQL/Redis/RabbitMQ

### 1. Клонирование

```bash
git clone <url-репозитория>
cd Note
```

### 2. Запуск инфраструктуры (PostgreSQL, pgAdmin, Prometheus, Grafana)

В каталоге `Note.API` подготовлен `docker-compose.yml`:

```bash
docker compose -f Note.API/docker-compose.yml up -d note.db pgadmin prometheus grafana
```

Redis и RabbitMQ в compose не описаны — поднимаем их отдельно:

```bash
# Redis (кэш)
docker run -d -p 6379:6379 --name note-redis redis

# RabbitMQ (брокер + management UI на :15672)
docker run -d -p 5672:5672 -p 15672:15672 --name note-rabbitmq rabbitmq:3-management
```

### 3. Конфигурация секретов

Строка подключения к БД и параметры Redis **не хранятся в `appsettings.json`** —
они берутся из **User Secrets** / переменных окружения. Проект уже
инициализирован для User Secrets (`UserSecretsId` в `Note.API.csproj`).

```bash
cd Note.API

dotnet user-secrets set "ConnectionStrings:PostgresSQL" "Host=localhost;Port=5438;Database=Note;Username=admin;Password=admin"
dotnet user-secrets set "RedisSettings:Url" "localhost:6379"
dotnet user-secrets set "RedisSettings:InstanceName" "Note"
```

> Указанные креды (`admin/admin`, БД `Note`, порт `5438`) соответствуют
> сервису `note.db` из `docker-compose.yml`.

### 4. Применение миграций и запуск

```bash
# из корня репозитория
dotnet restore
dotnet build

# (опционально) обновить схему БД — если в проект добавлены миграции EF Core
dotnet ef database update --project Note.DAL --startup-project Note.API

# запустить API
dotnet run --project Note.API
```

После запуска откройте Swagger: <http://localhost:5062/>

---

## ⚙️ Конфигурация

Основные секции `Note.API/appsettings.json`:

| Секция | Параметры | Назначение |
|---|---|---|
| `RabbitMqSettings` | `QueueName`, `RoutingKey`, `ExchangeName` | Параметры очереди/обмена RabbitMQ |
| `Jwt` | `Issuer`, `Audience`, `JwtKey`, `Lifetime`, `RefreshTokenValidityInDays` | Настройки JWT (access = 15 мин, refresh = 7 дней) |
| `Serilog` | sinks Console + File (`/logs/log-.txt`), Compact JSON | Структурированное логирование |
| `ConnectionStrings:PostgresSQL` | *(User Secrets / env)* | Строка подключения к PostgreSQL |
| `RedisSettings` | `Url`, `InstanceName` *(User Secrets / env)* | Подключение к Redis |

> 🔒 **Безопасность.** Не коммитьте секреты в репозиторий. Значение `Jwt:JwtKey`,
> строку подключения и параметры внешних сервисов выносите в User Secrets
> (разработка) или переменные окружения/секреты оркестратора (прод).

---

## 📊 Мониторинг и наблюдаемость

| Сервис | Порт | Доступ | Назначение |
|---|---|---|---|
| API `/metrics` | `:5062/metrics` | — | Экспорт метрик prometheus-net |
| Prometheus | http://localhost:9090 | — | Сбор и хранение метрик |
| Grafana | http://localhost:3000 | `admin` / `admin` | Визуализация дашбордов |
| pgAdmin | http://localhost:5050 | `admin@admin.ru` / `admin` | UI для PostgreSQL |

Конфиг scraping'а — `Note.API/prometheus.yml` (job `NoteProject`, путь `/metrics`).
При необходимости укажите в нём адрес запущенного приложения как target.

Логи пишутся в консоль и в файл `/logs/log-<date>.txt` в компактном JSON-формате.

---

## 🧪 Тесты

Unit-тесты реализованы на **xUnit + Moq + MockQueryable** и покрывают
сервисный слой (например, `ReportServiceTest`).

```bash
dotnet test
```

Для генерации покрытия:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🛠 Полезные команды

```bash
dotnet build Note.sln                 # собрать всё решение
dotnet run --project Note.API         # запустить API
dotnet test                           # прогнать тесты
dotnet ef migrations add <Name> \     # создать миграцию EF Core
  --project Note.DAL --startup-project Note.API
dotnet ef database update \           # применить миграции к БД
  --project Note.DAL --startup-project Note.API
dotnet user-secrets list -p Note.API  # посмотреть локальные секреты
```

---

## 📜 История изменений

Ключевые этапы развития проекта:

- ✅ Базовый каркас: контроллеры, Swagger, AutoMapper, Serilog
- ✅ Аутентификация: `AuthService`, `TokenService`, JWT, конфигурации сущностей
- ✅ Ролевая модель: `RoleController`, маппинг ролей, `UnitOfWork`
- ✅ Рефакторинг: чистый `UnitOfWork`, `BaseRepository` без `SaveChanges`
- ✅ Очереди: RabbitMQ, слои `Producer`/`Consumer`, `RabbitMqSettings`
- ✅ Производительность и наблюдаемость: кэш Redis, метрики Prometheus
- ✅ Тесты: unit-тесты сервисного слоя

---

## 👤 Автор

**Владислав Колганов (Vlad)**

---

## 📄 Лицензия

Лицензия не задана. При публикации уточните условия использования и добавьте
файл `LICENSE` (например, MIT).
