using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sovereign.Mods
{
    public static class ModLoader
    {
        public static GovernmentMod LoadGovernmentMod(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            options.Converters.Add(new JsonStringEnumConverter());

            return JsonSerializer.Deserialize<GovernmentMod>(json, options);
        }
    }
}