# ASP.NET Core Web API — ფინალური პროექტის ტექნიკური გზამკვლევი და იმპლემენტაციის გეგმა

**პროექტის დასახელება:** Online Store API (პროდუქტების, კატეგორიების, შეკვეთებისა და მომხმარებლების მართვა)  
**არქიტექტურა:** Clean Architecture  
**ტექნოლოგიები:** C#, ASP.NET Core Web API, MS SQL Server, EF Core, LINQ, CQRS & MediatR, FluentValidation, AutoMapper/Mapster, Serilog, xUnit, JWT Authentication.

---

## 1. Solution-ის სტრუქტურა (Clean Architecture)

პროექტის სტრუქტურა დაყოფილია 4 ძირითად შრედ (Layer), სადაც დამოკიდებულებები მიმართულია შიგნით — **Domain** შრისკენ.

```text
OnlineStore/
├── src/
│   ├── OnlineStore.Domain/          # Pure C# (No dependencies)
│   ├── OnlineStore.Application/     # Business logic, CQRS, DTOs, Interfaces
│   ├── OnlineStore.Infrastructure/  # EF Core, DbContext, Services, JWT
│   └── OnlineStore.API/             # Controllers, Middlewares, Program.cs
└── tests/
    └── OnlineStore.UnitTests/       # xUnit, Moq, FluentAssertions
```

### შრეების პასუხისმგებლობები

* **Domain Layer:** ბიზნეს ობიექტები (`Product`, `Category`, `Order`, `OrderItem`, `User`, `ProductImage`, `Tag`), Enum-ები (`OrderStatus`, `UserRole`) და Domain Exception-ები.
* **Application Layer:** MediatR Commands/Queries & Handlers, DTO-ები, FluentValidation, Mapping პროფილები და ინტერფეისები (`IApplicationDbContext`, `ICurrentUserService`, `IFileService`, `IJwtTokenGenerator`).
* **Infrastructure Layer:** `ApplicationDbContext`, EF Core Fluent API Configurations, Migrations, `FileService` (ლოკალური შენახვა), `JwtTokenGenerator`.
* **API Layer:** Minimal Controllers, `GlobalExceptionHandler`, Dependency Injection (DI) რეგისტრაციები, Serilog setup, Swagger (Bearer token support).

---

## 2. მონაცემთა ბაზა, Entity-ები და რელაციები

### Entity-ების აღწერა და კავშირები

| Entity | აღწერა | რელაციები |
| :--- | :--- | :--- |
| **User** | სისტემის მომხმარებელი | `1-to-Many` Order-თან |
| **Category** | პროდუქციის კატეგორია | `1-to-Many` Product-თან |
| **Product** | მაღაზიის პროდუქტი | `Many-to-1` Category-სთან, `1-to-Many` ProductImage-თან, `Many-to-Many` Tag-თან |
| **ProductImage** | პროდუქტის სურათის მეტამონაცემები | `Many-to-1` Product-თან |
| **Tag** | პროდუქტის ტეგი (მაგ. New, Sale) | `Many-to-Many` Product-თან (`ProductTag`-ით) |
| **ProductTag** | Explicit Join Entity | `Product`-ისა და `Tag`-ის დამაკავშირებელი |
| **Order** | მომხმარებლის შეკვეთა | `Many-to-1` User-თან, `1-to-Many` OrderItem-თან |
| **OrderItem** | შეკვეთაში შემავალი პროდუქტი | `Many-to-1` Order-თან, `Many-to-1` Product-თან |

### EF Core Fluent API კონფიგურაციების წესები (`Infrastructure/Persistence/Configurations`)

1. **`ProductConfiguration`**:
   * `Name`: Required, MaxLength(200)
   * `Description`: MaxLength(2000)
   * `Price`: Precision(18, 2)
   * `StockQuantity`: Required, Default `0`
   * `Category` (One-to-Many): `OnDelete(DeleteBehavior.Restrict)`
2. **`CategoryConfiguration`**:
   * `Name`: Required, MaxLength(100), Unique Index (`HasIndex(c => c.Name).IsUnique()`)
3. **`OrderConfiguration`**:
   * `TotalAmount`: Precision(18, 2)
   * `Status`: Enum-ის შენახვა ტექსტურად (`HasConversion<string>()`)
   * `User` (One-to-Many): `OnDelete(DeleteBehavior.Restrict)`
4. **`ProductTagConfiguration` (Many-to-Many Explicit Join Entity)**:
   * Composite Primary Key: `HasKey(pt => new { pt.ProductId, pt.TagId })`
