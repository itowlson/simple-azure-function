#r "Newtonsoft.Json.dll"
#r "System.Web.Http"
#r "System.Xml.Linq"

using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Xml.Linq;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    string text = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "text", true) == 0)
        .Value;

    var responseText = String.IsNullOrWhiteSpace(text)
        ? ""
        : await TranslateToFrench(text, Environment.GetEnvironmentVariable("TranslatorKey"));

    return req.CreateResponse(HttpStatusCode.OK, responseText);
}

public static async Task<string> TranslateToFrench(string text, string key)
{
    string authuri = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken?Subscription-Key=" + key;
    HttpWebRequest authWebRequest = (HttpWebRequest)WebRequest.Create(authuri);
    authWebRequest.Method = "POST";
    authWebRequest.ContentLength = 0;
    WebResponse authResponse = await authWebRequest.GetResponseAsync();
    string authToken;
    using (Stream stream = authResponse.GetResponseStream())
    {
        using (var reader = new StreamReader(stream))
        {
            authToken = reader.ReadToEnd();
        }
    }

    string headerValue = "Bearer " + authToken;

    string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Web.HttpUtility.UrlEncode(text) + "&from=en&to=fr";
    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
    httpWebRequest.Headers.Add("Authorization", headerValue);
    WebResponse response = await httpWebRequest.GetResponseAsync();
    using (Stream stream = response.GetResponseStream())
    {
        using (var reader = new StreamReader(stream))
        {
            var xml = XDocument.Parse(reader.ReadToEnd());
            return xml.Elements().Select(e => e.Value).FirstOrDefault();
        }
    }
}
