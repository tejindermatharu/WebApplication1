using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Infrastructure
{
    public static class SystemHelper
    {
        public static bool IsRunningInContainer { get { return System.Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"; } }
    }
}
