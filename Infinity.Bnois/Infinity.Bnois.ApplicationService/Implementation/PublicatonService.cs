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
   public class PublicationService: IPublicationService
    {

        private readonly IBnoisRepository<Publication> publicationRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PublicationService(IBnoisRepository<Publication> publicationRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.publicationRepository = publicationRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<PublicationModel> GetPublications(int ps, int pn, string qs, out int total)
        {
            IQueryable<Publication> publications = publicationRepository.FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.PublicationCategory.Name.Contains(qs) || String.IsNullOrEmpty(qs))), "PublicationCategory");
            total = publications.Count();
	        publications = publications.OrderByDescending(x => x.PublicationId).Skip((pn - 1) * ps).Take(ps);
            List<PublicationModel> models = ObjectConverter<Publication, PublicationModel>.ConvertList(publications.ToList()).ToList();
            return models;
        }

        public async Task<PublicationModel> GetPublication(int id)
        {
            if (id <= 0)
            {
                return new PublicationModel();
            }
            Publication publication = await publicationRepository.FindOneAsync(x => x.PublicationId == id);
            if (publication == null)
            {
                throw new InfinityNotFoundException("Publication not found");
            }
            PublicationModel model = ObjectConverter<Publication, PublicationModel>.Convert(publication);
            return model;
        }

    
        public async Task<PublicationModel> SavePublication(int id, PublicationModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Publication data missing");
            }

            bool isExistData = publicationRepository.Exists(x => x.PublicationCategoryId == model.PublicationCategoryId && x.Name == model.Name && x.PublicationId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Publication publication = ObjectConverter<PublicationModel, Publication>.Convert(model);
            if (id > 0)
            {
	            publication = await publicationRepository.FindOneAsync(x => x.PublicationId == id);
                if (publication == null)
                {
                    throw new InfinityNotFoundException("Publication not found !");
                }

	            publication.ModifiedDate = DateTime.Now;
	            publication.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Publication";
                bnLog.TableEntryForm = "Publication";
                bnLog.PreviousValue = "Id: " + model.PublicationId;
                bnLog.UpdatedValue = "Id: " + model.PublicationId;
                if (publication.PublicationCategoryId != model.PublicationCategoryId)
                {
                    if (publication.PublicationCategoryId > 0)
                    {
                        var prevPublicationCategory = employeeService.GetDynamicTableInfoById("PublicationCategory", "PublicationCategoryId", publication.PublicationCategoryId);
                        bnLog.PreviousValue += ", Publication Category: " + ((dynamic)prevPublicationCategory).Name;
                    }
                    if (model.PublicationCategoryId > 0)
                    {
                        var newPublicationCategory = employeeService.GetDynamicTableInfoById("PublicationCategory", "PublicationCategoryId", model.PublicationCategoryId);
                        bnLog.UpdatedValue += ", PublicationCategory: " + ((dynamic)newPublicationCategory).Name;
                    }
                }
                if (publication.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + publication.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (publication.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", Short Name: " + publication.ShortName;
                    bnLog.UpdatedValue += ", Short Name: " + model.ShortName;
                }
                

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (publication.PublicationCategoryId != model.PublicationCategoryId || publication.Name != model.Name || publication.ShortName != model.ShortName)
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
	            publication.IsActive = true;
	            publication.CreatedDate = DateTime.Now;
	            publication.CreatedBy = userId;
            }
	        publication.Name = model.Name;
	        publication.ShortName = model.ShortName;
	        publication.PublicationCategoryId = model.PublicationCategoryId;
            await publicationRepository.SaveAsync(publication);
            model.PublicationId = publication.PublicationId;
            return model;
        }


        public async Task<bool> DeletePublication(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Publication publication = await publicationRepository.FindOneAsync(x => x.PublicationId == id);
            if (publication == null)
            {
                throw new InfinityNotFoundException("Publication not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Publication";
                bnLog.TableEntryForm = "Publication";

                var prevPublicationCategory = employeeService.GetDynamicTableInfoById("PublicationCategory", "PublicationCategoryId", publication.PublicationCategoryId);
                
                bnLog.PreviousValue = "Id: " + publication.PublicationId + ", Publication Category: " + ((dynamic)prevPublicationCategory).Name + ", Name: " + publication.Name + ", ShortName: " + publication.ShortName;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);
                return await publicationRepository.DeleteAsync(publication);
            }
        }


        public async Task<List<SelectModel>> GetPublicationSelectModelsByPublicationCategory(int id)
        {
            ICollection<Publication> publications = await publicationRepository.FilterAsync(x => x.IsActive && x.PublicationCategoryId == id);
            List<SelectModel> selectModels = publications.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.PublicationId
            }).ToList();
            return selectModels;

        }

        public async Task<List<SelectModel>> GetPublicationSelectModels()
        {
            ICollection<Publication> publications = await publicationRepository.FilterAsync(x => x.IsActive );
            List<SelectModel> selectModels = publications.OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.PublicationId
            }).ToList();
            return selectModels;

        }

    }
}
