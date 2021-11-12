using System.Threading.Tasks;

namespace ETLib.Helpers
{
    public static class TaskResultHelper
    {
        public static object GetTaskResult(this Task<object> task)
        {
            return task.Result;
        }
    }
}