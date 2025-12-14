namespace Note.Domain.Dto.ReportDto;

/// <summary>
/// Дто для шеринга отчётов.
/// </summary>
/// <param name="OwnerUserId">Id владельца отчёта.</param>
/// <param name="TargetUserId">Id с кем делятся отчёт.</param>
/// <param name="ReportId">Id отчёта, кототрым делятся.</param>
public record ShareReportDto(long OwnerUserId, long TargetUserId, long ReportId);