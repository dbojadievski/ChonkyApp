#define WITH_TEST_DATA

using ChonkyApp.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

namespace ChonkyApp.Services
{
    public class MeasurementDataStore
    {
        private static bool CreateTableMeasurement()
        {
            var wasCreated = SQLiteProvider.Database.CreateTable<Measurement>();
            return wasCreated == SQLite.CreateTableResult.Created;
        }

        public static void Initialize()
        {
            CreateTableMeasurement();
#if WITH_TEST_DATA
            CreateTestData();
#endif

        }

        public static void Clear()
        {
            SQLiteProvider.Database.DeleteAll<Measurement>();
        }

        #if WITH_TEST_DATA
        private static void CreateTestData()
        {
            // Don't double-log test data.
            var allMeasurements = SQLiteProvider.Database.Query<Measurement>("SELECT * FROM Measurement m JOIN UnitEntry u ON m.Unit = u.Unit");
            if (allMeasurements.Count != 0)
                return;

            var now = DateTime.Now;
            var valueWeight = 95.1d;
            var valueFat = 18.0d;

            foreach (int value in Enumerable.Range(1, 14))
            {
                var day = now.AddDays(-value);

                var weight = new Measurement(day);
                weight.Value = valueWeight - (value * .1d);
                weight.Unit = Unit.Kilogram;

                var fat = new Measurement(day);
                fat.Value = valueFat - (value * .03);
                fat.Unit = Unit.Percent;

                AddItem(weight);
                AddItem(fat);
            }
        }
        #endif

        private MeasurementDataStore()
        {
        }

        public static bool AddItem(Measurement item)
        {
            var numAffected = 0;
            try
            {
                SQLiteProvider.Database.Insert(item);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            var isSuccess = (numAffected == 1);
            Debug.Assert(isSuccess);

            return isSuccess;
        }

        public static List<Measurement> GetWeightMeasurementsBetweenDates(DateTime startDate, DateTime endDate)
        {
            List<Measurement> weightDataPoints = null;

            Debug.Assert(startDate >= endDate);
            if (startDate <= endDate)
            {
                try
                {
                    weightDataPoints = SQLiteProvider.Database.Query<Measurement>("SELECT * FROM Measurement m JOIN UnitEntry u ON m.Unit = u.Unit WHERE u.UnitType = ? AND m.CreatedAt >= ? AND m.CreatedAt <= ? ORDER BY m.CreatedAt", new object[] { UnitType.Weight, startDate, endDate });
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }

            }

            return weightDataPoints;
        }

        public static List<Measurement> GetWeightMeasurements(uint numDaysInPast = 0)
        {
            List<Measurement> weightDataPoints = null;

            DateTime maxDateInPast = DateTime.Now.AddDays(-numDaysInPast);
            try
            {
                weightDataPoints = SQLiteProvider.Database.Query<Measurement>("SELECT * FROM Measurement m JOIN UnitEntry u ON m.Unit = u.Unit WHERE u.UnitType = ? AND m.CreatedAt >= ? ORDER BY m.CreatedAt", new object[] { UnitType.Weight, maxDateInPast });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }


            return weightDataPoints;
        }

        public static List<Measurement> GetBodyFatMeasurements(uint numDaysInPast = 0)
        {
            List<Measurement> fatDataPoints = null;
            DateTime maxDateInPast = DateTime.Now.AddDays(-numDaysInPast);
            
            try
            {
                fatDataPoints = SQLiteProvider.Database.Query<Measurement>(
                    "SELECT * FROM Measurement m JOIN UnitEntry u ON m.Unit = u.Unit WHERE u.UnitType= ? AND m.CreatedAt >= ? ORDER BY m.CreatedAt", 
                    new object[] 
                    { 
                        UnitType.Relative, 
                        maxDateInPast 
                    });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return fatDataPoints;
        }
    }
}