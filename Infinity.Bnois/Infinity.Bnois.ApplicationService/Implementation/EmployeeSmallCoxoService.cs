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
    public class EmployeeSmallCoxoService : IEmployeeSmallCoxoService
    {
        private readonly IBnoisRepository<CoXoService> EmployeeSmallCoxoServiceRepository;
        public EmployeeSmallCoxoService(IBnoisRepository<CoXoService> EmployeeSmallCoxoServiceRepository)
        {
            this.EmployeeSmallCoxoServiceRepository = EmployeeSmallCoxoServiceRepository;
        }

        public List<EmployeeCoxoServiceModel> GetEmployeeSmallCoxoServices(int type, int ps, int pn, string qs, out int total)
        {
            IQueryable<CoXoService> EmployeeCoxoServices = EmployeeSmallCoxoServiceRepository.FilterWithInclude(x => x.IsActive && x.Type == type
                 && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "Office");
            total = EmployeeCoxoServices.Count();
            EmployeeCoxoServices = EmployeeCoxoServices.OrderByDescending(x => x.CoXoServiceId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeCoxoServiceModel> models = ObjectConverter<CoXoService, EmployeeCoxoServiceModel>.ConvertList(EmployeeCoxoServices.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeCoxoServiceModel> GetEmployeeSmallCoxoService(int id)
        {
            if (id <= 0)
            {
                return new EmployeeCoxoServiceModel();
            }
            CoXoService EmployeeCoxoService = await EmployeeSmallCoxoServiceRepository.FindOneAsync(x => x.CoXoServiceId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (EmployeeCoxoService == null)
            {
                throw new InfinityNotFoundException("Employee PFT not found");
            }
            EmployeeCoxoServiceModel model = ObjectConverter<CoXoService, EmployeeCoxoServiceModel>.Convert(EmployeeCoxoService);
            return model;
        }



        public async Task<EmployeeCoxoServiceModel> SaveEmployeeSmallCoxoService(int id, EmployeeCoxoServiceModel model)
        {
            model.Employee = null;
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Coxo  data missing");
            }
            bool isExistData = EmployeeSmallCoxoServiceRepository.Exists(x => x.EmployeeId == model.EmployeeId && x.OfficeId == model.OfficeId && x.Type == model.Type && x.CoXoServiceId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            CoXoService EmployeeCoxoService = ObjectConverter<EmployeeCoxoServiceModel, CoXoService>.Convert(model);
            if (id > 0)
            {
                EmployeeCoxoService = await EmployeeSmallCoxoServiceRepository.FindOneAsync(x => x.CoXoServiceId == id);
                if (EmployeeCoxoService == null)
                {
                    throw new InfinityNotFoundException("Employee Coxo not found !");
                }

                EmployeeCoxoService.ModifiedDate = DateTime.Now;
                EmployeeCoxoService.ModifiedBy = userId;
            }
            else
            {
                EmployeeCoxoService.IsActive = true;
                EmployeeCoxoService.CreatedDate = DateTime.Now;
                EmployeeCoxoService.CreatedBy = userId;
            }
            EmployeeCoxoService.EmployeeId = model.EmployeeId;
            EmployeeCoxoService.OfficeId = model.OfficeId;
            EmployeeCoxoService.Type = model.Type;
            EmployeeCoxoService.Remarks = model.Remarks;
            EmployeeCoxoService.Appointment = model.Appointment;
            EmployeeCoxoService.ShipType = model.ShipType;
            EmployeeCoxoService.ProposedDate = model.ProposedDate;
            EmployeeCoxoService.CompleteStatus = model.CompleteStatus;



            await EmployeeSmallCoxoServiceRepository.SaveAsync(EmployeeCoxoService);
            model.CoXoServiceId = EmployeeCoxoService.CoXoServiceId;
            return model;
        }


        public async Task<bool> DeleteEmployeeSmallCoxoService(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            CoXoService EmployeeCoxoService = await EmployeeSmallCoxoServiceRepository.FindOneAsync(x => x.CoXoServiceId == id);
            if (EmployeeCoxoService == null)
            {
                throw new InfinityNotFoundException("Employee PFT not found");
            }
            else
            {
                return await EmployeeSmallCoxoServiceRepository.DeleteAsync(EmployeeCoxoService);
            }
        }

        public List<SelectModel> GetSmallCoxoTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(CoXoType)).Cast<CoXoType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public List<SelectModel> GetSmallCoxoAppoinmentSelectModels(int type)
        {
            if (type == 1 || type == 3)
            {
                List<SelectModel> selectModels = Enum.GetValues(typeof(CoXoAppointment)).Cast<CoXoAppointment>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) }).ToList();
                return selectModels;
            }
            if (type == 2)
            {
                List<SelectModel> selectModels = Enum.GetValues(typeof(EoLoSoAppointment)).Cast<EoLoSoAppointment>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) }).ToList();
                return selectModels;
            }
            return null;
        }


    }
}