using Domain.Model.Request;
using Domain.Model.Response;
using System.Collections.Generic;
using System;
using Domain.Model;

namespace Application.Service
{
    public interface IShippingCompanyService
    {
        ShippingCompanyResponse Create(ShippingCompanyRequest shippingCompany);
        ShippingCompanyResponse Update(ShippingCompany shippingCompany);
        bool Delete(List<Guid> id);
        List<ShippingCompany> GetShippingCompanies();
    }
}
