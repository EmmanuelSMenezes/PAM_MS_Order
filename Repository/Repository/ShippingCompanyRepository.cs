using Dapper;
using Domain.Model;
using Domain.Model.Request;
using Domain.Model.Response;
using Npgsql;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace Infrastructure.Repository
{
    public class ShippingCompanyRepository : IShippingCompanyRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public ShippingCompanyRepository(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public ShippingCompanyResponse Create(ShippingCompanyRequest shippingCompany)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    // INSERINDO NOVO ENDEREÇO DA TRANSPORTADORA
                    string sqlInsertShippingCompanyAddress = $@"INSERT INTO orders.address
                            (
                                street
                              , number
                              , complement
                              , district
                              , city
                              , state
                              , zip_code
                              , active
                              , created_at
                              , latitude
                              , longitude
                            )
                            VALUES
                            (
                                '{shippingCompany.Address.Street}'
                              , '{shippingCompany.Address.Number}'
                              , '{shippingCompany.Address.Complement}'
                              , '{shippingCompany.Address.District}'
                              , '{shippingCompany.Address.City}'
                              , '{shippingCompany.Address.State}'
                              , '{shippingCompany.Address.Zip_code}'
                              , true
                              , NOW()
                              , '{shippingCompany.Address.Latitude}'
                              , '{shippingCompany.Address.Longitude}'
                            ) RETURNING *;";

                    connection.Open();

                    var transaction = connection.BeginTransaction();

                    var insertedShippingCompanyAddress = connection.Query<Address>(sqlInsertShippingCompanyAddress).FirstOrDefault();

                    // INSERINDO NOVA TRANSPORTADORA
                    string sqlInsertShippingCompany = $@"INSERT INTO orders.shipping_company
                            (
                                company_name
                              , document
                              , address_id
                            )
                            VALUES
                            (
                                '{shippingCompany.Company_name}'
                              , '{shippingCompany.Document}'
                              , '{insertedShippingCompanyAddress.Address_id}'
                            ) RETURNING *;";

                    var insertedShippingCompany = connection.Query<ShippingCompany>(sqlInsertShippingCompany).FirstOrDefault();

                    insertedShippingCompany.Address = insertedShippingCompanyAddress;

                    if (insertedShippingCompanyAddress is null || insertedShippingCompany is null)
                    {
                        transaction.Dispose();
                        connection.Close();
                        throw new Exception("errorWhileInsertShippingCompanyOnDB");
                    }

                    transaction.Commit();
                    connection.Close();

                    return new ShippingCompanyResponse() { shippingCompany = insertedShippingCompany, created = true };
                }
            }
            catch (Exception)
            {
                throw new Exception("errorWhileInsertShippingCompanyOnDB");
            }
        }

        public ShippingCompany Update(ShippingCompany company)
        {
            try
            {
                string sqlUpdateShippingCompany = $@"
                                    UPDATE orders.shipping_company SET 
                                       company_name = '{company.Company_name}'
                                     , document = '{company.Document}'
                                    WHERE shipping_company_id = '{company.Shipping_company_id}';

                                    UPDATE orders.address SET 
                                       street = '{company.Address.Street}'
                                     , number = '{company.Address.Number}'
                                     , complement = '{company.Address.Complement}'
                                     , district = '{company.Address.District}'
                                     , city = '{company.Address.City}'
                                     , state = '{company.Address.State}'
                                     , zip_code = '{company.Address.Zip_code}'
                                     , created_at = '{company.Address.Created_at}'
                                     , updated_at = NOW()
                                     , latitude = '{company.Address.Latitude}'
                                     , longitude = '{company.Address.Longitude}'
                                    WHERE address_id = '{company.Address_id}';";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var returnedShippingCompany = connection.Query<ShippingCompany>(sqlUpdateShippingCompany).FirstOrDefault();
                    return returnedShippingCompany;
                }
            }
            catch (Exception)
            {
                throw new Exception("errorWhileUpdateShippingCompanyOnDB");
            }
        }

        public bool Delete(List<Guid> id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    foreach (var item in id)
                    {
                        // Verificar se existe o Shipping_company_id no banco de dados
                        string queryShippingCompanyId = $@"SELECT * FROM orders.shipping_company WHERE shipping_company_id = '{item}'";
                        var shippingCompanyId = connection.Query<ShippingCompany>(queryShippingCompanyId).FirstOrDefault();

                        if (shippingCompanyId != null)
                        {
                            // EXCLUINDO O ENDEREÇO
                            string sqlDeleteAddress = $@"DELETE FROM orders.address WHERE address_id = '{shippingCompanyId.Address_id}' RETURNING *";
                            connection.Execute(sqlDeleteAddress);

                            // EXCLUINDO A TRANSPORTADORA
                            string sqlDeleteShippingCompany = $@"DELETE FROM orders.shipping_company WHERE shipping_company_id = '{shippingCompanyId.Shipping_company_id}' RETURNING *";
                            connection.Execute(sqlDeleteShippingCompany);

                            transaction.Commit();
                            connection.Close();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                throw new Exception("errorWhileDeleteShippingCompanyOnDB");
            }
        }

        public List<ShippingCompany> GetShippingCompanies()
        {
            try
            {
                // Pegar todas transportadoras, sem filtro

                string sql = $@"SELECT * FROM orders.shipping_company;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var response = connection.Query<ShippingCompany>(sql).ToList();
                    if (response.Count > 0)
                    {
                        return response;
                    }
                    return new List<ShippingCompany>();
                }
            }
            catch (Exception)
            {

                throw new Exception("errorWhileListingShippingCompaniesOnDB");
            }
        }

        public ShippingCompany GetShippingCompanyById(string id)
        {
            try
            {
                string sql = $@"select * from orders.shipping_company
                                inner join orders.address
                                on orders.address.address_id = orders.shipping_company.address_id
                                WHERE orders.shipping_company.shipping_company_id = '{id}';";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var response = connection.Query<ShippingCompany>(sql).FirstOrDefault();
                    if (response is null)
                    {
                        throw new Exception("shippingCompanyNotCreated");
                    }
                    return response;
                }
            }
            catch (Exception)
            {

                throw new Exception("errorWhileListingShippingCompaniesOnDB");
            }
        }

        public Address GetShippingCompaniesAddress(Guid id)
        {
            try
            {
                string sql = $@"SELECT * FROM orders.shipping_company
                                INNER JOIN orders.address
                                ON orders.address.address_id = orders.shipping_company.address_id;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var response = connection.Query<Address>(sql).FirstOrDefault();
                    if (response != null)
                    {
                        return response;
                    }
                    return new Address();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
