using System.ComponentModel.DataAnnotations;

namespace MyCRM.Shared.Communications.Requests.Employee
{
    public class EmployeeAddRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsManager { get; set; }

        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}