using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hotel_MVP;

public partial class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("employeeId")]
    public Guid Id { get; set; }

    [ForeignKey("Department")]
    [Column("departmentId")]
    public Guid DepartmentId { get; set; }

    [ForeignKey("EmployeeSalary")]
    [Column("employeeSalaryId")]
    public Guid EmployeeSalaryId { get; set; }

    [Required]
    [Column("emplFirstname", TypeName = "varchar(128)")]
    public required string EmplFirstname { get; set; }

    [Required]
    [Column("emplLastname", TypeName = "varchar(128)")]
    public required string EmplLastname { get; set; }

    [Required]
    [Column("emplJobPosition", TypeName = "varchar(128)")]
    public required string EmplJobPosition { get; set; }

    [Required]
    [Column("emplStatus", TypeName = "varchar(128)")]
    public required string EmplStatus { get; set; }

    public virtual Department Department { get; set; }
    public virtual EmployeeSalary EmployeeSalary { get; set; }
}
