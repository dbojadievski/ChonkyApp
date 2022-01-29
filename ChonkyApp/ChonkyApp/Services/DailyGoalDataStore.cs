using ChonkyApp.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ChonkyApp.Services
{
    public class DailyGoalDataStore
    {
        private static readonly Guid sodiumGuid = new Guid("{2CCAD771-E94C-4738-81F1-82A050CB8866}");
        private static readonly Guid potassiumGuid = new Guid("{924E4C3A-3DA7-4AFA-8892-EBC4A399D5F1}");
        private static readonly Guid vitaminAGuid = new Guid("{85365624-3256-4238-B3D2-BB6C7F390DE1}");
        private static readonly Guid vitaminCGuid = new Guid("{34D5DDD8-997B-4EE1-8C01-4C208A6CA153}");
        private static readonly Guid vitaminEGuid = new Guid("{0E4427D6-ACE0-4B6C-9C3A-D165AC70028E}");

        private static readonly DailyGoal sodium = new DailyGoal("sodium", sodiumGuid);
        private static readonly DailyGoal potassium = new DailyGoal("potassium", potassiumGuid);
        private static readonly DailyGoal vitaminA = new DailyGoal("vitaminA", vitaminAGuid);
        private static readonly DailyGoal vitaminC = new DailyGoal("vitaminC", vitaminCGuid);
        private static readonly DailyGoal vitaminE = new DailyGoal("vitaminE", vitaminEGuid);

        public static DailyGoal SpaceGoalSodium
        {
            get => sodium;
        }

        public static DailyGoal SpaceGoalPotassium
        {
            get => potassium;
        }

        public static DailyGoal SpaceGoalVitaminA
        {
            get => vitaminA;
        }

        public static DailyGoal SpaceGoalVitaminC
        {
            get => vitaminC;
        }

        public static DailyGoal SpaceGoalVitaminE
        {
            get => vitaminE;
        }

        public static Guid SpaceGoalSodiumID
        {
            get => sodiumGuid;
        }

        public static Guid SpaceGoalPotassiumID
        {
            get => potassiumGuid;
        }

        public static Guid SpaceGoalVitaminAID
        {
            get => vitaminAGuid;
        }

        public static Guid SpaceGoalVitaminCID
        {
            get => vitaminCGuid;
        }

        public static Guid SpaceGoalVitaminEID
        {
            get => vitaminEGuid;
        }


        private static bool CreateDailyGoalTable()
        {
            var wasCreated = SQLiteProvider.Database.CreateTable<DailyGoal>() == SQLite.CreateTableResult.Created;

            if (wasCreated)
                InsertDefaultDailyGoals();
            
            return wasCreated;

            void InsertDefaultDailyGoals()
            {
                // Inserts SPACE goals.
                SQLiteProvider.Database.Insert(sodium);
                SQLiteProvider.Database.Insert(potassium);
                SQLiteProvider.Database.Insert(vitaminA);
                SQLiteProvider.Database.Insert(vitaminC);
                SQLiteProvider.Database.Insert(vitaminE);
            }

            void DeleteDailyGoalsTable()
            {
                SQLiteProvider.Database.DropTable<DailyGoalEntry>();
                SQLiteProvider.Database.DropTable<DailyGoal>();
            }
        }

        private static bool CreateGoalEntryTable()
        {
            var wasCreated = SQLiteProvider.Database.CreateTable<DailyGoalEntry>() == SQLite.CreateTableResult.Created;
            return wasCreated;
        }

        static DailyGoalDataStore()
        {
            CreateDailyGoalTable();
            CreateGoalEntryTable();
        }

        public async Task<bool> InsertGoalEntry(DailyGoalEntry entry)
        {
            var numAffected = SQLiteProvider.Database.Insert(entry);
            var isSuccess = (numAffected == 1);

            return await Task.FromResult(isSuccess);
        }

        public async Task<IEnumerable<DailyGoalEntry>> GetGoalEntriesForDay(DateTime day)
        {
            var items = SQLiteProvider.Database.Query<DailyGoalEntry>("SELECT * FROM DailyGoalEntry");
            var itemsAtDay = items.Where(e => e.CreatedAt.Date == day.Date);

            return await Task.FromResult(itemsAtDay);
        }

        public async Task<IEnumerable<DailyGoalEntry>> GetGoalEntriesOfTypeForDay(Guid id, DateTime day)
        {
            var items = SQLiteProvider.Database.Query<DailyGoalEntry>("SELECT * FROM DailyGoalEntry WHERE goalID = ?", id);
            IEnumerable<DailyGoalEntry> itemsAtDay = items.Where(e => e.CreatedAt.Date == day.Date);
            bool hasAny = itemsAtDay.Count() > 0;

            return await Task.FromResult(itemsAtDay);
        }

        public async Task<IEnumerable<DailyGoalEntry>> GetGoalEntries()
        {
            var items = SQLiteProvider.Database.Query<DailyGoalEntry>("SELECT * FROM DailyGoalEntry");
            return await Task.FromResult(items);
        }

        public async Task<IEnumerable<DailyGoalEntry>> GetGoalEntriesOfType(Guid id)
        {
            var items = SQLiteProvider.Database.Query<DailyGoalEntry>("SELECT * FROM DailyGoalEntry WHERE ID = ?", id);
            return await Task.FromResult(items);
        }

        public async Task<bool> DeleteGoalEntry(DailyGoalEntry goalEntry)
        {
            var numAffected = SQLiteProvider.Database.Execute("DELETE FROM DailyGoalEntry WHERE goalID = ?", goalEntry.GoalID);
            return await Task.FromResult(numAffected > 0);
        }
    }
}
