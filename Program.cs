using System;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;

namespace AzureDemo
{
     class Program
    {
        private void InsertData(CloudTable table) 
        {
            var entitiesList = new List<LogEntity>
            {
                new LogEntity(LogType.Error,"Wykryto błąd (komunikat 1)"),
                new LogEntity(LogType.Info,"Drugi komunikat z treścią - to tylko informacja",1),
                new LogEntity(LogType.Debug,"Third message is dedicated for developers",1),
                new LogEntity(LogType.Warning,"[4] Looks like somethin bad happens, but application is still working",2)
            };
            
            foreach (var entity in entitiesList) {
                table.Execute(TableOperation.Insert(entity));
            }
            Console.WriteLine("Inserted {0:D} rows",entitiesList.Count);
        }
        public void Execute() 
        {
            var connectionString = "DefaultEndpointsProtocol=https;" +
                "AccountName=" + ConfigurationManager.AppSettings["AzureAccountName"] + ";" +
                "AccountKey=" + ConfigurationManager.AppSettings["AzureAccountKey"] + ";" +
                "TableEndpoint=" + ConfigurationManager.AppSettings["AzureTableEndpoint"] + ";";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            
            CloudTable table = tableClient.GetTableReference("SensorLog");

            InsertData(table);
        }

        static void Main(string[] args)
        {
            new Program().Execute();
            Console.WriteLine("Job Done");
        }
    }
}
