using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class Device
    {
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public string Group { get; set; }
        public string MeasurementUnit { get; set; }
        public double MeasuredValue { get; set; }

        public Device()
        {
            Name = string.Empty;
            Timestamp = DateTime.Now;
            Group = string.Empty;
            MeasurementUnit = string.Empty;
            MeasuredValue = 0;
        }

        public Device(string name, DateTime timestamp, string group, string measurementUnit, double measuredValue)
        {
            Name = name;
            Timestamp = timestamp;
            Group = group;
            MeasurementUnit = measurementUnit;
            MeasuredValue = measuredValue;
        }


    }
}
