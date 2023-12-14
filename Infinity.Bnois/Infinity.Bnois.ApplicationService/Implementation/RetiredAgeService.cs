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
    public class RetiredAgeService : IRetiredAgeService
    {
        private readonly IBnoisRepository<RetiredAge> retiredAgeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public RetiredAgeService(IBnoisRepository<RetiredAge> retiredAgeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.retiredAgeRepository = retiredAgeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<RetiredAgeModel> GetRetiredAges(int ps, int pn, string qs, out int total)
        {
            IQueryable<RetiredAge> retiredAges = retiredAgeRepository.FilterWithInclude(x => x.IsActive
                && ((x.Category.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.SubCategory.Name.Contains(qs) || String.IsNullOrEmpty(qs))) || (x.Rank.FullName.Contains(qs) || String.IsNullOrEmpty(qs)), "Category", "SubCategory", "Rank");
            total = retiredAges.Count();
            retiredAges = retiredAges.OrderByDescending(x => x.RetiredAgeId).Skip((pn - 1) * ps).Take(ps);
            List<RetiredAgeModel> models = ObjectConverter<RetiredAge, RetiredAgeModel>.ConvertList(retiredAges.ToList()).ToList();
            models = models.Select(x =>
            {
                x.RStatusName = Enum.GetName(typeof(RStatus), x.RStatus);
                return x;
            }).ToList();
            return models;
        }

        public async Task<RetiredAgeModel> GetRetiredAge(int id)
        {
            if (id <= 0)
            {
                return new RetiredAgeModel();
            }
            RetiredAge RetiredAge = await retiredAgeRepository.FindOneAsync(x => x.RetiredAgeId == id);
            if (RetiredAge == null)
            {
                throw new InfinityNotFoundException("Retired Age not found");
            }
            RetiredAgeModel model = ObjectConverter<RetiredAge, RetiredAgeModel>.Convert(RetiredAge);
            return model;
        }


        public async Task<RetiredAgeModel> SaveRetiredAge(int id, RetiredAgeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Retired Age data missing");
            }


            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            RetiredAge retiredAge = ObjectConverter<RetiredAgeModel, RetiredAge>.Convert(model);
            if (id > 0)
            {
                retiredAge = await retiredAgeRepository.FindOneAsync(x => x.RetiredAgeId == id);
                if (retiredAge == null)
                {
                    throw new InfinityNotFoundException("Retired Age not found !");
                }
                retiredAge.ModifiedDate = DateTime.Now;
                retiredAge.ModifiedBy = userId;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "RetiredAge";
                bnLog.TableEntryForm = "Retired Age List Policy";
                bnLog.PreviousValue = "Id: " + model.RetiredAgeId;
                bnLog.UpdatedValue = "Id: " + model.RetiredAgeId;
                int bnoisUpdateCount = 0;

                if (retiredAge.CategoryId != model.CategoryId)
                {
                    if (retiredAge.CategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Category", "CategoryId", retiredAge.CategoryId);
                        bnLog.PreviousValue += ", Category: " + ((dynamic)prev).Name;
                    }
                    if (model.CategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Category", "CategoryId", model.CategoryId);
                        bnLog.UpdatedValue += ", Category: " + ((dynamic)newv).PNo;
                    }
                }
                if (retiredAge.SubCategoryId != model.SubCategoryId)
                {
                    if (retiredAge.SubCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("SubCategory", "SubCategoryId", retiredAge.SubCategoryId);
                        bnLog.PreviousValue += ", Sub Category: " + ((dynamic)prev).Name;
                    }
                    if (model.SubCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("SubCategory", "SubCategoryId", model.SubCategoryId);
                        bnLog.UpdatedValue += ", Sub Category: " + ((dynamic)newv).PNo;
                    }
                }
                if (retiredAge.RankId != model.RankId)
                {
                    if (retiredAge.RankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", retiredAge.RankId);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                    }
                    if (model.SubCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)newv).ShortName;
                    }
                }
                if (retiredAge.AgeLimit != model.AgeLimit)
                {
                    bnLog.PreviousValue += ", Retirement Date: " + retiredAge.AgeLimit;
                    bnLog.UpdatedValue += ", Retirement Date: " + model.AgeLimit;
                    bnoisUpdateCount += 1;
                }
                if (retiredAge.RStatus != model.RStatus)
                {
                    bnLog.PreviousValue += ", List Type: " + (retiredAge.RStatus == 1 ? "Retired" : retiredAge.RStatus == 2 ? "Emergency" : retiredAge.RStatus == 3 ? "Reserve" : "");
                    bnLog.UpdatedValue += ", List Type: " + (model.RStatus == 1 ? "Retired" : model.RStatus == 2 ? "Emergency" : model.RStatus == 3 ? "Reserve" : "");
                    bnoisUpdateCount += 1;
                }
                if (retiredAge.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + retiredAge.Remarks;
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
                retiredAge.IsActive = true;
                retiredAge.CreatedDate = DateTime.Now;
                retiredAge.CreatedBy = userId;
            }
            retiredAge.CategoryId = model.CategoryId;
            retiredAge.SubCategoryId = model.SubCategoryId;
            retiredAge.RankId = model.RankId;
            retiredAge.AgeLimit = model.AgeLimit;
            retiredAge.RStatus = model.RStatus;
            retiredAge.Remarks = model.Remarks;


            await retiredAgeRepository.SaveAsync(retiredAge);
            model.RetiredAgeId = retiredAge.RetiredAgeId;
            return model;
        }


        public async Task<bool> DeleteRetiredAge(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            RetiredAge retiredAge = await retiredAgeRepository.FindOneAsync(x => x.RetiredAgeId == id);
            if (retiredAge == null)
            {
                throw new InfinityNotFoundException("Retired Age not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "RetiredAge";
                bnLog.TableEntryForm = "Retired Age List Policy";
                bnLog.PreviousValue = "Id: " + retiredAge.RetiredAgeId;
                if (retiredAge.CategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Category", "CategoryId", retiredAge.CategoryId);
                    bnLog.PreviousValue += ", Category: " + ((dynamic)prev).Name;
                }
                if (retiredAge.SubCategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("SubCategory", "SubCategoryId", retiredAge.SubCategoryId);
                    bnLog.PreviousValue += ", Sub Category: " + ((dynamic)prev).Name;
                }
                if (retiredAge.RankId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", retiredAge.RankId);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prev).ShortName;
                }
                bnLog.PreviousValue += ", Retirement Date: " + retiredAge.AgeLimit + ", List Type: " + (retiredAge.RStatus == 1 ? "Retired" : retiredAge.RStatus == 2 ? "Emergency" : retiredAge.RStatus == 3 ? "Reserve" : "") + ", Remarks: " + retiredAge.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await retiredAgeRepository.DeleteAsync(retiredAge);
            }
        }



        public List<SelectModel> GetRStatusSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(RStatus)).Cast<RStatus>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value =Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

    }
}