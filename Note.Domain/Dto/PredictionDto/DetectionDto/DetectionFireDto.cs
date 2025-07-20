using System.Text.Json.Serialization;

namespace Note.Domain.Dto.PredictionDto.DetectionDto
{
    /// <summary>
    /// Дто ответа детекциии YOLOv8M
    /// </summary>
    /// <param name="CoordinatesFireArea">Координаты на картинке, если есть пожар.</param>
    /// <param name="AreaPix">Площадь в пикселях.</param>
    /// <param name="FireArea">Площадь в метрах квадратных.</param>
    /// <param name="PathToImage">Путь до картинки в докер контейнере, которая возвращает YOLO.</param>
    public record DetectionFireDto
    (
       [property: JsonPropertyName("fire_area")] int[][] CoordinatesFireArea,
       [property: JsonPropertyName("area_pix")] int[] AreaPix,
       [property: JsonPropertyName("area_m2")] double[] FireArea,
       [property: JsonPropertyName("result_img")] string PathToImage
    );
}