5. **`ProductImageConfiguration`**:
   * `FileName`: MaxLength(255), Required
   * `FilePath`: MaxLength(500), Required
   * `ContentType`: MaxLength(100), Required

---

## 3. CQRS, DTO-ები და Validation

### CQRS სტრუქტურა (MediatR Commands & Queries)

#### Commands (მონაცემთა ცვლილება)
* **Auth:** `RegisterUserCommand`, `LoginUserQuery`
* **Products:** `CreateProductCommand`, `UpdateProductCommand`, `DeleteProductCommand`, `UploadProductImageCommand`
* **Categories:** `CreateCategoryCommand`, `UpdateCategoryCommand`, `DeleteCategoryCommand`
* **Orders:** `CreateOrderCommand` (მარაგების შემოწმება + TotalAmount-ის გამოთვლა), `UpdateOrderStatusCommand`, `CancelOrderCommand`

#### Queries (მონაცემთა წაკითხვა)
* **Products:** `GetProductsPagedQuery` (Pagination, Filtering, Sorting, Search), `GetProductByIdQuery`
* **Categories:** `GetAllCategoriesQuery`, `GetCategoryByIdQuery`
* **Orders:** `GetOrdersQuery` (Admin/Customer ავტომატური ფილტრაციით), `GetOrderByIdQuery`

### DTO-ების როლი და უსაფრთხოება
* **Over-Posting Protection:** მომხმარებელი ვერ შეცვლის `Order.TotalAmount`-ს ან `User.Role`-ს.
* **Encapsulation:** `PasswordHash` არასდროს ბრუნდება Response-ში.
* **Circular Dependency Prevention:** JSON სერიალიზაციისას თავიდან იცილებს უსასრულო ციკლებს.
* **Performance:** AutoMapper-ის `ProjectTo<T>()` მეთოდით SQL-ში იგზავნება მხოლოდ საჭირო `SELECT` სვეტები.

### Validation Rules (FluentValidation)
1. **Product Rules:** `Name` არ უნდა იყოს ცარიელი (Max 200), `Price` > 0, `StockQuantity` >= 0.
2. **Order Rules:** კალათა (`Items`) არ უნდა იყოს ცარიელი, თითოეული ელემენტის `Quantity` > 0.
3. **Custom Validation Rules:**
   * ფაილის ატვირთვისას: გაფართოება (`.jpg`, `.jpeg`, `.png`, `.webp`) და ზომა (<= 5MB).
   * ბიზნეს-ვალიდაცია: შემოწმება `MustAsync`-ით, არსებობს თუ არა `CategoryId` ბაზაში.

---

## 4. ავთენტიკაცია, ავტორიზაცია და ICurrentUserService

### RBAC (Role-Based Access Control)
* **`Admin`**: სრული წვდომა პროდუქტების, კატეგორიების მართვასა და ყველა მომხმარებლის შეკვეთების ნახვა/სტატუსის ცვლილებაზე.
* **`Customer`**: პროდუქტების ნახვა, შეკვეთის გაფორმება და **მხოლოდ საკუთარი შეკვეთების** ნახვა.

### ICurrentUserService მექანიზმი
`Application` შრეში განსაზღვრული `ICurrentUserService` ინტერფეისი `API` შრეში `IHttpContextAccessor`-ის საშუალებით კითხულობს ავტორიზებული მომხმარებლის `UserId`-ს და `Role`-ს ტოკენის Claim-ებიდან.

**შეკვეთების წვდომის ლოგიკა `GetOrdersQueryHandler`-ში:**
```csharp
if (_currentUserService.Role != UserRole.Admin)
{
    // Customer-ისთვის მოთხოვნა ავტომატურად იფილტრება
    query = query.Where(o => o.UserId == _currentUserService.UserId);
}
```

---

## 5. API Controllers და Endpoints-ის სრული გეგმა

### 1. AuthController (`/api/auth`)
* `POST /api/auth/register` `[AllowAnonymous]` -> `RegisterUserCommand`
* `POST /api/auth/login` `[AllowAnonymous]` -> `LoginUserCommand`

### 2. ProductsController (`/api/products`)
* `GET /api/products` `[AllowAnonymous]` -> `GetProductsPagedQuery` (Pagination, Filtering, Sorting, Search)
* `GET /api/products/{id}` `[AllowAnonymous]` -> `GetProductByIdQuery`
* `POST /api/products` `[Authorize(Roles = "Admin")]` -> `CreateProductCommand`
* `PUT /api/products/{id}` `[Authorize(Roles = "Admin")]` -> `UpdateProductCommand`
* `DELETE /api/products/{id}` `[Authorize(Roles = "Admin")]` -> `DeleteProductCommand`

