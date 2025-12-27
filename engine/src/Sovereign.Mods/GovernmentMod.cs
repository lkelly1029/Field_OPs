using System;

namespace Sovereign.Mods
{
    /// <summary>
    /// Represents a set of government policies and modifiers loaded from a mod file.
    /// These settings control macroeconomic variables like taxes, UBI, and tariffs.
    /// </summary>
    public class GovernmentMod
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Fiscal Policy
        public long UniversalBasicIncomeCents { get; set; }
        public double IncomeTaxRate { get; set; }
        public double CorporateTaxRate { get; set; }
        
        // Trade Policy
        public double ImportTariffRate { get; set; }
        public double ExportSubsidyRate { get; set; }

        // Modifiers
        public double ConstructionCostMultiplier { get; set; } = 1.0;
        public double EnergyConsumptionMultiplier { get; set; } = 1.0;

        public static GovernmentMod Default => new GovernmentMod
        {
            Id = "default_gov",
            Name = "Default Administration",
            Description = "Standard balanced policies.",
            UniversalBasicIncomeCents = 0,
            IncomeTaxRate = 0.0,
            CorporateTaxRate = 0.0,
            ImportTariffRate = 0.0,
            ConstructionCostMultiplier = 1.0,
            EnergyConsumptionMultiplier = 1.0
        };
    }
}