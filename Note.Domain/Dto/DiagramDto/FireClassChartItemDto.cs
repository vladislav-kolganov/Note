namespace Note.Domain.Dto.DiagramDto;

/// <summary>
/// Данные для диаграммы количества меток пожара по местности и классу пожара.
/// </summary>
public class FireClassChartItemDto
{
    /// <summary>
    /// Название местности.
    /// </summary>
    public string LocationName { get; set; }

    /// <summary>
    /// Количество меток малого пожара.
    /// </summary>
    public int SmallCount { get; set; }

    /// <summary>
    /// Количество меток среднего пожара.
    /// </summary>
    public int MediumCount { get; set; }

    /// <summary>
    /// Количество меток большого пожара.
    /// </summary>
    public int LargeCount { get; set; }

    /// <summary>
    /// Количество меток без пожара.
    /// </summary>
    public int? NoFireCount { get; set; }

    /// <summary>
    /// Общее количество меток пожара по местности.
    /// </summary>
    public int TotalCount => SmallCount + MediumCount + LargeCount;
}