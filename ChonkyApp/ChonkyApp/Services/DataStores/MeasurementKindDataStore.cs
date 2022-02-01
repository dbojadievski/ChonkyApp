using ChonkyApp.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChonkyApp.Services
{
    public class MeasurementKindDataStore
    {
        private const String TABLE_NAME = "MeasurementKindEntry";
        public const Double WELLNESS_MIN = -1d;
        public const double WELLNESS_MAX = 1d;

        #region Optimization cache.
        public const String MEASUREMENT_NAME_SLEEP = "Sleep";
        public const String MEASUREMENT_NAME_MOOD = "Mood";
        public const String MEASUREMENT_NAME_SORENESS = "Soreness";
        public const String MEASUREMENT_NAME_STRESS = "Stress";
        public const String MEASUREMENT_NAME_NUTRITION = "Nutrition";


        private static MeasurementKindEntry sleepMeasurementKind;
        private static MeasurementKindEntry moodMeasurementKind;
        private static MeasurementKindEntry sorenessMeasurementKind;
        private static MeasurementKindEntry stressMeasurementKind;
        private static MeasurementKindEntry nutritionMeasurementKind;

        public static MeasurementKindEntry SleepMeasurementKind
        {
            get => sleepMeasurementKind;
        }

        public static MeasurementKindEntry MoodMeasurementKind
        {
            get => moodMeasurementKind;
        }

        public static MeasurementKindEntry SorenessMeasurementKind
        {
            get => sorenessMeasurementKind;
        }

        public static MeasurementKindEntry StressMeasurementKind
        {
            get => stressMeasurementKind;
        }
        public static MeasurementKindEntry NutritionMeasurementKind
        {
            get => nutritionMeasurementKind;
        }
        #endregion

        private static bool CreateTableMeasurementKindEntries()
        {
            var wasCreated = false;
            try
            {
                var result = SQLiteProvider.Database.CreateTable<MeasurementKindEntry>();
                wasCreated = (result == SQLite.CreateTableResult.Created);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return wasCreated;
        }

        private static bool CreateTableCustomMeasurements()
        {
            var wasCreated = false;
            try
            {
                var result = SQLiteProvider.Database.CreateTable<CustomMeasurement>();
                wasCreated = (result == SQLite.CreateTableResult.Created);
            }
            catch (Exception ex)
            {
                Trace.TraceError (ex.Message);
            }
            return wasCreated;
        }

        private static async Task InsertMissingDefaultData()
        {
            {
                var insertList = new List<MeasurementKindEntry>(10);

                // Wellness measurements.
                var unitTypeRelative = UnitDataStore.GetForUnitType(UnitType.Relative).Result;
                Debug.Assert(unitTypeRelative != null);

                sleepMeasurementKind = FindByName(MEASUREMENT_NAME_SLEEP);
                if (sleepMeasurementKind == null)
                {
                    sleepMeasurementKind = new MeasurementKindEntry(MEASUREMENT_NAME_SLEEP, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX);
                    insertList.Add(sleepMeasurementKind);
                }


                moodMeasurementKind = FindByName(MEASUREMENT_NAME_MOOD);
                if (moodMeasurementKind == null)
                {
                    moodMeasurementKind = new MeasurementKindEntry(MEASUREMENT_NAME_MOOD, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX);
                    insertList.Add(moodMeasurementKind);
                }

                sorenessMeasurementKind = FindByName(MEASUREMENT_NAME_SORENESS);
                if (sorenessMeasurementKind == null)
                {
                    sorenessMeasurementKind = new MeasurementKindEntry(MEASUREMENT_NAME_SORENESS, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX);
                    insertList.Add(sorenessMeasurementKind);
                }

                stressMeasurementKind = FindByName(MEASUREMENT_NAME_STRESS);
                if (stressMeasurementKind == null)
                {
                    stressMeasurementKind = new MeasurementKindEntry(MEASUREMENT_NAME_STRESS, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX);
                    insertList.Add(stressMeasurementKind);
                }

                nutritionMeasurementKind = FindByName(MEASUREMENT_NAME_NUTRITION);
                if (nutritionMeasurementKind == null)
                {
                    nutritionMeasurementKind = new MeasurementKindEntry(MEASUREMENT_NAME_NUTRITION, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX);
                    insertList.Add(nutritionMeasurementKind);
                }

                if (insertList.Count > 0)
                   InsertAndAssert(insertList);
            }

            void InsertAndAssert(List<MeasurementKindEntry> entries)
            {
                var inserted = InsertRange(entries);
                Debug.Assert(inserted);
                if (inserted)
                    Trace.TraceInformation($"Successfully inserted missing default data for table {TABLE_NAME}.");
                else
                    Trace.TraceError($"Could not insert missing default data for table {TABLE_NAME}.");

                // var all = GetAll();
            }
        }

        public static Boolean Clear()
        {
            var numAffected = SQLiteProvider.Database.DeleteAll<MeasurementKindEntry>();
            return (numAffected != 0);
        }

        public static async Task Initialize()
        {
            CreateTableMeasurementKindEntries();
            await InsertMissingDefaultData();

            CreateTableCustomMeasurements();
        }

        public static Boolean InsertRange(IEnumerable<MeasurementKindEntry> measurementKindEntries)
        {
            var result = false;

            try
            {
                var numAffected = SQLiteProvider.Database.InsertAll(measurementKindEntries);
                result = (numAffected == measurementKindEntries.Count());
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return result;
        }
        public static Boolean Insert(MeasurementKindEntry entry)
        {
            var result = false;

            try
            {
                var numAffected = SQLiteProvider.Database.Insert(entry);
                result = (numAffected == 1);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return result;
        }

        public static Boolean Insert(IEnumerable<CustomMeasurement> measurements)
        {
            var result = false;
            
            try
            {
                var numAffected = SQLiteProvider.Database.InsertAll(measurements);
                result = (numAffected == measurements.Count());
            }
            catch (Exception e)
            {
                Trace.TraceError (e.Message);
            }

            return result;
        }

        public static MeasurementKindEntry FindByName(String name)
        {
            MeasurementKindEntry result = null;
            try
            {
                var items = SQLiteProvider.Database.Query<MeasurementKindEntry>($"SELECT * FROM {TABLE_NAME} WHERE Name = ?", name);
                result = items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return result;
        }
        public static MeasurementKindEntry Find(Guid id)
        {
            MeasurementKindEntry result = null;

            try
            {
                var items = SQLiteProvider.Database.Query<MeasurementKindEntry>($"SELECT * FROM {TABLE_NAME} WHERE UnitID = ?", id);
                result = items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            return result;
        }

        public static IEnumerable<CustomMeasurement> Find(MeasurementKindEntry measurementKind)
        {
            IEnumerable<CustomMeasurement> result = null;

            try
            {
                result = SQLiteProvider.Database.Query<CustomMeasurement>("SELECT * FROM CustomMeasurement WHERE MeasurementKindID = ?", measurementKind.ID);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return result;
        }

        public static IEnumerable<CustomMeasurement> FindWellnessItems(DateTime? cutoff = null)
        {
            IEnumerable<CustomMeasurement> result = null;

            try
            {
                var paramList = new List<Object> { SleepMeasurementKind.ID, MoodMeasurementKind.ID, sorenessMeasurementKind.ID, StressMeasurementKind.ID, NutritionMeasurementKind.ID };

                var builder = new StringBuilder("SELECT * FROM CustomMeasurement WHERE MeasurementKindID in ?");
                if (cutoff.HasValue)
                {
                    paramList.Add(cutoff.Value);
                    builder.Append(" AND CreatedAt >= ?");
                }

                var paramArray = paramList.ToArray();
                result = SQLiteProvider.Database.Query<CustomMeasurement>(builder.ToString(), paramArray);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }

            return result;
        }

        public static IEnumerable<MeasurementKindEntry> GetAll()
        {
            IEnumerable<MeasurementKindEntry> result = null;

            try
            {
                result = SQLiteProvider.Database.Query<MeasurementKindEntry>($"SELECT * FROM {TABLE_NAME}");
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }

            return result;
        }
    }
}
