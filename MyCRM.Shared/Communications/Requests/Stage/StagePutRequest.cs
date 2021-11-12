namespace MyCRM.Shared.Communications.Requests.Stage
{
    public class StagePutRequest
    {
        public string Name { get; set; }
        public string ThisMonth { get; set; }
        public string ThisQuater { get; set; }
        

        public int? IconIndex { get; set; }
        public int DisplayIndex { get; set; }
    }
}