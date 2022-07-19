using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GT.DataSync.Core.Data;

public class Entity
{
    [Required]
    [StringLength(100)]
    public string? Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string? DisplayName { get; set; }
    
    [JsonIgnore] 
    public DateTimeOffset CreatedOn { get; set; }
    
    [JsonIgnore] 
    public DateTimeOffset ModifiedOn { get; set; }
}
