using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class ExamSubjectService : IExamSubjectService
    {
        private readonly IBnoisRepository<ExamSubject> examSubjectRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ExamSubjectService(IBnoisRepository<ExamSubject> examSubjectRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.examSubjectRepository = examSubjectRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }


        public List<ExamSubjectModel> GetExamSubjects(int ps, int pn, string qs, out int total)
        {
            IQueryable<ExamSubject> queryable = examSubjectRepository.FilterWithInclude(x => x.IsActive && ((x.Examination.Name.Contains(qs) || String.IsNullOrEmpty(qs)) 
            || ((x.Examination.ExamCategory.Name.Contains(qs) || String.IsNullOrEmpty(qs))) 
            || (x.Subject.Name.Contains(qs) || String.IsNullOrEmpty(qs))), "Subject", "Examination.ExamCategory");
            total = queryable.Count();
            queryable = queryable.OrderByDescending(x => x.ExamSubjectId).Skip((pn - 1) * ps).Take(ps);
            List<ExamSubjectModel> models = ObjectConverter<ExamSubject, ExamSubjectModel>.ConvertList(queryable.ToList()).ToList();
            return models;
        }

        public async Task<ExamSubjectModel> GetExamSubject(int examSubjectId)
        {
            if (examSubjectId <= 0)
            {
                return new ExamSubjectModel();
            }
            ExamSubject examSubject = await examSubjectRepository.FindOneAsync(x => x.ExamSubjectId == examSubjectId, new List<string>() { "Subject", "Examination" });

            if (examSubject == null)
            {
                throw new InfinityNotFoundException("ExamSubject not found!");
            }
            ExamSubjectModel model = ObjectConverter<ExamSubject, ExamSubjectModel>.Convert(examSubject);
            return model;
        }

        public async Task<ExamSubjectModel> SaveExamSubject(int examSubjectId, ExamSubjectModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("ExamSubject data missing!");
            }
            bool isExist = examSubjectRepository.Exists(x => x.ExamSubjectId != model.ExamSubjectId && (x.ExaminationId == model.ExaminationId && x.SubjectId == model.SubjectId));
            if (isExist)
            {
                throw new InfinityInvalidDataException("Exam Subject already Exists !");
            }
            model.Examination = null;
            ExamSubject examSubject = ObjectConverter<ExamSubjectModel, ExamSubject>.Convert(model);
            
            if (examSubjectId > 0)
            {
                examSubject = await examSubjectRepository.FindOneAsync(x => x.ExamSubjectId == examSubjectId, new List<string>() { "Subject", "Examination" });
                if (examSubject == null)
                {
                    throw new InfinityNotFoundException("ExamSubject not found!");
                }
                examSubject.ModifiedDate = DateTime.Now;
                examSubject.ModifiedBy = model.ModifiedBy;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ExamSubject";
                bnLog.TableEntryForm = "Exam Subject";
                bnLog.PreviousValue = "Id: " + model.ExamSubjectId;
                bnLog.UpdatedValue = "Id: " + model.ExamSubjectId;
                if (examSubject.ExaminationId != model.ExaminationId)
                {
                    //Examination exam = 
                    bnLog.PreviousValue += ", Examination: " + examSubject.Examination.Name;
                    bnLog.UpdatedValue += ", Examination: " + model.Examination.Name;
                }
                if (examSubject.SubjectId != model.SubjectId)
                {
                    bnLog.PreviousValue += ", Subject: " + examSubject.Subject.Name;
                    bnLog.UpdatedValue += ", Subject: " + model.Subject.Name;
                }
                if (examSubject.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + examSubject.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (examSubject.ExaminationId != model.ExaminationId || examSubject.SubjectId != model.SubjectId || examSubject.Remarks != model.Remarks)
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
                examSubject.CreatedBy = model.CreatedBy;
                examSubject.CreatedDate = DateTime.Now;
                examSubject.IsActive = true;
            }
            examSubject.ExaminationId = model.ExaminationId;
            examSubject.SubjectId = model.SubjectId;
            await examSubjectRepository.SaveAsync(examSubject);
            model.ExamSubjectId = examSubject.ExamSubjectId;
            return model;
        }

        public async Task<bool> DeleteExamSubject(int examSubjectId)
        {
            if (examSubjectId <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ExamSubject examSubject = await examSubjectRepository.FindOneAsync(x => x.ExamSubjectId == examSubjectId, new List<string>() { "Subject", "Examination" });
            if (examSubject == null)
            {
                throw new InfinityNotFoundException("ExamSubject not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ExamSubject";
                bnLog.TableEntryForm = "Exam Subject";
                bnLog.PreviousValue = "Id: " + examSubject.ExamSubjectId + ", Examination: " + examSubject.ExaminationId + ", Subject: " + examSubject.SubjectId + ", Remarks: " + examSubject.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await examSubjectRepository.DeleteAsync(examSubject);
            }
        }
    }
}
