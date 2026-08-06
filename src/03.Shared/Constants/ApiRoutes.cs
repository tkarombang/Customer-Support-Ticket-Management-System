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
        public const string Attachments = $"{ById}/attachments";
    }

    public static class TicketHistories
    {
        public const string Base = $"{ApiRoutes.Base}/ticket-histories";
    }

    public static class Reports
    {
        public const string Base = $"{ApiRoutes.Base}/reports";
        public const string ManagerReport = "manager-report";
        public const string SlaCompliance = "sla-compliance";
        public const string ResponseTime = "response-time";
        public const string Export = "export";
    }

    public static class Dashboard
    {
        public const string Base = $"{ApiRoutes.Base}/dashboard";
        public const string summary = "summary";
    }

    public static class Users
    {
        public const string Base = $"{ApiRoutes.Base}/users";
        public const string ById = "{id:guid}";
        public const string ToggleStatus = $"{ById}/status";
    }

    public static class Profile
    {
        public const string Base = $"{ApiRoutes.Base}/profile";
        public const string Password = "Password" ;
        public const string ActivityLog = "activity-log";
    }

    public static class Settings
    {
        public const string Base = $"{ApiRoutes.Base}/settings";
        public const string General = "general";
        public const string Sla = "sla";
        public const string Integrations = "integrations";
        public const string IntegrationById = "integrations/{id:guid}";
        public const string Backup = "backup";
        public const string Restore = "restore";
        public const string SystemLogs = "system-logs";
    }
}