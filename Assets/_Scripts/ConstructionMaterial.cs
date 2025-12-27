using Postgrest.Attributes;
using Postgrest.Models;

[Table("construction_materials")]
public class ConstructionMaterial : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("category")]
    public string Category { get; set; }

    [Column("unit_cost")]
    public float UnitCost { get; set; }

    [Column("labor_hours")]
    public float LaborHours { get; set; }
}
