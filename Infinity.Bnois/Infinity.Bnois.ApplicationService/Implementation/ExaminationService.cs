
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
    public class ExaminationService : IExaminationService
    {
        private readonly IBnoisRepository<Examination> _examinationRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public ExaminationService(IBnoisRepository<Examination> examinationRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            _examinationRepository = examinationRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<ExaminationModel> GetExaminations(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<Examination> examinations = _examinationRepository.FilterWithInclude(x => x.IsActive && ((x.Name.Contains(searchText) || String.IsNullOrEmpty(searchText)) || (x.ExamCategory.Name.Contains(searchText) || String.IsNullOrEmpty(searchText))), "ExamCategory");
            total = examinations.Count();
            examinations = examinations.OrderByDescending(x => x.ExaminationId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<ExaminationModel> models = ObjectConverter<Examination, ExaminationModel>.ConvertList(examinations.ToList()).ToList();
            return models;
        }
        public async Task<ExaminationModel> GetExamination(int examinationId)
        {
            if (examinationId == 0)
            {
                return new ExaminationModel();
            }
            Examination examination = await _examinationRepository.FindOneAsync(x => x.ExaminationId == examinationId);
            if (examination == null)
            {
                throw new InfinityNotFoundException("Examination not found !");
            }
            return ObjectConverter<Examination, ExaminationModel>.Convert(examination);
        }
        public async Task<ExaminationModel> SaveExamination(int examinationId, ExaminationModel model)
        {

            bool isExist = await _examinationRepository.ExistsAsync(x => x.Name == model.Name && x.ExaminationId != model.ExaminationId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Examination data already exist !");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Examination data missing");
            }
            Examination examination = ObjectConverter<ExaminationModel, Examination>.Convert(model);
            if (examinationId > 0)
            {
                examination = await _examinationRepository.FindOneAsync(x => x.ExaminationId == examinationId, new List<string>() { "ExamCategory" });
                if (examination == null)
                {
                    throw new InfinityNotFoundException("Examination not found!");
                }
                examination.ModifiedBy = model.ModifiedBy;
                examination.ModifiedDate = DateTime.Now;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Examination";
                bnLog.TableEntryForm = "Examination";
                bnLog.PreviousValue = "Id: " + model.ExaminationId;
                bnLog.UpdatedValue = "Id: " + model.ExaminationId;
                if (examination.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + examination.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (examination.ExaminationCode != model.ExaminationCode)
                {
                    bnLog.PreviousValue += ", ExaminationCode: " + examination.ExaminationCode;
                    bnLog.UpdatedValue += ", ExaminationCode: " + model.ExaminationCode;
                }
                if (examination.ExamCategoryId != model.ExamCategoryId)
                {
                    var exam = employeeService.GetDynamicTableInfoById("ExamCategory", "ExamCategoryId", model.ExamCategoryId);
                    bnLog.PreviousValue += ", ExamCategory: " + examination.ExamCategory.Name;
                    bnLog.UpdatedValue += ", ExamCategory: " + ((dynamic)exam).Name;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (examination.Name != model.Name || examination.ExaminationCode != model.ExaminationCode || examination.ExamCategoryId != model.ExamCategoryId)
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
                examination.CreatedBy = model.CreatedBy;
                examination.CreatedDate = DateTime.Now;
                examination.IsActive = true;
            }
            examination.ExaminationCode = model.ExaminationCode;
            examination.Name = model.Name;
            examination.ExamCategoryId = model.ExamCategoryId;
            examination.ExamCategory = null;
            await _examinationRepository.SaveAsync(examination);
            model.ExaminationId = examination.ExaminationId;
            return model;
        }
        public async Task<bool> DeleteExamination(int examinationId)
        {
            if (examinationId <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Examination examination = await _examinationRepository.FindOneAsync(x => x.ExaminationId == examinationId);
            if (examination == null)
            {
                throw new InfinityNotFoundException("Examination not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Examination";
                bnLog.TableEntryForm = "Examination";
                bnLog.PreviousValue = "Id: " + examination.ExaminationId + ", Name: " + examination.Name + ", ExaminationCode: " + examination.ExaminationCode + ", ExamCategory: " + examination.ExamCategoryId;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await _examinationRepository.DeleteAsync(examination);
            }
        }

        public List<SelectModel> GetExaminations()
        {
            List<Examination> examinations = _examinationRepository.AsQueryable().ToList();
            return examinations.OrderBy(x => x.Name).Select(x => new SelectModel { Text = x.Name, Value = x.ExaminationId }).ToList();
        }

        public async Task<List<SelectModel>> GetExaminationSelectModelByExamCategory(int? examCategoryId)
        {
            IQueryable<Examination> queryable = _examinationRepository.Where(x => x.IsActive && x.ExamCategoryId == examCategoryId).OrderBy(x => x.ExaminationCode);
            List<Examination> examinations = await queryable.ToListAsync();
            List<SelectModel> selectModels = examinations.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.ExaminationId
            }).ToList();

            return selectModels;
        }

        public async Task<List<SelectModel>> GetExaminationSelectModels()
        {
            IQueryable<Examination> queryable = _examinationRepository.Where(x => x.IsActive ).OrderBy(x => x.Name);
            List<Examination> examinations = await queryable.ToListAsync();
            List<SelectModel> selectModels = examinations.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.ExaminationId
            }).ToList();

            return selectModels;
        }

    }
}
