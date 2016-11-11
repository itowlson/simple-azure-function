using System;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    string text = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "text", true) == 0)
        .Value;

    var responseText = String.IsNullOrWhiteSpace(text)
        ? "Don't cower behind the clock of anonymity - get back to work"
        : $"By Crom, {text}, stop mucking around on Slack or you'll feel the lick of the cat";

    return req.CreateResponse(HttpStatusCode.OK, responseText);
}
