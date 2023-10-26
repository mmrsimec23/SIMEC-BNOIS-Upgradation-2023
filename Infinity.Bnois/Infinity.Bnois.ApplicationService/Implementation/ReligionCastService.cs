using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
	public class ReligionCastService : IReligionCastService
	{
		private readonly IBnoisRepository<ReligionCast> relegionCastRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public ReligionCastService(IBnoisRepository<ReligionCast> relegionCastRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
		{
			this.relegionCastRepository = relegionCastRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
        public async Task<ReligionCastModel> GetReligionCast(int id)
        {
            if (id <= 0)
            {
                return new ReligionCastModel();
            }
            ReligionCast religionCast = await relegionCastRepository.FindOneAsync(x => x.ReligionCastId == id);
            if (religionCast == null)
            {
                throw new InfinityNotFoundException("ReligionCast not found");
            }
            ReligionCastModel model = ObjectConverter<ReligionCast, ReligionCastModel>.Convert(religionCast);
            return model;
        }

        public List<ReligionCastModel> GetReligionCasts(int ps, int pn, string qs, out int total)
        {
            IQueryable<ReligionCast> religionCasts = relegionCastRepository.FilterWithInclude(x => x.IsActive
                  && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                  (x.Religion.Name.Contains(qs) || String.IsNullOrEmpty(qs))), "Religion");
            total = religionCasts.Count();
            religionCasts = religionCasts.OrderByDescending(x => x.ReligionCastId).Skip((pn - 1) * ps).Take(ps);
            List<ReligionCastModel> models = ObjectConverter<ReligionCast, ReligionCastModel>.ConvertList(religionCasts.ToList()).ToList();
            return models;
            
        }

        public async Task<List<SelectModel>> GetReligionSelectModels()
        {
            ICollection<ReligionCast> religionCasts= await relegionCastRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = religionCasts.Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.ReligionId
            }).ToList();
            return selectModels;
        }

        public async Task<ReligionCastModel> SaveReligionCast(int id, ReligionCastModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("ReligionCast data missing");
            }

            bool isExist = relegionCastRepository.Exists(x => x.ReligionId == model.ReligionId && x.Name == model.Name && x.ReligionCastId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            ReligionCast religionCast = ObjectConverter<ReligionCastModel, ReligionCast>.Convert(model);
            if (id > 0)
            {
                religionCast = await relegionCastRepository.FindOneAsync(x => x.ReligionCastId == id, new List<string>() { "Religion" });
                if (religionCast == null)
                {
                    throw new InfinityNotFoundException("ReligionCast not found !");
                }

                religionCast.Modified = DateTime.Now;
                religionCast.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ReligionCast";
                bnLog.TableEntryForm = "Religion Cast";
                bnLog.PreviousValue = "Id: " + model.ReligionId;
                bnLog.UpdatedValue = "Id: " + model.ReligionId;
                if (religionCast.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + religionCast.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (religionCast.ReligionId != model.ReligionId)
                {
                    var sub = employeeService.GetDynamicTableInfoById("Religion", "ReligionId", model.ReligionId);
                    bnLog.PreviousValue += ", Religion: " + religionCast.Religion.Name;
                    bnLog.UpdatedValue += ", Religion: " + ((dynamic)sub).Name;
                }
                if (religionCast.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + religionCast.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (religionCast.Name != model.Name || religionCast.ReligionId != model.ReligionId || religionCast.Remarks != model.Remarks)
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
                religionCast.IsActive = true;
                religionCast.Created = DateTime.Now;
                religionCast.CreatedBy = userId;
            }
            religionCast.Name = model.Name;
            religionCast.ReligionId = model.ReligionId;
    
            religionCast.Remarks = model.Remarks;
            await relegionCastRepository.SaveAsync(religionCast);
            model.ReligionCastId = religionCast.ReligionCastId;
            return model;
        }

        public async Task<bool> DeleteReligionCast(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ReligionCast religionCast = await relegionCastRepository.FindOneAsync(x => x.ReligionCastId == id);
            if (religionCast == null)
            {
                throw new InfinityNotFoundException("ReligionCast not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ReligionCast";
                bnLog.TableEntryForm = "Religion Cast";
                bnLog.PreviousValue = "Id: " + religionCast.ReligionId + ", Name: " + religionCast.Name + ", Religion: " + religionCast.ReligionId + ", Remarks: " + religionCast.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await relegionCastRepository.DeleteAsync(religionCast);
            }
        }

        public async Task<List<SelectModel>> GetReligionCastSelectModels()
        {
            ICollection<ReligionCast> religionCasts = await relegionCastRepository.FilterAsync(x => x.IsActive);
            List<ReligionCast> query = religionCasts.OrderBy(x => x.Name).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.ReligionCastId
            }).ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetReligionCasts(int religionId)
        {
            ICollection<ReligionCast> religionCasts = await relegionCastRepository.FilterAsync(x => x.ReligionId == religionId);
            return religionCasts.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.ReligionCastId
            }).ToList();
        }

    }
}
