using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using QRCoder.Core;

namespace API.BackgroundJobs
{
    public static class GlobalVariable
    {
        //thời gian chu kỳ cảu background job, đơn vị là giây (s).
        public static int BackgroundJobInterval { get; set; } = 10;
    }
}
