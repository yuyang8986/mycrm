using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCRM.Core.Management
{
    public class CompanyCompanyOwnedTemplates
    {
        //for company own companytemplate join table purpose, company to CompanyCompanyTemplates is 1 to Many,
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyCompanyTemplatesId { get; set; }
        public int CompanyId { get; set; }
        public CompanyOwnedTemplate CompanyOwnedTemplate  { get; set; }

        public Company Company { get; set; }
    }
}