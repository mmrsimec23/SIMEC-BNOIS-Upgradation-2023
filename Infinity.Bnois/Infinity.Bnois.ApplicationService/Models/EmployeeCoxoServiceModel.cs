using System;
using System.ComponentModel.DataAnnotations;
using Infinity.Bnois.Data;

namespace Infinity.Bnois.ApplicationService.Models
{
    public class EmployeeCoxoServiceModel
    {
        public int CoXoServiceId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<int> OfficeId { get; set; }
        public Nullable<int> Type { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> Appointment { get; set; }
        public Nullable<int> ShipType { get; set; }
        public Nullable<System.DateTime> ProposedDate { get; set; }
        public Nullable<int> CompleteStatus { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        public virtual OfficeModel Office { get; set; }

    }
}