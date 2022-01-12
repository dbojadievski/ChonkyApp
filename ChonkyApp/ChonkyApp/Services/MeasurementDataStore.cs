using ChonkyApp.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ChonkyApp.Services
{
    public class MeasurementDataStore : IDataStore<Measurement>
    {
        private static bool CreateTableUnit()
        {
            var wasCreated = SQLiteProvider.Database.CreateTable<UnitEntry>();
            if (wasCreated == SQLite.CreateTableResult.Created)
            {
                foreach (var x in Enum.GetValues(typeof(Unit)))
                {
                    Unit asUnit = (Unit) x;
                    var entry = new UnitEntry
                    {
                        Unit = asUnit,
                        UnitType = asUnit.GetUnitType()
                    };

                    SQLiteProvider.Database.Insert(entry);
                }
            }
            return false;
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

        public async Task<IEnumerable<Measurement>> GetItemsAsync(bool forceRefresh = false)
        {

            var items = SQLiteProvider.Database.Query<Measurement>("SELECT * FROM Measurement");
            return await Task.FromResult(items);
        }

        public async Task<IEnumerable<Measurement>> GetWeightMeasurements(bool forceRefresh = false)
        {
            IEnumerable<Measurement> weightDataPoints = null;
            try
            {
                weightDataPoints = SQLiteProvider.Database.Query<Measurement>("SELECT * FROM Measurement m JOIN UnitEntry u ON m.Unit = u.Unit WHERE u.UnitType=? ORDER BY m.CreatedAt", new object[] { UnitType.Weight });

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return await Task.FromResult(weightDataPoints);
        }

        public async Task<IEnumerable<Measurement>> GetBodyFatMeasurements(bool forceRefresh = false)
        {
            IEnumerable<Measurement> fatDataPoints = null;
            try
            {
                fatDataPoints = SQLiteProvider.Database.Query<Measurement>("SELECT * FROM Measurement m JOIN UnitEntry u ON m.Unit = u.Unit WHERE u.UnitType=? ORDER BY m.CreatedAt", new object[] { UnitType.Relative });

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return await Task.FromResult(fatDataPoints);
        }
    }
}