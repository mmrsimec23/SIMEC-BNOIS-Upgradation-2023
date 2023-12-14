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
    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly IBnoisRepository<CourseCategory> courseCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public CourseCategoryService(IBnoisRepository<CourseCategory> courseCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.courseCategoryRepository = courseCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }
       

        public List<CourseCategoryModel> GetCourseCategories(int ps, int pn, string qs, out int total)
        {
            IQueryable<CourseCategory> categories = courseCategoryRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) || (x.ShortName.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = categories.Count();
            categories = categories.OrderByDescending(x => x.CourseCategoryId).Skip((pn - 1) * ps).Take(ps);
            List<CourseCategoryModel> models = ObjectConverter<CourseCategory, CourseCategoryModel>.ConvertList(categories.ToList()).ToList();
            return models;
        }

        public async Task<CourseCategoryModel> GetCourseCategory(int id)
        {
            if (id <= 0)
            {
                return new CourseCategoryModel();
            }
           CourseCategory courseCategory = await courseCategoryRepository.FindOneAsync(x => x.CourseCategoryId == id);
            if (courseCategory == null)
            {
                throw new InfinityNotFoundException("Course Category not found");
            }
            CourseCategoryModel model = ObjectConverter<CourseCategory, CourseCategoryModel>.Convert(courseCategory);
            return model;
        }

        public async Task<CourseCategoryModel> SaveCourseCategory(int id, CourseCategoryModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Course Category data missing");
            }
            bool isExist = courseCategoryRepository.Exists(x => x.Name == model.Name && x.ShortName==model.ShortName && x.CourseCategoryId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            CourseCategory courseCategory = ObjectConverter<CourseCategoryModel, CourseCategory>.Convert(model);
            if (id > 0)
            {
                courseCategory = await courseCategoryRepository.FindOneAsync(x => x.CourseCategoryId == id);
                if (courseCategory == null)
                {
                    throw new InfinityNotFoundException("Course Category not found !");
                }

                courseCategory.ModifiedDate = DateTime.Now;
                courseCategory.ModifiedBy = userId;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "CourseCategory";
                bnLog.TableEntryForm = "Course Category";
                bnLog.PreviousValue = "Id: " + model.CourseCategoryId;
                bnLog.UpdatedValue = "Id: " + model.CourseCategoryId;
                int bnoisUpdateCount = 0;


                if (courseCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Full Name: " + courseCategory.Name;
                    bnLog.UpdatedValue += ", Full Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.NameBan != model.NameBan)
                {
                    bnLog.PreviousValue += ", Full Name (বাংলা): " + courseCategory.NameBan;
                    bnLog.UpdatedValue += ", Full Name (বাংলা): " + model.NameBan;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ",  Short Name: " + courseCategory.ShortName;
                    bnLog.UpdatedValue += ",  Short Name: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.ShortNameBan != model.ShortNameBan)
                {
                    bnLog.PreviousValue += ",  Short Name (বাংলা): " + courseCategory.ShortNameBan;
                    bnLog.UpdatedValue += ",  Short Name (বাংলা): " + model.ShortNameBan;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.Priority != model.Priority)
                {
                    bnLog.PreviousValue += ", Priority: " + courseCategory.Priority;
                    bnLog.UpdatedValue += ", Priority: " + model.Priority;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.Trace != model.Trace)
                {
                    bnLog.PreviousValue += ", Trace: " + courseCategory.Trace;
                    bnLog.UpdatedValue += ", Trace: " + model.Trace;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.SASB != model.SASB)
                {
                    bnLog.PreviousValue += ", SASB: " + courseCategory.SASB;
                    bnLog.UpdatedValue += ", SASB: " + model.SASB;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.PromotionBoard != model.PromotionBoard)
                {
                    bnLog.PreviousValue += ", Go To Promotion Board: " + courseCategory.PromotionBoard;
                    bnLog.UpdatedValue += ", Go To Promotion Board: " + model.PromotionBoard;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.BnList != model.BnList)
                {
                    bnLog.PreviousValue += ", BN List: " + courseCategory.BnList;
                    bnLog.UpdatedValue += ", BN List: " + model.BnList;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.BnListPriority != model.BnListPriority)
                {
                    bnLog.PreviousValue += ", BN List Priority: " + courseCategory.BnListPriority;
                    bnLog.UpdatedValue += ", BN List Priority: " + model.BnListPriority;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.TransferProposal != model.TransferProposal)
                {
                    bnLog.PreviousValue += ", Transfer Proposal: " + courseCategory.TransferProposal;
                    bnLog.UpdatedValue += ", Transfer Proposal: " + model.TransferProposal;
                    bnoisUpdateCount += 1;
                }
                if (courseCategory.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + courseCategory.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
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
                courseCategory.IsActive = true;
                courseCategory.CreatedDate = DateTime.Now;
                courseCategory.CreatedBy = userId;
            }
            courseCategory.Name = model.Name;
            courseCategory.NameBan = model.NameBan;
            courseCategory.ShortName = model.ShortName;
            courseCategory.ShortNameBan = model.ShortNameBan;
            courseCategory.Priority = model.Priority;
            courseCategory.Trace = model.Trace;
            courseCategory.SASB = model.SASB;
            courseCategory.Remarks = model.Remarks;
            courseCategory.PromotionBoard = model.PromotionBoard;
            courseCategory.BnList = model.BnList;
            courseCategory.BnListPriority = model.BnListPriority;
            courseCategory.TransferProposal = model.TransferProposal;
            await courseCategoryRepository.SaveAsync(courseCategory);
            return model;
        }

        public async Task<bool> DeleteCourseCategory(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            CourseCategory courseCategory = await courseCategoryRepository.FindOneAsync(x => x.CourseCategoryId == id);
            if (courseCategory == null)
            {
                throw new InfinityNotFoundException("Course Category not found");
            }
            else
            {

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "CourseCategory";
                bnLog.TableEntryForm = "Course Category";
                bnLog.PreviousValue = "Id: " + courseCategory.CourseCategoryId;
                
                bnLog.PreviousValue += ", Full Name: " + courseCategory.Name + ", Full Name (বাংলা): " + courseCategory.NameBan + ",  Short Name: " + courseCategory.ShortName + ",  Short Name (বাংলা): " + courseCategory.ShortNameBan + ", Priority: " + courseCategory.Priority + ", Trace: " + courseCategory.Trace + ", SASB: " + courseCategory.SASB + ", Go To Promotion Board: " + courseCategory.PromotionBoard + ", BN List: " + courseCategory.BnList + ", BN List Priority: " + courseCategory.BnListPriority + ", Transfer Proposal: " + courseCategory.TransferProposal + ", Remarks: " + courseCategory.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await courseCategoryRepository.DeleteAsync(courseCategory);
            }
        }

        public async Task<List<SelectModel>> GetCourseCategorySelectModels()
        {
            ICollection<CourseCategory> categories = await courseCategoryRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = categories.OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.CourseCategoryId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetCourseCategorySelectModelsByTrace()
        {
            ICollection<CourseCategory> categories = await courseCategoryRepository.FilterAsync(x => x.IsActive && x.Trace);
            List<SelectModel> selectModels = categories.OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.CourseCategoryId
            }).ToList();
            return selectModels;
        }
    }
}
