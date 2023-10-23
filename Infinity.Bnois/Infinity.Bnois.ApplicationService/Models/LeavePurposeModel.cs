﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
	public class LeavePurposeModel
	{
		public LeavePurposeModel()
		{
			this.EmployeeLeaves = new HashSet<EmployeeLeaveModel>();
		}

		public int PurposeId { get; set; }
		public string Name { get; set; }
		public string Remarks { get; set; }
		public System.DateTime CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<System.DateTime> ModifiedDate { get; set; }
		public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }

	
		public virtual ICollection<EmployeeLeaveModel> EmployeeLeaves { get; set; }
	}
}
