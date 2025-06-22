using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.Util
{
    public static class Advisory1to1Util
    {
        public static string GenerateSessionTitle(string? message, string staffFirstName, string staffLastName)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return $"{staffFirstName} {staffLastName} at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            }

            var trimmed = message.Trim();
            int endIdx = trimmed.IndexOfAny(new[] { '.', '!', '?' });
            return endIdx > 0 && endIdx < 40
                ? trimmed.Substring(0, endIdx + 1)
                : trimmed.Length > 40 ? trimmed.Substring(0, 40) + "..." : trimmed;
        }
    }
}