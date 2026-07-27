namespace TicketManagement.Shared.Constants;

public static class ApiRoutes
{
    public const string Base = "api";

    public static class Auth
    {
        public const string Base = $"{ApiRoutes.Base}/auth";

        public const string Login = "login";
    }

    public static class Tickets
    {
        public const string Base = $"{ApiRoutes.Base}/tickets";

        public const string ById = "{id:guid}";
        public const string Assign = $"{ById}/assign";
        public const string Report = "report";
    }

    public static class Reports
    {
        public const string Base = $"{ApiRoutes.Base}/reports";
        public const string ManagerSegment = "manager";
        public const string ManagerReport = "manager-report";
    }
}