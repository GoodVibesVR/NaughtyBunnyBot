using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaughtyBunnyBot.Cache.Services.Abstractions
{
    public interface IMemoryCacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T @object);
    }
}
