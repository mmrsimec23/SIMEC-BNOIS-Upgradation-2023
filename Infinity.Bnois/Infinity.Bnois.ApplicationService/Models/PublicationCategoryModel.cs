﻿using System;

namespace Infinity.Bnois.ApplicationService.Models
{
    public class PublicationCategoryModel
    {
        public int PublicationCategoryId { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
        public bool GoToTrace { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}