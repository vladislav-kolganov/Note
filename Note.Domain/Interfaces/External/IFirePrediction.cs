using Note.Domain.Dto.PredictionDto.DetectionDto;
using Note.Domain.Dto.PredictionDto.ForestDto;
using Refit;

namespace Note.Domain.Interfaces.External
{
    /// <summary>
    /// Интерфейс методов взаимодействия с Api на питоне.
    /// Пожар в лесу.
    /// </summary>
    public interface IFirePrediction
    {
        /// <summary>
        /// Метод определения на картинке есть ли лес.
        /// CNN.
        /// </summary>
        /// <param name="image">Входящее изображение</param>
        /// <param name="ct">Остановка работы.</param>
        /// <returns>Дто предикции леса на изображении.</returns>
        [Post("/classificate/forest")]
        Task<ForestPredictionDto> ClassifyForestAsync(Stream image, CancellationToken ct = default);

        /// <summary>
        /// Метод определения площади пожара на фото.
        /// YOLOv8M.
        /// </summary>
        /// <param name="image">Входящее изображение.</param>
        /// <param name="ct"></param>
        /// <returns>Дто расчетов YOLO</returns>
        [Post("/detect/fire")]
        Task<DetectionFireDto> DetectFireAsync(Stream image, CancellationToken ct = default);
    }
}
