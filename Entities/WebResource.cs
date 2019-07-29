using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Starship.Core.Data;
using Starship.Core.Extensions;
using Starship.Web.Enumerations;
using Starship.Web.Http;

namespace Starship.Web.Entities {
    public class WebResource : CosmosResource {

        public WebResource() {
        }

        public WebResource(string source, string pageUrl, ContentItem item) {

            Type = "webresource";
            Id = Guid.NewGuid().ToString();
            Source = source;
            ExternalId = CleanUrl(item.Id);
            ParentUrl = pageUrl;
            Url = item.FileUrl;
            Extension = new Uri(item.FileUrl).AbsolutePath.Split('.').Last();
            Title = item.Title;
            Duration = item.Duration;
            Description = item.Description;
            DateAdded = item.DateAdded;
            StatusText = string.Empty;
            ContentType = item.ContentType;

            foreach (var keyword in item.Keywords) {
                var value = keyword.Trim().ToLower();

                if (string.IsNullOrEmpty(value)) {
                    continue;
                }

                Keywords += value + " ";
            }
        }

        public List<KeyValuePair<string, int>> GetKeywordMatches() {
            
            var keywords = new Dictionary<string, int> {

                { "pre-vc", 5 },
                { "startup", 5 },
                { "idea", 5 },
                { "funding", 5 },
                { "co-founder", 5 },
                { "cofounder", 5 },
                { "entrepreneur", 5 },
                { "telecommut", 5 },

                { "consult", 4 },
                { "vue", 4 },

                { "work remotely", 3 },
                { "remote office", 3 },
                { "urgent", 3 },
                { "ad campaign", 3 },
                { "C#", 3 },
                { "fullstack", 3 },
                { "full-stack", 3 },
                { "asp.net", 3 },
                { "early stage", 3 },
                { "signalr", 3 },
                { "csharp", 3 },

                { "senior", 2 },
                { "javascript", 2 },
                { "google ad", 2 },
                { "adword", 2 },
                { "remote", 2 },
                { "local candidates preferred", 2 },

                { "development", 1 },
                { "developer", 1 },
                { "programmer", 1 },
                { "web design", 1 },
                { "facebook ad", 1 },
                { "app", 1 },
                { "software", 1 },

                { "python", -1 },
                { "ruby", -1 },
                { "apache", -1 },
                { "php", -1 },
                { "unix", -1 },
                { "sitecore", -1 },
                { "shell script", -1 },
                { "macos", -1 },
                { "vm ware", -1 },
                { "vmware", -1 },
                { "rabbitmq", -1 },
                { "nginx", -1 },
                { "click here to apply", -1 },
                { "our corporate culture", -1 },
                { "computer science preferred", -1 },
                { "perl", -1 },
                
                { "db2", -2 },
                { "elasticsearch", -2 },
                { "marketo", -2 },
                { "postgresql", -2 },
                { "greencard", -2 },
                { "docker", -2 },
                { "blockchain", -2 },
                { "specflow", -2 },
                { "jasmine", -2 },
                { "cybersecurity", -2 },
                { "bs/ba", -2 },
                { "sitefinity", -2 },
                { "kentico", -2 },
                { "bigcommerce", -2 },
                { "angular", -2 },
                { "magneto", -2 },
                { "hipaa complian", -2 },
                { "silverlight", -2 },
                { "visa sponsor", -2 },
                { "comptia", -2 },
                { "cissp", -2 },
                { "one-time survey", -2 },
                { "take the survey", -2 },

                { "paid sick time", -3 },
                { "complete your application", -3 },
                { "datapower", -3 },
                { "red hat", -3 },
                { "jboss", -3 },
                { "xstl", -3 },
                { "dynamics 365", -3 },
                { "no agencies", -3 },
                { "c++", -3 },
                { "golang", -3 },
                { "backbone.js", -3 },
                { "salesforce", -3 },
                { "associate's degree", -3 },
                { "bachelor's degree", -3 },
                { "associate degree", -3 },
                { "bachelor degree", -3 },
                { "tomcat", -3 },
                { "redhat", -3 },
                { "centos", -3 },
                { "django", -3 },
                { "vb.net", -3 },
                { "must have bs degree", -3 },
                { "travel up to ", -3 },
                { "submit your application", -3 },
                { "job opening id", -3 },
                { "objective-c", -3 },
                { "objective c", -3 },
                { "hadoop", -3 },
                { "osx", -3 },
                { "weeks paid vacation time", -3 },
                { "candidate will have a bachelor’s degree", -3 },
                { "proficient in microsoft office", -3 },
                { "our survey", -3 },
                { "participants will be compensated", -3 },

                { "sonicwall", -4 },
                { "bi-lingual", -4 },
                { "bilingual", -4 },
                { "bash/linux", -4 },
                { "seeking local candidates", -4 },
                
                { "day per week of remote", -5 },
                { "benefits package:", -5 },
                { "in-house full time position", -5 },
                { "in-house position", -5 },
                { "stl container", -5 },
                { "area candidates only", -5 },
                { "j2ee", -5 },
                { "catered lunches", -5 },
                { "sharepoint", -5 },
                { "valid us driver", -5 },
                { "equal opportunity employer", -5 },
                { "spanish", -5 },
                { "no remote", -5 },
                { "telework opportunities", -5 },
                { "college degree required", -5 },
                { "kubernetes", -5 },
                { "vbscript", -5 },
                { "vba macro", -5 },
                { "xcode", -5 },
                { "android studio", -5 },
                { "cth/ perm", -5 },
                { "cth / perm", -5 },
                { "laravel", -5 },
                { "technical product manager", -5 },
                { "% travel", -5 },
                { "h1-b", -5 },
                { "h1b", -5 },
                { "qualified candidates will", -5 },

                { "must be able to commute to office", -10 },
                { "casual business dress", -10 },
                { "401k", -10 },
                { "401(k)", -10 },
                { "pet friendly", -10 },
                { "pet-friendly", -10 },
                { "reference job posting id", -10 },
                { "drug test", -10 },
                { "drug screen", -10 },
                { "drug free workplace", -10 },
                { "drug-free workplace", -10 },
                { "high school diploma or GED", -10 },
                { "hs diploma", -10 },
                { "h.s. diploma", -10 },
                { "on-site only", -10 },
                { "LPCXpresso", -10 },
                { "not a remote position", -10 },
                { "this is not a remote", -10 },
                { "must work onsite", -10 },
                { "relocation provided", -10 }
            };

            return keywords.Where(keyword => Description.ToLower().Contains(keyword.Key.ToLower())).ToList();
        }

