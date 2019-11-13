using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class ThreadingExtensions
    {
        public static T WaitResult<T>(this Task<T> task)
        {
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            return task.Result;
        }
    }
}
