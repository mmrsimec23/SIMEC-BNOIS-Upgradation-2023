using Infinity.Bnois;
using Infinity.Bnois.ApplicationService.Implementation;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infinity.Ers.ApplicationService
{
    public class EducationService : IEducationService
    {
        private readonly IBnoisRepository<Education> educationRepository;
        private readonly IBnoisRepository<Result> resultRepository;
        private readonly IBnoisRepository<ExamCategory> examCategoryRepository;
        private readonly IBnoisRepository<Employee> employeeRepository;
        private readonly IBnoisRepository<Subject> subjectRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EducationService(IBnoisRepository<Employee> employeeRepository,
            IBnoisRepository<ExamCategory> examCategoryRepository,
            IBnoisRepository<Result> resultRepository,
            IBnoisRepository<Education> educationRepository,
            IBnoisRepository<Subject> subjectRepository, 
            IBnoisRepository<BnoisLog> bnoisLogRepository, 
            IEmployeeService employeeService)
        {
            this.educationRepository = educationRepository;
            this.resultRepository = resultRepository;
            this.examCategoryRepository = examCategoryRepository;
            this.employeeRepository = employeeRepository;
            this.employeeRepository = employeeRepository;
            this.subjectRepository = subjectRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<EducationModel> GetEducations(int employeeId)
        {
            List<Education> educations = educationRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "ExamCategory", "Examination", "Subject", "Result","Board","Institute","ResultGrade").OrderBy(x=>x.PassingYear).ToList();
            List<EducationModel> models = ObjectConverter<Education, EducationModel>.ConvertList(educations.ToList()).ToList();
            return models;
        }
        public async Task<EducationModel> GetEducation(int educationId)
        {
            if (educationId <= 0)
            {
                return new EducationModel();
            }
            Education education = await educationRepository.FindOneAsync(x => x.EducationId == educationId, new List<string> { "ExamCategory" });

            if (education == null)
            {
                throw new InfinityNotFoundException("Education not found!");
            }
            EducationModel model = ObjectConverter<Education, EducationModel>.Convert(education);
            return model;
        }

        public async Task<EducationModel> SaveEducation(int educationId, EducationModel model)
        {
            Result result = await resultRepository.FindOneAsync(x => x.ResultId == model.ResultId);
            CheckResult(result, model);


            if (model == null)
            {
                throw new InfinityArgumentMissingException("Education data missing");
            }
            Education education = ObjectConverter<EducationModel, Education>.Convert(model);

            if (educationId > 0)
            {
                education = await educationRepository.FindOneAsync(x => x.EducationId == educationId);
                if (education == null)
                {
                    throw new InfinityNotFoundException("Education not found!");
                }
                education.ModifiedBy = model.EmployeeId.ToString();
                education.ModifiedDate = DateTime.Now;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Education";
                bnLog.TableEntryForm = "Employee Education";
                bnLog.PreviousValue = "Id: " + model.EducationId;
                bnLog.UpdatedValue = "Id: " + model.EducationId;
                int bnoisUpdateCount = 0;
                if (education.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", education.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }

                if (education.ExamCategoryId != model.ExamCategoryId)
                {
                    if (education.ExamCategoryId > 0)
                    {
                        var prevExamCategory = employeeService.GetDynamicTableInfoById("ExamCategory", "ExamCategoryId", education.ExamCategoryId??0);
                        bnLog.PreviousValue += ", Exam Title: " + ((dynamic)prevExamCategory).Name;
                    }
                    if (model.ExamCategoryId > 0)
                    {
                        var newExamCategory = employeeService.GetDynamicTableInfoById("ExamCategory", "ExamCategoryId", model.ExamCategoryId??0);
                        bnLog.UpdatedValue += ", Exam Title: " + ((dynamic)newExamCategory).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (education.ExaminationId != model.ExaminationId)
                {
                    if (education.ExaminationId > 0)
                    {
                        var prevExamination = employeeService.GetDynamicTableInfoById("Examination", "ExaminationId", education.ExaminationId??0);
                        bnLog.PreviousValue += ", Examination: " + ((dynamic)prevExamination).Name;
                    }
                    if (model.ExaminationId > 0)
                    {
                        var newExamination = employeeService.GetDynamicTableInfoById("Examination", "ExaminationId", model.ExaminationId??0);
                        bnLog.UpdatedValue += ", Examination: " + ((dynamic)newExamination).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (education.BoardId != model.BoardId)
                {
                    if (education.BoardId > 0)
                    {
                        var prevBoard = employeeService.GetDynamicTableInfoById("Board", "BoardId", education.BoardId??0);
                        bnLog.PreviousValue += ", Board/University: " + ((dynamic)prevBoard).Name;
                    }
                    if (model.BoardId > 0)
                    {
                        var newBoard = employeeService.GetDynamicTableInfoById("Board", "BoardId", model.BoardId??0);
                        bnLog.UpdatedValue += ", Board/University: " + ((dynamic)newBoard).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (education.InstituteId != model.InstituteId)
                {
                    if (education.InstituteId > 0)
                    {
                        var prevInstitute = employeeService.GetDynamicTableInfoById("Institute", "InstituteId", (int)education.InstituteId);
                        bnLog.PreviousValue += ", Institute: " + ((dynamic)prevInstitute).Name;
                    }
                    if (model.InstituteId > 0)
                    {
                        var newInstitute = employeeService.GetDynamicTableInfoById("Institute", "InstituteId", (int)model.InstituteId);
                        bnLog.UpdatedValue += ", Institute: " + ((dynamic)newInstitute).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (education.Subjectid != model.Subjectid)
                {
                    if (education.Subjectid > 0)
                    {
                        var prevSubject = employeeService.GetDynamicTableInfoById("Subject", "Subjectid", education.Subjectid??0);
                        bnLog.PreviousValue += ", Subject/Group: " + ((dynamic)prevSubject).Name;
                    }
                    if (model.Subjectid > 0)
                    {
                        var newSubject = employeeService.GetDynamicTableInfoById("Subject", "Subjectid", model.Subjectid??0);
                        bnLog.UpdatedValue += ", Subject/Group: " + ((dynamic)newSubject).Name;
                    }
                    bnoisUpdateCount += 1;
                }

                if (education.Roll != model.Roll)
                {
                    bnLog.PreviousValue += ", Roll No.: " + education.Roll;
                    bnLog.UpdatedValue += ", Roll No.: " + model.Roll;

                    bnoisUpdateCount += 1;
                }
                if (education.RegNo != model.RegNo)
                {
                    bnLog.PreviousValue += ", Registration No.: " + education.RegNo;
                    bnLog.UpdatedValue += ", Registration No.: " + model.RegNo;

                    bnoisUpdateCount += 1;
                }
                if (education.ResultId != model.ResultId)
                {
                    if (education.ResultId > 0)
                    {
                        var prevResult = employeeService.GetDynamicTableInfoById("Result", "ResultId", education.ResultId??0);
                        bnLog.PreviousValue += ", Result: " + ((dynamic)prevResult).Name;
                    }
                    if (model.ResultId > 0)
                    {
                        var newResult = employeeService.GetDynamicTableInfoById("Result", "ResultId", model.ResultId??0);
                        bnLog.UpdatedValue += ", Result: " + ((dynamic)newResult).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (education.Gpa != model.Gpa)
                {
                    bnLog.PreviousValue += ", GPA: " + education.Gpa;
                    bnLog.UpdatedValue += ", GPA: " + model.Gpa;

                    bnoisUpdateCount += 1;
                }
                if (education.ResultGradeId != model.ResultGradeId)
                {
                    if (education.ResultGradeId > 0)
                    {
                        var prevGrade = employeeService.GetDynamicTableInfoById("ResultGrade", "ResultGradeId", education.ResultGradeId??0);
                        bnLog.PreviousValue += ", Grade: " + ((dynamic)prevGrade).Name;
                    }
                    if (model.ResultGradeId > 0)
                    {
                        var newGrade = employeeService.GetDynamicTableInfoById("ResultGrade", "ResultGradeId", model.ResultGradeId??0);
                        bnLog.UpdatedValue += ", Grade: " + ((dynamic)newGrade).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (education.Marks != model.Marks)
                {
                    bnLog.PreviousValue += ", Marks: " + education.Marks;
                    bnLog.UpdatedValue += ", Marks: " + model.Marks;

                    bnoisUpdateCount += 1;
                }
                if (education.Distiction != model.Distiction)
                {
                    bnLog.PreviousValue += ",  Distinction/Letter: " + education.Distiction;
                    bnLog.UpdatedValue += ", Distinction/Letter: " + model.Distiction;

                    bnoisUpdateCount += 1;
                }
                if (education.Percentage != model.Percentage)
                {
                    bnLog.PreviousValue += ", Percentage: " + education.Percentage;
                    bnLog.UpdatedValue += ", Percentage: " + model.Percentage;

                    bnoisUpdateCount += 1;
                }
                if (education.PassingYear != model.PassingYear)
                {
                    bnLog.PreviousValue += ", Passing Year: " + education.PassingYear;
                    bnLog.UpdatedValue += ", Passing Year: " + model.PassingYear;
                    bnoisUpdateCount += 1;
                }
                if (education.CourseDuration != model.CourseDuration)
                {
                    bnLog.PreviousValue += ", Course Duration (Year): " + education.CourseDuration;
                    bnLog.UpdatedValue += ", Course Duration (Year): " + model.CourseDuration;

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
                education.CreatedDate = DateTime.Now;
            }
            education.EmployeeId = model.EmployeeId;
            education.ExamCategoryId = model.ExamCategoryId;
            education.ExaminationId = model.ExaminationId;
            education.BoardId = model.BoardId;
            education.InstituteId = model.InstituteId;
            education.Subjectid = model.Subjectid;
            education.Roll = model.Roll;
            education.RegNo = model.RegNo;
            education.ResultId = model.ResultId;
            education.Gpa = model.Gpa;
            education.ResultGradeId = model.ResultGradeId;
            education.Marks = model.Marks;
            education.Distiction = model.Distiction;
            education.IsVarified = model.IsVarified;
            education.ModifiedDate = model.ModifiedDate;
            education.PassingYear = model.PassingYear;
            education.CourseDuration = model.CourseDuration ?? "N/A";
            education.OtherInstituteName = model.OtherInstituteName;
            education.OtherSubjectName = model.OtherSubjectName;
            education.ResultPublishDate = DateTime.Now;
            education.Percentage = model.Percentage;
            if (education.Gpa <= 0.0)
            {
                education.Gpa = GetAvgGpa(result.MinGPA, result.MaxGPA);
                education.ResultText = result.Name;
            }
            else
            {
                education.ResultText = education.Gpa.ToString();
            }
            //string[] dvCodes = new[] { "1", "2", "3" };
            //bool exist = !dvCodes.Contains(result.ResultCode);
            //if (exist)
            //{
            //    result = resultRepository.FindOne(x => (x.MinGPA <= education.Gpa || x.MinGPA >= education.Gpa) && dvCodes.Contains(x.ResultCode));
            //    if (result != null)
            //    {

            //        education.QResultCode = result.ResultCode;
            //    }
            //    else
            //    {
            //        education.QResultCode = "1";
            //    }
            //}
            //else
            //{
            //    education.QResultCode = result.ResultCode;
            //}
            await educationRepository.SaveAsync(education);
            model.EducationId = education.EducationId;
            return model;
        }
       

        public List<SelectModel> GetYearSelectModel()
        {
            int fromYear = PortalConfig.YearFrom;
            int toYear = DateTime.Now.Year;
            List<SelectModel> years = new List<SelectModel>();
            for (int year = fromYear; year <= toYear; year++)
            {
                SelectModel model = new SelectModel();
                model.Text = year.ToString();
                model.Value = year.ToString();
                years.Add(model);
            }
            return years.OrderByDescending(x=>x.Value).ToList();
        }

        public async Task<bool> DeleteEducation(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Education education = await educationRepository.FindOneAsync(x => x.EducationId == id);
            if (education == null)
            {
                throw new InfinityNotFoundException("Education not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Education";
                bnLog.TableEntryForm = "Employee Education";
                bnLog.PreviousValue = "Id: " + education.EducationId;

                if (education.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", education.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                
                }

                if (education.ExamCategoryId > 0)
                {
                    var prevExamCategory = employeeService.GetDynamicTableInfoById("ExamCategory", "ExamCategoryId", education.ExamCategoryId ?? 0);
                    bnLog.PreviousValue += ", Exam Title: " + ((dynamic)prevExamCategory).Name;
                }
                if (education.ExaminationId > 0)
                {
                    var prevExamination = employeeService.GetDynamicTableInfoById("Examination", "ExaminationId", education.ExaminationId ?? 0);
                    bnLog.PreviousValue += ", Examination: " + ((dynamic)prevExamination).Name;
                }
                    
                if (education.BoardId > 0)
                {
                    var prevBoard = employeeService.GetDynamicTableInfoById("Board", "BoardId", education.BoardId ?? 0);
                    bnLog.PreviousValue += ", Board/University: " + ((dynamic)prevBoard).Name;
                }
                if (education.InstituteId > 0)
                {
                    var prevInstitute = employeeService.GetDynamicTableInfoById("Institute", "InstituteId", (int)education.InstituteId);
                    bnLog.PreviousValue += ", Institute: " + ((dynamic)prevInstitute).Name;
                }
                    
                if (education.Subjectid > 0)
                {
                    var prevSubject = employeeService.GetDynamicTableInfoById("Subject", "Subjectid", education.Subjectid ?? 0);
                    bnLog.PreviousValue += ", Subject/Group: " + ((dynamic)prevSubject).Name;
                }
                bnLog.UpdatedValue += ", Roll No.: " + education.Roll + ", Registration No.: " + education.RegNo;
                if (education.ResultId > 0)
                {
                    var prevResult = employeeService.GetDynamicTableInfoById("Result", "ResultId", education.ResultId ?? 0);
                    bnLog.PreviousValue += ", Result: " + ((dynamic)prevResult).Name;
                }
                    
                bnLog.UpdatedValue += ", GPA: " + education.Gpa;

                if (education.ResultGradeId > 0)
                {
                    var prevGrade = employeeService.GetDynamicTableInfoById("ResultGrade", "ResultGradeId", education.ResultGradeId ?? 0);
                    bnLog.PreviousValue += ", Grade: " + ((dynamic)prevGrade).Name;
                }
                bnLog.UpdatedValue += ", Marks: " + education.Marks + ", Distinction/Letter: " + education.Distiction + ", Percentage: " + education.Percentage + ", Passing Year: " + education.PassingYear + ", Course Duration (Year): " + education.CourseDuration;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await educationRepository.DeleteAsync(education);
            }
        }


        public List<SelectModel> GetDurationSelectModel()
        {
            List<SelectModel> durations = new List<SelectModel>();
            for (int i = 1; i <= 5; i++)
            {
                SelectModel duration = new SelectModel();
                duration.Text = i.ToString();
                duration.Value = i.ToString();
                durations.Add(duration);
            }
            return durations;
        }

        private double GetAvgGpa(double min, double max)
        {
            return (min + min) / 2;
        }

  

        private void CheckResult(Result result, EducationModel model)
        {
            if (result.ResultCode == "4" || result.ResultCode == "5")
            {
                if (model.Gpa < result.MinGPA || model.Gpa > result.MaxGPA)
                {
                    throw new InfinityInvalidDataException("Please enter valid GPA !");
                }
            }
        }

        public async Task<EducationModel> UpdateEducation(EducationModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Education data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Education education = ObjectConverter<EducationModel, Education>.Convert(model);

            education = await educationRepository.FindOneAsync(x => x.EducationId == model.EducationId);
            if (education == null)
            {
                throw new InfinityNotFoundException("Education not found !");
            }

            if (model.FileName != null)
            {
                education.FileName = model.FileName;
            }

            education.ModifiedDate = DateTime.Now;
            education.ModifiedBy = userId;
            await educationRepository.SaveAsync(education);
            model.EducationId = education.EducationId;
            return model;
        }
    }
}
