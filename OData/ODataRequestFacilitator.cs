using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.OData;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Query;
using Microsoft.Data.Edm;
using Starship.Core.Data;
using Starship.Core.Extensions;
using Starship.Core.Interfaces;
using Starship.Core.Reflection;
using Starship.Core.Security;
using Starship.Core.Time;
using Starship.Core.Utility;
using Starship.Data;
using Starship.Data.Interfaces;

namespace Starship.Web.OData {
    public static class ODataRequestFacilitator {
        
        /*public static void Initialize(params Type[] entityTypes) {
            var builder = new ODataConventionModelBuilder();

            foreach (var type in entityTypes) {
                try {
                    builder.AddEntity(type);
                }
                catch (ReflectionTypeLoadException ex) {
                    throw new Exception("Failed to populate EdmModel with type: " + type.Name + "(" + string.Join(Environment.NewLine, ex.LoaderExceptions.Select(each => each.ToString())) + ")", ex);
                }
            }

            EdmModel = builder.GetEdmModel();
        }*/

        private static Type FindInterfaceInBaseTypes(Type source, Type interfaceType) {
            if ((interfaceType.Name == source.Name && source.GetGenericArguments().Length == interfaceType.GetGenericArguments().Length && source.GetGenericArguments().Length > 1) || interfaceType.IsAssignableFrom(source)) {
                return source;
            }

            foreach (var _interface in source.GetInterfaces()) {
                var type = FindInterfaceInBaseTypes(_interface, interfaceType);

                if (type != null) {
                    return type;
                }
            }

            return null;
        }

        public static IQueryable GetDataSource(Type outType, IQueryable underlyingFilter = null) {
            var dataInterface = FindInterfaceInBaseTypes(outType, typeof(IsDataModel<,>));

            if (dataInterface == null) {
                return GetDataStore(outType);
            }

            var method = dataInterface.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            if (underlyingFilter == null) {
                var parameterType = method.GetParameters().FirstOrDefault();
                var inType = parameterType.ParameterType.GenericTypeArguments.First();

                underlyingFilter = GetUnderlyingDataSource(inType);
            }

            var query = method.Invoke(Activator.CreateInstance(outType), new object[] {underlyingFilter}) as IQueryable;

            //query.InjectMissingMemberBindings(inType, outType);

            return query;
        }

        public static IQueryable GetUnderlyingDataSource(Type type, params string[] includes) {
            return GetDataStore(GetUnderlyingDataType(type), includes);
        }

        public static Type GetUnderlyingDataType(Type type) {
            var dataInterface = FindInterfaceInBaseTypes(type, typeof(IsDataModel<,>));

            if (dataInterface == null) {
                return type;
            }

            var method = dataInterface.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            var parameterType = method.GetParameters().FirstOrDefault();
            return parameterType.ParameterType.GenericTypeArguments.First();
        }

        private static IQueryable GetDataStore(Type type, params string[] includes) {
            var source = DataStore.Get(type).Include(includes) as IQueryable;

            if (typeof(IsDeletable).IsAssignableFrom(type)) {
                var time = TimeProvider.Now;
                source = source.Where<IsDeletable>(each => each.ValidUntil == null || each.ValidUntil > time);
            }

            return source;
        }

        public static IQueryable QueryEntityById(HttpRequestMessage request, Type type, string entityId, params string[] includes) {

            if (!int.TryParse(entityId, out var id)) {
                return null;
            }

            var pkName = DataStore.GetPrimaryKeyName(GetUnderlyingDataType(type));
            var source = GetUnderlyingDataSource(type, includes).Where(pkName, id);

            return Query(request, new ODataQuerySettings(), type, source);
        }

        public static IQueryable Query(HttpRequestMessage request, ODataQuerySettings settings, Type type, IQueryable source = null) {
            if (type.IsInterface && typeof(HasIdentity).IsAssignableFrom(type)) {
                var types = ReflectionCache.GetTypesOf(type, false)
                    .Where(each => typeof(IsDataModel).IsAssignableFrom(each))
                    .ToList();

                IQueryable data = null;
                Type sourceType = null;

                foreach (var eachType in types) {
                    var query = Query(request, new ODataQuerySettings(), eachType, DataStore.Get(eachType));
                    data = Invoke<IQueryable>("BuildInterfaceQuery", eachType, type, query, data);
                }

                return Invoke<IQueryable>("OrderQuery", data.GetGenericType(), data);
            }

            if (source == null) {
                source = GetDataSource(type);
            }

            if (typeof(IsDataModel).IsAssignableFrom(type)) {
                //var results = typeof(Enumerable).InvokeExtensionMethod("ToList", source.GetGenericType(), source) as IEnumerable;
                return source;
            }

            return request.ApplySecurityPolicy(settings, type, source);
        }

        private static T Invoke<T>(string name, Type genericParameter1, params object[] parameters) {
            return (T) typeof(ODataRequestFacilitator).GetMethod(name).MakeGenericMethod(genericParameter1).Invoke(null, parameters);
        }

        public static IQueryable OrderQuery<T>(IQueryable<T> query) {
            var parameters = new object[] {query, DynamicQuery.GetPropertyExpression(typeof(T), "Id")};
            var methods = typeof(Queryable).GetMethods().Where(each => each.Name == "OrderBy").ToList();

            return methods.FirstOrDefault().MakeGenericMethod(typeof(T), typeof(Guid)).Invoke(null, parameters) as IQueryable;
        }

        public static IQueryable BuildInterfaceQuery<T>(Type interfaceType, IQueryable<T> query, IQueryable source = null) {
            var interfaceQuery = query.SelectByInterface(interfaceType);
            var concreteType = interfaceQuery.GetType().GenericTypeArguments.First();

            return source == null ? interfaceQuery : Invoke<IQueryable>("Concat", concreteType, source, interfaceQuery);
        }

        public static IQueryable Concat<T>(IQueryable<T> source, IQueryable<T> query) {
            return source.Concat(query);
        }

        public static IQueryable ApplySecurityPolicy(Type type, IQueryable source) {

            // Todo:  Access security interception
            /*if (UserContext.Current.Role == RoleTypes.SiteAdmin) {
                return source;
            }*/

            if (Activator.CreateInstance(type) is HasSecurityPolicy instance) {
                return instance.ApplySecurity(source, AccessTypes.Read);
            }

            return source;
        }

        public static IQueryable ApplySecurityPolicy(this HttpRequestMessage request, ODataQuerySettings settings, Type type, IQueryable source) {
            return ApplySecurityPolicy(type, source);
            //var results = ApplySecurityPolicy(type, source);
            //return request.ApplyODataFilter(settings, results, type);
        }

        /*public static IQueryable ApplyODataFilter(this HttpRequestMessage request, ODataQuerySettings settings, IQueryable query, Type type) {
            var context = new ODataQueryContext(EdmModel, type);
            var options = new ODataQueryOptions(context, request);
            return options.ApplyTo(query, settings);
        }*/
        
        /*private static IEdmModel _edmModel;
        private static IEdmModel EdmModel {
            get {
                if(_edmModel == null) {
                    throw new Exception("ODataRequestFacilitator must be initialized prior to calling GetEdmModel().");
                }

                return _edmModel;
            }
            set => _edmModel = value;
        }*/
    }
}