using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;

namespace Starship.Web.Extensions {

    public static class HttpExtensions {

        public static Dictionary<string, object> ToDictionary(this HttpRequestMessage request) {
            var dictionary = new Dictionary<string, object>();

            foreach (var pair in request.GetQueryNameValuePairs()) {
                if (dictionary.ContainsKey(pair.Key)) {
                    var collection = dictionary[pair.Key] as IList;

                    if (collection == null) {
                        var existingValue = dictionary[pair.Key];
                        collection = (IList) (dictionary[pair.Key] = new List<object> {
                            existingValue
                        });
                    }

                    collection.Add(pair.Value);
                }
                else {
                    dictionary.Add(pair.Key, pair.Value);
                }
            }

            return dictionary;
        }

        public static Uri GetRoot(this HttpContext context) {
            return new Uri(context.Request.Url.Scheme + "://" + context.Request.Url.Authority + "/");
        }

        public static Uri GetRootSegment(this HttpContext context) {
            return new Uri(context.Request.Url.Scheme + "://" + context.Request.Url.Authority + "/" + context.Request.Url.Segments[1]);
        }
    }
}