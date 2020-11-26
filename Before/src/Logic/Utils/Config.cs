using System;
namespace Logic.Utils
{
    public class Config
    {
        public int NumberOfDatbaseRetries { get; }

        public Config(int numberOfDatabaseRetries)
        {
            NumberOfDatbaseRetries = numberOfDatabaseRetries;
        }
    }
}
