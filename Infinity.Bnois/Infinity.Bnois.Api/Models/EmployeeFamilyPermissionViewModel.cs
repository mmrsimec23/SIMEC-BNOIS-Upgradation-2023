using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;
using System.Collections.Generic;

namespace Infinity.Bnois.Api.Models
{
    public class EmployeeFamilyPermissionViewModel
    {
        public EmployeeFamilyPermissionModel EmployeeFamilyPermission { get; set; }
        public List<SelectModel> FamilyPermissionRelationTypes { get; set; }
        public List<SelectModel> FamilyPermissionCountryList { get; set; }
    }
}