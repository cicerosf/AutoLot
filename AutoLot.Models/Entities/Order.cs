using System;
using System.ComponentModel.DataAnnotations.Schema;
using AutoLot.Models.Entities.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace AutoLot.Models.Entities
{
    [Table("Orders", Schema ="dbo")]
    [Index(nameof(CarId), Name ="IX_Orders_CarId")]
    [Index(nameof(CustomerId), nameof(CarId), IsUnique =true, Name ="IX_Orders_CustomerId_CarId")]
    public partial class Order : BaseEntity
    {
        public int CustomerId { get; set; }
        public int CarId { get; set; }

        [ForeignKey(nameof(CarId))]
        [InverseProperty(nameof(Car.Orders))]
        public virtual Car CarNavigation { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(Customer.Orders))]
        public virtual Customer CustomerNavigation { get; set; }
    }
}
