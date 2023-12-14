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
    public class EmployeeServiceExtensionService : IEmployeeServiceExtensionService
    {
        private readonly IBnoisRepository<EmployeeServiceExt> employeeServiceExtensionRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeServiceExtensionService(IBnoisRepository<EmployeeServiceExt> employeeServiceExtensionRepository, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeServiceExtensionRepository = employeeServiceExtensionRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }


        public List<EmployeeServiceExtensionModel> GetEmployeeServiceExtensions(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmployeeServiceExt> employeeServiceExtensions = employeeServiceExtensionRepository.FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "Employee.Rank", "Employee.Batch");
            total = employeeServiceExtensions.Count();
            employeeServiceExtensions = employeeServiceExtensions.OrderByDescending(x => x.EmpSvrExtId).ThenBy(x=>x.Employee.PNo).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeServiceExtensionModel> models = ObjectConverter<EmployeeServiceExt, EmployeeServiceExtensionModel>.ConvertList(employeeServiceExtensions.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeServiceExtensionModel> GetEmployeeServiceExtension(int id)
        {
            if (id <= 0)
            {
                return new EmployeeServiceExtensionModel()
                {
                    Employee = new EmployeeModel()
                };
            }
            EmployeeServiceExt employeeServiceExtension = await employeeServiceExtensionRepository.FindOneAsync(x => x.EmpSvrExtId == id, new List<string> { "Employee.Rank", "Employee.Batch", "Employee.EmployeeGeneral" });
            if (employeeServiceExtension == null)
            {
                throw new InfinityNotFoundException("Officers Service Extension not found");
            }
            return ObjectConverter<EmployeeServiceExt, EmployeeServiceExtensionModel>.Convert(employeeServiceExtension); ;
        }
        
        public async Task<EmployeeServiceExtensionModel> GetEmployeeServiceExtensionLprDate(int id)
        {
            if (id <= 0)
            {
                return new EmployeeServiceExtensionModel()
                {
                    Employee = new EmployeeModel()
                };
            }
            EmployeeServiceExt employeeServiceExtension =  employeeServiceExtensionRepository.FilterWithInclude(x => x.EmployeeId == id).OrderByDescending(x=> x.EmpSvrExtId).FirstOrDefault();
            if (employeeServiceExtension == null)
            {
                throw new InfinityNotFoundException("Officers Service Extension not found");
            }
            return ObjectConverter<EmployeeServiceExt, EmployeeServiceExtensionModel>.Convert(employeeServiceExtension); ;
        }

        public async Task<EmployeeServiceExtensionModel> SaveEmployeeServiceExtension(int id, EmployeeServiceExtensionModel model)
        {

         
            model.Employee = null;
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Service Extension data missing");
            }


            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmployeeServiceExt employeeServiceExtension = ObjectConverter<EmployeeServiceExtensionModel, EmployeeServiceExt>.Convert(model);
            if (id > 0)
            {
                employeeServiceExtension = await employeeServiceExtensionRepository.FindOneAsync(x => x.EmpSvrExtId == id);
                if (employeeServiceExtension == null)
                {
                    throw new InfinityNotFoundException("Employee Service Extension not found !");
                }

                employeeServiceExtension.ModifiedDate = DateTime.Now;
                employeeServiceExtension.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeServiceExt";
                bnLog.TableEntryForm = "Officer Service Extention";
                bnLog.PreviousValue = "Id: " + model.EmpSvrExtId;
                bnLog.UpdatedValue = "Id: " + model.EmpSvrExtId;
                int bnoisUpdateCount = 0;

                if (employeeServiceExtension.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    if(employeeServiceExtension.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeServiceExtension.EmployeeId ?? 0);
                        bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                    if(model.EmployeeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId ?? 0);
                        bnLog.UpdatedValue += ", P No: " + ((dynamic)newv).PNo;
                    }
                }
                if (employeeServiceExtension.RetirementDate != model.RetirementDate)
                {
                    bnLog.PreviousValue += ", Retirement Date: " + employeeServiceExtension.RetirementDate.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Retirement Date: " + model.RetirementDate.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeServiceExtension.ExtMonth != model.ExtMonth)
                {
                    bnLog.PreviousValue += ", Extension For Months: " + employeeServiceExtension.ExtMonth;
                    bnLog.UpdatedValue += ", Extension For Months: " + model.ExtMonth;
                    bnoisUpdateCount += 1;
                }
                if (employeeServiceExtension.ExtDays != model.ExtDays)
                {
                    bnLog.PreviousValue += ", Extension For Days: " + employeeServiceExtension.ExtDays;
                    bnLog.UpdatedValue += ", Extension For Days: " + model.ExtDays;
                    bnoisUpdateCount += 1;
                }
                if (employeeServiceExtension.ExtLprDate != model.ExtLprDate)
                {
                    bnLog.PreviousValue += ", Ext Lpr Date: " + employeeServiceExtension.ExtLprDate.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Ext Lpr Date: " + model.ExtLprDate.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeServiceExtension.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeServiceExtension.Remarks;
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
                employeeServiceExtension.IsActive = true;
                employeeServiceExtension.CreatedDate = DateTime.Now;
                employeeServiceExtension.CreatedBy = userId;


            }

            employeeServiceExtension.EmployeeId = model.EmployeeId;
            employeeServiceExtension.RetirementDate = model.RetirementDate;
            employeeServiceExtension.ExtDays = model.ExtDays;
            employeeServiceExtension.ExtMonth = model.ExtMonth;
            employeeServiceExtension.ExtLprDate = model.RetirementDate.AddDays(model.ExtDays).AddMonths(model.ExtMonth);
            employeeServiceExtension.Remarks = model.Remarks;

            // Employee Contract End Date Update
            EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
            if (employeeGeneral == null)
            {
                throw new InfinityNotFoundException("Officer General Information data missing !");
            }
            if (employeeGeneral.IsContract)
            {
                employeeGeneral.ContractEndDate = employeeServiceExtension.ExtLprDate;
            }
            else
            {
                employeeGeneral.LprDate = employeeServiceExtension.ExtLprDate;
            }


            await employeeServiceExtensionRepository.SaveAsync(employeeServiceExtension);
            await employeeGeneralRepository.SaveAsync(employeeGeneral);
            model.EmpSvrExtId = employeeServiceExtension.EmpSvrExtId;
            return model;
        }

        public async Task<bool> DeleteEmployeeServiceExtension(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeServiceExt employeeServiceExtension = await employeeServiceExtensionRepository.FindOneAsync(x => x.EmpSvrExtId == id);
            if (employeeServiceExtension == null)
            {
                throw new InfinityNotFoundException("Employee Service Extension not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeServiceExt";
                bnLog.TableEntryForm = "Officer Service Extention";
                bnLog.PreviousValue = "Id: " + employeeServiceExtension.EmpSvrExtId;

                if (employeeServiceExtension.EmployeeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeServiceExtension.EmployeeId ?? 0);
                    bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                }
                bnLog.PreviousValue += ", Retirement Date: " + employeeServiceExtension.RetirementDate.ToString("dd/MM/yyyy") + ", Extension For Months: " + employeeServiceExtension.ExtMonth + ", Extension For Days: " + employeeServiceExtension.ExtDays + ", Ext Lpr Date: " + employeeServiceExtension.ExtLprDate.ToString("dd/MM/yyyy") + ", Remarks: " + employeeServiceExtension.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await employeeServiceExtensionRepository.DeleteAsync(employeeServiceExtension);
            }
        }
    }
}