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
	public class ReligionService : IReligionService
	{
		private readonly IBnoisRepository<Religion> religionRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ReligionService(IBnoisRepository<Religion> relegionRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
		{
			this.religionRepository = relegionRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

		public async Task<bool> DeleteReligion(int id)
		{
			if (id <= 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			Religion relegion = await religionRepository.FindOneAsync(x => x.ReligionId == id);
			if (relegion == null)
			{
				throw new InfinityNotFoundException("Religion not found");
			}
			else
			{
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Religion";
                bnLog.TableEntryForm = "Religion";
                bnLog.PreviousValue = "Id: " + relegion.ReligionId + ", Name: " + relegion.Name + ", Remarks: " + relegion.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await religionRepository.DeleteAsync(relegion);
			}
		}

		public async Task<ReligionModel> GetReligion(int id)
		{
			if (id <= 0)
			{
				return new ReligionModel();
			}
			Religion relegion = await religionRepository.FindOneAsync(x => x.ReligionId == id);
			if (relegion == null)
			{
				throw new InfinityNotFoundException("Relegion not found");
			}
			ReligionModel model = ObjectConverter<Religion, ReligionModel>.Convert(relegion);
			return model;
		}

		public List<ReligionModel> GetReligions(int ps, int pn, string qs, out int total)
		{
			IQueryable<Religion> relegions = religionRepository.FilterWithInclude(x => x.IsActive == true
	  && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs))));

			total = relegions.Count();
			relegions = relegions.OrderByDescending(x => x.ReligionId).Skip((pn - 1) * ps).Take(ps);
			List<ReligionModel> models = ObjectConverter<Religion, ReligionModel>.ConvertList(relegions.ToList()).ToList();
			return models;
		}

       

        public async Task<ReligionModel> SaveReligion(int id, ReligionModel model)
		{
			string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			if (model == null)
			{
				throw new InfinityArgumentMissingException("Religion data missing");
			}
			Religion religion = ObjectConverter<ReligionModel, Religion>.Convert(model);
			if (id > 0)
			{
				religion = await religionRepository.FindOneAsync(x => x.ReligionId == id);
				if (religion == null)
				{
					throw new InfinityNotFoundException("Religion not found !");
				}

			    religion.Modified = DateTime.Now;
			    religion.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Religion";
                bnLog.TableEntryForm = "Religion";
                bnLog.PreviousValue = "Id: " + model.ReligionId;
                bnLog.UpdatedValue = "Id: " + model.ReligionId;
                if (religion.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + religion.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (religion.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + religion.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (religion.Name != model.Name || religion.Remarks != model.Remarks)
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
				religion.Created = DateTime.Now;
				religion.CreatedBy = userId;
			}
			religion.Name = model.Name;
			religion.Remarks = model.Remarks;
		
			religion.IsActive = true;
			await religionRepository.SaveAsync(religion);
			model.ReligionId = religion.ReligionId;
			return model;
		}

        public async Task<List<SelectModel>> GetReligionSelectModels()
        {
            ICollection<Religion> nationalities = await religionRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = nationalities.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.ReligionId
            }).ToList();
            return selectModels;
        }
    }
}
