﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
	public class SubjectService : ISubjectService
	{
		private readonly IBnoisRepository<Subject> subjectRepository;
        private readonly IBnoisRepository<ExamSubject> examSubjectRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;

        public SubjectService(IBnoisRepository<Subject> subjectRepository, IBnoisRepository<ExamSubject> examSubjectRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
		{
            this.subjectRepository = subjectRepository;
            this.examSubjectRepository = examSubjectRepository; 
            this.bnoisLogRepository = bnoisLogRepository;
        }

		public async Task<bool> DeleteSubject(int id)
		{
			if (id <= 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			Subject subject = await subjectRepository.FindOneAsync(x => x.SubjectId == id);
			if (subject == null)
			{
				throw new InfinityNotFoundException("Subject not found");
			}
			else
			{
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Subject";
                bnLog.TableEntryForm = "Subject";
                bnLog.PreviousValue = "Id: " + subject.SubjectId + ", Name: " + subject.Name + ", Remarks: " + subject.Remarks + ", BnList: " + subject.GoToBnList;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await subjectRepository.DeleteAsync(subject);
			}
		}

		public List<SubjectModel> GetSubjects(int ps, int pn, string qs, out int total)
		{
			IQueryable<Subject> subjects = subjectRepository.FilterWithInclude(x => x.IsActive
	   && ((x.Name.Contains(qs)
	   || String.IsNullOrEmpty(qs))));
			total = subjects.Count();
			subjects = subjects.OrderByDescending(x => x.SubjectId).Skip((pn - 1) * ps).Take(ps);
			List<SubjectModel> models = ObjectConverter<Subject, SubjectModel>.ConvertList(subjects.ToList()).ToList();
			return models;
		}

		public async Task<SubjectModel> GetSubjects(int id)
		{
			if (id <= 0)
			{
				return new SubjectModel();
			}
			Subject subject = await subjectRepository.FindOneAsync(x => x.SubjectId == id);
			if (subject == null)
			{
				throw new InfinityNotFoundException("Subject not found");
			}
			SubjectModel model = ObjectConverter<Subject, SubjectModel>.Convert(subject);
			return model;
		}

        public async Task<List<SelectModel>> GetSubjectSelectModels()
        {
            ICollection<Subject> subjects = await subjectRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = subjects.Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.SubjectId
            }).ToList();
            return selectModels;
        }

        public async Task<SubjectModel> SaveSubject(int id, SubjectModel model)
		{
			string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			if (model == null)
			{
				throw new InfinityArgumentMissingException("Subject data missing");
			}
			Subject subject = ObjectConverter<SubjectModel, Subject>.Convert(model);
			if (id > 0)
			{
				subject = await subjectRepository.FindOneAsync(x => x.SubjectId == id);
				if (subject == null)
				{
					throw new InfinityNotFoundException("Subject not found !");
				}
                subject.ModifiedDate = DateTime.Now;
                subject.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Subject";
                bnLog.TableEntryForm = "Subject";
                bnLog.PreviousValue = "Id: " + model.SubjectId;
                bnLog.UpdatedValue = "Id: " + model.SubjectId;
                int bnoisUpdateCount = 0;
                if (subject.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + subject.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (subject.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + subject.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }
                if (subject.GoToBnList != model.GoToBnList)
                {
                    bnLog.PreviousValue += ", BnList: " + subject.GoToBnList;
                    bnLog.UpdatedValue += ", BnList: " + model.GoToBnList;
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
                subject.IsActive = true;
                subject.CreatedDate = DateTime.Now;
				subject.CreatedBy = userId;
			}
			subject.Name = model.Name;
			subject.Remarks = model.Remarks;
            subject.GoToBnList= model.GoToBnList;

            await subjectRepository.SaveAsync(subject);
			model.SubjectId = subject.SubjectId;
			return model;
		}

        public async Task<List<SelectModel>> FilterSubjects(string searchStr)
        {
            IQueryable<Subject> queryable = subjectRepository
               .Where(x => x.IsActive
               && ((x.Name.Contains(searchStr)|| String.IsNullOrEmpty(searchStr))));
            queryable = queryable.OrderBy(x => x.Name).Take(10);

            List<Subject> subjects = await queryable.ToListAsync();
            List<SelectModel> subjeSelectModels = subjects.Select(x => new SelectModel()
            {
                Text =  x.Name,
                Value = x.SubjectId
            }).ToList();
            return subjeSelectModels;
        }

        public async Task<List<SelectModel>> GetSubjectsSelectModelByExamination(int? examinationId)
        {
                IQueryable<Subject> query = examSubjectRepository.FilterWithInclude(x => x.IsActive && x.ExaminationId == examinationId, "Subject").Select(x => x.Subject);
                List<Subject> subjects = await query.OrderBy(x => x.Name).ToListAsync();
                List<SelectModel> subjectSelectModels = subjects.Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.SubjectId
                }).ToList();
                return subjectSelectModels;
        }
    }
}
