using AutoMapper;
using Note.Domain.Dto.ReportDto;
using Note.Domain.Entity;
using Note.Domain.Entity.Map;

namespace Note.Application.Mapping;

/// <summary>
/// Маппер для отчетов.
/// </summary>
public class ReportMapping : Profile
{
    public ReportMapping()
    {
        CreateMap<Report, ReportDto>()
        .ForCtorParam(ctorParamName: "Id", m => m.MapFrom(s => s.Id))
        .ForCtorParam(ctorParamName: "Name", m => m.MapFrom(s => s.Name))
        .ForCtorParam(ctorParamName: "Description", m => m.MapFrom(s => s.Description))
        .ForCtorParam(ctorParamName: "DateCreated", m => m.MapFrom(s => s.CreatedAt))
        .ForCtorParam(ctorParamName: "MapMarkers", m => m.MapFrom(s => s.MapMarkers))
        .ReverseMap();

        CreateMap<ReportMapMarker, ReportMapMarkerDto>()
        .ForCtorParam(ctorParamName: "Id", m => m.MapFrom(s => s.Id))
        .ForCtorParam(ctorParamName: "Latitude", m => m.MapFrom(s => s.Latitude))
        .ForCtorParam(ctorParamName: "Longitude", m => m.MapFrom(s => s.Longitude))
        .ForCtorParam(ctorParamName: "LocationName", m => m.MapFrom(s => s.LocationName))
        .ForCtorParam(ctorParamName: "FireClass", m => m.MapFrom(s => s.FireClass))
        .ForCtorParam(ctorParamName: "Comment", m => m.MapFrom(s => s.Comment))
        .ForCtorParam(ctorParamName: "Attachments", m => m.MapFrom(s => s.Attachments));

        CreateMap<ReportMapMarkerAttachment, ReportMapMarkerAttachmentDto>()
        .ForCtorParam(ctorParamName: "Id", m => m.MapFrom(s => s.Id))
        .ForCtorParam(ctorParamName: "FileName", m => m.MapFrom(s => s.FileName))
        .ForCtorParam(ctorParamName: "ContentType", m => m.MapFrom(s => s.ContentType))
        .ForCtorParam(ctorParamName: "Description", m => m.MapFrom(s => s.Description))
        .ForCtorParam(ctorParamName: "Base64Content", m => m.MapFrom(s => Convert.ToBase64String(s.Content)));
    }
}