### 3. CategoriesController (`/api/categories`)
* `GET /api/categories` `[AllowAnonymous]` -> `GetAllCategoriesQuery`
* `GET /api/categories/{id}` `[AllowAnonymous]` -> `GetCategoryByIdQuery`
* `POST /api/categories` `[Authorize(Roles = "Admin")]` -> `CreateCategoryCommand`
* `PUT /api/categories/{id}` `[Authorize(Roles = "Admin")]` -> `UpdateCategoryCommand`
* `DELETE /api/categories/{id}` `[Authorize(Roles = "Admin")]` -> `DeleteCategoryCommand`

### 4. OrdersController (`/api/orders`)
* `GET /api/orders` `[Authorize]` -> `GetOrdersQuery` (Customer ხედავს მხოლოდ თავისას, Admin — ყველას)
* `GET /api/orders/{id}` `[Authorize]` -> `GetOrderByIdQuery` (უსაფრთხოების შემოწმებით)
* `POST /api/orders` `[Authorize]` -> `CreateOrderCommand`
* `PATCH /api/orders/{id}/status` `[Authorize(Roles = "Admin")]` -> `UpdateOrderStatusCommand`
* `DELETE /api/orders/{id}` `[Authorize]` -> `CancelOrderCommand`

### 5. ProductImagesController (`/api/products/{productId}/images`)
* `POST /api/products/{productId}/images` `[Authorize(Roles = "Admin")]` -> `UploadProductImageCommand`
* `GET /api/products/images/{imageId}/download` `[AllowAnonymous]` -> `DownloadProductImageQuery`
* `DELETE /api/products/images/{imageId}` `[Authorize(Roles = "Admin")]` -> `DeleteProductImageCommand`

---

## 6. Pagination, Filtering, Sorting & Search (Products API Response)

### GET `/api/products` Response Structure (`PagedList<T>`):
```json
{
  "items": [
    {
      "id": 10,
      "name": "Gaming Laptop",
      "price": 3500.00,
      "categoryName": "Laptops",
      "mainImageUrl": "/uploads/products/laptop.jpg"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 4,
  "totalCount": 35,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

## 7. Global Exception Handling და Serilog

### Standardized Error Response (ProblemDetails RFC 7807)
`GlobalExceptionHandler` აკონვერტირებს Exception-ებს შესაბამის სტატუს კოდებში:
* `NotFoundException` -> `404 Not Found`
* `ValidationException` -> `400 Bad Request` (შეცდომების დეტალური სიით)
* `BadRequestException` -> `400 Bad Request`

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "Price": ["Price must be greater than 0."]
  }
}
```

### Serilog Logging
* კონფიგურაცია `appsettings.json`-ში: Console და File sink (`Logs/app-log-.txt`).
* `RollingInterval.Day` ყოველდღიური ფაილების შექმნისთვის.
* ტექნიკური Stack Trace ინახება მხოლოდ ლოგებში და არასდროს უბრუნდება მომხმარებელს API Response-ში.

---

## 8. Unit Testing Strategy (xUnit + Moq)

ტესტები იწერება `OnlineStore.UnitTests` პროექტში Application Layer-ის Handlers & Validators-ისთვის:

1. **Success Case:** `CreateProductCommandHandler_WithValidData_ShouldCreateProductAndReturnId`
2. **Validation Rule Test:** `CreateProductCommandValidator_WithNegativePrice_ShouldHaveValidationError`
3. **Business Logic Test:** `CreateOrderCommandHandler_WithValidItems_ShouldCalculateCorrectTotalAmount`
4. **Edge Case / Stock Exception:** `CreateOrderCommandHandler_WhenStockIsInsufficient_ShouldThrowBadRequestException`
5. **Not Found Exception Test:** `GetProductByIdQueryHandler_WhenProductDoesNotExist_ShouldThrowNotFoundException`

---

## 9. ნაბიჯ-ნაბიჯ იმპლემენტაციის 7-ეტაპიანი გეგმა

### ეტაპი 1: სოლუშენისა და არქიტექტურის მომზადება
1. შექმენით Solution და 4 ძირითადი პროექტი (`Domain`, `Application`, `Infrastructure`, `API`) + `UnitTests`.
2. დააკავშირეთ პროექტები და დააინსტალირეთ NuGet პაკეტები (`MediatR`, `EF Core SQLServer`, `FluentValidation`, `AutoMapper`, `Serilog`, `JwtBearer`, `Moq`).

