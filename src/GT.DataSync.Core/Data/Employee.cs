﻿using System.ComponentModel.DataAnnotations;

namespace GT.DataSync.Core.Data;


public class Employee : Entity
{    
    [Required]
    [StringLength(100)]
    public string? GivenName { get; set; }
    [Required]
    [StringLength(100)]
    public string? FamilyName { get; set; }
    [StringLength(50)]
    public string? BadgeCode { get; set; }
    [Required]
    [StringLength(50)]
    public string? KeypadId { get; set; }
    [StringLength(50)]
    public string? PinCode { get; set; }
    [StringLength(50)]
    public string? Language { get; set; }
    public List<string>? Roles { get; set; }
    public List<string>? VerifyOrder { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? ExtendedData { get; set; }
    public List<string>? DistributionGroups { get; set; }
    public bool? Delete { get; set; }
    
}
