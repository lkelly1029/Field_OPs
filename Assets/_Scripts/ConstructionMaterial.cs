using Postgrest.Attributes;  // CHANGED THIS LINE
using Postgrest.Models;      // CHANGED THIS LINE

[Table("material_library")]
public class ConstructionMaterial : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("category")]
    public string Category { get; set; }

    [Column("unit_cost")]
    public float UnitCost { get; set; }

    [Column("unit_labor_hours")]
    public float LaborHours { get; set; }
}