using Domain.Model;
using Domain.Model.Request;
using Domain.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public interface IShippingCompanyRepository
    {
        ShippingCompanyResponse Create(ShippingCompanyRequest shippingCompany);
        ShippingCompany Update(ShippingCompany company);
        bool Delete(List<Guid> id);
        List<ShippingCompany> GetShippingCompanies();
        Address GetShippingCompaniesAddress(Guid id);
        ShippingCompany GetShippingCompanyById(string id);
    }
}
