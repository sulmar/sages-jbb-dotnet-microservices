namespace ProductCatalog.Api.Models;

// DTO = Data Transfer Object
//public class PriceDto
//{
//    public decimal Price { get; set; }

//    public override string ToString()
//    {
//        return $"Price: {Price}";
//    }

//    public override bool Equals(object? obj)
//    {       
//        return base.Equals(obj);
//    }


//}

public record PriceDto(decimal Price);
