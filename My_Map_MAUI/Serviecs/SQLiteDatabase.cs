using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections;
using System.Security.Cryptography;

namespace My_Map_MAUI.Serviecs
{
    public class SQLiteDatabase
    {
        private SQLiteAsyncConnection database;

        public SQLiteAsyncConnection Database { get => database; }

        public SQLiteDatabase()
        {
        }

        ~SQLiteDatabase()
        {
            Database.CloseAsync();
        }
        public async Task Init()
        {
            if (Database is not null)
                return;

            var path = Config.DatabasePath;
            if (File.Exists(path))
            {
                database = new SQLiteAsyncConnection(path, Config.Flags);
                await Database.EnableWriteAheadLoggingAsync();
                return;
            }

            database = new SQLiteAsyncConnection(path, Config.Flags);
            await Database.EnableWriteAheadLoggingAsync();
            await Database.CreateTableAsync<Detail>();
            await Database.CreateTableAsync<ItemType>();
            await Database.CreateTableAsync<MapItem>();
            await Database.CreateTableAsync<User>();
            await Database.CreateTableAsync<Models.Location>();
            await Database.CreateTableAsync<Models.Image>();
            try
            {
                var testUser = new User()
                {
                    Email = "test@gmail.com",
                    State = UserState.Activated,
                    Name = "Test"
                };
                testUser.Password = EncodePassword(testUser.Email, "@Test1234");
                
                await Database.InsertAsync(testUser);
            }
            catch (Exception ex) { return; }
        }


        public async Task<List<User>> GetAllUserAsync()
        {
            await Init();
            return await Database.Table<User>().ToListAsync();
        }
        public async Task<List<Models.Location>> GetAllLocationAsync()
        {
            await Init();
            return await Database.Table<Models.Location>().ToListAsync();
        }
        public async Task<List<Detail>> GetAllDetailAsync()
        {
            await Init();
            return await Database.Table<Detail>().ToListAsync();
        }

        public async Task<List<Models.Image>> GetAllImageAsync()
        {
            await Init();
            return await Database.Table<Models.Image>().ToListAsync();
        }

        public async Task<List<ItemType>> GetAllItemTypeAsync()
        {
            await Init();
            return await Database.Table<Models.ItemType>().ToListAsync();
        }
        public async Task<List<MapItem>> GetAllMapItemAsync()
        {
            await Init();
            return await Database.Table<Models.MapItem>().ToListAsync();
        }
        public async Task<MapItem> GetLatestMapItemAsync()
        {
            await Init();
            return await Database.Table<Models.MapItem>().OrderByDescending( i => i.Id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(ITableSQLite item)
        {
            await Init();
            if (item.Id != 0)
                return await Database.UpdateAsync(item);
            else
                return await Database.InsertAsync(item);
        }
        public async Task<int> InsertItemAsync(ITableSQLite item)
        {
            await Init();
            return await Database.InsertAsync(item);
        }

        public async Task<int> DeleteItemAsync(ITableSQLite item)
        {
            await Init();
            return await Database.DeleteAsync(item);

        }

        public static string EncodePassword(string salt, string password)
        {
            var bytesalt = Encoding.Unicode.GetBytes(salt);

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                 password: password!,
                 salt: bytesalt,
                 prf: KeyDerivationPrf.HMACSHA256,
                 iterationCount: 100000,
                 numBytesRequested: 256 / 8));
        }

    }
}







