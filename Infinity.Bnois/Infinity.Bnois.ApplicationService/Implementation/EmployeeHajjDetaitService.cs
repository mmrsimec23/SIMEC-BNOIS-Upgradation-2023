using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class EmployeeHajjDetaitService : IEmployeeHajjDetaitService
    {
        private readonly IBnoisRepository<EmployeeHajjDetail> _employeeHajjDetailRepository;
        private readonly IBnoisRepository<Employee> _employeeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;

        public EmployeeHajjDetaitService(IBnoisRepository<EmployeeHajjDetail> employeeHajjDetailRepository, IBnoisRepository<Employee> employeeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            _employeeHajjDetailRepository = employeeHajjDetailRepository;
            _employeeRepository = employeeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<bool> DeleteEmployeeHajjDetail(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeHajjDetail employeeHajjDetail = await _employeeHajjDetailRepository.FindOneAsync(x => x.EmployeeHajjDetailId == id);
            if (employeeHajjDetail == null)
            {
                throw new InfinityNotFoundException("Employee Hajj Detail not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeHajjDetail";
                bnLog.TableEntryForm = "Hajj Information";
                bnLog.PreviousValue = "Id: " + employeeHajjDetail.EmployeeHajjDetailId;
                if (employeeHajjDetail.EmployeeId > 0)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeHajjDetail.EmployeeId ?? 0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                
                bnLog.PreviousValue += ", Baloty : " + employeeHajjDetail.BalotyNonBaloty + ", Royel Guest: " + employeeHajjDetail.RoyelGuest + ", Hajj Or Omra: " + (employeeHajjDetail.HajjOrOmra==true?"Hajj":"Umrah") + ", Arranged By: " + employeeHajjDetail.ArrangedBy
                    + ", Accompanied By: " + employeeHajjDetail.ACompanyBy + ", From Date: " + employeeHajjDetail.FromDate.ToString("dd/MM/yyyy") + ", To Date: " + employeeHajjDetail.ToDate.ToString("dd/MM/yyyy");
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await _employeeHajjDetailRepository.DeleteAsync(employeeHajjDetail);
            }
        }

        public async Task<EmployeeHajjDetailModel> getEmployeeHajjDetail(int id)
        {
            if (id <= 0)
            {
                return new EmployeeHajjDetailModel();
            }
            EmployeeHajjDetail employeeHajjDetail = await _employeeHajjDetailRepository.FindOneAsync(x => x.EmployeeHajjDetailId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (employeeHajjDetail == null)
            {
                throw new InfinityNotFoundException("Employee Hajj Detail not found");
            }
            EmployeeHajjDetailModel model = ObjectConverter<EmployeeHajjDetail, EmployeeHajjDetailModel>.Convert(employeeHajjDetail);
            return model;
        }

        public List<EmployeeHajjDetailModel> GetEmployeeHajjDetails(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmployeeHajjDetail> employeeHajjDetails = _employeeHajjDetailRepository.FilterWithInclude(x => x.Active && (x.Employee.PNo.Contains(qs) || string.IsNullOrEmpty(qs)), "Employee");
            total = _employeeHajjDetailRepository.Count();
            employeeHajjDetails = employeeHajjDetails.OrderByDescending(x => x.EmployeeHajjDetailId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeHajjDetailModel> models = ObjectConverter<EmployeeHajjDetail, EmployeeHajjDetailModel>.ConvertList(employeeHajjDetails.ToList()).ToList();
            return models;
        }

        public List<EmployeeHajjDetailModel> GetEmployeeHajjDetailsByPno(string PNo)
        {
            int employeeId = _employeeRepository.Where(x => x.PNo == PNo).Select(x => x.EmployeeId).SingleOrDefault();
            List<EmployeeHajjDetail> employeeHajjDetails = _employeeHajjDetailRepository.Where(x => x.EmployeeId == employeeId && x.HajjOrOmra==true).ToList();
            List<EmployeeHajjDetailModel> models = ObjectConverter<EmployeeHajjDetail, EmployeeHajjDetailModel>.ConvertList(employeeHajjDetails.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeHajjDetailModel> SaveEmployeeHajjDetail(int id, EmployeeHajjDetailModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Hajj Detail  data missing");
            }
            //bool isExistData = _employeeHajjDetailRepository.Exists(x => x.EmployeeHajjDetailId == model.EmployeeId  && x.EmployeeHajjDetailId != id);
            //if (isExistData)
            //{
            //    throw new InfinityInvalidDataException("Data already exists !");
            //}
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmployeeHajjDetail employeeHajjDetail = ObjectConverter<EmployeeHajjDetailModel, EmployeeHajjDetail>.Convert(model);
            if (id > 0)
            {
                employeeHajjDetail = _employeeHajjDetailRepository.FindOne(x => x.EmployeeHajjDetailId == id);
                if (employeeHajjDetail == null)
                {
                    throw new InfinityNotFoundException("Employee Hajj  Detail not found !");
                }

                employeeHajjDetail.Modified = DateTime.Now;
                employeeHajjDetail.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeHajjDetail";
                bnLog.TableEntryForm = "Hajj Information";
                bnLog.PreviousValue = "Id: " + model.EmployeeHajjDetailId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeHajjDetailId;
                if (employeeHajjDetail.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeHajjDetail.EmployeeId ?? 0);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId ?? 0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                if (employeeHajjDetail.BalotyNonBaloty != model.BalotyNonBaloty)
                {
                    bnLog.PreviousValue += ", Baloty: " + employeeHajjDetail.BalotyNonBaloty;
                    bnLog.UpdatedValue += ", Baloty: " + model.BalotyNonBaloty;
                }
                if (employeeHajjDetail.RoyelGuest != model.RoyelGuest)
                {
                    bnLog.PreviousValue += ", Royel Guest: " + employeeHajjDetail.RoyelGuest;
                    bnLog.UpdatedValue += ", Royel Guest: " + model.RoyelGuest;
                }
                if (employeeHajjDetail.HajjOrOmra != model.HajjOrOmra)
                {
                    bnLog.PreviousValue += ", Hajj Or Omra: " + (employeeHajjDetail.HajjOrOmra == true ? "Hajj" : "Umrah");
                    bnLog.UpdatedValue += ", Hajj Or Omra: " + (model.HajjOrOmra == true ? "Hajj" : "Umrah");
                }
                if (employeeHajjDetail.ArrangedBy != model.ArrangedBy)
                {
                    bnLog.PreviousValue += ", Arranged By: " + employeeHajjDetail.ArrangedBy;
                    bnLog.UpdatedValue += ", Arranged By: " + model.ArrangedBy;
                }
                if (employeeHajjDetail.ACompanyBy != model.ACompanyBy)
                {
                    bnLog.PreviousValue += ", Accompanied By: " + employeeHajjDetail.ACompanyBy;
                    bnLog.UpdatedValue += ", Accompanied By: " + model.ACompanyBy;
                }
                if (employeeHajjDetail.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + employeeHajjDetail.FromDate.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.FromDate.ToString("dd/MM/yyyy");
                }
                if (employeeHajjDetail.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", To Date: " + employeeHajjDetail.ToDate.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", To Date: " + model.ToDate.ToString("dd/MM/yyyy");
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (employeeHajjDetail.EmployeeId != model.EmployeeId || employeeHajjDetail.BalotyNonBaloty != model.BalotyNonBaloty || employeeHajjDetail.RoyelGuest != model.RoyelGuest
                    || employeeHajjDetail.HajjOrOmra != model.HajjOrOmra || employeeHajjDetail.ArrangedBy != model.ArrangedBy || employeeHajjDetail.ACompanyBy != model.ACompanyBy
                    || employeeHajjDetail.FromDate != model.FromDate || employeeHajjDetail.ToDate != model.ToDate)
                {
                    await bnoisLogRepository.SaveAsync(bnLog);

                }
                else
                {
                    throw new InfinityNotFoundException("Please Update Any Field!");
                }
                //data log section end
            }
            else
            {
                employeeHajjDetail.Active = true;
                employeeHajjDetail.Created = DateTime.Now;
                employeeHajjDetail.CreatedBy = userId;
            }
            employeeHajjDetail.EmployeeId = model.EmployeeId;
            employeeHajjDetail.BalotyNonBaloty = model.BalotyNonBaloty;
            employeeHajjDetail.RoyelGuest = model.RoyelGuest;
            employeeHajjDetail.HajjOrOmra = model.HajjOrOmra;
            employeeHajjDetail.ArrangedBy = model.ArrangedBy;
            employeeHajjDetail.ACompanyBy = model.ACompanyBy;
            employeeHajjDetail.FromDate = model.FromDate;
            employeeHajjDetail.ToDate = model.ToDate;
            await _employeeHajjDetailRepository.SaveAsync(employeeHajjDetail);
            return model;
        }
    }
}
