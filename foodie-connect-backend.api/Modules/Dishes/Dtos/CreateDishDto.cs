using System.ComponentModel.DataAnnotations;
using foodie_connect_backend.Data;

namespace foodie_connect_backend.Modules.Dishes.Dtos;

public class CreateDishDto
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; } = 0;
    public string[] Categories { get; set; } = [];
}