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
    public class AddressService : IAddressService
    {
        private readonly IBnoisRepository<Address> addressRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public AddressService(IBnoisRepository<Address> addressRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.addressRepository = addressRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<AddressModel> GetAddress(int addressId)
        {
            if (addressId <= 0)
            {
                return new AddressModel();
            }
            Address address = await addressRepository.FindOneAsync(x => x.AddressId == addressId);

            if (address == null)
            {
                throw new InfinityNotFoundException("Address not found!");
            }
            AddressModel model = ObjectConverter<Address, AddressModel>.Convert(address);
            return model;
        }

        public List<AddressModel> GetAddresses(int employeeId)
        {
            List<Address> addresses = addressRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "Employee", "Division", "District", "Upazila").ToList();
            List<AddressModel> models = ObjectConverter<Address, AddressModel>.ConvertList(addresses.ToList()).ToList();
            models = models.Select(x =>
            {
                x.AddressTypeName = Enum.GetName(typeof(AddressType), x.AddressType);
                return x;
            }).ToList();
            return models;
        }

        public List<SelectModel> GetAddressTypeSelectModels()
        {
            List<SelectModel> selectModels =
              Enum.GetValues(typeof(AddressType)).Cast<AddressType>()
                   .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                   .ToList();
            return selectModels;
        }

        public async Task<AddressModel> SaveAddress(int addressId, AddressModel model)
        {
            bool isExist = await addressRepository.ExistsAsync(x => x.AddressType == model.AddressType && x.EmployeeId == model.EmployeeId && x.AddressId != model.AddressId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Address already exist !");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Address data missing");
            }
            Address address = ObjectConverter<AddressModel, Address>.Convert(model);

            if (addressId > 0)
            {
                address = await addressRepository.FindOneAsync(x => x.AddressId == addressId);
                if (address == null)
                {
                    throw new InfinityNotFoundException("Address not found!");
                }
                address.ModifiedBy = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                address.ModifiedDate = DateTime.Now;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Address";
                bnLog.TableEntryForm = "Employee Address";
                bnLog.PreviousValue = "Id: " + model.AddressId;
                bnLog.UpdatedValue = "Id: " + model.AddressId;
                int bnoisUpdateCount = 0;
                if (address.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", address.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }

                if (address.AddressType != model.AddressType)
                {
                    bnLog.PreviousValue += ", Address Type: " + (address.AddressType== 1 ? "Permanent" : "Present");
                    bnLog.UpdatedValue += ", Address Type: " + (model.AddressType == 1 ? "Permanent" : "Present");
                    bnoisUpdateCount += 1;
                }
                if (address.AddressDetailEnglish != model.AddressDetailEnglish)
                {
                    bnLog.PreviousValue += ", Address (English): " + address.AddressDetailEnglish;
                    bnLog.UpdatedValue += ", Address (English): " + model.AddressDetailEnglish;
                    bnoisUpdateCount += 1;
                }
                if (address.AddressDetailBangla != model.AddressDetailBangla)
                {
                    bnLog.PreviousValue += ", Address (Bangla): " + address.AddressDetailBangla;
                    bnLog.UpdatedValue += ", Address (Bangla): " + model.AddressDetailBangla;
                    bnoisUpdateCount += 1;
                }
                if (address.DivisionId != model.DivisionId)
                {
                    if (address.DivisionId > 0)
                    {
                        var prevDivision = employeeService.GetDynamicTableInfoById("Division", "DivisionId", address.DivisionId ?? 0);
                        bnLog.PreviousValue += ", Division: " + ((dynamic)prevDivision).Name;
                    }
                    if (model.DivisionId > 0)
                    {
                        var newDivision = employeeService.GetDynamicTableInfoById("Division", "DivisionId", model.DivisionId);
                        bnLog.UpdatedValue += ", Division: " + ((dynamic)newDivision).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (address.DistrictId != model.DistrictId)
                {
                    if (address.DistrictId > 0)
                    {
                        var prevDistrict = employeeService.GetDynamicTableInfoById("District", "DistrictId", address.DistrictId ?? 0);
                        bnLog.PreviousValue += ", District: " + ((dynamic)prevDistrict).Name;
                    }
                    if (model.DistrictId > 0)
                    {
                        var newDistrict = employeeService.GetDynamicTableInfoById("District", "DistrictId", model.DistrictId);
                        bnLog.UpdatedValue += ", District: " + ((dynamic)newDistrict).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (address.UpazilaId != model.UpazilaId)
                {
                    if (address.UpazilaId > 0)
                    {
                        var prevUpazila = employeeService.GetDynamicTableInfoById("Upazila", "UpazilaId", (int)address.UpazilaId);
                        bnLog.PreviousValue += ", Upazila: " + ((dynamic)prevUpazila).Name;
                    }
                    if (model.UpazilaId > 0)
                    {
                        var newUpazila = employeeService.GetDynamicTableInfoById("Upazila", "UpazilaId", (int)model.UpazilaId);
                        bnLog.UpdatedValue += ", Upazila: " + ((dynamic)newUpazila).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (address.PostOfficeName != model.PostOfficeName)
                {
                    bnLog.PreviousValue += ", Post Office: " + address.PostOfficeName;
                    bnLog.UpdatedValue += ", Post Office: " + model.PostOfficeName;
                    
                    bnoisUpdateCount += 1;
                }

                if (address.PostCode != model.PostCode)
                {
                    bnLog.PreviousValue += ", Post Code: " + address.PostCode;
                    bnLog.UpdatedValue += ", Post Code: " + model.PostCode;

                    bnoisUpdateCount += 1;
                }
                if (address.Phone != model.Phone)
                {
                    bnLog.PreviousValue += ", Contact No: " + address.Phone;
                    bnLog.UpdatedValue += ", Contact No: " + model.Phone;

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
                address.IsActive = true;
                address.CreatedBy = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                address.CreatedDate = DateTime.Now;
            }
            address.EmployeeId = model.EmployeeId;
            address.AddressType = model.AddressType;
            address.CareOf = model.CareOf;
            if (model.DivisionId == 0)
            {
                address.DivisionId = null;
            }
            else
            {
                address.DivisionId = model.DivisionId;
            }

            if (model.DistrictId == 0)
            {
                address.DistrictId = null;
            }
            else
            {
                address.DistrictId = model.DistrictId;
            }
            if (model.UpazilaId == 0)
            {
                address.UpazilaId = null;
            }
            else
            {
                address.UpazilaId = model.UpazilaId;
            }
            address.AddressDetailBangla = model.AddressDetailBangla;
            address.AddressDetailEnglish = model.AddressDetailEnglish;
            address.EmailAddress = model.EmailAddress;
            address.Phone = model.Phone;
            address.PostOfficeName = model.PostOfficeName;
            address.PostCode = model.PostCode;
            await addressRepository.SaveAsync(address);
            model.AddressId = address.AddressId;
            return model;
        }
    }
}
