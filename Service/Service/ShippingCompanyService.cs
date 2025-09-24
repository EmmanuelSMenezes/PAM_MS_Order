using Domain.Model;
using Domain.Model.Request;
using Domain.Model.Response;
using Infrastructure.Repository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ShippingCompanyService : IShippingCompanyService
    {
        private readonly IShippingCompanyRepository _repository;
        private readonly ILogger _logger;
        private readonly string _privateSecretKey;
        private readonly string _tokenValidationMinutes;

        public ShippingCompanyService(IShippingCompanyRepository repository, ILogger logger, string privateSecretKey, string tokenValidationMinutes)
        {
            _repository = repository;
            _logger = logger;
            _privateSecretKey = privateSecretKey;
            _tokenValidationMinutes = tokenValidationMinutes;
        }

        public ShippingCompanyResponse Create(ShippingCompanyRequest shippingCompany)
        {
            try
            {
                var postShippingCompanyResponse = _repository.Create(shippingCompany);
                if (postShippingCompanyResponse != null)
                {
                    return postShippingCompanyResponse;
                }
                else
                {
                    throw new Exception("shippingCompanyNotCreated");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ShippingCompanyResponse Update(ShippingCompany shippingCompany)
        {
            try
            {
                string shippingCompanyId = shippingCompany.Shipping_company_id.ToString();
                var exisitingShippingCompanyResponse = _repository.GetShippingCompanyById(shippingCompanyId);
                if (exisitingShippingCompanyResponse != null)
                {
                    var postShippingCompanyResponse = _repository.Update(shippingCompany);
                    if (postShippingCompanyResponse is null)
                    {
                        return new ShippingCompanyResponse { shippingCompany = postShippingCompanyResponse, created = false };
                    }
                    else
                    {
                        return new ShippingCompanyResponse { shippingCompany = exisitingShippingCompanyResponse, created = true };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public bool Delete(List<Guid> id)
        {
            try
            {
                var response = _repository.Delete(id);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ShippingCompany> GetShippingCompanies()
        {
            try
            {
                var company = _repository.GetShippingCompanies();
                if (company.Any())
                {
                    foreach (var item in company)
                    {
                        item.Address = _repository.GetShippingCompaniesAddress(item.Address_id);
                    }
                }
                return company;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
