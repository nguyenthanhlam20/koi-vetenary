﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace KoiVetenary.Data.Models
{
    public partial class AnimalType
    {
        public AnimalType()
        {
            Animals = new HashSet<Animal>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public virtual ICollection<Animal> Animals { get; set; }
    }
}