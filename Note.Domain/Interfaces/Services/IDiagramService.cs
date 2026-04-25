using Note.Domain.Dto.DiagramDto;
using Note.Domain.Result;

namespace Note.Domain.Interfaces.Services;

/// <summary>
/// Сервис для получения данных, необходимых для построения диаграмм.
/// </summary>
public interface IDiagramService
{
    /// <summary>
    /// Получает данные для построения диаграммы количества меток пожара
    /// по местности и классу пожара.
    /// </summary>
    /// <param name="reportIds">
    /// Массив идентификаторов отчётов, по которым необходимо построить диаграмму.
    /// </param>
    /// <returns>
    /// Результат выполнения операции, содержащий коллекцию элементов диаграммы.
    /// Каждый элемент содержит название местности и количество меток пожара
    /// малого, среднего и большого класса.
    /// </returns>
    public Task<CollectionResult<FireClassChartItemDto>> GetFireClassChartAsync(long[] reportIds);
}