using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_MVP;

public partial class Service
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("serviceId")]
    public Guid Id { get; set; }

    [Required]
    [Column("serviceName", TypeName = "varchar(128)")]
    public required string ServiceName { get; set; }

    [Column("servicePrice", TypeName = "decimal(9,2)")]
    public decimal ServicePrice { get; set; }

    [Column("serviceIsActive", TypeName = "boolean")]
    public bool IsActive { get; set; } = false;
}
