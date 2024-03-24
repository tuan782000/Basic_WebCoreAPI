# Câu lệnh khởi tạo dự án Backend

dotnet new webapi -o ten_du_an

cd du_an

dotnet watch run

```c#
public string Name { get; set; } = string.Empty; // Mặc định null

public DateTime CreatedOn { get; set; } = DateTime.Now;//Mặc định Lấy ngày hiện tại

public Product? Product { get; set; } // giúp cho các thằng khác tham chiếu vào Product được

public List<Comment> Comments { get; set; } = new List<Comment>(); // Thể hiện danh sách
```

# ORM - Object Realtion Maper

```js
{
    Name: "Sản phẩm A";
    Price: 2000;
}
```

Microsoft.EntityFrameworkCore.SqlServer

Microsoft.EntityFrameworkCore.Tools

Microsoft.EntityFrameworkCore.Design

ctrl + shift + p => nugget để cài

Tạo Data - Tạo file ApplicationDBContext sử dụng thư viện DbContext

ctor - tạo nhanh constructor không tham số

Program.cs

```c#
// đoạn này là kết nối giữa Data - ApplicationDBContext với appsettings.json
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
```

dotnet ef migrations add init

dotnet ef database update

ProductController kế thừa ControllerBase - để viết ra các tính năng

```c#
    [Route("api/[product]")] // đường dẫn
    [ApiController] //
    public class ProductController : ControllerBase
    {
        // đọc dữ liệu từ trong databse
        private readonly ApplicationDBContext _context;
        public ProductController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll() {
            var products = _context.Products.ToList();
        }
    }
```

Vai trò của Mappers là ánh xạ dữ liệu từ Models sang Dtos

chuyển đổi dữ liệu thành đối tượng Model - chuyển đổi dạng JSON sang đối tượng ngôn ngữ lập trình

ánh xạ dữ liệu Model và Dtos với nhau thì phải thông qua mappers

firstOrDefault: là một phương thức LINQ trong C# được sử dụng để trả về phần tử đầu tiên hoặc một phần tử thỏa mãn một điều kiện từ một tập hợp (collection).

# Pattern + DI

Code trong controller hiện tại trùng nhiều

Ví dụ:

Get:

```c#
_context.Product.FirstOrDefault(i)
```

GetALL:

```c#
_context.Product.FirstOrDefault(i)
```

Update:

```c#
_context.Product.FirstOrDefault(i)
```

Delete:

```c#
_context.Product.FirstOrDefault(i)
```

Trùng lặp khá nhiều

Giải pháp là tạo ra 1 Repository, sau đó viết 1 hàm chuyên để xử lý vấn đề này. Xong đem qua controller và sử dụng.

Tái sử dụng code được nhiều lần

Repository

```C#
FindProduct(int i) {
    _context.Product.FirstOrDefault(i).ToList();
}
```

Controllers

```c#
// Get
_repo.FindProduct()
// GetAll
_repo.FindProduct()
// Update
_repo.FindProduct()
// delete
_repo.FindProduct()

```
