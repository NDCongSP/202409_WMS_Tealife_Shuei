namespace Application.Extentions
{
    public static class ConstantExtention
    {
        public const string BrowserStorageKey = "x-key";
        public const string HttpClientName = "WebUIClient";
        public const string HttpClientHeaderScheme = "Bearer";

        public static class Roles
        {
            public const string WarehouseAdmin = "Warehouse Admin";
            public const string WarehouseStaff = "Warehouse Staff";
            public const string WarehouseSystem = "Warehouse System";
        }

        public static class StorageConst
        {
            public const string AuthToken = "AuthToken";
            public const string RefreshToken = "RefreshToken";
            public const string RemeberMe = "RememberMe";
            public const string SessionId = "SessionId";
        }

        public static class ViewMode
        {
            public const string View = "View";
            public const string Edit = "Edit";
            public const string Create = "Create";
        }
    }
}
