using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Dishes.Dtos;
using foodie_connect_backend.Modules.DishReviews.Dtos;
using foodie_connect_backend.Modules.Promotions.Mapper;
using foodie_connect_backend.Shared.Dtos;

namespace foodie_connect_backend.Modules.Dishes.Mapper;

public static class DishesMapper
{
    public static DishResponseDto ToResponseDto(this Dish dish, ScoreResponseDto? score = null)
    {
        return new DishResponseDto
        {
            DishId = dish.Id,
            RestaurantId = dish.RestaurantId,
            Name = dish.Name,
            Description = dish.Description,
            ImageId = dish.ImageId,
            Price = dish.Price,
            Categories = dish.Categories.Select(x => x.CategoryName).ToArray(),
            ScoreOverview = score ?? new ScoreResponseDto(),
            Promotions = dish.PromotionDetails.Count > 0 ? 
                dish.PromotionDetails.Select(promotionDetails => promotionDetails.ToFullResponseDto()).ToList() 
                : []
        };
    }
}