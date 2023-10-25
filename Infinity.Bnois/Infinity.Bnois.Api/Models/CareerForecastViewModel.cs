using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.Api.Models
{
  public  class CareerForecastViewModel
    {
        public CareerForecastModel CareerForecast { get; set; }
        public List<SelectModel> CareerForecastSettingList { get; set; }
        //public List<SelectModel> SceeList { get; set; }
    }
}
