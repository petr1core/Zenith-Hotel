using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hotel_MVP;

public partial class ServiceOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("serviceOrderId")]
    public Guid Id { get; set; }

    [ForeignKey("Service")]
    [Column("serviceId")]
    public Guid ServiceId { get; set; }

    [ForeignKey("User")]
    [Column("userId")]
    public Guid UserId { get; set; }

    [ForeignKey("Room")]
    [Column("roomId")]
    public Guid RoomId { get; set; }

    [ForeignKey("Department")]
    [Column("departmentId")]
    public Guid DepartmentId { get; set; }

    [Required]
    [Column("orderDate")]
    public DateOnly OrderDate { get; set; }

    [Required]
    [Column("orderStatus", TypeName = "varchar(128)")]
    public string OrderStatus { get; set; } = null!;

    public virtual Service Service { get; set; }
    public virtual User User { get; set; }
    public virtual Room Room { get; set; }
    public virtual Department Department { get; set; }
}
