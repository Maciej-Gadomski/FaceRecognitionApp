using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FaceRecognitonApp
{
    public class FaceApi
    {
        public static string subscriptionKey = "yourKey";
        public static string uriBase = "yourUri";

       public static bool CheckFileType(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            switch (ext.ToLower())
            {
                case ".jpg":
                    return true;
                case ".jpeg":
                    return true;
                case ".png":
                    return true;
                default:
                    return false;
            }
        }

        public static byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }
            return buffer;
        }


        public static async Task<string> FaceDetection(byte[] byteData)
        {
            string wynik = "";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            string requestParameters =
                "returnFaceId=true&returnFaceLandmarks=false,&returnfaceRectangle=false";
            // Assemble the URI for the REST API Call.
            string uri = uriBase + "/face/v1.0/detect?" + requestParameters;

            HttpResponseMessage response;

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                // Execute the REST API call.
                response = await client.PostAsync(uri, content);
                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                dynamic res = JsonConvert.DeserializeObject(contentString);
                try
                {
                    wynik = res[0].faceId;
                    return wynik;
                }
                catch
                {
                    wynik = "[]";
                    return wynik;
                }
            }
        }

        public static async Task<string> imageCompare(string wynik, string wynik2)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            object data = new
            {
                faceId1 = wynik,
                faceId2 = wynik2
            };

            // Assemble the URI for the REST API Call.

            string uri = uriBase + "/face/v1.0/verify?";

            HttpResponseMessage response;

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            response = await client.PostAsync(uri, content);
            string contentString = await response.Content.ReadAsStringAsync();

            dynamic res = JsonConvert.DeserializeObject(contentString);
            return res.isIdentical;
        }

    }
    }