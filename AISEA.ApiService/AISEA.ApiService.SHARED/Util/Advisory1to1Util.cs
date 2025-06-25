using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.Util
{
    public static class Advisory1to1Util
    {

        public static string GenerateChatBotSessionTitle(string message)
        {
            var trimmed = message.Trim();
            int endIdx = trimmed.IndexOfAny(new[] { '.', '!', '?' });
            return endIdx > 0 && endIdx < 40
                ? trimmed.Substring(0, endIdx + 1)
                : trimmed.Length > 40 ? trimmed.Substring(0, 40) + "..." : trimmed;
        }
        public static string GenerateHumanSessionTitle(string staffName) => $"{staffName} {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

    }
}