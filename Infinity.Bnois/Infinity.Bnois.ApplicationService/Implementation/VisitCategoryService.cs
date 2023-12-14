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
   

    public class VisitCategoryService : IVisitCategoryService
    {
        private readonly IBnoisRepository<VisitCategory> visitCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public VisitCategoryService(IBnoisRepository<VisitCategory> visitCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.visitCategoryRepository = visitCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<VisitCategoryModel> GetVisitCategories(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<VisitCategory> visitCategories = visitCategoryRepository
                .FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(searchText)) || String.IsNullOrEmpty(searchText)));
            total = visitCategories.Count();
            visitCategories = visitCategories.OrderByDescending(x => x.VisitCategoryId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<VisitCategoryModel> models = ObjectConverter<VisitCategory, VisitCategoryModel>.ConvertList(visitCategories.ToList()).ToList();
            return models;
        }

        public async Task<VisitCategoryModel> GetVisitCategory(int  id)
        {
            if (id <= 0)
            {
                return new VisitCategoryModel();
            }
            VisitCategory visitCategory = await visitCategoryRepository.FindOneAsync(x => x.VisitCategoryId == id);

            if (visitCategory == null)
            {
                throw new InfinityNotFoundException("Visit Category not found!");
            }
            VisitCategoryModel model = ObjectConverter<VisitCategory, VisitCategoryModel>.Convert(visitCategory);
            return model;
        }

        public async Task<VisitCategoryModel> SaveVisitCategory(int id, VisitCategoryModel model)
        {
            bool isExist = await visitCategoryRepository.ExistsAsync(x => (x.Name == model.Name) && x.VisitCategoryId != model.VisitCategoryId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Visit Category data already exist !");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Visit Category data missing!");
            }

            VisitCategory visitCategory = ObjectConverter<VisitCategoryModel, VisitCategory>.Convert(model);

            if (id > 0)
            {
                visitCategory = await visitCategoryRepository.FindOneAsync(x => x.VisitCategoryId == id);
                if (visitCategory == null)
                {
                    throw new InfinityNotFoundException("Visit Category not found!");
                }
                visitCategory.ModifiedDate = DateTime.Now;
                visitCategory.ModifiedBy = model.ModifiedBy;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "VisitCategory";
                bnLog.TableEntryForm = "Visit Category";
                bnLog.PreviousValue = "Id: " + model.VisitCategoryId;
                bnLog.UpdatedValue = "Id: " + model.VisitCategoryId;
                int bnoisUpdateCount = 0;

                
                if (visitCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ",  Name: " + visitCategory.Name;
                    bnLog.UpdatedValue += ",  Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (visitCategory.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ",  Remarks: " + visitCategory.Remarks;
                    bnLog.UpdatedValue += ",  Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
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
                visitCategory.CreatedBy = model.CreatedBy;
                visitCategory.CreatedDate = DateTime.Now;
                visitCategory.IsActive = true;
            }
            visitCategory.Name = model.Name;
            visitCategory.Remarks = model.Remarks;

            await visitCategoryRepository.SaveAsync(visitCategory);
            model.VisitCategoryId = visitCategory.VisitCategoryId;
            return model;
        }

        public async Task<bool> DeleteVisitCategory(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            VisitCategory visitCategory = await visitCategoryRepository.FindOneAsync(x => x.VisitCategoryId == id);
            if (visitCategory == null)
            {
                throw new InfinityNotFoundException("Visit Category not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "VisitCategory";
                bnLog.TableEntryForm = "Visit Category";
                bnLog.PreviousValue = "Id: " + visitCategory.VisitCategoryId;
               
                bnLog.PreviousValue += ",  Name: " + visitCategory.Name + ",  Remarks: " + visitCategory.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                return await visitCategoryRepository.DeleteAsync(visitCategory);
            }
        }

        public async Task<List<SelectModel>> GetVisitCategorySelectModels()
        {
            ICollection<VisitCategory> models = await visitCategoryRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x=>x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.VisitCategoryId
            }).ToList();
        }

    }
}