using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WOWSClanCollector
{
    public static class Proxy
    {
        public static async Task<ClanPack> GetClanPack(int page)
        {
            ClanPack clanPack = new ClanPack();

            bool flag = true;
            while (flag)
            {
                try
                {
                    var url = String.Format("https://api.worldofwarships.asia/wows/clans/list/?application_id=ff57f966d5a13e4a240ab7218b889a18&page_no={0}", page.ToString());
                    var http = new HttpClient();
                    var response = await http.GetAsync(url);
                    Console.WriteLine("http response got");

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("response.StatusCode != HttpStatusCode.OK");
                    }
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("string result got");

                    clanPack = JsonConvert.DeserializeObject<ClanPack>(result);
                    Console.WriteLine("clanPack deserialized");

                    if (clanPack.status != "ok")
                    {
                        throw new Exception("clanPack.status != \"ok\"");
                    }

                    flag = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("now retry...");
                }
            }

            return clanPack;
        }

        public static async Task<ClanDetail> GetClanDetail(int clan_id)
        {
            return new ClanDetail();
        }
    }
}
