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
    public class AgeServicePolicyService :IAgeServicePolicyService
    {
        private readonly IBnoisRepository<AgeServicePolicy> ageServicePolicyRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public AgeServicePolicyService(IBnoisRepository<AgeServicePolicy> ageServicePolicyRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.ageServicePolicyRepository = ageServicePolicyRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<AgeServicePolicyModel> GetAgeServicePolicies(int ps, int pn, string qs, out int total)
        {
            IQueryable<AgeServicePolicy> ageServicePolicies = ageServicePolicyRepository.FilterWithInclude(x => x.IsActive
                && ((x.Category.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.SubCategory.Name.Contains(qs) || String.IsNullOrEmpty(qs))) || (x.Rank.FullName.Contains(qs) || String.IsNullOrEmpty(qs)), "Category","SubCategory", "Rank");
            total = ageServicePolicies.Count();
            ageServicePolicies = ageServicePolicies.OrderByDescending(x => x.AgeServiceId).Skip((pn - 1) * ps).Take(ps);
            List<AgeServicePolicyModel> models = ObjectConverter<AgeServicePolicy, AgeServicePolicyModel>.ConvertList(ageServicePolicies.ToList()).ToList();
           
            models = models.Select(x =>
            {
                x.EarlyStatusName = Enum.GetName(typeof(EarlyStatus), x.EarlyStatus);
                return x;
            }).ToList();
            return models;
        }

        public async Task<AgeServicePolicyModel> GetAgeServicePolicy(int id)
        {
            if (id <= 0)
            {
                return new AgeServicePolicyModel();
            }
            AgeServicePolicy ageServicePolicy = await ageServicePolicyRepository.FindOneAsync(x => x.AgeServiceId == id);
            if (ageServicePolicy == null)
            {
                throw new InfinityNotFoundException("Age Service Policy not found");
            }
            AgeServicePolicyModel model = ObjectConverter<AgeServicePolicy, AgeServicePolicyModel>.Convert(ageServicePolicy);
            return model;
        }


        public async Task<AgeServicePolicyModel> SaveAgeServicePolicy(int id, AgeServicePolicyModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Age Service Policy data missing");
            }


            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            AgeServicePolicy ageServicePolicy = ObjectConverter<AgeServicePolicyModel, AgeServicePolicy>.Convert(model);
            if (id > 0)
            {
                ageServicePolicy = await ageServicePolicyRepository.FindOneAsync(x => x.AgeServiceId == id);
                if (ageServicePolicy == null)
                {
                    throw new InfinityNotFoundException("Age Service Policy not found !");
                }
                ageServicePolicy.ModifiedDate = DateTime.Now;
                ageServicePolicy.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "AgeServicePolicy";
                bnLog.TableEntryForm = "Service Age Policy";
                bnLog.PreviousValue = "Id: " + model.AgeServiceId;
                bnLog.UpdatedValue = "Id: " + model.AgeServiceId;
                int bnoisUpdateCount = 0;

                if (ageServicePolicy.CategoryId != model.CategoryId)
                {
                    if (ageServicePolicy.CategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Category", "CategoryId", ageServicePolicy.CategoryId);
                        bnLog.PreviousValue += ", Category: " + ((dynamic)prev).Name;
                    }
                    if (model.CategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Category", "CategoryId", model.CategoryId);
                        bnLog.UpdatedValue += ", Category: " + ((dynamic)newv).Name;
                    }
                }
                if (ageServicePolicy.SubCategoryId != model.SubCategoryId)
                {
                    if (ageServicePolicy.SubCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("SubCategory", "SubCategoryId", ageServicePolicy.SubCategoryId);
                        bnLog.PreviousValue += ", Sub Category: " + ((dynamic)prev).Name;
                    }
                    if (model.SubCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("SubCategory", "SubCategoryId", model.SubCategoryId);
                        bnLog.UpdatedValue += ", Sub Category: " + ((dynamic)newv).Name;
                    }
                }
                if (ageServicePolicy.RankId != model.RankId)
                {
                    if (ageServicePolicy.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", ageServicePolicy.RankId);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).FullName;
                    }
                    if (model.RankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)newv).FullName;
                    }
                }
                if (ageServicePolicy.ServiceLimit != model.ServiceLimit)
                {
                    bnLog.PreviousValue += ", Service Limit (In years): " + ageServicePolicy.ServiceLimit;
                    bnLog.UpdatedValue += ", Service Limit (In years): " + model.ServiceLimit;
                    bnoisUpdateCount += 1;
                }
                if (ageServicePolicy.AgeLimit != model.AgeLimit)
                {
                    bnLog.PreviousValue += ", Age Limit (In years): " + ageServicePolicy.AgeLimit;
                    bnLog.UpdatedValue += ", Age Limit (In years): " + model.AgeLimit;
                    bnoisUpdateCount += 1;
                }
                if (ageServicePolicy.EarlyStatus != model.EarlyStatus)
                {
                    bnLog.PreviousValue += ", Status: " + (ageServicePolicy.EarlyStatus == 1 ? "Early" : ageServicePolicy.EarlyStatus == 2 ? "Later" : "");
                    bnLog.UpdatedValue += ", Status: " + (model.EarlyStatus == 1 ? "Early" : model.EarlyStatus == 2 ? "Later" : "");
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
                ageServicePolicy.IsActive = true;
                ageServicePolicy.CreatedDate = DateTime.Now;
                ageServicePolicy.CreatedBy = userId;
            }
            ageServicePolicy.CategoryId = model.CategoryId;
            ageServicePolicy.SubCategoryId = model.SubCategoryId;
            ageServicePolicy.RankId = model.RankId;
            ageServicePolicy.ServiceLimit = model.ServiceLimit;
            ageServicePolicy.AgeLimit = model.AgeLimit;
            ageServicePolicy.EarlyStatus = model.EarlyStatus;

            await ageServicePolicyRepository.SaveAsync(ageServicePolicy);
            model.AgeServiceId = ageServicePolicy.AgeServiceId;
            return model;
        }


        public async Task<bool> DeleteAgeServicePolicy(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            AgeServicePolicy ageServicePolicy = await ageServicePolicyRepository.FindOneAsync(x => x.AgeServiceId == id);
            if (ageServicePolicy == null)
            {
                throw new InfinityNotFoundException("Age Service Policy not found");
            }
            else
            {

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "AgeServicePolicy";
                bnLog.TableEntryForm = "Service Age Policy";
                bnLog.PreviousValue = "Id: " + ageServicePolicy.AgeServiceId;

                if (ageServicePolicy.CategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Category", "CategoryId", ageServicePolicy.CategoryId);
                    bnLog.PreviousValue += ", Category: " + ((dynamic)prev).Name;
                }
                if (ageServicePolicy.SubCategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("SubCategory", "SubCategoryId", ageServicePolicy.SubCategoryId);
                    bnLog.PreviousValue += ", Sub Category: " + ((dynamic)prev).Name;
                }
                if (ageServicePolicy.RankId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", ageServicePolicy.RankId);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).FullName;
                }
                bnLog.PreviousValue += ", Service Limit (In years): " + ageServicePolicy.ServiceLimit + ", Age Limit (In years): " + ageServicePolicy.AgeLimit + ", Status: " + (ageServicePolicy.EarlyStatus == 1 ? "Early" : ageServicePolicy.EarlyStatus == 2 ? "Later" : "");
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await ageServicePolicyRepository.DeleteAsync(ageServicePolicy);
            }
        }



        public List<SelectModel> GetEarlyStatusSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(EarlyStatus)).Cast<EarlyStatus>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

	    public async Task<DateTime?> GetLprServiceDate(EmployeeGeneralModel employee)
	    {
		    AgeServicePolicy policy = await ageServicePolicyRepository.FindOneAsync(x =>
			    x.CategoryId == employee.CategoryId && x.SubCategoryId == employee.SubCategoryId &&
			    x.RankId == employee.Employee.RankId);
		    if (policy != null)
		    {
			    DateTime ageLimit, serviceLimit;
			    serviceLimit = Convert.ToDateTime(employee.CommissionDate).AddYears(policy.ServiceLimit);
			    ageLimit = Convert.ToDateTime(employee.DoB).AddYears(policy.AgeLimit);

			    return serviceLimit > ageLimit ? serviceLimit : ageLimit;
			}
		    return null;
	    }

	
    }
}