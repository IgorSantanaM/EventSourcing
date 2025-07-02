namespace BeerSender.Domain.Boxes;

public record BeerBottle(

    string Brewery,
    string Name,
    double AlcoholPercentage,
    BeerType BeerType
);