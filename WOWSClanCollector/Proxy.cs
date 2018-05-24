using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
                    Console.WriteWarning(e.Message);
                }
            }

            return clanPack;
        }

        public static async Task<ClanDetail> GetClanDetail(uint clan_id)
        {
            ClanDetailPack clanDetailPack = new ClanDetailPack();

            bool flag = true;
            while (flag)
            {
                try
                {
                    var url = String.Format("https://api.worldofwarships.asia/wows/clans/info/?application_id=ff57f966d5a13e4a240ab7218b889a18&clan_id={0}", clan_id.ToString());
                    var http = new HttpClient();
                    var response = await http.GetAsync(url);
                    Console.WriteLine("http response got");

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("response.StatusCode != HttpStatusCode.OK");
                    }
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("string result got");

                    string pattern = String.Format("\"{0}\"", clan_id);
                    Regex regex = new Regex(pattern);
                    result = regex.Replace(result, "\"clan_detail\"");

                    clanDetailPack = JsonConvert.DeserializeObject<ClanDetailPack>(result);
                    Console.WriteLine("clanPack deserialized");

                    if (clanDetailPack.status != "ok")
                    {
                        throw new Exception("clanPack.status != \"ok\"");
                    }

                    flag = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("now retry...");
                    Console.WriteWarning(e.Message);
                }
            }

            return clanDetailPack.data.clan_detail;
        }
    }
}
