using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using ZenegyCalendar.Infrastructure;
using System.Data.Entity;
using ZenegyCalendar.DAL;

namespace ZenegyCalendar.GCalendarService
{
    /// <summary>
    /// The DataStore is an implementation of Google's interface IDataStore. The methods inside determine how the Oauth token is stored, deleted, and retrieved.
    /// </summary>
    public class EFDataStore : IDataStore
    {
        public async Task ClearAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                await objectContext.ExecuteStoreCommandAsync("TRUNCATE TABLE [GoogleAuthItems]").ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            using (var context = new ApplicationDbContext())
            {                
                var item = context.GoogleAuthItems.FirstOrDefault(x => x.Key == key);
                if (item != null)
                {
                    context.GoogleAuthItems.Remove(item);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            using (var context = new ApplicationDbContext())
            {                
                var item = context.GoogleAuthItems.FirstOrDefault(x => x.Key == key);
                T value = item == null ? default(T) : JsonConvert.DeserializeObject<T>(item.Value);
                return Task.FromResult<T>(value);
            }
        }

        public async Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            using (var context = new ApplicationDbContext())
            {
                string json = JsonConvert.SerializeObject(value);
                var item = await context.GoogleAuthItems.FirstOrDefaultAsync(x => x.Key == key).ConfigureAwait(false);

                if ( item == null)
                {
                    context.GoogleAuthItems.Add(new GoogleAuthItem { Key = key, Value = json });
                }
                else
                {
                    item.Value = json;
                }


                await context.SaveChangesAsync().ConfigureAwait(false);

                // When tried using the below code line, we get error as
                // This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.
                //return Task.FromResult(task);
            }
        }      
    }
}