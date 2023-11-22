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
    public class EmployeePftService : IEmployeePftService
    {
        private readonly IBnoisRepository<EmployeePft> employeePftRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeePftService(IBnoisRepository<EmployeePft> employeePftRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeePftRepository = employeePftRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<EmployeePftModel> GetEmployeePfts(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmployeePft> employeePfts = employeePftRepository.FilterWithInclude(x => x.IsActive
				 && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "PftType","PftResult");
            total = employeePfts.Count();
            employeePfts = employeePfts.OrderByDescending(x => x.EmployeePftId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeePftModel> models = ObjectConverter<EmployeePft, EmployeePftModel>.ConvertList(employeePfts.ToList()).ToList();
            return models;
        }

        public async Task<EmployeePftModel> GetEmployeePft(int id)
        {
            if (id <= 0)
            {
                return new EmployeePftModel();
            }
            EmployeePft employeePft = await employeePftRepository.FindOneAsync(x => x.EmployeePftId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (employeePft == null)
            {
                throw new InfinityNotFoundException("Employee PFT not found");
            }
            EmployeePftModel model = ObjectConverter<EmployeePft, EmployeePftModel>.Convert(employeePft);
            return model;
        }



        public async Task<EmployeePftModel> SaveEmployeePft(int id, EmployeePftModel model)
        {
            model.Employee = null;
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee PFT  data missing");
            }
            bool isExistData = employeePftRepository.Exists(x =>x.PftDate==model.PftDate && x.PftTypeId == model.PftTypeId && x.EmployeeId ==model.EmployeeId && x.EmployeePftId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmployeePft employeePft = ObjectConverter<EmployeePftModel, EmployeePft>.Convert(model);
            if (id > 0)
            {
                employeePft = await employeePftRepository.FindOneAsync(x => x.EmployeePftId == id);
                if (employeePft == null)
                {
                    throw new InfinityNotFoundException("Employee PFT not found !");
                }

                employeePft.ModifiedDate = DateTime.Now;
                employeePft.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeePft";
                bnLog.TableEntryForm = "Employee PFT";
                bnLog.PreviousValue = "Id: " + model.EmployeePftId;
                bnLog.UpdatedValue = "Id: " + model.EmployeePftId;
                int bnoisUpdateCount = 0;
                if (employeePft.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeePft.EmployeeId??0);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId??0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (employeePft.PftTypeId != model.PftTypeId)
                {
                    if (employeePft.PftTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("PftType", "PftTypeId", employeePft.PftTypeId ?? 0);
                        bnLog.PreviousValue += ", PFT Type: " + ((dynamic)prev).PftTitle;
                    }
                    if (model.PftTypeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("PftType", "PftTypeId", model.PftTypeId ?? 0);
                        bnLog.UpdatedValue += ", PFT Type: " + ((dynamic)newv).PftTitle;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeePft.PftResultId != model.PftResultId)
                {
                    if (employeePft.PftResultId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("PftResult", "PftResultId", employeePft.PftResultId??0);
                        bnLog.PreviousValue += ", PFT Result: " + ((dynamic)prev).ResultTitle;
                    }
                    if (model.PftResultId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("PftResult", "PftResultId", model.PftResultId??0);
                        bnLog.UpdatedValue += ", PFT Result: " + ((dynamic)newv).ResultTitle;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeePft.PftDate != model.PftDate)
                {
                    bnLog.PreviousValue += ", PFT Date: " + employeePft.PftDate.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", PFT Date: " + model.PftDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
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
                employeePft.IsActive = true;
                employeePft.CreatedDate = DateTime.Now;
                employeePft.CreatedBy = userId;
            }
            employeePft.EmployeeId = model.EmployeeId;
            employeePft.PftTypeId = model.PftTypeId;
            employeePft.PftResultId = model.PftResultId;
            employeePft.PftDate = (DateTime) model.PftDate;
            
           

            await employeePftRepository.SaveAsync(employeePft);
            model.EmployeePftId = employeePft.EmployeePftId;
            return model;
        }


        public async Task<bool> DeleteEmployeePft(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeePft employeePft = await employeePftRepository.FindOneAsync(x => x.EmployeePftId == id);
            if (employeePft == null)
            {
                throw new InfinityNotFoundException("Employee PFT not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeePft";
                bnLog.TableEntryForm = "Employee PFT";
                bnLog.PreviousValue = "Id: " + employeePft.EmployeePftId;
                if (employeePft.EmployeeId > 0)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeePft.EmployeeId??0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                if (employeePft.PftTypeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("PftType", "PftTypeId", employeePft.PftTypeId ?? 0);
                    bnLog.PreviousValue += ", PFT Type: " + ((dynamic)prev).PftTitle;
                }
                if (employeePft.PftResultId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("PftResult", "PftResultId", employeePft.PftResultId ?? 0);
                    bnLog.PreviousValue += ", PFT Result: " + ((dynamic)prev).ResultTitle;
                }
                bnLog.PreviousValue += ", PFT Date: " + employeePft.PftDate.ToString("dd/MM/yyyy");
                
                
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await employeePftRepository.DeleteAsync(employeePft);
            }
        }




    }
}
