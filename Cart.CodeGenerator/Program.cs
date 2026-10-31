using System.Reflection;
using Cart.Domain.Entities; // BaseEntity ve Entity modellerinin olduğu namespace

// =========================================================================
// 1. Dizin ve Hedef Entity Belirleme
// =========================================================================

// Konsol çıktısının bin/Debug/netX.X klasöründen Solution kök dizinine çıkıyoruz (4 seviye yukarı)
var solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

// Kod üretilecek Entity tipi (Örn: typeof(Product), typeof(Coupon), typeof(Category))
// Dilersen komut satırından parametreyle de alabilirsin: args.Length > 0 ? ... : typeof(Product)
var entityType = typeof(TestEntity);

Console.WriteLine($"===> {entityType.Name} için CRUD kodları üretiliyor...");

// =========================================================================
// 2. Entity Analizi (Reflection)
// =========================================================================

// BaseEntity<TKey>'den Key tipini bul (Guid, int, long vb.)
var baseEntityType = entityType.BaseType;
var keyType = "Guid";

while (baseEntityType != null && baseEntityType != typeof(object))
{
    if (baseEntityType.IsGenericType && baseEntityType.Name.StartsWith("BaseEntity"))
    {
        keyType = baseEntityType.GetGenericArguments()[0].Name switch
        {
            "Int32" => "int",
            "Int64" => "long",
            "Guid" => "Guid",
            var other => other
        };
        break;
    }
    baseEntityType = baseEntityType.BaseType;
}

// Entity'nin doğrudan kendi tanımlı public property'lerini çek (Navigation ve koleksiyonları filtrele)
var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
    .Where(p => !p.PropertyType.IsGenericType || p.PropertyType.GetGenericTypeDefinition() != typeof(ICollection<>))
    .Where(p => p.Name != "Id") // Id zaten BaseDto'dan gelir
    .Select(p => new
    {
        Name = p.Name,
        Type = GetFriendlyTypeName(p.PropertyType)
    })
    .ToList();

// Property satırlarını üret
var dtoProperties = string.Join(Environment.NewLine, properties.Select(p =>
    $"        public {p.Type} {p.Name} {{ get; set; }} = null!;"));

var requiredProperties = string.Join(Environment.NewLine, properties.Select(p =>
    $"        public required {p.Type} {p.Name} {{ get; set; }}"));

// =========================================================================
// 3. Ayrı Dosyalarda DTO Üretimi
// =========================================================================
var dtoDir = Path.Combine(solutionDir, "Cart.Services", "DTOs", entityType.Name);
Directory.CreateDirectory(dtoDir);

// 3.1. {Entity}Dto.cs
var dtoCode = $$"""
using Cart.Services.DTOs;

namespace Cart.Services.DTOs.{{entityType.Name}}
{
    public class {{entityType.Name}}Dto : BaseDto<{{keyType}}>
    {
{{dtoProperties}}
    }
}
""";
File.WriteAllText(Path.Combine(dtoDir, $"{entityType.Name}Dto.cs"), dtoCode);
Console.WriteLine($"✔ {entityType.Name}Dto.cs oluşturuldu.");

// 3.2. {Entity}CreateDto.cs
var createDtoCode = $$"""
namespace Cart.Services.DTOs.{{entityType.Name}}
{
    public class {{entityType.Name}}CreateDto
    {
{{requiredProperties}}
    }
}
""";
File.WriteAllText(Path.Combine(dtoDir, $"{entityType.Name}CreateDto.cs"), createDtoCode);
Console.WriteLine($"✔ {entityType.Name}CreateDto.cs oluşturuldu.");

// 3.3. {Entity}UpdateDto.cs
var updateDtoCode = $$"""
using Cart.Services.DTOs;

namespace Cart.Services.DTOs.{{entityType.Name}}
{
    public class {{entityType.Name}}UpdateDto : BaseDto<{{keyType}}>
    {
{{requiredProperties}}
    }
}
""";
File.WriteAllText(Path.Combine(dtoDir, $"{entityType.Name}UpdateDto.cs"), updateDtoCode);
Console.WriteLine($"✔ {entityType.Name}UpdateDto.cs oluşturuldu.");

