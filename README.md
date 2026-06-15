# SwiftPick — Интернет-магазин игровых аксессуаров

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![React](https://img.shields.io/badge/React-19-61DAFB?style=for-the-badge&logo=react)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?style=for-the-badge&logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-✓-2496ED?style=for-the-badge&logo=docker)

**SwiftPick** — современный полнофункциональный интернет-магазин игровых аксессуаров и периферии для геймеров.

## Особенности

### Backend (.NET 8)
- ASP.NET Core Web API с многослойной архитектурой
- Entity Framework Core + PostgreSQL
- JWT-аутентификация
- OAuth 2.0 (Google, VK, Yandex)
- ASP.NET Core Identity
- Swagger/OpenAPI документация

### Frontend (React + Vite)
- Современный UI с адаптивным дизайном
- React Router для навигации
- Контекст для управления состоянием (Auth, Cart)
- Ленивая загрузка изображений
- Русский интерфейс

### Функционал
- Каталог товаров с фильтрами и поиском
- Корзина и оформление заказов
- Личный кабинет с историей заказов
- Регистрация и авторизация
- Админ-панель с CRUD операциями
- Управление пользователями и заказами

---

## Быстрый запуск

### Требования
- .NET 8 SDK
- Node.js 18+
- Docker и Docker Compose (опционально)
- Visual Studio 2022 (опционально)

### Важное примечание по Docker

Перед запуском через Docker убедитесь, что **Docker Desktop запущен**:
1. Откройте Docker Desktop из меню Пуск
2. Дождитесь появления зелёного индикатора "Engine running"
3. Только после этого выполняйте `docker-compose up --build`

### Запуск через Docker (рекомендуется)

```bash
# Клонировать репозиторий
git clone https://github.com/lolboxik/SwiftPick.git
cd SwiftPick

# Запустить Docker Desktop (если не запущен)
# Затем запустить все сервисы
docker-compose up --build

# Открыть в браузере:
# Frontend: http://localhost
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Локальный запуск (без Docker)

#### 1. Запуск базы данных (PostgreSQL)

Установите PostgreSQL 15+ или используйте Docker только для БД:
```bash
docker run -d --name swiftpick-db ^
  -e POSTGRES_DB=swiftpick ^
  -e POSTGRES_USER=postgres ^
  -e POSTGRES_PASSWORD=postgres ^
  -p 5432:5432 ^
  postgres:15-alpine
```

Или установите PostgreSQL локально и создайте базу данных `swiftpick`.

#### 2. Запуск Backend

```bash
cd SwiftPick
dotnet restore
dotnet run --project SwiftPick.API
```

API будет доступно по адресу: http://localhost:5000

#### 3. Запуск Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend будет доступен по адресу: http://localhost:5173

---

## Тестовые учётные данные

| Роль | Email | Пароль |
|------|-------|--------|
| Администратор | admin@swiftpick.com | Admin123! |
| Менеджер | manager@swiftpick.com | Manager123! |
| Пользователь | user@swiftpick.com | User123! |

---

## Структура проекта

```
SwiftPick/
├── SwiftPick.API/              # Веб-API проект
│   ├── Controllers/            # API контроллеры
│   ├── Program.cs              # Точка входа и конфигурация
│   └── appsettings.json        # Конфигурация
├── SwiftPick.Core/             # Доменный слой
│   ├── Entities/               # Модели данных
│   ├── Interfaces/             # Интерфейсы репозиториев и сервисов
│   └── DTOs/                   # Объекты передачи данных
├── SwiftPick.Infrastructure/   # Слой доступа к данным
│   ├── Data/                   # DbContext
│   └── Repositories/           # Реализация репозиториев
├── SwiftPick.Services/         # Бизнес-логика
│   └── Services/               # Реализация сервисов
├── frontend/                   # React приложение
│   ├── src/
│   │   ├── api/                # API клиенты
│   │   ├── components/         # UI компоненты
│   │   ├── context/            # React контекст
│   │   ├── pages/              # Страницы
│   │   └── App.jsx             # Главный компонент
│   └── package.json
├── docker-compose.yml          # Docker конфигурация
└── README.md
```

---

## API Endpoints

### Аутентификация
| Метод | Endpoint | Описание |
|-------|----------|----------|
| POST | /api/auth/register | Регистрация |
| POST | /api/auth/login | Вход |
| POST | /api/auth/oauth/{provider} | OAuth вход |

### Товары
| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | /api/products | Получить все товары |
| GET | /api/products/{id} | Получить товар по ID |
| GET | /api/products/category/{categoryId} | Товары категории |
| GET | /api/products/search?query= | Поиск товаров |
| GET | /api/products/filter | Фильтрация товаров |
| POST | /api/products | Создать товар (Admin/Manager) |
| PUT | /api/products/{id} | Обновить товар (Admin/Manager) |
| DELETE | /api/products/{id} | Удалить товар (Admin) |

### Категории
| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | /api/categories | Получить все категории |
| POST | /api/categories | Создать категорию (Admin/Manager) |
| PUT | /api/categories/{id} | Обновить категорию (Admin/Manager) |
| DELETE | /api/categories/{id} | Удалить категорию (Admin) |

### Корзина
| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | /api/cart | Получить корзину |
| POST | /api/cart/add | Добавить в корзину |
| PUT | /api/cart/update | Обновить количество |
| DELETE | /api/cart/remove/{productId} | Удалить из корзины |
| DELETE | /api/cart/clear | Очистить корзину |

### Заказы
| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | /api/orders | Мои заказы |
| GET | /api/orders/{id} | Детали заказа |
| POST | /api/orders | Создать заказ |
| GET | /api/orders/admin/all | Все заказы (Admin/Manager) |
| PUT | /api/orders/admin/{id}/status | Обновить статус (Admin/Manager) |

### Админ-панель
| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | /api/admin/users | Все пользователи |
| PUT | /api/admin/users/{id}/role | Изменить роль |
| PUT | /api/admin/users/{id}/activate | Активировать/блокировать |
| GET | /api/admin/dashboard | Статистика |

---

## Технологии

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- PostgreSQL
- ASP.NET Core Identity
- JWT Bearer Authentication
- AutoMapper
- BCrypt

### Frontend
- React 19
- Vite
- React Router DOM
- Axios
- CSS3 с градиентами и анимациями

### DevOps
- Docker & Docker Compose
- GitHub Actions (CI/CD готов)

---

## Лицензия

MIT License

---

## Контакты

- Email: support@swiftpick.ru
- GitHub: lolboxik

---

**Разработано в рамках дипломного проекта**  
**2026 SwiftPick**
