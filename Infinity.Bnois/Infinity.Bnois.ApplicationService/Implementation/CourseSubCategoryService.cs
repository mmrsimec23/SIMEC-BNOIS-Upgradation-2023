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
    public class CourseSubCategoryService : ICourseSubCategoryService
    {
        private readonly IBnoisRepository<CourseSubCategory> courseSubCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public CourseSubCategoryService(IBnoisRepository<CourseSubCategory> courseSubCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.courseSubCategoryRepository = courseSubCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<CourseSubCategoryModel> GetCourseSubCategories(int ps, int pn, string qs, out int total)
        {
            IQueryable<CourseSubCategory> courseSubCategories = courseSubCategoryRepository.FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) || (x.ShortName.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.CourseCategory.Name.Contains(qs) || String.IsNullOrEmpty(qs))), "CourseCategory");
            total = courseSubCategories.Count();
            courseSubCategories = courseSubCategories.OrderByDescending(x => x.CourseSubCategoryId).Skip((pn - 1) * ps).Take(ps);
            List<CourseSubCategoryModel> models = ObjectConverter<CourseSubCategory, CourseSubCategoryModel>.ConvertList(courseSubCategories.ToList()).ToList();
            return models;
        }

        public async Task<CourseSubCategoryModel> GetCourseSubCategory(int id)
        {
            if (id <= 0)
            {
                return new CourseSubCategoryModel();
            }
            CourseSubCategory courseSubCategory = await courseSubCategoryRepository.FindOneAsync(x => x.CourseSubCategoryId == id);
            if (courseSubCategory == null)
            {
                throw new InfinityNotFoundException("Course Sub Category not found");
            }
            CourseSubCategoryModel model = ObjectConverter<CourseSubCategory, CourseSubCategoryModel>.Convert(courseSubCategory);
            return model;
        }

        public async Task<CourseSubCategoryModel> SaveCourseSubCategory(int id, CourseSubCategoryModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Course Sub Category data missing");
            }

            bool isExistData = courseSubCategoryRepository.Exists(x => x.CourseCategoryId == model.CourseCategoryId && x.Name == model.Name && x.CourseSubCategoryId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            CourseSubCategory courseSubCategory = ObjectConverter<CourseSubCategoryModel, CourseSubCategory>.Convert(model);
            if (id > 0)
            {
                courseSubCategory = await courseSubCategoryRepository.FindOneAsync(x => x.CourseSubCategoryId == id);
                if (courseSubCategory == null)
                {
                    throw new InfinityNotFoundException("Course Sub Category not found !");
                }

                courseSubCategory.ModifiedDate = DateTime.Now;
                courseSubCategory.ModifiedBy = userId;



                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "CourseSubCategory";
                bnLog.TableEntryForm = "Course Sub Category";
                bnLog.PreviousValue = "Id: " + model.CourseCategoryId;
                bnLog.UpdatedValue = "Id: " + model.CourseCategoryId;
                int bnoisUpdateCount = 0;


                if (courseSubCategory.CourseCategoryId != model.CourseCategoryId)
                {
                    if (courseSubCategory.CourseCategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("CourseCategory", "CourseCategoryId", courseSubCategory.CourseCategoryId);
                        bnLog.PreviousValue += ", Course Category: " + ((dynamic)prev).Name;
                    }
                    if (model.CourseCategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("CourseCategory", "CourseCategoryId", model.CourseCategoryId);
                        bnLog.UpdatedValue += ", Course Category: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Full Name: " + courseSubCategory.Name;
                    bnLog.UpdatedValue += ", Full Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.NameBan != model.NameBan)
                {
                    bnLog.PreviousValue += ", Full Name (বাংলা): " + courseSubCategory.NameBan;
                    bnLog.UpdatedValue += ", Full Name (বাংলা): " + model.NameBan;
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ",  Short Name: " + courseSubCategory.ShortName;
                    bnLog.UpdatedValue += ",  Short Name: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.ShortNameBan != model.ShortNameBan)
                {
                    bnLog.PreviousValue += ",  Short Name (বাংলা): " + courseSubCategory.ShortNameBan;
                    bnLog.UpdatedValue += ",  Short Name (বাংলা): " + model.ShortNameBan;
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.Priority != model.Priority)
                {
                    bnLog.PreviousValue += ", Priority: " + courseSubCategory.Priority;
                    bnLog.UpdatedValue += ", Priority: " + model.Priority;
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.ANmCon != model.ANmCon)
                {
                    bnLog.PreviousValue += ", ANmCon: " + courseSubCategory.ANmCon;
                    bnLog.UpdatedValue += ", ANmCon: " + model.ANmCon;
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.NmRGF != model.NmRGF)
                {
                    bnLog.PreviousValue += ", NmRGF: " + courseSubCategory.NmRGF;
                    bnLog.UpdatedValue += ", NmRGF: " + model.NmRGF;
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.BnListPriority != model.BnListPriority)
                {
                    bnLog.PreviousValue += ", BN List Priority: " + courseSubCategory.BnListPriority;
                    bnLog.UpdatedValue += ", BN List Priority: " + model.BnListPriority;
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + courseSubCategory.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }
                if (courseSubCategory.Trace != model.Trace)
                {
                    bnLog.PreviousValue += ", Trace: " + courseSubCategory.Trace;
                    bnLog.UpdatedValue += ", Trace: " + model.Trace;
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
                courseSubCategory.IsActive = true;
                courseSubCategory.CreatedDate = DateTime.Now;
                courseSubCategory.CreatedBy = userId;
            }
            courseSubCategory.Name = model.Name;
            courseSubCategory.NameBan = model.NameBan;
            courseSubCategory.CourseCategoryId = model.CourseCategoryId;
            courseSubCategory.ShortName = model.ShortName;
            courseSubCategory.ShortNameBan = model.ShortNameBan;
            courseSubCategory.Priority = model.Priority;
            courseSubCategory.BnListPriority = model.BnListPriority;
            courseSubCategory.Trace = model.Trace;
            courseSubCategory.ANmCon = model.ANmCon;
            courseSubCategory.NmRGF = model.NmRGF;
            
            courseSubCategory.Remarks = model.Remarks;
            await courseSubCategoryRepository.SaveAsync(courseSubCategory);
            model.CourseSubCategoryId = courseSubCategory.CourseSubCategoryId;
            return model;
        }


        public async Task<bool> DeleteCourseSubCategory(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            CourseSubCategory courseSubCategory = await courseSubCategoryRepository.FindOneAsync(x => x.CourseSubCategoryId == id);
            if (courseSubCategory == null)
            {
                throw new InfinityNotFoundException("Course Sub Category not found");
            }
            else
            {

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "CourseSubCategory";
                bnLog.TableEntryForm = "Course Sub Category";
                bnLog.PreviousValue = "Id: " + courseSubCategory.CourseCategoryId;

                if (courseSubCategory.CourseCategoryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("CourseCategory", "CourseCategoryId", courseSubCategory.CourseCategoryId);
                    bnLog.PreviousValue += ", Course Category: " + ((dynamic)prev).Name;
                }
                bnLog.PreviousValue += ", Full Name: " + courseSubCategory.Name + ", Full Name (বাংলা): " + courseSubCategory.NameBan + ",  Short Name: " + courseSubCategory.ShortName + ",  Short Name (বাংলা): " + courseSubCategory.ShortNameBan + ", Priority: " + courseSubCategory.Priority + ", ANmCon: " + courseSubCategory.ANmCon + ", NmRGF: " + courseSubCategory.NmRGF + ", BN List Priority: " + courseSubCategory.BnListPriority + ", Remarks: " + courseSubCategory.Remarks + ", Trace: " + courseSubCategory.Trace;

                bnLog.UpdatedValue = "This Record has been Deleted!";


                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await courseSubCategoryRepository.DeleteAsync(courseSubCategory);
            }
        }



        public async Task<List<SelectModel>> GetCourseSubCategorySelectModels(int id)
        {
            ICollection<CourseSubCategory> categories = await courseSubCategoryRepository.FilterAsync(x => x.IsActive && x.CourseCategoryId==id);
            List<SelectModel> selectModels = categories.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.CourseSubCategoryId
            }).ToList();
            return selectModels;

        }
        public async Task<List<SelectModel>> GetCourseSubCategorySelectModels()
        {
            ICollection<CourseSubCategory> categories = await courseSubCategoryRepository.FilterAsync(x => x.IsActive );
            List<CourseSubCategory> query = categories.OrderBy(x => x.CourseCategoryId).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.CourseSubCategoryId
            }).ToList();
            return selectModels;

        }
    }
}
