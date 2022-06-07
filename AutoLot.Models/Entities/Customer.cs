﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoLot.Models.Entities.Base;
using AutoLot.Models.Entities.Owned;

namespace AutoLot.Models.Entities
{
    [Table("Customers", Schema = "dbo")]
    public partial class Customer : BaseEntity
    {
        [JsonIgnore]
        [InverseProperty(nameof(CreditRisk.CustomerNavigation))]
        public virtual ICollection<CreditRisk> CreditRisks { get; set; } = new List<CreditRisk>();

        [JsonIgnore]
        [InverseProperty(nameof(Order.CustomerNavigation))]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public Person PersonalInformation { get; set; } = new Person();
    }
}
