using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;
using System.Collections.Generic;

namespace Infinity.Bnois.Api.Models
{
    public class EmployeeMscEducationViewModel
    {
        public EmployeeMscEducationModel EmployeeMscEducation { get; set; }
        public List<SelectModel> MscEducationTypeList { get; set; }
        public List<SelectModel> MscInstituteList { get; set; }
        public List<SelectModel> MscPermissionTypeList { get; set; }
        public List<SelectModel> CountryList { get; set; }
    }
}