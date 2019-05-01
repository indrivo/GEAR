using System;

namespace ST.Core
{
    public static class Settings
    {
        public const string DefaultLanguage = "English";

        public static Guid TenantId = Guid.Parse("d11eeb3d-9545-4f1a-a199-632257326765");

        public const string SuperAdmin = "Administrator";

        public struct Tables
        {
            public const string Nomenclature = "Nomenclature";
            public const string NomKpiCategory = "NomKPICategory";
            public const string NomMeasurement = "NomMeasurement";
            public const string NomPeriod = "NomPeriod";
            public const string NomFulfillment = "NomFulfillment";
            public const string NomGoal = "NomGoal";
            public const string NomRequirement = "NomRequirement";
            public const string NomInterestedPartyType = "NomInterestedPartyType";
            public const string NomInterestedParty = "NomInterestedParty";
            public const string Kpi = "KPI";
            public const string PartyRequirement = "PartyRequirement";
            public const string GoalPartyRequirement = "GoalPartyRequirement";
            public const string Standards = "Standards";
            public const string StandardCategories = "StandardCategories";
            public const string CategoryRequirements = "CategoryRequirements";
        }
    }
}
