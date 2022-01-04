using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Games.Helpers
{
    public class HttpHelper
    {
        static readonly HttpClient client = new HttpClient();
        public async Task<string> SendGetRequest(string path)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(path);
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                var response = new
                {
                    ErrorCode = 1000,
                    ErrorMessage = "Didn't connect to API",
                    IsSuccess = false
                };
                string responseString = JsonConvert.SerializeObject(response);
                return responseString;
            }

        }
        public async Task<string> SendPostRequest(string path, string serializedRequest)
        {
            try
            {
                StringContent content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(path, content);
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                var response = new
                {
                    ErrorCode = 1000,
                    ErrorMessage = "Didn't connect to API",
                    IsSuccess = false
                };
                string responseString = JsonConvert.SerializeObject(response);
                return responseString;
            }

        }
        public async Task<string> SendPutRequest(string path, string serializedRequest)
        {
            try
            {
                StringContent content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(path, content);
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                var response = new
                {
                    ErrorCode = 1000,
                    ErrorMessage = "Didn't connect to API",
                    IsSuccess = false
                };
                string responseString = JsonConvert.SerializeObject(response);
                return responseString;
            }

        }
        public async Task<string> SendDeleteRequest(string path)
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(path);
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                var response = new
                {
                    ErrorCode = 1000,
                    ErrorMessage = "Didn't connect to API",
                    IsSuccess = false
                };
                string responseString = JsonConvert.SerializeObject(response);
                return responseString;
            }

        }
    }
}
