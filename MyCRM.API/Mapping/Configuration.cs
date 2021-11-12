using AutoMapper;
using MyCRM.Shared.Communications.Requests.Appointment;
using MyCRM.Shared.Communications.Requests.Company;
using MyCRM.Shared.Communications.Requests.Employee;
using MyCRM.Shared.Communications.Requests.Event;
using MyCRM.Shared.Communications.Requests.Organization;
using MyCRM.Shared.Communications.Requests.People;
using MyCRM.Shared.Communications.Requests.Pipeline;
using MyCRM.Shared.Communications.Requests.Stage;
using MyCRM.Shared.Communications.Requests.TargetTemplate;
using MyCRM.Shared.Communications.Requests.Task;
using MyCRM.Shared.Communications.Responses.Schedule;
using MyCRM.Shared.Communications.Responses.User;
using MyCRM.Shared.Models.Activities;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.Models.Events;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.Stages;
using MyCRM.Shared.Models.TargetTemplate;
using MyCRM.Shared.Models.Tasks;
using MyCRM.Shared.Models.User;
using MyCRM.Shared.ViewModels.ApplicationUser;
using MyCRM.Shared.ViewModels.Contact.CompanyViewModel;
using MyCRM.Shared.ViewModels.Contact.PersonViewModel;
using MyCRM.Shared.ViewModels.PipelineViewModels;
using MyCRM.Shared.ViewModels.ScheduleViewModels;
using MyCRM.Shared.ViewModels.StageViewModels;
using MyCRM.Shared.ViewModels.TargetTemplateViewModels;

namespace MyCRM.API.Mapping
{
    public class Configuration : Profile
    {
        public Configuration()
        {
            //CreateMap<Product, ProductResource>()
            //    .ForMember(src => src.UnitOfMeasurement,
            //        opt => opt.MapFrom(src => src.UnitOfMeasurement.ToDescriptionString()));

            CreateMap<EmployeeAddRequest, ApplicationUser>().ReverseMap();
            CreateMap<EmployeePutRequest, Stage>().ReverseMap();
            CreateMap<StagePutRequest, Stage>().ReverseMap();
            CreateMap<StageAddRequest, Stage>().ReverseMap();
            CreateMap<StageGetModel, Stage>().ReverseMap();
            CreateMap<AppointmentAddRequest, Appointment>().ReverseMap();
            CreateMap<AppointmentPutRequest, Appointment>().ReverseMap();
            CreateMap<CompanyAddRequest, Company>().ReverseMap();
            CreateMap<EmployeeAddRequest, ApplicationUser>().ReverseMap();
            CreateMap<EmployeePutRequest, ApplicationUser>().ReverseMap();
            CreateMap<TaskAddRequest, Task>().ReverseMap();
            CreateMap<EventAddRequest, Event>().ReverseMap();
            CreateMap<EventPutRequest, Event>().ReverseMap();
            CreateMap<OrganizationAddRequest, Organization>().ReverseMap();
            CreateMap<PeopleAddRequest, People>().ReverseMap();
            CreateMap<PeoplePutRequest, People>().ReverseMap();
            CreateMap<PipelineGetAllModel, Pipeline>().ForMember(x => x.ChangeStageDate,opt => opt.Ignore()).ReverseMap();
            CreateMap<PipelineAddRequest, Pipeline>().ReverseMap();
            CreateMap<PipelinePutRequest, Pipeline>().ForMember(x => x.ChangeStageDate, opt => opt.Ignore()).ReverseMap();
            CreateMap<PipelinePutRequest, Pipeline>().ForMember(x => x.CreatedDate, opt => opt.Ignore()).ReverseMap();
            CreateMap<PipelineGetAllModel, Pipeline>().ReverseMap()
                .ForMember(x => x.NextActivity, opt => opt.Ignore());
            CreateMap<TargetTemplateAddRequest, TargetTemplate>().ReverseMap();
            CreateMap<TargetTemplatePutRequest, TargetTemplate>().ReverseMap();
            CreateMap<ApplicationUser, UserDataResponseModel>();
            CreateMap<CompanyGetModel, Company>().ReverseMap();
            CreateMap<PersonGetModel, People>().ReverseMap();
            CreateMap<ScheduleEventModel, ScheduleGetModel>().ReverseMap();
            CreateMap<ActivityViewModel, Activity>().ReverseMap();
            CreateMap<AppointmentGetModelForSchedule, Appointment>().ReverseMap();
            CreateMap<EventGetModelForSchedule, Event>().ReverseMap();
            CreateMap<TaskGetModelForSchedule, Task>().ReverseMap();
            CreateMap<TargetTemplateGetModel, TargetTemplate>().ReverseMap();
            CreateMap<ApplicationUserGetAllViewModel, ApplicationUser>().ReverseMap(); 
            CreateMap<PipelineGetModelForAppUser, Pipeline>().ReverseMap();
        }
    }
}