### ეტაპი 2: Domain Layer-ის აგება
1. შექმენით Enum-ები (`UserRole`, `OrderStatus`).
2. შექმენით Entity კლასები (`User`, `Category`, `Product`, `ProductImage`, `Tag`, `ProductTag`, `Order`, `OrderItem`).
3. შექმენით `DomainException`.

### ეტაპი 3: Application Layer-ის გამართვა
1. შექმენით აბსტრაქციები (`IApplicationDbContext`, `ICurrentUserService`, `IFileService`, `IJwtTokenGenerator`).
2. შექმენით DTO-ები და Mapping პროფილები.
3. შექმენით FluentValidation კლასები.
4. დაწერეთ MediatR Commands, Queries და მათი Handlers.

### ეტაპი 4: Infrastructure Layer-ის იმპლემენტაცია
1. შექმენით `ApplicationDbContext` და Fluent API Entity Configurations.
2. განახორციელეთ `FileService` და `JwtTokenGenerator`.
3. შექმენით პირველადი მიგრაცია (`Add-Migration InitialCreate`) და Data Seeder.

### ეტაპი 5: API Layer-ისა და Endpoints-ის გამართვა
1. `Program.cs`-ში დაარეგისტრირეთ სერვისები (EF Core, MediatR, FluentValidation, Serilog, JWT, Swagger).
2. შექმენით `CurrentUserService` (`IHttpContextAccessor`-ით).
3. შექმენით `GlobalExceptionHandler` Middleware.
4. შექმენით Controller-ები (`AuthController`, `ProductsController`, `CategoriesController`, `OrdersController`, `ProductImagesController`).

### ეტაპი 6: Unit Testing (xUnit)
1. შექმენით 5 სავალდებულო Unit Test `Application` შრის ლოგიკისთვის.
2. დაატესტეთ ვალიდატორები, მარაგების შემოწმება და Exception-ების ტყორცნა.

### ეტაპი 7: საბოლოო ტესტირება და Swagger-ით შემოწმება
1. ბაზის განახლება (`Update-Database`).
2. Swagger UI-ით Customer-ისა და Admin-ის როლების ტესტირება JWT ტოკენის გამოყენებით.
3. ლოგების შემოწმება `Logs/` საქაღალდეში.

---

## base models and dtos

## --------------------- User -----------------------------
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer; // Admin = 1, Customer = 2
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

## Auth DTOs:
RegisterRequestDto: FirstName, LastName, Email, Password
LoginRequestDto: Email, Password
AuthResponseDto: Token, Email, Role, ExpiresAt
UserResponseDto: Id, FirstName, LastName, Email, Role

## ----------------------- Category --------------------------------

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

Category DTOs:
CreateCategoryRequestDto: Name, Description
UpdateCategoryRequestDto: Name, Description
CategoryResponseDto: Id, Name, Description, ProductsCount

## ---------------------- Product and ProductImage ----------------------------------

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class ProductImage
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsMain { get; set; } = false;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

Product DTOs:
CreateProductRequestDto: Name, Description, Price, StockQuantity, CategoryId, TagIds (List)
UpdateProductRequestDto: Name, Description, Price, StockQuantity, CategoryId
ProductResponseDto: Id, Name, Description, Price, StockQuantity, CategoryId, CategoryName, MainImageUrl, Images (List), Tags (List)
ProductBriefResponseDto: Id, Name, Price, CategoryName, MainImageUrl
ProductImageResponseDto: Id, FileName, FilePath, IsMain

## ------------------------- Tag and ProductTag -----------------------

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}

// Domain/Entities/ProductTag.cs (Join Entity)
public class ProductTag
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}

Tag DTOs:
CreateTagRequestDto: Name
TagResponseDto: Id, Name

## -------------------- Order & OrderItem ----------------------

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending; // Pending, Processing, Shipped, Delivered, Cancelled

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

Order DTOs:
CreateOrderRequestDto: Items (List)
CreateOrderItemRequestDto: ProductId, Quantity
UpdateOrderStatusRequestDto: Status (OrderStatus Enum)
OrderResponseDto: Id, UserId, UserEmail, OrderDate, TotalAmount, Status, Items (List)
OrderItemResponseDto: ProductId, ProductName, UnitPrice, Quantity, TotalPrice

