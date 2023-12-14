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
    public class CourseService : ICourseService
    {
        private readonly IBnoisRepository<Course> courseRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public CourseService(IBnoisRepository<Course> courseRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.courseRepository = courseRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<CourseModel> GetCourses(int ps, int pn, string qs, out int total)
        {
            IQueryable<Course> courses = courseRepository.FilterWithInclude(x => x.IsActive
                && ((x.FullName.Contains(qs) || String.IsNullOrEmpty(qs)) || (x.ShortName.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.CourseCategory.Name.Contains(qs) || String.IsNullOrEmpty(qs)) || (x.CourseSubCategory.Name.Contains(qs) || String.IsNullOrEmpty(qs))),"Country", "CourseCategory", "CourseSubCategory");
            total = courses.Count();
            courses = courses.OrderByDescending(x => x.CourseId).Skip((pn - 1) * ps).Take(ps);
            List<CourseModel> models = ObjectConverter<Course, CourseModel>.ConvertList(courses.ToList()).ToList();
            return models;
        }

        public async Task<CourseModel> GetCourse(int id)
        {
            if (id <= 0)
            {
                return new CourseModel();
            }
            Course course = await courseRepository.FindOneAsync(x => x.CourseId == id);
            if (course == null)
            {
                throw new InfinityNotFoundException("Course not found");
            }
            CourseModel model = ObjectConverter<Course, CourseModel>.Convert(course);
            return model;
        }

        public async Task<CourseModel> SaveCourse(int id, CourseModel model)
        {
            if (model == null) 
            {
                throw new InfinityArgumentMissingException("Course  data missing");
            }

            bool isExistData = courseRepository.Exists(x => x.CourseCategoryId == model.CourseCategoryId && x.CountryId == model.CountryId && x.FullName == model.FullName  && x.CourseSubCategoryId==model.CourseSubCategoryId && x.CourseId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Course course = ObjectConverter<CourseModel, Course>.Convert(model);
            if (id > 0)
            {
                course = await courseRepository.FindOneAsync(x => x.CourseId == id);
                if (course == null)
                {
                    throw new InfinityNotFoundException("Course not found !");
                }

                course.ModifiedDate = DateTime.Now;
                course.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Course";
                bnLog.TableEntryForm = "Course";
                bnLog.PreviousValue = "Id: " + model.CourseId;
                bnLog.UpdatedValue = "Id: " + model.CourseId;
                int bnoisUpdateCount = 0;


                if (course.CourseCategoryId != model.CourseCategoryId)
                {
                    if (course.CourseCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("CourseCategory", "CourseCategoryId", course.CourseCategoryId);
                        bnLog.PreviousValue += ", Course Category: " + ((dynamic)prev).Name;
                    }
                    if (model.CourseCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("CourseCategory", "CourseCategoryId", model.CourseCategoryId);
                        bnLog.UpdatedValue += ", Course Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (course.CourseSubCategoryId != model.CourseSubCategoryId)
                {
                    if (course.CourseSubCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("CourseSubCategory", "CourseSubCategoryId", course.CourseSubCategoryId);
                        bnLog.PreviousValue += ", Course Sub Category: " + ((dynamic)prev).Name;
                    }
                    if (model.CourseSubCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("CourseSubCategory", "CourseSubCategoryId", model.CourseSubCategoryId);
                        bnLog.UpdatedValue += ", Course Sub Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (course.CountryId != model.CountryId)
                {
                    if (course.CountryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", course.CountryId??0);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId??0);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (course.FullName != model.FullName)
                {
                    bnLog.PreviousValue += ", Full Name: " + course.FullName;
                    bnLog.UpdatedValue += ", Full Name: " + model.FullName;
                    bnoisUpdateCount += 1;
                }
                if (course.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ",  Short Name: " + course.ShortName;
                    bnLog.UpdatedValue += ",  Short Name: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (course.NameInBangla != model.NameInBangla)
                {
                    bnLog.PreviousValue += ",  Name In Bangla: " + course.NameInBangla;
                    bnLog.UpdatedValue += ",  Name In Bangla: " + model.NameInBangla;
                    bnoisUpdateCount += 1;
                }
                if (course.Priority != model.Priority)
                {
                    bnLog.PreviousValue += ", Priority: " + course.Priority;
                    bnLog.UpdatedValue += ", Priority: " + model.Priority;
                    bnoisUpdateCount += 1;
                }
                if (course.ANGF != model.ANGF)
                {
                    bnLog.PreviousValue += ", ANGF: " + course.ANGF;
                    bnLog.UpdatedValue += ", ANGF: " + model.ANGF;
                    bnoisUpdateCount += 1;
                }
                if (course.SplQualification != model.SplQualification)
                {
                    bnLog.PreviousValue += ",  Spl Qualification: " + course.SplQualification;
                    bnLog.UpdatedValue += ",  Spl Qualification: " + model.SplQualification;
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
                course.IsActive = true;
                course.CreatedDate = DateTime.Now;
                course.CreatedBy = userId;
            }
            course.FullName = model.FullName;
            course.CourseCategoryId = model.CourseCategoryId;
            course.CourseSubCategoryId = model.CourseSubCategoryId;
            course.CountryId = model.CountryId;
            course.ShortName = model.ShortName;
            course.NameInBangla = model.NameInBangla;
            course.Priority = model.Priority;
            course.ANGF = model.ANGF;
            course.SplQualification = model.SplQualification;
            await courseRepository.SaveAsync(course);
            model.CourseId = course.CourseId;
            return model;
        }


        public async Task<bool> DeleteCourse(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Course course = await courseRepository.FindOneAsync(x => x.CourseId == id);
            if (course == null)
            {
                throw new InfinityNotFoundException("Course not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Course";
                bnLog.TableEntryForm = "Course";
                bnLog.PreviousValue = "Id: " + course.CourseId;
                


                if (course.CourseCategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("CourseCategory", "CourseCategoryId", course.CourseCategoryId);
                    bnLog.PreviousValue += ", Course Category: " + ((dynamic)prev).Name;
                }
                    
                if (course.CourseSubCategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("CourseSubCategory", "CourseSubCategoryId", course.CourseSubCategoryId);
                    bnLog.PreviousValue += ", Course Sub Category: " + ((dynamic)prev).Name;
                }
                if (course.CountryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", course.CountryId ?? 0);
                    bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                }
                bnLog.PreviousValue += ", Full Name: " + course.FullName + ",  Short Name: " + course.ShortName + ",  Name In Bangla: " + course.NameInBangla + ", Priority: " + course.Priority + ", ANGF: " + course.ANGF + ",  Spl Qualification: " + course.SplQualification;


                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await courseRepository.DeleteAsync(course);
            }
        }

    
        public async Task<List<SelectModel>> GetCourseSelectModels(int catId, int subCatId,int countryId)
        {
            ICollection<Course> categories = await courseRepository.FilterAsync(x => x.IsActive && x.CourseCategoryId==catId && x.CourseSubCategoryId==subCatId && x.CountryId==countryId);
            List<SelectModel> selectModels = categories.OrderBy(x => x.FullName).Select(x => new SelectModel
            {
                Text = x.FullName,
                Value = x.CourseId
            }).ToList();
            return selectModels;

        }



        public async Task<List<SelectModel>> GetCourseBySubCategory(int subCatId)
        {
            ICollection<Course> categories = await courseRepository.FilterAsync(x => x.IsActive &&  x.CourseSubCategoryId==subCatId);
            List<SelectModel> selectModels = categories.OrderBy(x => x.FullName).Select(x => new SelectModel
            {
                Text = x.FullName,
                Value = x.CourseId
            }).ToList();
            return selectModels;

        }


        public async Task<List<SelectModel>> GetCourseByCategory(int catId)
        {
            ICollection<Course> categories = await courseRepository.FilterAsync(x => x.IsActive && x.CourseCategoryId == catId);
            List<SelectModel> selectModels = categories.OrderBy(x => x.FullName).Select(x => new SelectModel
            {
                Text = x.FullName,
                Value = x.CourseId
            }).ToList();
            return selectModels;

        }


        public async Task<List<SelectModel>> GetCourseSelectModels()
        {
            ICollection<Course> categories = await courseRepository.FilterAsync(x => x.IsActive);
            List<Course> query = categories.OrderBy(x => x.CourseCategoryId).ToList();
            List<SelectModel> selectModels = categories.Select(x => new SelectModel
            {
                Text = x.FullName,
                Value = x.CourseId
            }).ToList();
            return selectModels;

        }


    }
}
