using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AbpEfConsoleApp.Entities;
using Castle.Core.Logging;

namespace AbpEfConsoleApp
{
    //Entry class of the test. It uses constructor-injection to get a repository and property-injection to get a Logger.
    public class Tester : ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IRepository<ConnectionString,long> _connectionstringRepository;
        private readonly IRepository<TableReference, long> _tableReferenceRepository;
        private readonly IRepository<Analytics, long> _analyticsRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public Tester(IRepository<ConnectionString, long> connectionstringRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TableReference, long> tableReferenceRepository,
            IRepository<Analytics, long> analyticsRepository)
        {
            _connectionstringRepository = connectionstringRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _tableReferenceRepository = tableReferenceRepository;
            _analyticsRepository = analyticsRepository;


            Logger = NullLogger.Instance;
        }

        public void Run()
        {
            Logger.Debug("Started Tester.Run()");

            
        }


        public void StartSharding( string dbname,DateTime from,DateTime to)
        {
            if (!string.IsNullOrEmpty(dbname))
            {
                Console.WriteLine("Starting the sharding operation for database :"+dbname);
                string connectionString = string.Empty;

                using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        var conStrRecord= _connectionstringRepository.GetAll().Where(x => x.DatabaseName.ToLower() == dbname.ToLower()).FirstOrDefault();
                        if (conStrRecord!=null && !string.IsNullOrEmpty(conStrRecord.ConnectionStringValue))
                        {
                            connectionString = conStrRecord.ConnectionStringValue;
                            var columnsDetails= 

                        }
                        else
                        {
                            Console.WriteLine("No connection string found for the dbname :" +dbname);
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Exception happens while getting connection string from db" + ex.Message);
                    }
                    uow.Complete();
                }



            }
            else
            {
                Console.WriteLine("Dbname is empty.Aborting the sharding operation");
            }

        }

        private DataTable GetRecords(string connectionString, string query)
        {
            int recCount = 0;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet recDataSet = new DataSet();
            DataTable recDataTable = new DataTable();
            try
            {
                Console.WriteLine("Starting to get records");
                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    SqlCommand recordsCmd = new SqlCommand(query, connection);
                    recordsCmd.CommandTimeout = 300;
                    connection.Open();
                    da.Fill(recDataSet,"RevenueTable");
                    recDataTable = recDataSet.Tables["RevenueTable"];
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception happend while getting the records : "+ex.Message);
            }
            return recDataTable;
        }


    }
}