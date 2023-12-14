using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class EmpRunMissingService : IEmpRunMissingService
    {

        private readonly IBnoisRepository<EmpRunMissing> empRunMissingRepository;
        private readonly IBnoisRepository<Employee> employeeRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<EmployeeStatus> employeeStatusRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;

        public EmpRunMissingService(IBnoisRepository<EmpRunMissing> empRunMissingRepository,
            IBnoisRepository<Employee> employeeRepository,IBnoisRepository<EmployeeGeneral> employeeGeneralRepository,
            IBnoisRepository<EmployeeStatus> employeeStatusRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.empRunMissingRepository = empRunMissingRepository;
            this.employeeRepository = employeeRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.employeeStatusRepository = employeeStatusRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }


        public List<EmpRunMissingModel> GetEmpRunMissings(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmpRunMissing> empRunMissings = empRunMissingRepository.FilterWithInclude(x => x.IsActive && x.Type != (int)OfficerCurrentStatus.Back_TO_Unit && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee");

            total = empRunMissings.Count();
            empRunMissings = empRunMissings.OrderByDescending(x => x.EmpRunMissingId).Skip((pn - 1) * ps).Take(ps);
            List<EmpRunMissingModel> models = ObjectConverter<EmpRunMissing, EmpRunMissingModel>.ConvertList(empRunMissings.ToList()).ToList();
            models = models.Select(x =>
            {
                x.TypeName = Enum.GetName(typeof(OfficerCurrentStatus), x.Type);
                return x;
            }).ToList();

            return models;

        }

        public async Task<EmpRunMissingModel> GetEmpRunMissing(int id)
        {
            if (id <= 0)
            {
                return new EmpRunMissingModel();
            }
            EmpRunMissing empRunMissing = await empRunMissingRepository.FindOneAsync(x => x.EmpRunMissingId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (empRunMissing == null)
            {
                throw new InfinityNotFoundException(" Officer Run Or Missing not found");
            }
            EmpRunMissingModel model = ObjectConverter<EmpRunMissing, EmpRunMissingModel>.Convert(empRunMissing);
            return model;
        }

        public async Task<EmpRunMissingModel> SaveEmpRunMissing(int id, EmpRunMissingModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer's Run Or Missing data not found");
            }


            bool isExist = await empRunMissingRepository.ExistsAsync(x => x.EmployeeId == model.EmployeeId && x.Date == model.Date && x.EmpRunMissingId != model.EmpRunMissingId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Same date entry not allowed. !");
            }

            EmployeeStatus employeeStatus = await employeeStatusRepository.FindOneAsync(x => x.SLCode == model.Type);

            if (employeeStatus == null)
            {
                throw new InfinityNotFoundException("Officer's Status Not Found!");
            }

            Employee employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
            if (employee == null)
            {
                throw new InfinityInvalidDataException("Officer not exist!");
            }



            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmpRunMissing empRunMissing = ObjectConverter<EmpRunMissingModel, EmpRunMissing>.Convert(model);
            if (id > 0)
            {
                empRunMissing = await empRunMissingRepository.FindOneAsync(x => x.EmpRunMissingId == id);
                if (empRunMissing == null)
                {
                    throw new InfinityNotFoundException("Officer's Run Or Missing data not found !");
                }

                empRunMissing.ModifiedDate = DateTime.Now;
                empRunMissing.ModifiedBy = userId;



                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmpRunMissing";
                bnLog.TableEntryForm = "Officer's Run/Missing";
                bnLog.PreviousValue = "Id: " + model.EmpRunMissingId;
                bnLog.UpdatedValue = "Id: " + model.EmpRunMissingId;
                int bnoisUpdateCount = 0;

                if (empRunMissing.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    if (empRunMissing.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", empRunMissing.EmployeeId);
                        bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                    if (model.EmployeeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                        bnLog.UpdatedValue += ", P No: " + ((dynamic)newv).PNo;
                    }
                }
                if (empRunMissing.IsBackLog != model.IsBackLog)
                {
                    bnLog.PreviousValue += ", Back Log: " + empRunMissing.IsBackLog;
                    bnLog.UpdatedValue += ", Back Log: " + model.IsBackLog;
                    bnoisUpdateCount += 1;
                }
                if (empRunMissing.TransferId != model.TransferId)
                {
                    if (empRunMissing.TransferId > 0)
                    {
                        var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", empRunMissing.TransferId ?? 0);
                        bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                    }
                    if (model.TransferId > 0)
                    {
                        var newTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", model.TransferId ?? 0);
                        bnLog.UpdatedValue += ", Born/Attach/Appointment: " + ((dynamic)newTransfer).BornOffice + '/' + ((dynamic)newTransfer).CurrentAttach + '/' + ((dynamic)newTransfer).Appointment;
                    }
                }
                if (empRunMissing.RankId != model.RankId)
                {
                    if (empRunMissing.RankId > 0)
                    {
                        var prevTransfer = employeeService.GetDynamicTableInfoById("Rank", "RankId", empRunMissing.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prevTransfer).ShortName;
                    }
                    if (model.TransferId > 0)
                    {
                        var newTransfer = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)newTransfer).ShortName;
                    }
                }
                if (empRunMissing.BranchId != model.BranchId)
                {
                    if (empRunMissing.BranchId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Branch", "BranchId", empRunMissing.BranchId ?? 0);
                        bnLog.PreviousValue += ", Branch: " + ((dynamic)prev).Name;
                    }
                    if (model.BranchId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Branch", "BranchId", model.BranchId ?? 0);
                        bnLog.UpdatedValue += ", Branch: " + ((dynamic)newv).Name;
                    }
                }
                if (empRunMissing.Type != model.Type)
                {
                    bnLog.PreviousValue += ", Status: " + (empRunMissing.Type == 3 ? "Run" : empRunMissing.Type == 4 ? "Missing" : empRunMissing.Type == 9 ? "Return from Run/Missing" : "");
                    bnLog.UpdatedValue += ", Status: " + (model.Type == 3 ? "Run" : model.Type == 4 ? "Missing" : model.Type == 9 ? "Return from Run/Missing" : "");
                    bnoisUpdateCount += 1;
                }
                if (empRunMissing.Date != model.Date)
                {
                    bnLog.PreviousValue += ", Date: " + empRunMissing.Date.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date: " + model.Date.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (empRunMissing.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + empRunMissing.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString(); ;
                bnLog.LogCreatedDate = DateTime.Now;

                if (bnoisUpdateCount > 0)
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
                if (employee.EmployeeStatusId == employeeStatus.GCode && employee.SLCode == employeeStatus.SLCode)
                {
                    throw new InfinityInvalidDataException("Officer's Status Aleady updated !");
                }
                empRunMissing.IsActive = true;
                empRunMissing.CreatedDate = DateTime.Now;
                empRunMissing.CreatedBy = userId;
            }
            empRunMissing.EmployeeId = model.EmployeeId;
            empRunMissing.Type = model.Type;
            empRunMissing.Remarks = model.Remarks;
            empRunMissing.Date = model.Date;
            empRunMissing.Employee = null;

            await empRunMissingRepository.SaveAsync(empRunMissing);
            model.EmpRunMissingId = empRunMissing.EmpRunMissingId;
            if (model.IsBackLog)
            {
                return model;
            }

            #region -----Employee is updating when Run/Missing execution is successful----
            employee.EmployeeStatusId = employeeStatus.GCode;
            employee.SLCode = employeeStatus.SLCode;
            await employeeRepository.SaveAsync(employee);
            #endregion

            return model;
        }

        public async Task<bool> DeleteEmpRunMissing(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmpRunMissing empRunMissing = await empRunMissingRepository.FindOneAsync(x => x.EmpRunMissingId == id);
            if (empRunMissing == null)
            {
                throw new InfinityNotFoundException("Officer's Run Or Missing data not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmpRunMissing";
                bnLog.TableEntryForm = "Officer's Run/Missing";
                bnLog.PreviousValue = "Id: " + empRunMissing.EmpRunMissingId;
                if (empRunMissing.EmployeeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", empRunMissing.EmployeeId);
                    bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                }
                bnLog.PreviousValue += ", Back Log: " + empRunMissing.IsBackLog;
                if (empRunMissing.TransferId > 0)
                {
                    var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", empRunMissing.TransferId ?? 0);
                    bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                }
                if (empRunMissing.RankId > 0)
                {
                    var prevTransfer = employeeService.GetDynamicTableInfoById("Rank", "RankId", empRunMissing.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevTransfer).ShortName;
                }
                if (empRunMissing.BranchId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Branch", "BranchId", empRunMissing.BranchId ?? 0);
                    bnLog.PreviousValue += ", Branch: " + ((dynamic)prev).Name;
                }
                bnLog.PreviousValue += ", Status: " + (empRunMissing.Type == 3 ? "Run" : empRunMissing.Type == 4 ? "Missing" : empRunMissing.Type == 9 ? "Return from Run/Missing" : "") + ", Date: " + empRunMissing.Date.ToString("dd/MM/yyyy") + ", Remarks: " + empRunMissing.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await empRunMissingRepository.DeleteAsync(empRunMissing);
            }
        }

        public List<SelectModel> GetStatusTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(OfficerCurrentStatus)).Cast<OfficerCurrentStatus>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }
        //----Employee Back to Unit-------
        public List<EmpRunMissingModel> GetBackToUnits(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmpRunMissing> empRunMissings = empRunMissingRepository.FilterWithInclude(x => x.IsActive && x.Type == (int)OfficerCurrentStatus.Back_TO_Unit && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee");

            total = empRunMissings.Count();
            empRunMissings = empRunMissings.OrderByDescending(x => x.EmpRunMissingId).Skip((pn - 1) * ps).Take(ps);
            List<EmpRunMissingModel> models = ObjectConverter<EmpRunMissing, EmpRunMissingModel>.ConvertList(empRunMissings.ToList()).ToList();
            models = models.Select(x =>
            {
                x.TypeName = Enum.GetName(typeof(OfficerCurrentStatus), x.Type);
                return x;
            }).ToList();

            return models;
        }

        public async Task<EmpRunMissingModel> SaveEmpBackToUnit(int id, EmpRunMissingModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer Back to Unit data missing !");
            }
            Employee employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == model.Employee.EmployeeId);
            if (employee == null)
            {
                throw new InfinityArgumentMissingException("Officer not found!");
            }

//            EmployeeStatus employeeStatus = await employeeStatusRepository.FindOneAsync(x => x.EmployeeStatusId == employee.SLCode);
//            if (employeeStatus == null)
//            {
//                throw new InfinityNotFoundException("Officer Back to Unit data missing !");
//            }
            //bool isExist = empRunMissingRepository.Exists(x => x.EmployeeId == model.Employee.EmployeeId && x.EmpRunMissingId != model.EmpRunMissingId);

            //if (isExist)
            //{
            //    throw new InfinityInvalidDataException("Officer already Back to Unit !");
            //}
            EmployeeGeneral isDeputatedOfficer =  employeeGeneralRepository.FindOne(x=>x.EmployeeId== model.Employee.EmployeeId);
            if (isDeputatedOfficer.CategoryId!=6)
            {
                throw new InfinityInvalidDataException("Enter A Valid Deputed Officer!");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmpRunMissing empRunMissing = ObjectConverter<EmpRunMissingModel, EmpRunMissing>.Convert(model);
            if (id > 0)
            {
                empRunMissing = await empRunMissingRepository.FindOneAsync(x => x.EmpRunMissingId == id);
                if (empRunMissing == null)
                {
                    throw new InfinityNotFoundException("Officer's Back to Unit data not found !");
                }

                empRunMissing.ModifiedDate = DateTime.Now;
                empRunMissing.ModifiedBy = userId;
            }
            else
            {
                empRunMissing.IsActive = true;
                empRunMissing.CreatedDate = DateTime.Now;
                empRunMissing.CreatedBy = userId;
            }
            empRunMissing.EmployeeId = model.Employee.EmployeeId;
            empRunMissing.Type = (int)OfficerCurrentStatus.Back_TO_Unit;
            empRunMissing.Remarks = model.Remarks;
            empRunMissing.Date = model.Date;
            empRunMissing.Employee = null;

            await empRunMissingRepository.SaveAsync(empRunMissing);
            model.EmpRunMissingId = empRunMissing.EmpRunMissingId;

            #region -----Employee is updating when Run/Missing execution is successful----
            employee.EmployeeStatusId = (int)OfficerCurrentStatus.Back_TO_Unit;
            employee.SLCode = (int)OfficerCurrentStatus.Back_TO_Unit;
            await employeeRepository.SaveAsync(employee);
            #endregion

            return model;
        }
    }
}