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
    public class EmpRejoinService : IEmpRejoinService
    {

        private readonly IBnoisRepository<EmpRejoin> empRejoinRepository;
        private readonly IBnoisRepository<Employee> employeeRepository;
        private readonly IBnoisRepository<AgeServicePolicy> ageServicePolicyRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<EmployeeStatus> employeeStatusRepository;
        private readonly IProcessRepository processRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;

        public EmpRejoinService(IBnoisRepository<EmpRejoin> empRejoinRepository,IBnoisRepository<Employee> employeeRepository, 
            IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService, 
            IBnoisRepository<AgeServicePolicy> ageServicePolicyRepository, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository,
            IBnoisRepository<EmployeeStatus> employeeStatusRepository, IProcessRepository processRepository)
        {
            this.empRejoinRepository = empRejoinRepository;
            this.employeeRepository = employeeRepository;
            this.ageServicePolicyRepository = ageServicePolicyRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.employeeStatusRepository = employeeStatusRepository;
            this.processRepository = processRepository; 
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }



        public List<EmpRejoinModel> GetEmpRejoins(int ps, int pn, string qs, out int total)
        {

            IQueryable<EmpRejoin> empRejoins = empRejoinRepository
                .FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "Rank");
            total = empRejoins.Count();
            empRejoins = empRejoins.OrderByDescending(x => x.RejoinDate).Skip((pn - 1) * ps).Take(ps);
            List<EmpRejoinModel> models = ObjectConverter<EmpRejoin, EmpRejoinModel>.ConvertList(empRejoins.ToList()).ToList();
            return models;
        }

        public async Task<EmpRejoinModel> GetEmpRejoin(int id)
        {
            if (id == 0)
            {
                return new EmpRejoinModel();
            }
            EmpRejoin empRejoin = await empRejoinRepository.FindOneAsync(x => x.EmpRejoinId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (empRejoin == null)
            {
                throw new InfinityNotFoundException("Employee Rejoin not found");
            }
            EmpRejoinModel model = ObjectConverter<EmpRejoin, EmpRejoinModel>.Convert(empRejoin);
            return model;
        }

        public async Task<EmpRejoinModel> SaveEmpRejoin(int id, EmpRejoinModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer Rejoin data missing !");
            }
            Employee employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
            if (employee == null)
            {
                throw new InfinityArgumentMissingException("Officer not found!");
            }
            if (employee.EmployeeStatusId==(int)OfficerCurrentStatus.Active)
            {
                throw new InfinityInvalidDataException("Officer already Active!");
            }
            EmployeeStatus employeeStatus = await employeeStatusRepository.FindOneAsync(x => x.EmployeeStatusId == employee.SLCode);
            if (employeeStatus == null)
            {
                throw new InfinityNotFoundException("Officer Current Status data missing !");
            }
            if (employee.EmployeeStatusId==employeeStatus.GCode && employee.SLCode==employeeStatus.SLCode && employee.SLCode==(int)OfficerCurrentStatus.Rejoin)
            {
                throw new InfinityInvalidDataException("Officer already Rejoined !");
            }

            EmpRejoin empRejoin = ObjectConverter<EmpRejoinModel, EmpRejoin>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

            if (id > 0)
            {
                empRejoin = await empRejoinRepository.FindOneAsync(x => x.EmpRejoinId == id);
                if (empRejoin == null)
                {
                    throw new InfinityNotFoundException("Officer Rejoin data not found !");
                }


                empRejoin.ModifiedDate = DateTime.Now;
                empRejoin.ModifiedBy = userId;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmpRejoin";
                bnLog.TableEntryForm = "Officer Rejoin";
                bnLog.PreviousValue = "Id: " + model.EmpRejoinId;
                bnLog.UpdatedValue = "Id: " + model.EmpRejoinId;
                int bnoisUpdateCount = 0;

                if (empRejoin.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    if (empRejoin.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", empRejoin.EmployeeId);
                        bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                    if (model.EmployeeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                        bnLog.UpdatedValue += ", P No: " + ((dynamic)newv).PNo;
                    }
                }
                
                if (empRejoin.RankId != model.RankId)
                {
                    if (empRejoin.RankId > 0)
                    {
                        var prevTransfer = employeeService.GetDynamicTableInfoById("Rank", "RankId", empRejoin.RankId);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prevTransfer).ShortName;
                    }
                    if (model.RankId > 0)
                    {
                        var newTransfer = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)newTransfer).ShortName;
                    }
                }
                if (empRejoin.RejoinDate != model.RejoinDate)
                {
                    bnLog.PreviousValue += ", Rejoin Date: " + empRejoin.RejoinDate.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Rejoin Date: " + model.RejoinDate.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (empRejoin.ReasonToJoin != model.ReasonToJoin)
                {
                    bnLog.PreviousValue += ", Reason To Join: " + empRejoin.ReasonToJoin;
                    bnLog.UpdatedValue += ", Reason To Join: " + model.ReasonToJoin;
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
                empRejoin.CreatedDate = DateTime.Now;
                empRejoin.CreatedBy = userId;
                empRejoin.IsActive = true;
            }
            empRejoin.EmployeeId = model.EmployeeId;
            empRejoin.RankId = model.RankId;
            empRejoin.ReasonToJoin = model.ReasonToJoin;

            empRejoin.Employee = null;

            await empRejoinRepository.SaveAsync(empRejoin);
            model.EmpRejoinId = empRejoin.EmpRejoinId;

            #region---Employee is updating when Rejoin execution is successful---

            employee.EmployeeStatusId = (int)OfficerCurrentStatus.Active;
            employee.SLCode = (int)OfficerCurrentStatus.Rejoin;
            employee.RankId = model.RankId;
            await employeeRepository.SaveAsync(employee);

            EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == employee.EmployeeId);
            if (employeeGeneral == null)
            {
                throw new InfinityNotFoundException("Officer General data not found !");
            }

            if (employeeGeneral.CommissionDate != null)
            {
                GetAgeLimitServiceLimitLprDate(employeeGeneral.CategoryId, employeeGeneral.SubCategoryId, employee.RankId, employeeGeneral.CommissionDate?? new DateTime(), employeeGeneral.DoB, out DateTime ageLimit, out DateTime serviceLimit, out DateTime lprDate);

                employeeGeneral.AgeLimit = ageLimit;
                employeeGeneral.ServiceLimit = serviceLimit;
                employeeGeneral.LprDate = lprDate;
                employeeGeneral.RetireDate = lprDate;
            }
            await employeeGeneralRepository.SaveAsync(employeeGeneral);
            #endregion

            await processRepository.UpdateNamingConvention(empRejoin.EmployeeId);
            return model;
        }

        private void GetAgeLimitServiceLimitLprDate(int category, int? subCategory, int rank, DateTime commissionDate, DateTime dateOfBirth, out DateTime ageLimit, out DateTime serviceLimit, out DateTime LprDate)
        {
            AgeServicePolicy ageServicePolicy = ageServicePolicyRepository.FindOne(x => x.CategoryId == category && x.SubCategoryId == subCategory && x.RankId == rank);

            if (ageServicePolicy == null)
            {
                throw new InfinityNotFoundException("Age Service Policy not found!");
            }
            ageLimit = dateOfBirth.AddYears(ageServicePolicy.AgeLimit);
            serviceLimit = commissionDate.AddYears(ageServicePolicy.ServiceLimit);
            LprDate = ageLimit > serviceLimit ? ageLimit : serviceLimit;

            ageLimit = ageLimit.AddDays(-1);
            serviceLimit = serviceLimit.AddDays(-1);
        }

        public async Task<bool> DeleteEmpRejoin(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmpRejoin empRejoin = await empRejoinRepository.FindOneAsync(x => x.EmpRejoinId == id);
            if (empRejoin == null)
            {
                throw new InfinityNotFoundException("Employee Rejoin not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmpRejoin";
                bnLog.TableEntryForm = "Officer Rejoin";
                bnLog.PreviousValue = "Id: " + empRejoin.EmpRejoinId;
                if (empRejoin.EmployeeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", empRejoin.EmployeeId);
                    bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                }
                if (empRejoin.RankId > 0)
                {
                    var prevTransfer = employeeService.GetDynamicTableInfoById("Rank", "RankId", empRejoin.RankId);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevTransfer).ShortName;
                }
                bnLog.PreviousValue += ", Rejoin Date: " + empRejoin.RejoinDate.ToString("dd/MM/yyyy") + ", Reason To Join: " + empRejoin.ReasonToJoin;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await empRejoinRepository.DeleteAsync(empRejoin);
            }
        }
    }
}