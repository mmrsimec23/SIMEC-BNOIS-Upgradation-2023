using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class TerminationTypeService : ITerminationTypeService
    {
        private readonly IBnoisRepository<TerminationType> terminationTypeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public TerminationTypeService(IBnoisRepository<TerminationType> terminationTypeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.terminationTypeRepository = terminationTypeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }


        public List<TerminationTypeModel> GetTerminationTypes(int ps, int pn, string qs, out int total)
        {
            IQueryable<TerminationType> terminationTypes = terminationTypeRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = terminationTypes.Count();
            terminationTypes = terminationTypes.OrderByDescending(x => x.TerminationTypeId).Skip((pn - 1) * ps).Take(ps);
            List<TerminationTypeModel> models = ObjectConverter<TerminationType, TerminationTypeModel>.ConvertList(terminationTypes.ToList()).ToList();
            return models;
        }

        public async Task<TerminationTypeModel> GetTerminationType(int id)
        {
            if (id <= 0)
            {
                return new TerminationTypeModel();
            }
            TerminationType terminationType = await terminationTypeRepository.FindOneAsync(x => x.TerminationTypeId == id);
            if (terminationType == null)
            {
                throw new InfinityNotFoundException("Termination Type not found");
            }
            TerminationTypeModel model = ObjectConverter<TerminationType, TerminationTypeModel>.Convert(terminationType);
            return model;
        }

        public async Task<TerminationTypeModel> SaveTerminationType(int id, TerminationTypeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Termination Type data missing");
            }
            bool isExist = terminationTypeRepository.Exists(x => x.Name == model.Name && x.TerminationTypeId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            TerminationType terminationType = ObjectConverter<TerminationTypeModel, TerminationType>.Convert(model);
            if (id > 0)
            {
                terminationType = await terminationTypeRepository.FindOneAsync(x => x.TerminationTypeId == id);
                if (terminationType == null)
                {
                    throw new InfinityNotFoundException("Termination Type not found !");
                }

                terminationType.ModifiedDate = DateTime.Now;
                terminationType.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "TerminationType";
                bnLog.TableEntryForm = "Termination Type";
                bnLog.PreviousValue = "Id: " + model.TerminationTypeId;
                bnLog.UpdatedValue = "Id: " + model.TerminationTypeId;
                int bnoisUpdateCount = 0;
                if (terminationType.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + terminationType.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (terminationType.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + terminationType.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString(); ;
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
                terminationType.IsActive = true;
                terminationType.CreatedDate = DateTime.Now;
                terminationType.CreatedBy = userId;
            }
            terminationType.Name = model.Name;
            terminationType.Remarks = model.Remarks;


            await terminationTypeRepository.SaveAsync(terminationType);
            model.TerminationTypeId = terminationType.TerminationTypeId;
            return model;
        }

        public async Task<bool> DeleteTerminationType(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            TerminationType terminationType = await terminationTypeRepository.FindOneAsync(x => x.TerminationTypeId == id);
            if (terminationType == null)
            {
                throw new InfinityNotFoundException("Termination Type not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "TerminationType";
                bnLog.TableEntryForm = "Termination Type";
                bnLog.PreviousValue = "Id: " + terminationType.TerminationTypeId + ", Name: " + terminationType.Name + ", Remarks: " + terminationType.Remarks;
                
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await terminationTypeRepository.DeleteAsync(terminationType);
            }
        }

        public async Task<List<SelectModel>> GetTerminationTypeSelectModels()
        {
            ICollection<TerminationType> models = await terminationTypeRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x=>x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.TerminationTypeId
            }).ToList();

        }


    }
}