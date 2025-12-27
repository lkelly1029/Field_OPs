namespace Sovereign.Core
{
    public class ResourceDefinition
    {
        public ResourceType Type { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public bool IsCritical { get; set; }
        public double DecayRate { get; set; } // Percentage lost per tick (0.0 to 1.0)

        public ResourceDefinition(ResourceType type, string name, string unit, bool isCritical = false, double decayRate = 0)
        {
            Type = type;
            Name = name;
            Unit = unit;
            IsCritical = isCritical;
            DecayRate = decayRate;
        }

        public static ResourceDefinition Power => new ResourceDefinition(ResourceType.Power, "Power", "Wh", true);
        public static ResourceDefinition Water => new ResourceDefinition(ResourceType.Water, "Water", "Liter", true);
        public static ResourceDefinition Food => new ResourceDefinition(ResourceType.Food, "Food", "kg", true, 0.1); // 10% decay per tick
        public static ResourceDefinition Steel => new ResourceDefinition(ResourceType.Steel, "Steel", "Tonne", false);
        public static ResourceDefinition Iron => new ResourceDefinition(ResourceType.Iron, "Iron", "Tonne", false);

        public static double GetDecayRate(ResourceType type)
        {
            return type switch
            {
                ResourceType.Food => 0.1,
                _ => 0
            };
        }
    }
}