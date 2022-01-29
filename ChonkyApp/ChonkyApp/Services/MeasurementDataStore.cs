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
        private static bool CreateTableUnit()
        {
            var wasCreated = SQLiteProvider.Database.CreateTable<UnitEntry>() == SQLite.CreateTableResult.Created;
            if (wasCreated)
            {
                InsertUnitData();
            }

            return wasCreated;

            void InsertUnitData()
            {
                foreach (var x in Enum.GetValues(typeof(Unit)))
                {
                    Unit asUnit = (Unit)x;
                    var entry = new UnitEntry
                    {
                        Unit = asUnit,
                        UnitType = asUnit.GetUnitType()
                    };

                    SQLiteProvider.Database.Insert(entry);
                }
            }
        }

        private static bool CreateTableMeasurement()
        {
            var wasCreated = SQLiteProvider.Database.CreateTable<Measurement>();
            return wasCreated == SQLite.CreateTableResult.Created;
        }

        static MeasurementDataStore()
        {
            CreateTableUnit();
            CreateTableMeasurement();
        }

        public MeasurementDataStore()
        {
        }

        public async Task<bool> AddItemAsync(Measurement item)
        {
            var numAffected = SQLiteProvider.Database.Insert(item);
            var isSuccess = (numAffected == 1);

            Debug.Assert(isSuccess);

            return await Task.FromResult(isSuccess);
        }

        public async Task<bool> UpdateItemAsync(Measurement item)
        {
            var numAffected = SQLiteProvider.Database.Update(item);
            var isSuccess = (numAffected == 1);
            Debug.Assert(isSuccess);

            return await Task.FromResult(isSuccess);
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var numAffected = SQLiteProvider.Database.Delete(id);
            var isSuccess = (numAffected == 1);
            Debug.Assert(isSuccess);

            return await Task.FromResult(isSuccess);
        }

        public async Task<Measurement> GetItemAsync(Guid id)
        {
            var item = SQLiteProvider.Database.Get<Measurement>(id);
            return await Task.FromResult(item);
        }

        /// <summary>
        /// Gets all measurements for the specified period.
        /// </summary>
        /// <param name="numDaysInPast">The number of days for which to search, or 0 for all.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Measurement>> GetItemsAsync(uint numDaysInPast = 0)
        {
            IEnumerable<Measurement> items = null;

            DateTime maxDateInPast = DateTime.Now.AddDays(-numDaysInPast);
            items = SQLiteProvider.Database.Query<Measurement>("SELECT * FROM Measurement WHERE CreatedAt >= ?", maxDateInPast);

            return await Task.FromResult(items);
        }

        public async Task<IEnumerable<Measurement>> GetWeightMeasurementsBetweenDates(DateTime startDate, DateTime endDate)
        {
            IEnumerable<Measurement> weightDataPoints = null;

            Debug.Assert(startDate >= endDate);
            if (startDate <= endDate)
                weightDataPoints = SQLiteProvider.Database.Query<Measurement>("SELECT * FROM Measurement m JOIN UnitEntry u ON m.Unit = u.Unit WHERE u.UnitType = ? AND m.CreatedAt >= ? AND m.CreatedAt <= ? ORDER BY m.CreatedAt", new object[] { UnitType.Weight, startDate, endDate });

            return weightDataPoints;
        }

        public async Task<IEnumerable<Measurement>> GetWeightMeasurements(uint numDaysInPast = 0)
        {
            IEnumerable<Measurement> weightDataPoints = null;

            DateTime maxDateInPast = DateTime.Now.AddDays(-numDaysInPast);
            weightDataPoints = SQLiteProvider.Database.Query<Measurement>("SELECT * FROM Measurement m JOIN UnitEntry u ON m.Unit = u.Unit WHERE u.UnitType = ? AND m.CreatedAt >= ? ORDER BY m.CreatedAt", new object[] { UnitType.Weight, maxDateInPast });


            return await Task.FromResult(weightDataPoints);
        }

        public async Task<IEnumerable<Measurement>> GetBodyFatMeasurements(uint numDaysInPast = 0)
        {
            IEnumerable<Measurement> fatDataPoints = null;
            DateTime maxDateInPast = DateTime.Now.AddDays(-numDaysInPast);
            
            fatDataPoints = SQLiteProvider.Database.Query<Measurement>(
                "SELECT * FROM Measurement m JOIN UnitEntry u ON m.Unit = u.Unit WHERE u.UnitType= ? AND m.CreatedAt >= ? ORDER BY m.CreatedAt", 
                new object[] 
                { 
                    UnitType.Relative, 
                    maxDateInPast 
                });

            return await Task.FromResult(fatDataPoints);
        }
    }
}