using Infinity.Bnois;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.ApplicationService.Interface;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class CountryService : ICountryService
    {
        private readonly IBnoisRepository<Country> countryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public CountryService(IBnoisRepository<Country> countryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.countryRepository = countryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<CountryModel> GetCountries(int pageSize, int pageNumber, string queryText, out int total)
        {

            IQueryable<Country> countries = countryRepository
                .FilterWithInclude(x => x.IsActive
                && ((x.FullName.Contains(queryText) || string.IsNullOrEmpty(queryText)) ||
                (x.ShortName.Contains(queryText) || string.IsNullOrEmpty(queryText)) ||
                 x.Iso.Contains(queryText) || string.IsNullOrEmpty(queryText)));
            total = countries.Count();
            countries = countries.OrderBy(x => x.FullName).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<CountryModel> models = ObjectConverter<Country, CountryModel>.ConvertList(countries.ToList()).ToList();
            return models;
        }

        public async Task<CountryModel> GetCountry(int id)
        {
            if (id == 0)
            {
                return new CountryModel();
            }
            Country country = await countryRepository.FindOneAsync(x => x.CountryId == id);
            if (country == null)
            {
                throw new InfinityNotFoundException("Country not found");
            }
            CountryModel model = ObjectConverter<Country, CountryModel>.Convert(country);
            return model;
        }

        public async Task<CountryModel> SaveCountry(int id, CountryModel model)
        {
            bool isExist = await countryRepository.ExistsAsync(x => x.FullName == model.FullName && x.ShortName == model.ShortName && x.CountryId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Country data already exist !");
            }

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Country data missing");
            }
            Country country = ObjectConverter<CountryModel, Country>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

            if (id > 0)
            {
                country = await countryRepository.FindOneAsync(x => x.CountryId == id);
                if (country == null)
                {
                    throw new InfinityNotFoundException("Country not found !");
                }


                country.ModifiedDate = DateTime.Now;
                country.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Country";
                bnLog.TableEntryForm = "Country";
                bnLog.PreviousValue = "Id: " + model.CountryId;
                bnLog.UpdatedValue = "Id: " + model.CountryId;
                if (country.FullName != model.FullName)
                {
                    bnLog.PreviousValue += ", FullName: " + country.FullName;
                    bnLog.UpdatedValue += ", FullName: " + model.FullName;
                }
                if (country.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", ShortName: " + country.ShortName;
                    bnLog.UpdatedValue += ", ShortName: " + model.ShortName;
                }
                if (country.Iso != model.Iso)
                {
                    bnLog.PreviousValue += ", IsoCode: " + country.Iso;
                    bnLog.UpdatedValue += ", IsoCode: " + model.Iso;
                }
                if (country.Iso3 != model.Iso3)
                {
                    bnLog.PreviousValue += ", Iso3: " + country.Iso3;
                    bnLog.UpdatedValue += ", Iso3: " + model.Iso3;
                }
                if (country.Nationality != model.Nationality)
                {
                    bnLog.PreviousValue += ", Nationality: " + country.Nationality;
                    bnLog.UpdatedValue += ", Nationality: " + model.Nationality;
                }
                if (country.NumCode != model.NumCode)
                {
                    bnLog.PreviousValue += ", NumberCode: " + country.NumCode;
                    bnLog.UpdatedValue += ", NumberCode: " + model.NumCode;
                }
                if (country.PhoneCode != model.PhoneCode)
                {
                    bnLog.PreviousValue += ", PhoneCode: " + country.PhoneCode;
                    bnLog.UpdatedValue += ", PhoneCode: " + model.PhoneCode;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (country.FullName != model.FullName || country.ShortName != model.ShortName || country.Iso != model.Iso || country.Iso3 != model.Iso3 || country.Nationality != model.Nationality || country.NumCode != model.NumCode || country.PhoneCode != model.PhoneCode)
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
                country.CreatedDate = DateTime.Now;
                country.CreatedBy = userId;
                country.IsActive = true;
            }
            country.FullName = model.FullName;
            country.ShortName = model.ShortName;
            country.Iso = model.Iso;
            country.Nationality = model.Nationality;
            country.Iso3 = model.Iso3;
            country.NumCode = model.NumCode;
            country.ShortName = model.ShortName;
            country.PhoneCode = model.PhoneCode;

            await countryRepository.SaveAsync(country);
            model.CountryId = country.CountryId;
            return model;
        }
        public async Task<bool> DeleteCountry(int id)
        {

            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Country country = await countryRepository.FindOneAsync(x => x.CountryId == id);
            if (country == null)
            {
                throw new InfinityNotFoundException("Country not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Country";
                bnLog.TableEntryForm = "Country";
                bnLog.PreviousValue = "Id: " + country.CountryId + ", FullName: " + country.FullName + ", ShortName: " + country.ShortName + ", IsoCode: " + country.Iso + ", Iso3: " + country.Iso3 + ", Nationality: " + country.Nationality + ", NumCode: " + country.NumCode + ", PhoneCode: " + country.PhoneCode;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await countryRepository.DeleteAsync(country);
            }
        }


        public async Task<List<SelectModel>> GetCountrySelectModels()
        {
            ICollection<Country> countries = await countryRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = countries.ToList().Select(x => new SelectModel()
            {
                Text = x.FullName + " [" + x.Iso + "]",
                Value = x.CountryId
            }).ToList();

            return selectModels;
        }

		public async Task<List<SelectModel>> GetCountriesTypeSelectModel()
		{
			ICollection<Country> countryType = await countryRepository.FilterAsync(x => x.IsActive);
			List<SelectModel> selectModels = countryType.Select(x => new SelectModel
			{
				Text = x.FullName,
				Value = x.CountryId
			}).ToList();
			return selectModels;
		}
	}
}
