using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class SuitabilityTestService : ISuitabilityTestService
    {
        private readonly IBnoisRepository<SuitabilityTest> SuitabilityTestRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeRepository;
        public SuitabilityTestService(IBnoisRepository<SuitabilityTest> SuitabilityTestRepository, IBnoisRepository<EmployeeGeneral> employeeRepository)
        {
            this.SuitabilityTestRepository = SuitabilityTestRepository;
            this.employeeRepository = employeeRepository;
        }

        public List<SuitabilityTestModel> GetSuitabilityTests(int type, int ps, int pn, string qs, out int total)
        {
            IQueryable<SuitabilityTest> SuitabilityTests = SuitabilityTestRepository.FilterWithInclude(x => x.IsActive && (type == 0 || x.SuitabilityTestType == type)
                 && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee");
            total = SuitabilityTests.Count();
            SuitabilityTests = SuitabilityTests.OrderByDescending(x => x.SuitabilityTestId).Skip((pn - 1) * ps).Take(ps);
            List<SuitabilityTestModel> models = ObjectConverter<SuitabilityTest, SuitabilityTestModel>.ConvertList(SuitabilityTests.ToList()).ToList();
            return models;
        }

        public async Task<SuitabilityTestModel> GetSuitabilityTest(int id)
        {
            if (id <= 0)
            {
                return new SuitabilityTestModel();
            }
            SuitabilityTest SuitabilityTest = await SuitabilityTestRepository.FindOneAsync(x => x.SuitabilityTestId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (SuitabilityTest == null)
            {
                throw new InfinityNotFoundException("Employee PFT not found");
            }
            SuitabilityTestModel model = ObjectConverter<SuitabilityTest, SuitabilityTestModel>.Convert(SuitabilityTest);
            return model;
        }



        public async Task<SuitabilityTestModel> SaveSuitabilityTest(int id, SuitabilityTestModel model)
        {
            model.Employee = null;
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Course Forecast data missing");
            }
            EmployeeGeneral employeeData = await employeeRepository.FindOneAsync(x => x.Employee.EmployeeId == model.EmployeeId);
            if (model.SuitabilityTestType == 1 && (employeeData.BranchId != 2 || employeeData.BranchId != 7))
            {
                throw new InfinityNotFoundException("Employee Branch Does not Matched!!!");
            }
            if (model.SuitabilityTestType == 2 && employeeData.BranchId != 1)
            {
                throw new InfinityNotFoundException("Employee Branch Does not Matched!!!");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            SuitabilityTest SuitabilityTest = ObjectConverter<SuitabilityTestModel, SuitabilityTest>.Convert(model);
            if (id > 0)
            {
                SuitabilityTest = await SuitabilityTestRepository.FindOneAsync(x => x.SuitabilityTestId == id);
                if (SuitabilityTest == null)
                {
                    throw new InfinityNotFoundException("Course Forecast not found !");
                }

                SuitabilityTest.ModifiedDate = DateTime.Now;
                SuitabilityTest.ModifiedBy = userId;
            }
            else
            {
                SuitabilityTest.IsActive = true;
                SuitabilityTest.CreatedDate = DateTime.Now;
                SuitabilityTest.CreatedBy = userId;
            }
            SuitabilityTest.EmployeeId = model.EmployeeId??0;
            SuitabilityTest.SuitabilityTestType = model.SuitabilityTestType;
            SuitabilityTest.Remarks = model.Remarks;
            SuitabilityTest.ExpiryDate = model.ExpiryDate;
            SuitabilityTest.Status = model.Status;



            await SuitabilityTestRepository.SaveAsync(SuitabilityTest);
            model.SuitabilityTestId = SuitabilityTest.SuitabilityTestId;
            return model;
        }


        public async Task<bool> DeleteSuitabilityTest(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            SuitabilityTest SuitabilityTest = await SuitabilityTestRepository.FindOneAsync(x => x.SuitabilityTestId == id);
            if (SuitabilityTest == null)
            {
                throw new InfinityNotFoundException("Employee PFT not found");
            }
            else
            {
                return await SuitabilityTestRepository.DeleteAsync(SuitabilityTest);
            }
        }


        public async Task<bool> DeleteSuitabilityTestTypeList(int type)
        {
            if (type < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            List<SuitabilityTest> suitabilityTests =  SuitabilityTestRepository.FilterWithInclude(x => x.SuitabilityTestType == type).ToList();
            if (suitabilityTests == null)
            {
                throw new InfinityNotFoundException("Employee PFT not found");
            }
            else
            {
                foreach (var test in suitabilityTests)
                {
                    SuitabilityTest data = await SuitabilityTestRepository.FindOneAsync(x => x.SuitabilityTestId == test.SuitabilityTestId);
                    await SuitabilityTestRepository.DeleteAsync(data);
                }
                return true;
            }
        }

        public async Task<bool> SaveSuitabilityTestTypeList(int type, int batchId)
        {
            if (type < 0 || batchId < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            List<EmployeeGeneral> employeeList =  employeeRepository.FilterWithInclude(x => x.Employee.BatchId == batchId && x.Employee.EmployeeStatusId == 1, "Employee").ToList();
            if(type == 1)
            {
                employeeList = employeeList.Where(x => x.BranchId == 2 || x.BranchId == 7).ToList();

                foreach (var test in employeeList)
                {

                    SuitabilityTest suitabilityTest = new SuitabilityTest();

                    suitabilityTest.EmployeeId = test.EmployeeId;
                    suitabilityTest.SuitabilityTestType = type;

                    suitabilityTest.CreatedDate = DateTime.Now;
                    suitabilityTest.CreatedBy = userId;
                    suitabilityTest.IsActive = true;

                    await SuitabilityTestRepository.SaveAsync(suitabilityTest);

                }
                return true;
            }
            else if (type == 2)
            {
                employeeList = employeeList.Where(x => x.BranchId == 1).ToList();
                foreach (var test in employeeList)
                {

                    SuitabilityTest suitabilityTest = new SuitabilityTest();

                    suitabilityTest.EmployeeId = test.EmployeeId;
                    suitabilityTest.SuitabilityTestType = type;

                    suitabilityTest.CreatedDate = DateTime.Now;
                    suitabilityTest.CreatedBy = userId;

                    suitabilityTest.IsActive = true;
                    await SuitabilityTestRepository.SaveAsync(suitabilityTest);

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<SelectModel> GetMajorSuitabilityTestTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(SuitabilityTestType)).Cast<SuitabilityTestType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }


        public List<object> GetSuitabilityTestListByType(int type)
        {
            DataTable dataTable = SuitabilityTestRepository.ExecWithSqlQuery(String.Format("exec [spGetSuitabalityTestByOfficerList] {0} ", type));

            return dataTable.ToJson().ToList();
        }

        //public List<SelectModel> GetCoxoAppoinmentSelectModels(int type)
        //{
        //    if (type == 1 || type == 3)
        //    {
        //        List<SelectModel> selectModels = Enum.GetValues(typeof(CoXoAppointment)).Cast<CoXoAppointment>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) }).ToList();
        //        return selectModels;
        //    }
        //    if (type == 2)
        //    {
        //        List<SelectModel> selectModels = Enum.GetValues(typeof(EoLoSoAppointment)).Cast<EoLoSoAppointment>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) }).ToList();
        //        return selectModels;
        //    }
        //    return null;
        //}


    }
}