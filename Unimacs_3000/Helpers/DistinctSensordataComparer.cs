using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unimacs_3000.Models;

namespace Unimacs_3000.Helpers
{
    public class DistinctSensordataComparer : IEqualityComparer<SensorData>
    {
        //Custom EqualityComparer
        public bool Equals(SensorData x, SensorData y)
        {
            //Compare only on sensorname
            return x.sensor_name == y.sensor_name;
        }

        public int GetHashCode(SensorData obj)
        {
            return obj.sensor_name.GetHashCode();
        }
    }
}