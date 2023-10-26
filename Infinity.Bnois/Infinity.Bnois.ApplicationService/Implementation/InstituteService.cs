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
    public class InstituteService : IInstituteService
    {
        private readonly IBnoisRepository<Institute> instituteRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public InstituteService(IBnoisRepository<Institute> instituteRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.instituteRepository = instituteRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
        
        public async Task<InstituteModel> GetInstitute(int id)
        {
            if (id <= 0)
            {
                return new InstituteModel();
            }
            Institute institute = await instituteRepository.FindOneAsync(x => x.InstituteId == id);
            if (institute == null)
            {
                throw new InfinityNotFoundException("Institute not found");
            }
            InstituteModel model = ObjectConverter<Institute, InstituteModel>.Convert(institute);
            return model;
        }

        public List<InstituteModel> Institutes(int ps, int pn, string qs, out int total)
        {
            IQueryable<Institute> institutes = instituteRepository.FilterWithInclude(x => x.IsActive&& (x.Name.Contains(qs) || String.IsNullOrEmpty(qs) || (x.Board.Name.Contains(qs) || String.IsNullOrEmpty(qs))),"Board");
            total = institutes.Count();
            institutes = institutes.OrderByDescending(x => x.InstituteId).Skip((pn - 1) * ps).Take(ps);
            List<InstituteModel> models = ObjectConverter<Institute, InstituteModel>.ConvertList(institutes.ToList()).ToList();
            models = models.Select(x =>
            {
                x.BoardTypeName= Enum.GetName(typeof(BoardType), x.BoardType);
                return x;
            }).ToList();
            return models;
        }

        public async Task<InstituteModel> SaveInstitute(int id, InstituteModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Institute data missing");
            }

            bool isExistData = instituteRepository.Exists(x => x.Name == model.Name &&x.BoardId==model.BoardId && x.BoardType == model.BoardType && x.InstituteId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Institute institute = ObjectConverter<InstituteModel, Institute>.Convert(model);
            if (id > 0)
            {
                institute = await instituteRepository.FindOneAsync(x => x.InstituteId == id, new List<string>() { "Board" });
                if (institute == null)
                {
                    throw new InfinityNotFoundException("Institute not found !");
                }
                institute.ModifiedDate = DateTime.Now;
                institute.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Institute";
                bnLog.TableEntryForm = "Institute";
                bnLog.PreviousValue = "Id: " + model.InstituteId;
                bnLog.UpdatedValue = "Id: " + model.InstituteId;
                if (institute.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + institute.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (institute.BoardType != model.BoardType)
                {
                    bnLog.PreviousValue += ", BoardType: " + institute.BoardType;
                    bnLog.UpdatedValue += ", BoardType: " + model.BoardType;
                }
                if (institute.BoardId != model.BoardId)
                {
                    var sub = employeeService.GetDynamicTableInfoById("Board", "BoardId", model.BoardId);
                    bnLog.PreviousValue += ", Board: " + institute.Board.Name;
                    bnLog.UpdatedValue += ", Board: " + ((dynamic)sub).Name;
                }
                if (institute.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + institute.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (institute.Name != model.Name || institute.BoardType != model.BoardType || institute.BoardId != model.BoardId || institute.Remarks != model.Remarks)
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
                institute.IsActive = true;
                institute.CreatedDate = DateTime.Now;
                institute.CreatedBy = userId;
            }
            institute.Name = model.Name;
            institute.BoardType = model.BoardType;
            institute.Remarks = model.Remarks;
            //institute.Board = null;
            await instituteRepository.SaveAsync(institute);
            model.InstituteId = institute.InstituteId;
            return model;
        }
        public async Task<bool> DeleteInstitute(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Institute institute = await instituteRepository.FindOneAsync(x => x.InstituteId == id);
            if (institute == null)
            {
                throw new InfinityNotFoundException("Institute not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Institute";
                bnLog.TableEntryForm = "Institute";
                bnLog.PreviousValue = "Id: " + institute.InstituteId + ", Name: " + institute.Name + ", BoardType: " + institute.BoardType + ", Board: " + institute.BoardId + ", Remarks: " + institute.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await instituteRepository.DeleteAsync(institute);
            }
        }

        public List<SelectModel> getBoardTypeSelectModel()
        {
            List<SelectModel> selectModels =
                 Enum.GetValues(typeof(BoardType)).Cast<BoardType>()
                      .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                      .ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetInstitutesSelectModelByBoard(long? boardId)
        {
            IQueryable<Institute> query = instituteRepository.FilterWithInclude(x => x.IsActive && x.BoardId == boardId);
            List<Institute> institutes = await query.OrderBy(x => x.Name).ToListAsync();
            List<SelectModel> instituteSelectModels = institutes.Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.InstituteId
            }).ToList();
            return instituteSelectModels;
        }
        public async Task<List<SelectModel>> GetInstitutesSelectModels()
        {
            IQueryable<Institute> query = instituteRepository.FilterWithInclude(x => x.IsActive );
            List<Institute> institutes = await query.OrderBy(x => x.Name).ToListAsync();
            List<SelectModel> instituteSelectModels = institutes.Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.InstituteId
            }).ToList();
            return instituteSelectModels;
        }
    }
}