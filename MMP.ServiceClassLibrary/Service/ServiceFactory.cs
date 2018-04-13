using Mehdime.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MMPModel.Service
{
    public class ServiceFactory
    {
        protected readonly IDbContextScopeFactory _dbContextScopeFactory;
        protected IDictionary<Type, MMPModel.Service.BaseService> _initializedServices;

        public ServiceFactory()
        {
            _dbContextScopeFactory = new DbContextScopeFactory();
            _initializedServices = new Dictionary<Type, MMPModel.Service.BaseService>();
        }

        public TService Get<TService>() where TService : MMPModel.Service.BaseService
        {
            var requestedType = typeof(TService);

            if (!_initializedServices.ContainsKey(requestedType))
            {
                //Instantiation du service si celui-ci n'a pas déjà été créé 
                try
                {
                    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    CultureInfo culture = null; // use InvariantCulture or other if you prefer
                    object service = Activator.CreateInstance(requestedType, flags, null, new object[] { _dbContextScopeFactory }, culture);
                    _initializedServices.Add(requestedType, service as MMPModel.Service.BaseService);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return _initializedServices[requestedType] as TService;
        }

    }
}
