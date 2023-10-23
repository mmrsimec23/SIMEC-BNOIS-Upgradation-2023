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
    public class MscInstituteService : IMscInstituteService
    {
        private readonly IBnoisRepository<MscInstitute> mscInstituteRepository;
        public MscInstituteService(IBnoisRepository<MscInstitute> mscInstituteRepository)
        {
            this.mscInstituteRepository = mscInstituteRepository;
        }
        
        public async Task<MscInstituteModel> GetMscInstitute(int id)
        {
            if (id <= 0)
            {
                return new MscInstituteModel();
            }
            MscInstitute MscInstitute = await mscInstituteRepository.FindOneAsync(x => x.MscInstituteId == id);
            if (MscInstitute == null)
            {
                throw new InfinityNotFoundException("Msc Institute not found");
            }
            MscInstituteModel model = ObjectConverter<MscInstitute, MscInstituteModel>.Convert(MscInstitute);
            return model;
        }

        public List<MscInstituteModel> GetMscInstitutes(int ps, int pn, string qs, out int total)
        {
            IQueryable<MscInstitute> mscInstitutes = mscInstituteRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = mscInstitutes.Count();
            mscInstitutes = mscInstitutes.OrderByDescending(x => x.MscInstituteId).Skip((pn - 1) * ps).Take(ps);
            List<MscInstituteModel> models = ObjectConverter<MscInstitute, MscInstituteModel>.ConvertList(mscInstitutes.ToList()).ToList();
            return models;
        }

        public async Task<MscInstituteModel> SaveMscInstitute(int id, MscInstituteModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Msc Institute data missing");
            }
            bool isExist = mscInstituteRepository.Exists(x => x.Name == model.Name && x.MscInstituteId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            MscInstitute MscInstitute = ObjectConverter<MscInstituteModel, MscInstitute>.Convert(model);
            if (id > 0)
            {
                MscInstitute = await mscInstituteRepository.FindOneAsync(x => x.MscInstituteId == id);
                if (MscInstitute == null)
                {
                    throw new InfinityNotFoundException("Msc Permission Type not found !");
                }

                MscInstitute.ModifiedDate = DateTime.Now;
               MscInstitute.ModifiedBy = userId;
            }
            else
            {
                MscInstitute.IsActive = true;
                MscInstitute.CreatedDate = DateTime.Now;
                MscInstitute.CreatedBy = userId;
            }
            MscInstitute.Name = model.Name;
            MscInstitute.Remarks = model.Remarks;


            await mscInstituteRepository.SaveAsync(MscInstitute);
            model.MscInstituteId = MscInstitute.MscInstituteId;
            return model;
        }
        public async Task<bool> DeleteMscInstitute(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            MscInstitute MscInstitute = await mscInstituteRepository.FindOneAsync(x => x.MscInstituteId == id);
            if (MscInstitute == null)
            {
                throw new InfinityNotFoundException("Msc Permission Types not found");
            }
            else
            {
                return await mscInstituteRepository.DeleteAsync(MscInstitute);
            }
        }

        public async Task<List<SelectModel>> GetMscInstitutesSelectModels()
        {
            ICollection<MscInstitute> MscInstitutes = await mscInstituteRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = MscInstitutes.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.MscInstituteId
            }).ToList();
            return selectModels;
        }
    }
}
