using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Hotel_MVP;
[Keyless]
public partial class ServiceEmployee
{
    [ForeignKey("ServiceOrder")]
    [Column("serviceOrderId")]
    public Guid ServiceOrderId { get; set; }

    [ForeignKey("Employee")]
    [Column("employeeId")]
    public Guid EmployeeId { get; set; }

    public virtual ServiceOrder ServiceOrder { get; set; }
    public virtual Employee Employee { get; set; }
}
