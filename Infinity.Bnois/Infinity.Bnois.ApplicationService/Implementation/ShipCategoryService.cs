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
    public class ShipCategoryService : IShipCategoryService
    {
        private readonly IBnoisRepository<ShipCategory> shipCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ShipCategoryService(IBnoisRepository<ShipCategory> shipCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.shipCategoryRepository = shipCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

    
        public List<ShipCategoryModel> GetShipCategories(int ps, int pn, string qs, out int total)
        {
            IQueryable<ShipCategory> shipCategories = shipCategoryRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = shipCategories.Count();
	        shipCategories = shipCategories.OrderByDescending(x => x.ShipCategoryId).Skip((pn - 1) * ps).Take(ps);
            List<ShipCategoryModel> models = ObjectConverter<ShipCategory, ShipCategoryModel>.ConvertList(shipCategories.ToList()).ToList();
            return models;
        }
        public async Task<ShipCategoryModel> GetShipCategory(int id)
        {
            if (id <= 0)
            {
                return new ShipCategoryModel();
            }
            ShipCategory shipCategory = await shipCategoryRepository.FindOneAsync(x => x.ShipCategoryId == id);
            if (shipCategory == null)
            {
                throw new InfinityNotFoundException("Ship Category not found");
            }
            ShipCategoryModel model = ObjectConverter<ShipCategory, ShipCategoryModel>.Convert(shipCategory);
            return model;
        }

        public async Task<ShipCategoryModel> SaveShipCategory(int id, ShipCategoryModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Ship Category data missing");
            }
            bool isExist = shipCategoryRepository.Exists(x => x.Name == model.Name && x.ShipCategoryId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            ShipCategory shipCategory = ObjectConverter<ShipCategoryModel, ShipCategory>.Convert(model);
            if (id > 0)
            {
	            shipCategory = await shipCategoryRepository.FindOneAsync(x => x.ShipCategoryId == id);
                if (shipCategory == null)
                {
                    throw new InfinityNotFoundException("Ship Category not found !");
                }

	            shipCategory.ModifiedDate = DateTime.Now;
	            shipCategory.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ShipCategory";
                bnLog.TableEntryForm = "Ship Category";
                bnLog.PreviousValue = "Id: " + model.ShipCategoryId;
                bnLog.UpdatedValue = "Id: " + model.ShipCategoryId;
                int bnoisUpdateCount = 0;


                if (shipCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + shipCategory.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (shipCategory.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", Short Name: " + shipCategory.ShortName;
                    bnLog.UpdatedValue += ", Short Name: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (shipCategory.Priority != model.Priority)
                {
                    bnLog.PreviousValue += ", Priority: " + shipCategory.Priority;
                    bnLog.UpdatedValue += ", Priority: " + model.Priority;
                    bnoisUpdateCount += 1;
                }
                if (shipCategory.Purpose != model.Purpose)
                {
                    bnLog.PreviousValue += ", Purpose: " + shipCategory.Purpose;
                    bnLog.UpdatedValue += ", Purpose: " + model.Purpose;
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
	            shipCategory.IsActive = true;
	            shipCategory.CreatedDate = DateTime.Now;
	            shipCategory.CreatedBy = userId;
            }
	        shipCategory.Name = model.Name;
	        shipCategory.Priority = model.Priority;
	        shipCategory.ShortName = model.ShortName;
	        shipCategory.Purpose = model.Purpose;
        
            await shipCategoryRepository.SaveAsync(shipCategory);
            model.ShipCategoryId = shipCategory.ShipCategoryId;
            return model;
        }

        public async Task<bool> DeleteShipCategory(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ShipCategory shipCategory = await shipCategoryRepository.FindOneAsync(x => x.ShipCategoryId == id);
            if (shipCategory == null)
            {
                throw new InfinityNotFoundException("Ship Category not found");
            }
            else
            {

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ShipCategory";
                bnLog.TableEntryForm = "Ship Category";
                bnLog.PreviousValue = "Id: " + shipCategory.ShipCategoryId;


                bnLog.PreviousValue += ", Name: " + shipCategory.Name + ", Short Name: " + shipCategory.ShortName + ", Priority: " + shipCategory.Priority + ", Purpose: " + shipCategory.Purpose;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);
                //data log section end
                return await shipCategoryRepository.DeleteAsync(shipCategory);
            }
        }

        public async Task<List<SelectModel>> GetShipCategorySelectModels()
        {
            ICollection<ShipCategory> shipCategories = await shipCategoryRepository.FilterAsync(x => x.IsActive);
            return shipCategories.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.ShipCategoryId
            }).ToList();
        }
    }
}
