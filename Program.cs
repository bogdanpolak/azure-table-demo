using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;

namespace AzureDemo
{
    class Program
    {
        private List<LogEntity> BuildSampleData()
        {
            return new List<LogEntity>
            {
                new LogEntity(LogType.Error,"Wykryto błąd (komunikat 1)"),
                new LogEntity(LogType.Info,"Drugi komunikat z treścią - to tylko informacja",1),
                new LogEntity(LogType.Debug,"Third message is dedicated for developers",1),
                new LogEntity(LogType.Warning,"[4] Looks like somethin bad happens, but application is still working",2),
                new LogEntity(LogType.Info,"Fifth message with level 99",99)
            };
        }

        private List<LogEntity> BuildNewDataForAppend()
        {
            return new List<LogEntity>
            {
                new LogEntity(LogType.Info,"New message with processing info - just info",10),
                new LogEntity(LogType.Debug,"Append Debugging message for developers",10),
                new LogEntity(LogType.Info,"Again just info message",10)
            };
        }

        private void ResetTable(CloudTable table) 
        {
            if (table.Exists())
            {
                table.Delete();
                Console.WriteLine("Previous table was deleted");
            }
            table.CreateIfNotExists();
            Console.WriteLine("New table was created");
        }

        private void InsertData(CloudTable table, List<LogEntity> data) 
        {
            foreach (LogEntity entity in data) {
                table.Execute(TableOperation.Insert(entity));
            }
            Console.WriteLine("Inserted {0:D} rows",data.Count);
        }

        private void UpdateOneRow (CloudTable table, string rowKey)
        {
            TableResult result = table.Execute(
                TableOperation.Retrieve<LogEntity>(LogEntity.myPartition, rowKey));
            if ( result.HttpStatusCode == 200 ) {
                var entity = (LogEntity)result.Result;
                entity.Level = 0;
                table.Execute (
                    TableOperation.Replace(entity));
                Console.WriteLine("Updated entity level to 0 for entity rowKey = {0}", rowKey);
            }
            else
                Console.WriteLine("[Error] Entity not found (can't update level) rowKey = {0}", rowKey);
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

            ResetTable(table);
            InsertData(table, BuildSampleData());
            InsertData(table, BuildNewDataForAppend());
            UpdateOneRow (table, "7b16217a-4046-4c30-81cf-cf46325d5d42");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Program started ...");
            new Program().Execute();
            Console.WriteLine("Job Done");
        }
    }
}
