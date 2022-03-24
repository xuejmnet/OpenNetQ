using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OpenNetQ
{
    public abstract class InjectService
    {
        public IServiceProvider ServiceProvider { get; }

        public InjectService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        protected TService? GetService<TService>() => ServiceProvider.GetService<TService>();
        protected TService GetRequiredService<TService>() => ServiceProvider.GetRequiredService<TService>();
        
        protected TService LazyGet<TService>(ref TService reference)
            => LazyTypeGet(typeof(TService), ref reference);

        protected TRef LazyTypeGet<TRef>(Type serviceType, ref TRef reference)
        {
            return reference ??= (TRef)ServiceProvider.GetRequiredService(serviceType);
        }
    }
}
