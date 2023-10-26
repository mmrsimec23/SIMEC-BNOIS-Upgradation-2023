using System;
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
	public class ZoneService : IZoneService
	{
		private readonly IBnoisRepository<Zone> zoneRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ZoneService(IBnoisRepository<Zone> zoneRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
		{
			this.zoneRepository = zoneRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }
        public async Task<ZoneModel> GetZone(int id)
        {
            if (id <= 0)
            {
                return new ZoneModel();
            }
            Zone zone = await zoneRepository.FindOneAsync(x => x.ZoneId == id);
            if (zone == null)
            {
                throw new InfinityNotFoundException("Zone not found");
            }
            ZoneModel model = ObjectConverter<Zone, ZoneModel>.Convert(zone);
            return model;
        }

        public List<ZoneModel> GetZones(int ps, int pn, string qs, out int total)
        {
            IQueryable<Zone> zones = zoneRepository.FilterWithInclude(x => x.IsActive
                  && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs))));
            total = zones.Count();
	        zones = zones.OrderByDescending(x => x.ZoneId).Skip((pn - 1) * ps).Take(ps);
            List<ZoneModel> models = ObjectConverter<Zone, ZoneModel>.ConvertList(zones.ToList()).ToList();
            return models;
            
        }



        public async Task<ZoneModel> SaveZone(int id, ZoneModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Zone data missing");
            }

            bool isExist = zoneRepository.Exists(x =>  x.Name == model.Name && x.ZoneId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Zone zone = ObjectConverter<ZoneModel, Zone>.Convert(model);
            if (id > 0)
            {
	            zone = await zoneRepository.FindOneAsync(x => x.ZoneId == id);
                if (zone == null)
                {
                    throw new InfinityNotFoundException("Zone not found !");
                }

	            zone.ModifiedDate = DateTime.Now;
	            zone.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Zone";
                bnLog.TableEntryForm = "Zone";
                bnLog.PreviousValue = "Id: " + model.ZoneId;
                bnLog.UpdatedValue = "Id: " + model.ZoneId;
                if (zone.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + zone.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (zone.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + zone.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (zone.Name != model.Name || zone.Remarks != model.Remarks)
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
	            zone.IsActive = true;
	            zone.CreatedDate = DateTime.Now;
	            zone.CreatedBy = userId;
            }
	        zone.Name = model.Name;
	        zone.Remarks = model.Remarks;
            await zoneRepository.SaveAsync(zone);
            model.ZoneId = zone.ZoneId;
            return model;
        }

        public async Task<bool> DeleteZone(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Zone Zone = await zoneRepository.FindOneAsync(x => x.ZoneId == id);
            if (Zone == null)
            {
                throw new InfinityNotFoundException("Zone not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Zone";
                bnLog.TableEntryForm = "Zone";
                bnLog.PreviousValue = "Id: " + Zone.ZoneId + ", Name: " + Zone.Name + ", Remarks: " + Zone.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await zoneRepository.DeleteAsync(Zone);
            }
        }

        public async Task<List<SelectModel>> GetZoneSelectModels()
        {
            ICollection<Zone> Zones = await zoneRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = Zones.OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.ZoneId
            }).ToList();
            return selectModels;
        }


    }
}
