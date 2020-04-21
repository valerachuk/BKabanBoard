using Microsoft.AspNetCore.Http;

namespace BKabanApi.Utils
{
    public static class AuthHelper
    {
        private const int DebugUserId = 1;

        private const string KeyName = "userId";

        public static int? GetUserId(HttpContext ctx)
        {
#if DEBUG
            return DebugUserId;
#else
            return ctx.Session.GetInt32(KeyName);
#endif
        }

        public static void AddUserToSession(HttpContext ctx, int id)
        {
            ctx.Session.SetInt32(KeyName, id);
        }

        public static void LogOut(HttpContext ctx)
        {
            ctx.Session.Remove(KeyName);
        }

    }
}
