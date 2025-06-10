using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_MVP;

public partial class Department
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("deptId")]
    public Guid Id { get; set; }

    [Required]
    [Column("deptName", TypeName = "varchar(255)")]
    public required string DeptName { get; set; }

    [Column("deptDesc", TypeName = "varchar(255)")]
    public string? DeptDescription { get; set; }

}
