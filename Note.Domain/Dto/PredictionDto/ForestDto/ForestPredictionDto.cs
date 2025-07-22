using System.Text.Json.Serialization;

namespace Note.Domain.Dto.PredictionDto.ForestDto
{
    /// <summary>
    /// Дто содержащая результат, которая возвращает CNN.
    /// </summary>
    /// <param name="IsForest">На фото лес.</param>
    /// <param name="Probability">Вероятность леса на фото.</param>
    public record ForestPredictionDto
    (
       [property: JsonPropertyName("forest")] bool IsForest,
       [property: JsonPropertyName("proba")] double Probability
    );
}
