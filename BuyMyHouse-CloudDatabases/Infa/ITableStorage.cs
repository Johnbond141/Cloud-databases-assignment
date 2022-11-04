using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface ITableStorage<T> where T : TableEntity, new()
    {
        Task<T> CreateEntity(T entity);
        Task<T> GetEntityByPartitionKeyAndRowKey(string pk, string rk);
        Task<bool> UpdateEntity(T entity);
        Task<bool> DeleteEntity(string pk, string rk);

    }
}
