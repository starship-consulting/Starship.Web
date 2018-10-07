using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Web.Http;
using System.Web.Http.OData.Query;
using CsQuery.ExtensionMethods;
using Newtonsoft.Json.Linq;
using Starship.Core.Data;
using Starship.Core.Extensions;
using Starship.Core.Reflection;
using Starship.Core.Utility;
using Starship.Data;
using Starship.Web.OData;

namespace Starship.Web.Controllers {

    public abstract class DataController : ApiController {

        [HttpGet, Route("data")]
        public virtual List<string> DataTypes() {
            return GetDataTypes()
                .Select(each => each.Name)
                .Distinct()
                .OrderBy(each => each)
                .ToList();
        }

        private IEnumerable<Type> GetDataTypes() {
            return DataStore.GetTypes();

            /*var entityTypes = DataStore.GetTypes();
            var dataModelTypes = ReflectionCache.GetTypesOf<IsDataModel>();

            return entityTypes.Concat(dataModelTypes).ToList();*/
        }

        [HttpGet, Route("data/{typeName}"), ODataFormat]
        public virtual object Query([FromUri] string typeName) {
            var type = TryGetType(typeName);
            return ODataRequestFacilitator.Query(Request, new ODataQuerySettings(), type);
        }

        private Type TryGetType(string typeName) {
            typeName = typeName.ToLower();

            var type = GetDataTypes().FirstOrDefault(each => each.Name.ToLower() == typeName || each.Name.ToLower() == typeName.Substring(0, typeName.Length - 1));

            if (type == null) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return type;
        }

        /*[HttpDelete, Route("data/{typeName}/{entityId}")]
        public object Delete([FromUri] string typeName, [FromUri] string entityId)
        {
            var type = TryGetType(typeName);

            if (!typeof(IsDeletable).IsAssignableFrom(type) || !CurrentUser.IsAuthenticated || CurrentUser.Role != RoleTypes.SiteAdmin)
            {
                throw new AppRequestException(AppLocalization.InsufficientPermission);
            }

            var entity = GetEntityById(type, entityId) as IsDeletable;

            if (entity != null)
            {
                entity.Delete();
                DataStore.Save();
            }

            return new HttpResponseMessage();
        }

        [HttpGet, Route("data/{typeName}/{idOrMethod}")]
        public object Query([FromUri] string typeName, [FromUri] string idOrMethod)
        {
            var type = TryGetType(typeName);

            var entity = GetEntityById(type, idOrMethod);

            if (entity != null)
            {
                return entity;
            }

            // Query by method
            var method = type.GetMethods().FirstOrDefault(each => each.Name.ToLower() == idOrMethod.ToLower());

            if (!method.IsStatic)
            {
                return this.Error("Only static methods can be invoked without an aggregate id.");
            }

            var access = method.GetCustomAttribute<AccessAttribute>();

            if (!CurrentUser.IsAuthenticated || CurrentUser.Role != RoleTypes.SiteAdmin)
            {
                if (access == null)
                {
                    return this.Error("Only administrators can access this resource.");
                }
            }

            var invoker = MethodInvoker.GetDefault(method);

            return invoker.Invoke(null, Request.ToDictionary());
        }

        [HttpPost, Route("data/{typeName}")]
        public object Save([FromUri] string typeName, [FromBody] object parameters)
        {
            var type = ReflectionCache.Lookup(typeName);

            if (type == null) // || !typeof (IDataModel).IsAssignableFrom(type))
            {
                throw new Exception("Unable to save type: " + typeName);
            }

            var jArray = parameters as JArray;
            object result = null;

            if (jArray != null)
            {
                var items = new List<object>();

                jArray.ForEach(each =>
                {
                    var item = TrySave(each, type);

                    if (item != null)
                    {
                        items.Add(item);
                    }
                });

                result = items;
            }
            else
            {
                result = TrySave(parameters, type);
            }

            DataStore.Save();

            return result;
        }

        [HttpPost, Route("data/{typeName}/{entityId}")]
        public object Save([FromUri] string typeName, [FromUri] string entityId, [FromBody] object parameters)
        {
            var type = TryGetType(typeName);

            if (!CurrentUser.IsAuthenticated)
            {
                throw new AppRequestException(AppLocalization.InsufficientPermission);
            }

            var entity = GetEntityById(type, entityId);

            if (entity != null)
            {
                var result = TrySave(parameters, type);
                DataStore.Save();
                return result;
            }

            return new HttpResponseMessage();
        }

        private object TrySave(object item, Type type)
        {
            var saveable = JObject.FromObject(item).ToObject(type);
            var dataModel = saveable as IsDataModel;

            if (dataModel != null)
            {
                dataModel.Save();
                return dataModel;
            }

            var pk = Context.GetPrimaryKeyProperty(type);

            if (pk != null)
            {
                var id = pk.GetValue(saveable);
                var instance = Context.Load(id, type);

                if (instance == null)
                {
                    instance = saveable;
                    Context.Add(saveable);
                }
                else
                {
                    foreach (var property in type.GetProperties())
                    {
                        if (property.GetSetMethod() != null)
                        {
                            property.SetValue(instance, property.GetValue(saveable));
                        }
                    }
                }

                Context.Save();
                return instance;
            }

            return null;
        }

        private object GetEntityById(Type type, string entityId)
        {
            int id = 0;

            if (int.TryParse(entityId, out id))
            {
                var pkName = Context.GetPrimaryKeyName(ODataRequestProvider.GetUnderlyingDataType(type));
                var filter = ODataRequestProvider.GetUnderlyingDataSource(type).Where(pkName, id);
                var source = ODataRequestProvider.GetDataSource(type, filter);

                return ODataRequestProvider.Query(Request, new ODataQuerySettings(), type, source).FirstOrDefault();
            }

            return null;
        }*/
    }
}