// =========================================================================
// 4. Service Dosyalarını Üret (Abstract & Concrete)
// =========================================================================
var serviceAbstractDir = Path.Combine(solutionDir, "Cart.Services", "Abstracts");
var serviceConcreteDir = Path.Combine(solutionDir, "Cart.Services", "Concretes");

Directory.CreateDirectory(serviceAbstractDir);
Directory.CreateDirectory(serviceConcreteDir);

// 4.1. I{Entity}Service.cs
var serviceInterfaceCode = $$"""
using Cart.Domain.Entities;
using Cart.Services.DTOs.{{entityType.Name}};

namespace Cart.Services.Abstracts
{
    public interface I{{entityType.Name}}Service 
        : ICrudService<{{entityType.Name}}, {{entityType.Name}}Dto, {{entityType.Name}}CreateDto, {{entityType.Name}}UpdateDto, {{keyType}}>
    {
    }
}
""";
File.WriteAllText(Path.Combine(serviceAbstractDir, $"I{entityType.Name}Service.cs"), serviceInterfaceCode);
Console.WriteLine($"✔ I{entityType.Name}Service.cs oluşturuldu.");

// 4.2. {Entity}Service.cs
var serviceConcreteCode = $$"""
using AutoMapper;
using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.{{entityType.Name}};

namespace Cart.Services.Concretes
{
    public class {{entityType.Name}}Service(
        IRepository<{{entityType.Name}}, {{keyType}}> repo, IMapper mapper, IServiceProvider serviceProvider)
        : CrudService<{{entityType.Name}}, {{entityType.Name}}Dto, {{entityType.Name}}CreateDto, {{entityType.Name}}UpdateDto, {{keyType}}>(repo, mapper, serviceProvider), 
          I{{entityType.Name}}Service
    {
    }
}
""";
File.WriteAllText(Path.Combine(serviceConcreteDir, $"{entityType.Name}Service.cs"), serviceConcreteCode);
Console.WriteLine($"✔ {entityType.Name}Service.cs oluşturuldu.");

// =========================================================================
// 5. Controller Dosyasını Üret
// =========================================================================
var controllerDir = Path.Combine(solutionDir, "Cart.API", "Controllers");
Directory.CreateDirectory(controllerDir);

var controllerCode = $$"""
using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.{{entityType.Name}};
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class {{entityType.Name}}sController(I{{entityType.Name}}Service service)
        : BaseCrudController<{{entityType.Name}}, {{entityType.Name}}Dto, {{entityType.Name}}CreateDto, {{entityType.Name}}UpdateDto, {{keyType}}>(service)
    {
    }
}
""";
File.WriteAllText(Path.Combine(controllerDir, $"{entityType.Name}sController.cs"), controllerCode);
Console.WriteLine($"✔ {entityType.Name}sController.cs oluşturuldu.");

Console.WriteLine($"\n🎉 İşlem başarılı! {entityType.Name} için tüm katmanlar hazır.");

// =========================================================================
// Yardımcı Metod: C# Tip İsimlerini Formatlama
// =========================================================================
static string GetFriendlyTypeName(Type type)
{
    if (type == typeof(string)) return "string";
    if (type == typeof(int)) return "int";
    if (type == typeof(long)) return "long";
    if (type == typeof(decimal)) return "decimal";
    if (type == typeof(double)) return "double";
    if (type == typeof(bool)) return "bool";
    if (type == typeof(Guid)) return "Guid";
    if (type == typeof(DateTime)) return "DateTime";

    var nullableUnderlying = Nullable.GetUnderlyingType(type);
    if (nullableUnderlying != null)
        return $"{GetFriendlyTypeName(nullableUnderlying)}?";

    return type.Name;
}