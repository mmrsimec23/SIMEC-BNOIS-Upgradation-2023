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
	public class PatternService : IPatternService
	{
		private readonly IBnoisRepository<Pattern> patternRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PatternService(IBnoisRepository<Pattern> patternRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
			this.patternRepository = patternRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

		public async Task<bool> DeletePattern(int id)
		{
			if (id < 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			Pattern pattern = await patternRepository.FindOneAsync(x => x.PatternId == id);
			if (pattern == null)
			{
				throw new InfinityNotFoundException("Pattern not found");
			}
			else
			{
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Pattern";
                bnLog.TableEntryForm = "Pattern";
                bnLog.PreviousValue = "Id: " + pattern.PatternId;
				
				bnLog.PreviousValue += ", Name: " + pattern.Name + ", Type: " + (pattern.PType == "S" ? "Sea" : pattern.PType == "L" ? "Shore" : "") + ", MoveAble: " + pattern.IsMoveAble;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await patternRepository.DeleteAsync(pattern);
			}
		}

		public async Task<PatternModel> GetPattern(int id)
		{
			if (id <= 0)
			{
				return new PatternModel();
			}
			Pattern pattern = await patternRepository.FindOneAsync(x => x.PatternId == id);
			if (pattern == null)
			{
				throw new InfinityNotFoundException("Pattern not found");
			}
			PatternModel model = ObjectConverter<Pattern, PatternModel>.Convert(pattern);
			return model;
		}

		public List<PatternModel> GetPatterns(int ps, int pn, string qs, out int total)
		{
			IQueryable<Pattern> patterns = patternRepository.FilterWithInclude(x => x.Name.Contains(qs) || String.IsNullOrEmpty(qs));
			total = patterns.Count();
			patterns = patterns.OrderByDescending(x => x.PatternId).Skip((pn - 1) * ps).Take(ps);
			List<PatternModel> models = ObjectConverter<Pattern, PatternModel>.ConvertList(patterns.ToList()).ToList();
			return models;
		}

		public async Task<PatternModel> SavePattern(int id, PatternModel model)
		{
			if (model == null)
			{
				throw new InfinityArgumentMissingException("Pattern data missing");
			}
			bool isExist = patternRepository.Exists(x => x.Name == model.Name  && x.PatternId != id);
			if (isExist)
			{
				throw new InfinityInvalidDataException("Data already exists !");
			}

			string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			Pattern pattern = ObjectConverter<PatternModel, Pattern>.Convert(model);
		    if (id > 0)
		    {
		        pattern = await patternRepository.FindOneAsync(x => x.PatternId == id);
		        if (pattern == null)
		        {
		            throw new InfinityNotFoundException("Pattern not found !");
		        }

		        pattern.ModifiedDate = DateTime.Now;
		        pattern.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Pattern";
                bnLog.TableEntryForm = "Pattern";
                bnLog.PreviousValue = "Id: " + model.PatternId;
                bnLog.UpdatedValue = "Id: " + model.PatternId;
                int bnoisUpdateCount = 0;


                if (pattern.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + pattern.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (pattern.PType != model.PType)
                {
                    bnLog.PreviousValue += ", Type: " + (pattern.PType == "S" ? "Sea": pattern.PType == "L" ? "Shore" : "");
                    bnLog.UpdatedValue += ", Type: " + (model.PType == "S" ? "Sea" : model.PType == "L" ? "Shore" : "");
                    bnoisUpdateCount += 1;
                }
                if (pattern.IsMoveAble != model.IsMoveAble)
                {
                    bnLog.PreviousValue += ", MoveAble: " + pattern.IsMoveAble;
                    bnLog.UpdatedValue += ", MoveAble: " + model.IsMoveAble;
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
		        pattern.IsActive = true;
		        pattern.CreatedDate = DateTime.Now;
		        pattern.CreatedBy = userId;
		    }

            pattern.Name = model.Name;
			pattern.PType = model.PType;
			pattern.IsMoveAble = model.IsMoveAble;		
		
			await patternRepository.SaveAsync(pattern);
			model.PatternId = pattern.PatternId;
			return model;
		}


	    public async Task<List<SelectModel>> GetPatternTypeSelectModels()
	    {
	        ICollection<Pattern> patterns = await patternRepository.FilterAsync(x=>x.IsActive);
            List<Pattern> query = patterns.OrderBy(x => x.Name).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
	        {
	            Text = x.Name,
	            Value = x.PatternId
	        }).ToList();
	        return selectModels;
	    }
    }
}
