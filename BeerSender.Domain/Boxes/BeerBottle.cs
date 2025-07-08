namespace BeerSender.Domain.Boxes;

public record BeerBottle(

    string Brewery,
    string Name,
    double AlcoholPercentage,
    BeerType BeerType
)
{
    public string BottleId => $"[{Brewery}] {Name}";
}