        public int GetScore() {
            
            var dealbreakerKeywords = new List<string> {
                "sql",
                "entry level",
                "entry-level",
                "data annotation",
                "junior developer",
                "jr developer",
                "qa analyst",
                "quality assurance",
                "tester/qa",
                "lead instructor",
                "software tester",
                "project manager",
                "c++",
                "product manager",
                "salesforce",
                "wordpress",
                "django",
                "j2ee",
                "mac os",
                "help desk",
                "oracle",
                "sharepoint"
            };

            foreach(var keyword in dealbreakerKeywords) {
                if(Title.ToLower().Contains(keyword.ToLower())) {
                    return 0;
                }
            }
            
            var score = GetKeywordMatches().Sum(each => each.Value);

            var titleKeywords = new List<string> {
                "intern",
                "junior",
                "training program",
                "qa automation",
                "quality assurance",
                "assistant",
                "pc tech",
                "tester",
                "business analyst",
                "customer support",
                "customer service",
                "instructor"
            };

            foreach(var keyword in titleKeywords) {
                if(Title.ToLower().StartsWith(keyword + " ") || Title.ToLower().EndsWith(" " + keyword) || Title.ToLower().Contains(" " + keyword + " ")) {
                    score -= 5;
                }
            }

            var bonusKeywords = new List<string> {
                "pre-vc",
                "startup",
                "idea",
                "funding",
                "co-founder",
                "senior",
                "cofounder",
                "consult"
            };

            foreach(var keyword in bonusKeywords) {
                if(Title.ToLower().Contains(keyword)) {
                    score += 5;
                }
            }
            
            score -= Description.ToLower().CountOccurancesOf("www.");

            if(Description.ToLower().CountOccurancesOf("+ years") >= 3) {
                score -= 5;
            }

            if(Description.Length <= 1000) {
                score += 3;
            }

            if(Description.Length > 2000) {
                score -= Description.Length / 1000;
            }

            return score;
        }

