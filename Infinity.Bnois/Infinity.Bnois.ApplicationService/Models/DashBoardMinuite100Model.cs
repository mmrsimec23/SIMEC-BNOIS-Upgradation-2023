using Infinity.Bnois.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
	public class DashBoardMinuite100Model
    {
        public int MinuiteId { get; set; }
        public string MinuiteNo { get; set; }
        public string NextMinuiteNo { get; set; }
        public Nullable<int> MinuiteCategory { get; set; }
        public string MinuiteName { get; set; }
        public string LetterNo { get; set; }
        public string OfferedBy { get; set; }
        public Nullable<int> Vacency { get; set; }
        public Nullable<int> Nominate { get; set; }
        public string Purpose { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Location { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string RankName { get; set; }
        public string BranchName { get; set; }
        public string officerRankof { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<System.DateTime> MinuiteDate { get; set; }
        public string PreRequQul { get; set; }
        public string Remarks { get; set; }
        public string ServiceExperiance { get; set; }
        public string Competencies { get; set; }
        public string ExtraRemarks1 { get; set; }
        public string ExtraRemarks2 { get; set; }
        public string ExtraRemarks3 { get; set; }
        public string ExtraRemarks4 { get; set; }
        public string ExtraRemarks5 { get; set; }
        public Nullable<int> ExtraRemarks6 { get; set; }
        public Nullable<int> ExtraRemarks7 { get; set; }
        public Nullable<int> ExtraRemarks8 { get; set; }
        public Nullable<int> ExtraRemarks9 { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual CountryModel Country { get; set; }
        public virtual EmployeeModel Employee { get; set; }
    }
}
