using ChonkyApp.Models;

using SQLite;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ChonkyApp.Services
{
    public class UserDataStore : IDataStore<UserProfile>
    {
        private readonly Guid defaultUserGuid = Guid.Parse("05835dee-2058-4083-b6ae-a3c8facf797e");
        private readonly UserProfile defaultProfile = new UserProfile
        {
            EmailAddress = "sample@gmail.com",
            FirstName = "Konstantin",
            LastName = "Dimitrov",
            Height = 183,
            BirthDate = new DateTime(1992, 08, 28),
            ID = Guid.Parse("05835dee-2058-4083-b6ae-a3c8facf797e")
        };

        public UserDataStore()
        {
            var wasCreated = SQLiteProvider.Database.CreateTable<UserProfile>();
            if (wasCreated == CreateTableResult.Created)
            {
                var numAffected = SQLiteProvider.Database.Insert(defaultProfile);
                Debug.Assert(numAffected == 1);
            }
        }
        public async Task<UserProfile> GetCurrentProfile()
        {
            UserProfile profile = SQLiteProvider.Database.Get<UserProfile>(p => p.ID == defaultUserGuid);
            return await Task.FromResult(profile);
        }

        public async Task<bool> AddItemAsync(UserProfile item)
        {
            var numAffected = SQLiteProvider.Database.Insert(item);
            var isSuccess = (numAffected == 1);
            Debug.Assert(isSuccess);

            return await Task.FromResult(isSuccess);
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var numAffected = SQLiteProvider.Database.Delete<UserProfile>(id);
            bool isSuccess = (numAffected == 1);
            Debug.Assert(isSuccess);

            return await Task.FromResult(isSuccess);
        }

        public async Task<UserProfile> GetItemAsync(Guid id)
        {
            var item = SQLiteProvider.Database.Get<UserProfile>(id);
            return await Task.FromResult(item);
        }

        public async Task<IEnumerable<UserProfile>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotSupportedException();
        }

        public async Task<bool> UpdateItemAsync(UserProfile item)
        {
            var isSuccess = false;
            var numAffected = 0;
            try
            {
                numAffected = SQLiteProvider.Database.Update(item);
                
            }
            finally
            {
                isSuccess = (numAffected == 1);
                Debug.Assert(isSuccess);

            }

            return await Task.FromResult(isSuccess);
        }
    }
}
