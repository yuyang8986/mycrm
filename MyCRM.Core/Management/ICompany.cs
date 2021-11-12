using System.Collections.Generic;

namespace MyCRM.Core.Management
{
    public interface ICompany<TTemplate> where TTemplate:struct
    {
        IEnumerable<TTemplate> CompanyOwnedTemplate { get; }
    }
}