using System;

namespace MyCRM.Shared.Communications.Requests.Pipeline
{
    public class PipelineAddRequest
    {
        public string DealName { get; set; }
        public double DealAmount { get; set; }
        public string Note { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Type { get; set; }
        public double? CogsAmount { get; set; }
        public double? Margin { get; set; }

        public DateTime? AttainDate { get; set; }
        public string ApplicationUserId { get; set; }//can add later?
        public int StageId { get; set; }
        public int? PeopleId { get; set; }//optional?
        public int? CompanyId { get; set; }//optional?
    }
}