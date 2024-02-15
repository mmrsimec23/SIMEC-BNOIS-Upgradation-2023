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
    public class MinuteCandidateService : IMinuteCandidateService
    {
        private readonly IBnoisRepository<DashBoardMinuite110> minuteCandidateRepository;
        private readonly IEmployeeService employeeService;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public MinuteCandidateService(IBnoisRepository<DashBoardMinuite110> minuteCandidateRepository,
            IBnoisRepository<BnoisLog> bnoisLogRepository,
            IEmployeeService employeeService)
        {
            this.minuteCandidateRepository = minuteCandidateRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

       
        public List<DashBoardMinuite110Model> GetMinuteCandidates(int minuiteId)
        {
            IQueryable<DashBoardMinuite110> minuteCandidates = minuteCandidateRepository.FilterWithInclude(x => x.IsActive && x.MinuiteId == minuiteId, "Employee");
            minuteCandidates = minuteCandidates.OrderByDescending(x => x.MinuiteCandidateId);
            List<DashBoardMinuite110Model> models = ObjectConverter<DashBoardMinuite110, DashBoardMinuite110Model>.ConvertList(minuteCandidates.ToList()).ToList();
            return models;
        }
        public async Task<DashBoardMinuite110Model> getMinuteCadidate(int id)
        {
            if (id <= 0)
            {
                return new DashBoardMinuite110Model();
            }
            DashBoardMinuite110 minuteCandidate = await minuteCandidateRepository.FindOneAsync(x => x.MinuiteCandidateId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (minuteCandidate == null)
            {
                throw new InfinityNotFoundException("Minute Candidate not found");
            }
            DashBoardMinuite110Model model = ObjectConverter<DashBoardMinuite110, DashBoardMinuite110Model>.Convert(minuteCandidate);
            return model;
        }
        public async Task<DashBoardMinuite110Model> SaveMinuteCadidate(int id, DashBoardMinuite110Model model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Minute Candidate Officer data missing");
            }

            bool isExistData = minuteCandidateRepository.Exists(x => x.MinuiteId == model.MinuiteId && x.EmployeeId == model.EmployeeId && x.MinuiteCandidateId != model.MinuiteCandidateId);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            DashBoardMinuite110 minuteCadidate = ObjectConverter<DashBoardMinuite110Model, DashBoardMinuite110>.Convert(model);

            if (model.MinuiteCandidateId > 0)
            {
                minuteCadidate = minuteCandidateRepository.FindOne(x => x.MinuiteCandidateId == model.MinuiteCandidateId);
                if (minuteCadidate == null)
                {
                    throw new InfinityNotFoundException("Employee Car Loan not found !");
                }

                minuteCadidate.ModifiedDate = DateTime.Now;
                minuteCadidate.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                int isUpdateData = 0;
                bnLog.TableName = "DashBoardMinuite110";
                bnLog.TableEntryForm = "Minute Candidate";
                bnLog.PreviousValue = "Id: " + model.MinuiteCandidateId;
                bnLog.UpdatedValue = "Id: " + model.MinuiteCandidateId;
                if (minuteCadidate.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", minuteCadidate.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                if (minuteCadidate.MinuiteId != model.MinuiteId)
                {
                    isUpdateData = isUpdateData + 1;
                    if (minuteCadidate.MinuiteId > 0)
                    {
                        var prevrank = employeeService.GetDynamicTableInfoById("DashBoardMinuite100", "MinuiteId", minuteCadidate.MinuiteId);
                        bnLog.PreviousValue += ", Minute Name: " + ((dynamic)prevrank).MinuiteName;
                    }
                    if (model.MinuiteId > 0)
                    {
                        var rank = employeeService.GetDynamicTableInfoById("DashBoardMinuite100", "MinuiteId", model.MinuiteId);
                        bnLog.UpdatedValue += ", Minute Name: " + ((dynamic)rank).MinuiteName;
                    }
                }
                if (minuteCadidate.ProposedBillet != model.ProposedBillet)
                {
                    isUpdateData = isUpdateData + 1;
                    bnLog.PreviousValue += ", Remarks: " + minuteCadidate.ProposedBillet;
                    bnLog.UpdatedValue += ", Remarks: " + model.ProposedBillet;
                }
                if (minuteCadidate.Remarks7 != model.Remarks7)
                {
                    isUpdateData = isUpdateData + 1;
                    bnLog.PreviousValue += ", Position: " + minuteCadidate.Remarks7;
                    bnLog.UpdatedValue += ", Position: " + model.Remarks7;
                }
                if (minuteCadidate.Remarks2 != model.Remarks2)
                {
                    isUpdateData = isUpdateData + 1;
                    bnLog.PreviousValue += ", Remarks: " + minuteCadidate.Remarks2;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks2;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (isUpdateData > 0)
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
                minuteCadidate.IsActive = true;
                minuteCadidate.CreatedDate = DateTime.Now;
                minuteCadidate.CreatedBy = userId;
            }

            minuteCadidate.MinuiteId = model.MinuiteId;
            minuteCadidate.ProposedBillet = model.ProposedBillet;
            minuteCadidate.Remarks1 = model.Remarks1;
            minuteCadidate.Remarks2 = model.Remarks2;
            minuteCadidate.Remarks3 = model.Remarks3;
            minuteCadidate.Remarks4 = model.Remarks4;
            minuteCadidate.Remarks5 = model.Remarks5;
            minuteCadidate.Remarks6 = model.Remarks6;
            minuteCadidate.Remarks7 = model.Remarks7;
            minuteCadidate.EmployeeId = model.EmployeeId;
            minuteCadidate.Employee = null;
            try
            {
                await minuteCandidateRepository.SaveAsync(minuteCadidate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //return model;
            
            model.MinuiteCandidateId = minuteCadidate.MinuiteCandidateId;
            return model;
        }

        public async Task<bool> DeleteMinuteCadidate(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            DashBoardMinuite110 cadidate = await minuteCandidateRepository.FindOneAsync(x => x.MinuiteCandidateId == id);
            if (cadidate == null)
            {
                throw new InfinityNotFoundException("Proposal Cadidate not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeTrace";
                bnLog.TableEntryForm = "Officer Car Loan";
                bnLog.PreviousValue = "Id: " + cadidate.MinuiteCandidateId;
                
                if (cadidate.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", cadidate.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                }
                if (cadidate.MinuiteId > 0)
                {
                    var prevrank = employeeService.GetDynamicTableInfoById("DashBoardMinuite100", "MinuiteId", cadidate.MinuiteId);
                    bnLog.PreviousValue += ", Minute Name: " + ((dynamic)prevrank).MinuiteName;
                }                    

                bnLog.PreviousValue += ", Remarks: " + cadidate.ProposedBillet;
                bnLog.PreviousValue += ", Position: " + cadidate.Remarks7;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                //return await _employeeTraceRepository.DeleteAsync(employeeTrace);
                return await minuteCandidateRepository.DeleteAsync(cadidate);
            }
        }
    }
}
