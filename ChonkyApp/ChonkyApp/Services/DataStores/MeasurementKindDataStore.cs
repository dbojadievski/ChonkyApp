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
        private const Double WELLNESS_MIN = -1d;
        private const double WELLNESS_MAX = 1d;

        #region Optimization cache.
        private const String MEASUREMENT_NAME_SLEEP = "Sleep";
        private const String MEASUREMENT_NAME_MOOD = "Mood";
        private const String MEASUREMENT_NAME_SORENESS = "Soreness";
        private const String MEASUREMENT_NAME_STRESS = "Stress";
        private const String MEASUREMENT_NAME_NUTRITION = "Nutrition";


        private static MeasurementKindEntry sleepMeasurementKind;
        private static MeasurementKindEntry moodMeasurementKind;
        private static MeasurementKindEntry sorenessMeasurementKind;
        private static MeasurementKindEntry stressMeasurementKind;
        private static MeasurementKindEntry nutritionMeasurementKind;
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
                var numAffected =SQLiteProvider.Database.Insert(entry);
                result = (numAffected == 1);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
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
