using UnityEngine;
using Supabase;
using System.Threading.Tasks;

public class SupabaseManager : MonoBehaviour
{
    public static Supabase.Client Client;

    // References your Secrets file
    private string supabaseUrl = Secrets.SUPABASE_URL;
    private string supabaseKey = Secrets.SUPABASE_KEY;

    async void Start()
    {
        // 1. Connect
        await InitializeSupabase();

        // 2. TEST: Upload a new item to Supabase
        await CreateTestMaterial();
    }

    private async Task InitializeSupabase()
    {
        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        Client = new Supabase.Client(supabaseUrl, supabaseKey, options);
        await Client.InitializeAsync();
        Debug.Log($"<color=green>SUCCESS: Connected to Supabase!</color>");
    }

    // --- NEW FUNCTION: UPLOADS DATA ---
    public async Task CreateTestMaterial()
    {
        // 1. Create the object in C# memory
        var newItem = new ConstructionMaterial
        {
            // We don't set ID because Supabase generates it automatically
            Name = "Unity Upload Test Item",
            Category = "Software Test",
            UnitCost = 123.45f,
            LaborHours = 2.0f
        };

        // 2. Send it to the cloud
        // 'Insert' sends the data. 'Single()' retrieves the result so we know it worked.
        var result = await Client.From<ConstructionMaterial>().Insert(newItem);

        Debug.Log($"<color=green>UPLOAD COMPLETE: Sent '{newItem.Name}' to database!</color>");
    }
}