        private string CleanUrl(string url) {
            const string token = "-";

            return url
                .Replace("/", token)
                .Replace(@"\", token)
                .Replace("#", token)
                .Replace("?", token);
        }

        public string GetFilename() {
            return ExternalId + "." + Extension;
        }

        public bool IsDownloadable() {
            if(DateAdded.HasValue && DateAdded.HasElapsed(TimeSpan.FromDays(3))) {
                return false;
            }

            return ContentType == FileContentTypes.Video || ContentType == FileContentTypes.Image || ContentType == FileContentTypes.Html;
        }
        
        [JsonProperty("externalId")]
        public string ExternalId {
            get => Get<string>("externalId");
            set => Set("externalId", value);
        }

        [JsonProperty("source")]
        public string Source {
            get => Get<string>("source");
            set => Set("source", value);
        }
        
        [JsonProperty("extension")]
        public string Extension {
            get => Get<string>("extension");
            set => Set("extension", value);
        }

        [JsonProperty("url")]
        public string Url {
            get => Get<string>("url");
            set => Set("url", value);
        }

        [JsonProperty("parentUrl")]
        public string ParentUrl {
            get => Get<string>("parentUrl");
            set => Set("parentUrl", value);
        }

        [JsonProperty("keywords")]
        public string Keywords {
            get => Get<string>("keywords");
            set => Set("keywords", value);
        }

        [JsonProperty("title")]
        public string Title {
            get => Get<string>("title");
            set => Set("title", value);
        }

        [JsonProperty("description")]
        public string Description {
            get => Get<string>("description");
            set => Set("description", value);
        }

        [JsonProperty("duration")]
        public int Duration {
            get => Get<int>("duration");
            set => Set("duration", value);
        }

        [JsonProperty("dateAdded")]
        public DateTime? DateAdded {
            get => Get<DateTime?>("dateAdded");
            set => Set("dateAdded", value);
        }

        [JsonProperty("isRemoved")]
        public bool IsRemoved {
            get => Get<bool>("isRemoved");
            set => Set("isRemoved", value);
        }

        [JsonProperty("size")]
        public long Size {
            get => Get<long>("size");
            set => Set("size", value);
        }

        [JsonProperty("hash")]
        public string Hash {
            get => Get<string>("hash");
            set => Set("hash", value);
        }

        [JsonProperty("status")]
        public FileStatusTypes Status {
            get => Get<FileStatusTypes>("status");
            set => Set("status", value);
        }

        [JsonProperty("statusText")]
        public string StatusText {
            get => Get<string>("statusText");
            set => Set("statusText", value);
        }

        [JsonProperty("contentType")]
        public FileContentTypes ContentType {
            get => Get<FileContentTypes>("contentType");
            set => Set("contentType", value);
        }

        [JsonProperty("score")]
        public int Score {
            get => Get<int>("score");
            set => Set("score", value);
        }
    }

    public static class BaseEntityExtensions {
        public static IQueryable<WebResource> ByRemoteIdentifier(this IQueryable<WebResource> resource, string source, string identifier) {
            return resource.Where(each => each.Type == "webresource" && each.Source == source && each.ExternalId == identifier);
        }
    }
}