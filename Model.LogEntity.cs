using System;
using Microsoft.Azure.Cosmos.Table;

namespace AzureDemo
{

    public enum LogType { Error, Warning, Info, Debug };

    class LogEntity : TableEntity, ICloneable {
        public static readonly string myPartition = "part-A001";
        public string LogTypeText { get; set; }   
        public int Level { get; set; }
        public string Message { get; set; }   
        public void AssignRowKey() => this.RowKey = Guid.NewGuid().ToString();
        public void AssignPartitionKey() => this.PartitionKey = myPartition;
        public static string EnumLogTypeToStr (LogType logType) 
        {
            if (logType == LogType.Error)
                return "Error";
            else if (logType == LogType.Warning)
                return "Warning";
            else if (logType == LogType.Info)
                return "Info";
            else if (logType == LogType.Debug)
                return "Debug";
            else 
                return "";
        }
        public LogEntity (LogType logType, string message, int level=0)
        {
            LogTypeText = EnumLogTypeToStr(logType);
            Message = message;
            Level = level;
            AssignRowKey();
            AssignPartitionKey();
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}