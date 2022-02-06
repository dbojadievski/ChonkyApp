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
        public const Double WELLNESS_MAX = 1d;
        public const Double SKINFOLD_MIN = 0d;
        public const Double SKINFOLD_MAX = 1000d;


        #region Optimization cache.
        public const String MEASUREMENT_NAME_SLEEP = "Sleep";
        public const String MEASUREMENT_NAME_MOOD = "Mood";
        public const String MEASUREMENT_NAME_SORENESS = "Soreness";
        public const String MEASUREMENT_NAME_STRESS = "Stress";
        public const String MEASUREMENT_NAME_NUTRITION = "Nutrition";

        public const String MEASUREMENT_NAME_SKINFOLD_CHEST = "Chest Skinfold";
        public const String MEASUREMENT_NAME_SKINFOLD_ABDOMEN = "Abdomen Skinfold";
        public const String MEASUREMENT_NAME_SKINFOLD_THIGH = "Thigh Skinfold";
        public const String MEASUREMENT_NAME_SKINFOLD_TRICEPS = "Triceps Skinfold";
        public const String MEASUREMENT_NAME_SKINFOLD_SUPRAILIUM = "Suprailium Skinfold";

        private static MeasurementKindEntry sleepMeasurementKind;
        private static MeasurementKindEntry moodMeasurementKind;
        private static MeasurementKindEntry sorenessMeasurementKind;
        private static MeasurementKindEntry stressMeasurementKind;
        private static MeasurementKindEntry nutritionMeasurementKind;

        private static MeasurementKindEntry chestSkinfoldMeasurementKind;
        private static MeasurementKindEntry abdomenSkinfoldMeasurementKind;
        private static MeasurementKindEntry thighSkinfoldMeasurementKind;
        private static MeasurementKindEntry tricepsSkinfoldMeasurementKind;
        private static MeasurementKindEntry suprailiumSkinfoldMeasurementKind;

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

        public static MeasurementKindEntry ChestSkinfoldMeasurementKind
        {
            get => chestSkinfoldMeasurementKind;
        }

        public static MeasurementKindEntry AbdomenSkinfoldMeasurementKind
        {
            get => abdomenSkinfoldMeasurementKind;
        }

        public static MeasurementKindEntry ThighSkinfoldMeasurementKind
        {
            get => thighSkinfoldMeasurementKind;
        }

        public static MeasurementKindEntry TricepsSkinfoldMeasurementKind
        {
            get => tricepsSkinfoldMeasurementKind;
        }

        public static MeasurementKindEntry SuprailiumSkinfoldMeasurementKind
        {
            get => suprailiumSkinfoldMeasurementKind;
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
            #region Wellness measurements.
            {
                var insertList = new List<MeasurementKindEntry>(10);

                var unitTypeRelative = UnitDataStore.GetForUnitType(UnitType.Relative).Result;
                Debug.Assert(unitTypeRelative != null);

                InsertMeasurement(ref sleepMeasurementKind, MEASUREMENT_NAME_SLEEP, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX, insertList);
                InsertMeasurement(ref moodMeasurementKind, MEASUREMENT_NAME_MOOD, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX, insertList);
                InsertMeasurement(ref sorenessMeasurementKind, MEASUREMENT_NAME_SORENESS, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX, insertList);
                InsertMeasurement(ref stressMeasurementKind, MEASUREMENT_NAME_STRESS, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX, insertList);
                InsertMeasurement(ref nutritionMeasurementKind, MEASUREMENT_NAME_NUTRITION, unitTypeRelative, WELLNESS_MIN, WELLNESS_MAX, insertList);

                if (insertList.Count > 0)
                   ExecuteInsertList(insertList);
            }
            #endregion

            #region Skinfold measurements
            {
                var insertList = new List<MeasurementKindEntry>();
                var unitTypeSkinfold = UnitDataStore.GetForUnit(Unit.Millimeter).Result;
                Debug.Assert(unitTypeSkinfold != null);

                InsertMeasurement(ref chestSkinfoldMeasurementKind, MEASUREMENT_NAME_SKINFOLD_CHEST, unitTypeSkinfold, SKINFOLD_MIN, SKINFOLD_MAX, insertList);
                InsertMeasurement(ref abdomenSkinfoldMeasurementKind, MEASUREMENT_NAME_SKINFOLD_ABDOMEN, unitTypeSkinfold, SKINFOLD_MIN, SKINFOLD_MAX, insertList);
                InsertMeasurement(ref thighSkinfoldMeasurementKind, MEASUREMENT_NAME_SKINFOLD_THIGH, unitTypeSkinfold, SKINFOLD_MIN, SKINFOLD_MAX, insertList);
                InsertMeasurement(ref tricepsSkinfoldMeasurementKind, MEASUREMENT_NAME_SKINFOLD_TRICEPS, unitTypeSkinfold, SKINFOLD_MIN, SKINFOLD_MAX, insertList);
                InsertMeasurement(ref suprailiumSkinfoldMeasurementKind, MEASUREMENT_NAME_SKINFOLD_SUPRAILIUM, unitTypeSkinfold, SKINFOLD_MIN, SKINFOLD_MAX, insertList);

                if (insertList.Count > 0)
                    ExecuteInsertList(insertList);
            }
#endregion

            void ExecuteInsertList(List<MeasurementKindEntry> entries)
            {
                var inserted = InsertRange(entries);
                Debug.Assert(inserted);
                if (inserted)
                    Trace.TraceInformation($"Successfully inserted missing default data for table {TABLE_NAME}.");
                else
                    Trace.TraceError($"Could not insert missing default data for table {TABLE_NAME}.");

                // var all = GetAll();
            }

            void InsertMeasurement(ref MeasurementKindEntry entry, String measurementName, UnitEntry unit, Double minVal, Double maxVal, List<MeasurementKindEntry> insertList)
            {
                entry = FindByName(measurementName);
                if (entry == null)
                {
                    entry = new MeasurementKindEntry(measurementName, unit, minVal, maxVal);
                    insertList.Add(entry);
                }
            }
        }

        public static bool ClearWellnessMeasurements()
        {
            bool wasDeleted = false;
            var paramList = new List<Object> { SleepMeasurementKind.ID, MoodMeasurementKind.ID, sorenessMeasurementKind.ID, StressMeasurementKind.ID, NutritionMeasurementKind.ID }.ToArray();
            
            try
            {
                SQLiteProvider.Database.Execute("DELETE FROM CustomMeasurement WHERE MeasurementKindID IN (?)", paramList);
                wasDeleted = true;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return wasDeleted;
        }

        public void ClearCaliperMeasurements()
        {
            bool wasDeleted = false;

            var paramList = new List<Object> 
            { 
                  ChestSkinfoldMeasurementKind.ID
                , AbdomenSkinfoldMeasurementKind.ID
                , ThighSkinfoldMeasurementKind.ID
                , TricepsSkinfoldMeasurementKind.ID
                , SuprailiumSkinfoldMeasurementKind.ID 
            }.ToArray();

            try
            {
                SQLiteProvider.Database.Execute("DELETE FROM CustomMeasurement WHERE MeasurementKindID IN (?)", paramList);
                wasDeleted = true;
            }
            catch (Exception ex)
            {
                Trace.TraceError (ex.Message);
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
            //ClearWellnessMeasurements();
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

        public static IEnumerable<CustomMeasurement> Find(MeasurementKindEntry measurementKind, DateTime? cutoff)
        {
            IEnumerable<CustomMeasurement> result = null;

            List<Object> param = new List<Object>() { measurementKind.ID};
            StringBuilder sb = new StringBuilder("SELECT * FROM CustomMeasurement WHERE MeasurementKindID = ? ");
            if (cutoff.HasValue)
            {
                sb.Append("AND CreatedAt >= ?");
                param.Add(cutoff.Value);
            }

            try
            {

                result = SQLiteProvider.Database.Query<CustomMeasurement>(sb.ToString(), param.ToArray());
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return result;
        }

        public static IEnumerable<CustomMeasurement> FindWellnessItems()
        {
            IEnumerable<CustomMeasurement> result = null;

            try
            {
                var paramList = new List<Object> { SleepMeasurementKind.ID, MoodMeasurementKind.ID, sorenessMeasurementKind.ID, StressMeasurementKind.ID, NutritionMeasurementKind.ID };

                var builder = new StringBuilder("SELECT * FROM CustomMeasurement WHERE MeasurementKindID in (?) ");
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
