namespace MyCRM.Shared.Communications.Requests.Stage
{
    public class StageAddRequest
    {
        public string Name { get; set; }
        public string PrimarySummaryName { get; set; }
        public string SecondarySummaryName { get; set; }
        public string ThirdSummaryName { get; set; }

        public int? IconIndex { get; set; }
        public int DisplayIndex { get; set; }
    }
}