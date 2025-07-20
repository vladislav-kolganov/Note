using System.Text.Json.Serialization;

namespace Note.Domain.Dto.PredictionDto.ForestDto
{
    public record ForestPredictionDto
    (
       [property: JsonPropertyName("forest")] bool IsForest,
       [property: JsonPropertyName("proba")] double Probability
    );
}
