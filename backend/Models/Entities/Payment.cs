// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.ComponentModel.DataAnnotations;
// using Hotel_MVP.Models.Entities;
// using System.Text.Json.Serialization;

// namespace Hotel_MVP;

// public partial class Payment
// {
//     [Key]
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     [Column("paymentId")]
//     public Guid Id { get; set; }

//     [ForeignKey("User")]
//     [Column("userId")]
//     public Guid UserId { get; set; }
//     [JsonIgnore]
//     public User? User { get; set; } //

//     [Column("timeWorked", TypeName = "decimal(5,2)")]
//     public decimal TimeWorked { get; set; }

//     [Column("paymentAmount", TypeName = "decimal(9,2)")]
//     public decimal PaymentAmount { get; set; }

//     [Column("paymentMethod", TypeName = "varchar(128)")]
//     public string PaymentMethod { get; set; } = null!;
// }
