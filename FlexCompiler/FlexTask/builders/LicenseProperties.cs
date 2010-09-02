using System;
using System.Collections.Generic;
using System.Text;

namespace BuildTask.Flex.builders
{
    public class LicenseProperties
    {
        public string ProductName;
        public string SerialNumber;

        public LicenseProperties()
        {
        }

        public LicenseProperties(string productName, string serialNumber)
        {
            ProductName = productName;
            SerialNumber = serialNumber;
        }
    }
}
