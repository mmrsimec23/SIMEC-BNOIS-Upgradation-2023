using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using Infinity.Ers.ApplicationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class ExamCategoryService : IExamCategoryService
    {
        private readonly IBnoisRepository<ExamCategory> _examCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ExamCategoryService(IBnoisRepository<ExamCategory> examCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            _examCategoryRepository = examCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }
        public List<ExamCategoryModel> GetExamCategories(int pageSize, int pageNumber,string searchText, out int total)
        {
            
            IQueryable<ExamCategory> examCategories = _examCategoryRepository
                .FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(searchText))
                || String.IsNullOrEmpty(searchText)
                || (x.ExamCategoryCode.Contains(searchText)
                || String.IsNullOrEmpty(searchText))));
            total = examCategories.Count();
            examCategories = examCategories.OrderByDescending(x => x.ExamCategoryId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<ExamCategoryModel> models = ObjectConverter<ExamCategory, ExamCategoryModel>.ConvertList(examCategories.ToList()).ToList();
            return models;
        }

        public async Task<ExamCategoryModel> GetExamCategory(int examCategoryId)
        {
            if (examCategoryId <= 0)
            {
                return new ExamCategoryModel();
            }
            ExamCategory examCategory = await _examCategoryRepository.FindOneAsync(x => x.ExamCategoryId == examCategoryId);
            if (examCategory == null)
            {
                throw new InfinityNotFoundException("Exam Category Not Found!");
            }
            ExamCategoryModel model = ObjectConverter<ExamCategory, ExamCategoryModel>.Convert(examCategory);
            return model;
        }

        public async Task<ExamCategoryModel> SaveExamCategory(int examCategoryId, ExamCategoryModel model)
        {

            bool isExist = await _examCategoryRepository.ExistsAsync(x => (x.ExamCategoryCode == model.ExamCategoryCode || x.Name == model.Name) && x.ExamCategoryId != model.ExamCategoryId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Exam category data already exist !");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Exam Category Data Missing");
            }
            ExamCategory examCategory = ObjectConverter<ExamCategoryModel, ExamCategory>.Convert(model);
            if (examCategoryId > 0)
            {
                examCategory = await _examCategoryRepository.FindOneAsync(x => x.ExamCategoryId == examCategoryId);
                if (examCategory == null)
                {
                    throw new InfinityNotFoundException("Exam Category Not Found!");
                }
                examCategory.ModifiedBy = model.ModifiedBy;
                examCategory.ModifiedDate = DateTime.Now;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ExamCategory";
                bnLog.TableEntryForm = "Exam Category";
                bnLog.PreviousValue = "Id: " + model.ExamCategoryId;
                bnLog.UpdatedValue = "Id: " + model.ExamCategoryId;
                if (examCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + examCategory.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (examCategory.ExamCategoryCode != model.ExamCategoryCode)
                {
                    bnLog.PreviousValue += ", Exam Category Code: " + examCategory.ExamCategoryCode;
                    bnLog.UpdatedValue += ", Exam Category Code: " + model.ExamCategoryCode;
                }
                if (examCategory.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + examCategory.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
                }
                if (examCategory.BoardType != model.BoardType)
                {
                    bnLog.PreviousValue += ", Board Type: " + examCategory.BoardType;
                    bnLog.UpdatedValue += ", Board Type: " + model.BoardType;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (examCategory.Name != model.Name || examCategory.ExamCategoryCode != model.ExamCategoryCode || examCategory.Description != model.Description || examCategory.BoardType != model.BoardType)
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
                examCategory.CreatedBy = model.CreatedBy;
                examCategory.CreatedDate = DateTime.Now;
                examCategory.IsActive = true;
            }
            examCategory.ExamCategoryCode = model.ExamCategoryCode;
            examCategory.BoardType = model.BoardType;
            examCategory.Name = model.Name;
            examCategory.Description = model.Description;
            await _examCategoryRepository.SaveAsync(examCategory);
            model.ExamCategoryId = examCategory.ExamCategoryId;
            return model;
        }
        public async Task<bool> DeleteExamCategory(int examCategoryId)
        {
            if (examCategoryId <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request!");
            }
            ExamCategory examCategory = await _examCategoryRepository.FindOneAsync(x => x.ExamCategoryId == examCategoryId);
            if (examCategory == null)
            {
                throw new InfinityNotFoundException("Exam Category Not Found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ExamCategory";
                bnLog.TableEntryForm = "Exam Category";
                bnLog.PreviousValue = "Id: " + examCategory.ExamCategoryId + ", Name: " + examCategory.Name + ", Exam Category Code: " + examCategory.ExamCategoryCode + ", Description: " + examCategory.Description + ", Board Type: " + examCategory.BoardType;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await _examCategoryRepository.DeleteAsync(examCategory);
            }
        }

        public List<SelectModel> GetExamCategories()
        {
            List<ExamCategory> models = _examCategoryRepository.AsQueryable().ToList();
            return models.OrderBy(x => x.Name).Select(x => new SelectModel { Text = x.Name, Value = x.ExamCategoryId }).ToList();
        }

        public async Task<List<SelectModel>> GetExamCategorySelectModels()
        {
            ICollection<ExamCategory> examCategories = await _examCategoryRepository.FilterAsync(x => x.IsActive);         
            List<SelectModel> selectModels = examCategories.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text =  x.Name,
                Value = x.ExamCategoryId
            }).ToList();
            return selectModels;
        }
    }
}