## ------------------------- Endpoints & Authorization Matrix --------------------------------------
Controller, Endpoint, Method, Authorization, აღწერა
Auth,/api/auth/register,POST,[AllowAnonymous],ახალი Customer-ის რეგისტრაცია
Auth,/api/auth/login,POST,[AllowAnonymous],შესვლა და JWT Token-ის მიღება
Products,/api/products,GET,[AllowAnonymous],"პროდუქტები (Pagination, Filter, Sort, Search)"
Products,/api/products/{id},GET,[AllowAnonymous],დეტალური ინფორმაცია
Products,/api/products,POST,"[Authorize(Roles=""Admin"")]",პროდუქტის დამატება
Products,/api/products/{id},PUT,"[Authorize(Roles=""Admin"")]",პროდუქტის განახლება
Products,/api/products/{id},DELETE,"[Authorize(Roles=""Admin"")]",პროდუქტის წაშლა
Categories,/api/categories,GET,[AllowAnonymous],კატეგორიების სია
Categories,/api/categories,POST,"[Authorize(Roles=""Admin"")]",კატეგორიის შექმნა
Orders,/api/orders,GET,[Authorize],Customer: საკუთარი შეკვეთებიAdmin: ყველა შეკვეთა
Orders,/api/orders/{id},GET,[Authorize],შეკვეთის დეტალები (უსაფრთხოების შემოწმებით)
Orders,/api/orders,POST,[Authorize],შეკვეთის გაფორმება
Orders,/api/orders/{id}/status,PATCH,"[Authorize(Roles=""Admin"")]",სტატუსის ცვლილება
Images,/api/products/{id}/images,POST,"[Authorize(Roles=""Admin"")]",სურათის ატვირთვა
Images,/api/products/images/{id}/download,GET,[AllowAnonymous],სურათის ჩამოტვირთვა

## ----- 4. ნაბიჯ-ნაბიჯ იმპლემენტაციის გეგმა ---------

Phase 1: Solution Setup & Package Installation
შექმენით Solution და 4 Class Library (Domain, Application, Infrastructure, API) + 1 Test Project (UnitTests).
დააკავშირეთ დამოკიდებულებები Clean Architecture-ის მიხედვით.
დააინსტალირეთ NuGet პაკეტები:
Application: MediatR, FluentValidation.DependencyInjectionExtensions, AutoMapper / Mapster
Infrastructure: Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Tools, System.IdentityModel.Tokens.Jwt
API: Microsoft.AspNetCore.Authentication.JwtBearer, Serilog.AspNetCore, Swashbuckle.AspNetCore
UnitTests: xUnit, Moq, FluentAssertions

Phase 2: Domain Layer Implementation
შექმენით UserRole და OrderStatus Enum-ები.
შექმენით ყველა Entity კლასი (User, Category, Product, ProductImage, Tag, ProductTag, Order, OrderItem).
შექმენით DomainException კლასი.

Phase 3: Application Layer Implementation
განსაზღვრეთ ინტერფეისები: IApplicationDbContext, ICurrentUserService, IFileService, IJwtTokenGenerator.

შექმენით ყველა Request და Response DTO.

შექმენით FluentValidation კლასები (CreateProductCommandValidator, CreateOrderCommandValidator, UploadProductImageCommandValidator).

დაწერეთ MediatR Commands/Queries და მათი Handler-ები (მათ შორის GetOrdersQueryHandler-ში Customer/Admin ფილტრაციის ლოგიკა).

Phase 4: Infrastructure Layer Implementation
შექმენით ApplicationDbContext და Fluent API კონფიგურაციები (EntityConfigurations).

განახორციელეთ FileService (ლოკალური შენახვა wwwroot/uploads/) და JwtTokenGenerator.

გააშვით Add-Migration InitialCreate და დაწერეთ Seeder საწყისი Admin-ისთვის.

Phase 5: API Layer Integration
Program.cs-ში დაარეგისტრირეთ DI სერვისები, Serilog, Swagger Bearer Auth და JWT Authentication.

შექმენით CurrentUserService : ICurrentUserService (IHttpContextAccessor-ის გამოყენებით).

შექმენით GlobalExceptionHandler (RFC 7807 ProblemDetails ფორმატი).

ააგეთ Controller-ები (AuthController, ProductsController, CategoriesController, OrdersController, ProductImagesController).

Phase 6: Unit Testing (xUnit)
დაწერეთ 5 ძირითადი Unit Test:

CreateProductCommandHandler - წარმატებული დამატება.

CreateProductCommandValidator - უარყოფითი ფასის შემოწმება.

CreateOrderCommandHandler - TotalAmount-ის სწორი გამოთვლა.

CreateOrderCommandHandler - უმარაგობისას BadRequestException-ის ისროლება (Edge case).

GetProductByIdQueryHandler - არარსებული ID-ისას NotFoundException-ის ისროლება.

Phase 7: Verification
გააშვით Update-Database.

Swagger UI-ის საშუალებით გაიარეთ ავტორიზაცია Customer და Admin როლებით და შეამოწმეთ წვდომის უფლებები.