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
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IBnoisRepository<SubCategory> subCategoryRepository;
	    private readonly IProcessRepository processRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public SubCategoryService(IBnoisRepository<SubCategory> subCategoryRepository, IProcessRepository processRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.subCategoryRepository = subCategoryRepository;
            this.processRepository = processRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<SubCategoryModel> GetSubCategories(int ps, int pn, string qs, out int total)
        {
            IQueryable<SubCategory> subCategories = subCategoryRepository.FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.Category.Name.Contains(qs) || String.IsNullOrEmpty(qs))), "Category");
            total = subCategories.Count();
            subCategories = subCategories.OrderByDescending(x => x.SubCategoryId).Skip((pn - 1) * ps).Take(ps);
            List<SubCategoryModel> models = ObjectConverter<SubCategory, SubCategoryModel>.ConvertList(subCategories.ToList()).ToList();
            return models;
        }

        public async Task<SubCategoryModel> GetSubCategory(int id)
        {
            if (id <= 0)
            {
                return new SubCategoryModel();
            }
            SubCategory subCategory = await subCategoryRepository.FindOneAsync(x => x.SubCategoryId == id, new List<string> {"Category"});
            if (subCategory == null)
            {
                throw new InfinityNotFoundException("SubCategory not found");
            }
            SubCategoryModel model = ObjectConverter<SubCategory, SubCategoryModel>.Convert(subCategory);
            return model;
        }
 
        public async Task<SubCategoryModel> SaveSubCategory(int id, SubCategoryModel model)
        { 
            if (model == null)
            {
                throw new InfinityArgumentMissingException("SubCategory data missing");
            }

            bool isExistData = subCategoryRepository.Exists(x => x.CategoryId == model.CategoryId && x.Name == model.Name && x.SubCategoryId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            SubCategory subCategory = ObjectConverter<SubCategoryModel, SubCategory>.Convert(model);
            if (id > 0)
            {
                subCategory = await subCategoryRepository.FindOneAsync(x => x.SubCategoryId == id, new List<string> { "Category" });
                if (subCategory == null)
                {
                    throw new InfinityNotFoundException("SubCategory not found !");
                }

                subCategory.ModifiedDate = DateTime.Now;
                subCategory.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SubCategory";
                bnLog.TableEntryForm = "Officer Naming Convention/Sub Category";
                bnLog.PreviousValue = "Id: " + model.SubCategoryId;
                bnLog.UpdatedValue = "Id: " + model.SubCategoryId;
                if (subCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + subCategory.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (subCategory.CategoryId != model.CategoryId)
                {
                    var cat = employeeService.GetDynamicTableInfoById("Category", "CategoryId", model.CategoryId);
                    bnLog.PreviousValue += ", Category: " + subCategory.Category.Name;
                    bnLog.UpdatedValue += ", Category: " + ((dynamic)cat).Name;
                }
                if (subCategory.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", ShortName: " + subCategory.ShortName;
                    bnLog.UpdatedValue += ", ShortName: " + model.ShortName;
                }
                if (subCategory.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + subCategory.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
                }
                if (subCategory.Prefix != model.Prefix)
                {
                    bnLog.PreviousValue += ", Prefix: " + subCategory.Prefix;
                    bnLog.UpdatedValue += ", Prefix: " + model.Prefix;
                }
                if (subCategory.Rank != model.Rank)
                {
                    bnLog.PreviousValue += ", Rank: " + subCategory.Rank;
                    bnLog.UpdatedValue += ", Rank: " + model.Rank;
                }
                if (subCategory.Branch != model.Branch)
                {
                    bnLog.PreviousValue += ", Branch: " + subCategory.Branch;
                    bnLog.UpdatedValue += ", Branch: " + model.Branch;
                }
                if (subCategory.SubBranch != model.SubBranch)
                {
                    bnLog.PreviousValue += ", SubBranch: " + subCategory.SubBranch;
                    bnLog.UpdatedValue += ", SubBranch: " + model.SubBranch;
                }
                if (subCategory.Course != model.Course)
                {
                    bnLog.PreviousValue += ", Course: " + subCategory.Course;
                    bnLog.UpdatedValue += ", Course: " + model.Course;
                }
                if (subCategory.Medal != model.Medal)
                {
                    bnLog.PreviousValue += ", Medal: " + subCategory.Medal;
                    bnLog.UpdatedValue += ", Medal: " + model.Medal;
                }
                if (subCategory.Award != model.Award)
                {
                    bnLog.PreviousValue += ", Award: " + subCategory.Award;
                    bnLog.UpdatedValue += ", Award: " + model.Award;
                }
                if (subCategory.Prefix2 != model.Prefix2)
                {
                    bnLog.PreviousValue += ", Prefix2: " + subCategory.Prefix2;
                    bnLog.UpdatedValue += ", Prefix2: " + model.Prefix2;
                }
                if (subCategory.NmConEx != model.NmConEx)
                {
                    bnLog.PreviousValue += ", NmConEx: " + subCategory.NmConEx;
                    bnLog.UpdatedValue += ", NmConEx: " + model.NmConEx;
                }
                if (subCategory.Priority != model.Priority)
                {
                    bnLog.PreviousValue += ", Priority: " + subCategory.Priority;
                    bnLog.UpdatedValue += ", Priority: " + model.Priority;
                }
                if (subCategory.BN != model.BN)
                {
                    bnLog.PreviousValue += ", BN: " + subCategory.BN;
                    bnLog.UpdatedValue += ", BN: " + model.BN;
                }
                if (subCategory.BNVR != model.BNVR)
                {
                    bnLog.PreviousValue += ", BNVR: " + subCategory.BNVR;
                    bnLog.UpdatedValue += ", BNVR: " + model.BNVR;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (subCategory.Name != model.Name || subCategory.CategoryId != model.CategoryId || subCategory.ShortName != model.ShortName
                    || subCategory.Description != model.Description || subCategory.Prefix != model.Prefix || subCategory.Rank != model.Rank
                    || subCategory.Branch != model.Branch || subCategory.SubBranch != model.SubBranch || subCategory.Course != model.Course
                    || subCategory.Medal != model.Medal || subCategory.Award != model.Award || subCategory.Prefix2 != model.Prefix2
                    || subCategory.NmConEx != model.NmConEx || subCategory.Priority != model.Priority || subCategory.BN != model.BN
                    || subCategory.BNVR != model.BNVR)
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
                subCategory.IsActive = true;
                subCategory.CreatedDate = DateTime.Now;
                subCategory.CreatedBy = userId;
            }
            subCategory.Name = model.Name;
            subCategory.CategoryId = model.CategoryId;
	        subCategory.ShortName = model.ShortName;
	        subCategory.Description = model.Name;
	        subCategory.Prefix = model.Prefix;
	        subCategory.Rank = model.Rank;
	        subCategory.Branch = model.Branch;
	        subCategory.SubBranch = model.SubBranch;
	        subCategory.Course = model.Course;
	        subCategory.Medal = model.Medal;
	        subCategory.Award = model.Award;
	        subCategory.Prefix2 = model.Prefix2;
	        subCategory.NmConEx = model.NmConEx;
	        subCategory.Priority = model.Priority;
	        subCategory.BN = model.BN;
	        subCategory.BNVR = model.BNVR;

			
            await subCategoryRepository.SaveAsync(subCategory);
            model.SubCategoryId = subCategory.SubCategoryId;

	        await processRepository.UpdateNamingConvention(-1);
			return model;
        }


        public async Task<bool> DeleteSubCategory(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            SubCategory subCategory = await subCategoryRepository.FindOneAsync(x => x.SubCategoryId == id);
            if (subCategory == null)
            {
                throw new InfinityNotFoundException("SubCategory not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SubCategory";
                bnLog.TableEntryForm = "Officer Naming Convention/Sub Category";
                bnLog.PreviousValue = "Id: " + subCategory.SubCategoryId + ", Name: " + subCategory.Name + ", Category: " + subCategory.CategoryId 
                    + ", ShortName: " + subCategory.ShortName + ", Description: " + subCategory.Description + ", Prefix: " + subCategory.Prefix
                    + ", Rank: " + subCategory.Rank + ", Branch: " + subCategory.Branch + ", SubBranch: " + subCategory.SubBranch
                    + ", Course: " + subCategory.Course + ", Medal: " + subCategory.Medal + ", Award: " + subCategory.Award
                    + ", Prefix2: " + subCategory.Prefix2 + ", NmConEx: " + subCategory.NmConEx + ", Priority: " + subCategory.Priority
                    + ", BN: " + subCategory.BN + ", BNVR: " + subCategory.BNVR;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await subCategoryRepository.DeleteAsync(subCategory);
            }
        }

        public async Task<List<SelectModel>> GetSubCategorySelectModels()
        {
            ICollection<SubCategory> categories = await subCategoryRepository.FilterAsync(x => x.IsActive);
            List<SubCategory> query = categories.OrderBy(x => x.Name).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.SubCategoryId
            }).ToList();
            return selectModels;

        }
      
        public async Task<List<SelectModel>> GetSubCategorySelectModelsByCategory(int categoryId)
        {
            ICollection<SubCategory> categories = await subCategoryRepository.FilterAsync(x => x.CategoryId==categoryId);
            List<SubCategory> query = categories.OrderBy(x => x.Name).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.SubCategoryId
            }).ToList();
            return selectModels;

        }
    }
}
