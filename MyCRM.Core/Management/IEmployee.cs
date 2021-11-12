namespace MyCRM.Core.Management
{
    public interface IEmployee<TRole> where TRole:struct 
    {
        bool IsSupervisor { get; set; }
        EmployeeRole EmployeeRole { get; set; }
    }
}