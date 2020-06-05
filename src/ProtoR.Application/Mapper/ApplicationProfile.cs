namespace ProtoR.Application.Mapper
{
    using AutoMapper;
    using ProtoR.Application.Permission;
    using ProtoR.Application.Schema;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;

    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            this.CreateMap<RuleViolation, RuleViolationDto>()
                .ForMember(d => d.IsFatal, opt => opt.MapFrom(s => s.Severity.IsFatal))
                .ForMember(d => d.RuleCode, opt => opt.MapFrom(s => s.ValidationResult.RuleCode.ToString()))
                .ForMember(d => d.Severity, opt => opt.MapFrom(s => s.Severity.Id))
                .ForMember(d => d.IsForwardIncompatible, opt => opt.MapFrom(s => !s.BackwardCompatibilityViolation))
                .ForMember(d => d.IsBackwardIncompatible, opt => opt.MapFrom(s => s.BackwardCompatibilityViolation))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.ValidationResult.Description))
                .ForMember(d => d.ConflictingVersion, opt => opt.MapFrom(s => s.OldVersion.VersionNumber));

            this.CreateMap<Permission, PermissionDto>();
        }
    }
}
