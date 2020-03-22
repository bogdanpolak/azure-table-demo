using System;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;

namespace AzureDemo
{
    class LogEntity : TableEntity {
        public int Level { get; set; }
        public string Message { get; set; }   
        public void AssignRowKey() => this.RowKey = Guid.NewGuid().ToString();
        public void AssignPartitionKey() => this.PartitionKey = "part-A001";
        public LogEntity (int level, string message)
        {
            Level = level;
            Message = message;
            AssignRowKey();
            AssignPartitionKey();
        }
    }
    class Program
    {
        private void InsertData(CloudTable table) 
        {
            var entitiesList = new List<LogEntity>
            {
                new LogEntity(1,"Komunikat pierwszy"),
                new LogEntity(2,"drugi komunikat z treścią"),
                new LogEntity(2,"Komunikat trzeci"),
                new LogEntity(1,"czwarty i trochę dłuższy komunikat")
            };
            
            foreach (var entity in entitiesList) {
                table.Execute(TableOperation.Insert(entity));
            }
            Console.WriteLine("Inserted {0:N} rows",entitiesList.Count);
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
