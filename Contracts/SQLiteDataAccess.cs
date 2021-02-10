using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class SQLiteDataAccess
    {
        #region load
        public static List<Device> LoadHeatControlDevices()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Device>("select * from \"Heat control devices\"", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Device> LoadHumidityControlDevices()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Device>("select * from \"Humidity control devices\"", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Device> LoadPressureControlDevices()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Device>("select * from \"Pressure control devices\"", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Device> LoadWindControlDevices()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Device>("select * from \"Wind control devices\"", new DynamicParameters());
                return output.ToList();
            }
        }
        #endregion load

        #region store
        public static void SaveHeatControlDevice(Device device)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                    cnn.Execute("insert into \"Heat control devices\" (Name, Timestamp, 'Group', MeasurementUnit, MeasuredValue)" +
                        " values (@Name, @Timestamp, @Group, @MeasurementUnit, @MeasuredValue)", device);
            }
        }

        public static void SaveHumidityControlDevice(Device device)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into \"Humidity control devices\" (Name, Timestamp, 'Group', MeasurementUnit, MeasuredValue)" +
                    "values (@Name, @Timestamp, @Group, @MeasurementUnit, @MeasuredValue)", device);
            }
        }

        public static void SavePressureControlDevice(Device device)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into \"Pressure control devices\" (Name, Timestamp, 'Group', MeasurementUnit, MeasuredValue)" +
                    " values (@Name, @Timestamp, @Group, @MeasurementUnit, @MeasuredValue)", device); ;
            }
        }

        public static void SaveWindControlDevice(Device device)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into \"Wind control devices\" (Name, Timestamp, 'Group', MeasurementUnit, MeasuredValue)" +
                    " values (@Name, @Timestamp, @Group, @MeasurementUnit, @MeasuredValue)", device);
            }
        }
        #endregion store

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
