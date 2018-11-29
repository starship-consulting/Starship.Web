using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.OData.Query;
using Starship.Core.Extensions;
using Starship.Core.Json;
using Starship.Core.Reflection;
using Starship.Data;
using Starship.Data.Attributes;
using Starship.Web.Extensions;
using Starship.Web.OData;

namespace Starship.Web.Controllers {
    public class DataController : ApiController {

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
            return ODataRequestFacilitator.Query(new ODataQuerySettings(), type);
        }

        private Type TryGetType(string typeName) {
            typeName = typeName.ToLower();

            var type = GetDataTypes().FirstOrDefault(each => each.Name.ToLower() == typeName || each.Name.ToLower() == typeName.Substring(0, typeName.Length - 1));

            if (type == null) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return type;
        }

        [HttpGet, Route("data/{typeName}/{idOrMethod}")]
        public virtual object Query([FromUri] string typeName, [FromUri] string idOrMethod) {

            var type = TryGetType(typeName);
            var entity = ODataRequestFacilitator.QueryEntityById(type, idOrMethod).FirstOrDefault();

            if (entity != null) {
                return Json(entity);
            }

            // Query by method
            var method = type.GetMethods().FirstOrDefault(each => each.Name.ToLower() == idOrMethod.ToLower());

            if (!method.IsStatic) {
                throw new Exception("Only static methods can be invoked without an entity id.");
            }

            /*var access = method.GetCustomAttribute<AccessAttribute>();

            if (!CurrentUser.IsAuthenticated || CurrentUser.Role != RoleTypes.SiteAdmin) {
                if (access == null) {
                    return this.Error("Only administrators can access this resource.");
                }
            }*/

            var invoker = MethodInvoker.GetDefault(method);
            var results = invoker.Invoke(null, Request.ToDictionary());
            
            return Json(results);
        }

        [HttpGet, Route("data/{typeName}/{id}/{methodName}")]
        public virtual object Query([FromUri] string typeName, [FromUri] string id, [FromUri] string methodName) {

            var type = TryGetType(typeName);
            var method = type.GetMethods().FirstOrDefault(each => each.Name.ToLower() == methodName.ToLower());

            if (method.IsStatic) {
                throw new Exception("Only non-static methods can be invoked with an entity id.");
            }

            var includes = method.GetAttributes<IncludeAttribute>().Select(each => each.PropertyName).ToArray();
            var entity = ODataRequestFacilitator.QueryEntityById(type, id, includes).FirstOrDefault();

            if(entity == null) {
                throw new Exception("Invalid entity.");
            }

            /*if (entity == null) {
                throw new Exception("Invalid entity id.");
            }*/

            /*var access = method.GetCustomAttribute<AccessAttribute>();

            if (!CurrentUser.IsAuthenticated || CurrentUser.Role != RoleTypes.SiteAdmin) {
                if (access == null) {
                    return this.Error("Only administrators can access this resource.");
                }
            }*/

            var invoker = MethodInvoker.GetDefault(method);
            var results = invoker.Invoke(entity, Request.ToDictionary());
            
            return Json(results);
        }

        private object Json(object data) {
            return Content(HttpStatusCode.OK, data, Formatter, new MediaTypeHeaderValue("application/json"));
        }

        private static readonly JsonMediaTypeFormatter Formatter = new JsonMediaTypeFormatter {
            SerializerSettings = JsonSerializerSettingPresets.Minimal
        };

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
        */
    }
}