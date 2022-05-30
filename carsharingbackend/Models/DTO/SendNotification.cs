using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace carsharingbackend.Models.DTO
{
    public class SendNotification
    {
        public static string Send(string token, string title, string body)
        {
            var result = "-1";
            var webAddr = "https://fcm.googleapis.com/fcm/send";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "key=AAAAZ_luCMs:APA91bEm_pTMyLwsPgyCA440wjD1if9lShxRBtPRzJTX2CSaMlcnsvOQp7sr9iOKDRwoIWa8f7i0yWVzS7aNf323CW0p-ZUU37GqhTd8YR04Nn0exZilwAAQrceLyp1GsbgMYcQ12y0R");
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string strNJson = ObtenerCuerpoNotificacion(token, title, body);
                streamWriter.Write(strNJson);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }

        private static string ObtenerCuerpoNotificacion(string token, string titulo, string cuerpo)
        {
            string strn = @"{{""to"": ""{0}"", ""notification"": {{ ""title"": ""{1}"", ""body"": ""{2}"", ""sound"": ""default""}}}}";
            string strNJson = string.Format(strn, token, titulo, cuerpo);
            return strNJson;
        }
    }
}