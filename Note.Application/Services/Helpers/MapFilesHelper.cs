using Note.Application.Services.Extensions;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Entity;
using Note.Domain.Entity.Map;
using Note.Domain.Enum.BusinessEnums;
using Note.Domain.Result;

namespace Note.Application.Services.Helpers;

public static class MapFilesHelper
{
    public static List<ReportPhoto>? BuildReportPhotos(Dictionary<string, string>? photos)
    {
        if (photos is null || photos.Count == 0)
            return null;

        return photos
            .Where(p => !string.IsNullOrWhiteSpace(p.Key))
            .Select(p => new ReportPhoto
            {
                Content = Convert.FromBase64String(p.Key),
                Description = string.IsNullOrWhiteSpace(p.Value) ? string.Empty : p.Value
            })
            .ToList();
    }

    public static List<ReportMapMarker>? BuildMapMarkers(List<CreateReportMapMarkerDto>? mapMarkers)
    {
        if (mapMarkers is null || mapMarkers.Count == 0)
            return null;

        var result = new List<ReportMapMarker>();

        foreach (var markerDto in mapMarkers)
        {
            var validationResult = ValidateMarker(markerDto);
            if (!validationResult.IsSuccess)
            {
                throw new InvalidOperationException(validationResult.ErrorMessage);
            }

            result.Add(new ReportMapMarker
            {
                Latitude = markerDto.Latitude,
                Longitude = markerDto.Longitude,
                LocationName = markerDto.LocationName.Trim(),
                FireClass = (FireClassEnum)markerDto.FireClass,
                Comment = string.IsNullOrWhiteSpace(markerDto.Comment) ? null : markerDto.Comment.Trim(),
                CreatedAt = DateTime.UtcNow,
                Attachments = BuildMarkerAttachments(markerDto.Attachments) ?? new List<ReportMapMarkerAttachment>()
            });
        }

        return result;
    }

    public static List<ReportMapMarkerAttachment>? BuildMarkerAttachments(List<CreateReportMapMarkerAttachmentDto>? attachments)
    {
        if (attachments is null || attachments.Count == 0)
            return null;

        var result = new List<ReportMapMarkerAttachment>();

        foreach (var attachment in attachments)
        {
            if (string.IsNullOrWhiteSpace(attachment.Base64Content))
                continue;

            var bytes = Convert.FromBase64String(attachment.Base64Content);

            if (bytes.IsNullOrEmpty())
            {
                throw new InvalidOperationException($"Недопустимый формат файла: {attachment.FileName}. Разрешены только PNG и TIF/TIFF.");
            }

            result.Add(new ReportMapMarkerAttachment
            {
                FileName = attachment.FileName,
                ContentType = attachment.ContentType,
                Description = attachment.Description,
                Content = bytes
            });
        }

        return result;
    }

    private static BaseResult ValidateMarker(CreateReportMapMarkerDto marker)
    {
        if (marker is null)
        {
            return new BaseResult()
            {
                ErrorMessage = "Маркер не передан.",
                ErrorCode = 1001
            };
        }

        if (double.IsNaN(marker.Latitude) || double.IsInfinity(marker.Latitude))
        {
            return new BaseResult()
            {
                ErrorMessage = "Latitude имеет некорректное значение.",
                ErrorCode = 1001
            };
        }

        if (double.IsNaN(marker.Longitude) || double.IsInfinity(marker.Longitude))
        {
            return new BaseResult()
            {
                ErrorMessage = "Longitude имеет некорректное значение.",
                ErrorCode = 1001
            };
        }

        if (marker.Latitude < -90 || marker.Latitude > 90)
        {
            return new BaseResult()
            {
                ErrorMessage = "Latitude должна быть в диапазоне от -90 до 90.",
                ErrorCode = 1001
            };
        }

        if (marker.Longitude < -180 || marker.Longitude > 180)
        {
            return new BaseResult()
            {
                ErrorMessage = "Longitude должна быть в диапазоне от -180 до 180.",
                ErrorCode = 1001
            };
        }

        if (string.IsNullOrWhiteSpace(marker.LocationName))
        {
            return new BaseResult()
            {
                ErrorMessage = "Название местности обязательно.",
                ErrorCode = 1001
            };
        }

        if (marker.LocationName.Length > 300)
        {
            return new BaseResult()
            {
                ErrorMessage = "Название местности не должно превышать 300 символов.",
                ErrorCode = 1001
            };
        }

        if (marker.Comment is not null && marker.Comment.Length > 2000)
        {
            return new BaseResult()
            {
                ErrorMessage = "Комментарий не должен превышать 2000 символов.",
                ErrorCode = 1001
            };
        }

        if (!Enum.IsDefined(typeof(FireClassEnum), marker.FireClass))
        {
            return new BaseResult()
            {
                ErrorMessage = "Некорректный класс пожара.",
                ErrorCode = 1001
            };
        }

        return new BaseResult();
    }
}