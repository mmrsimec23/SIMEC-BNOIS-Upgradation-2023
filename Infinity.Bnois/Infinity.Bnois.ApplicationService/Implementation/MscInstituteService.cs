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
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public MscInstituteService(IBnoisRepository<MscInstitute> mscInstituteRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.mscInstituteRepository = mscInstituteRepository;
            this.bnoisLogRepository = bnoisLogRepository;
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
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MscInstitute";
                bnLog.TableEntryForm = "Msc Institute";
                bnLog.PreviousValue = "Id: " + model.MscInstituteId;
                bnLog.UpdatedValue = "Id: " + model.MscInstituteId;
                if (MscInstitute.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + MscInstitute.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (MscInstitute.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + MscInstitute.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (MscInstitute.Name != model.Name || MscInstitute.Remarks != model.Remarks)
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
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MscInstitute";
                bnLog.TableEntryForm = "Msc Institute";
                bnLog.PreviousValue = "Id: " + MscInstitute.MscInstituteId + ", Name: " + MscInstitute.Name + ", Remarks: " + MscInstitute.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
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
