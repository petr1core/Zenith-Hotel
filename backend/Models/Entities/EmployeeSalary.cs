using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_MVP;

public partial class EmployeeSalary
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("employeeSalaryId")]
    public Guid Id { get; set; }

    [Required]
    [Column("hourlyRate", TypeName = "decimal(9,2)")]
    public required decimal HourlyRate { get; set; }

    [Required]
    [Column("monthlyHours", TypeName = "decimal(9,2)")]
    public required decimal MonthlyHours { get; set; }

    [Required]
    [Column("timeWorked", TypeName = "decimal(5,2)")]
    public decimal TimeWorked { get; set; }

    [Required]
    [Column("paymentAmount", TypeName = "decimal(9,2)")]
    public decimal PaymentAmount { get; set; }

    [Required]
    [Column("paymentDate")]
    public DateOnly PaymentDate { get; set; }
}
