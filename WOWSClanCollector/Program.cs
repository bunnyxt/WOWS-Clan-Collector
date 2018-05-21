using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace WOWSClanCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WOWS Clan Collector");
            Console.WriteLine("By.bunnyxt 2018-5-21");
            Console.WriteLine();

            Console.WriteLine("Start");
            Start();
            Console.ReadLine();

        }

        static async void Start()
        {
            List<Clan> clans = new List<Clan>();

            //get pageMax
            Console.WriteLine("Start get pageMax...");
            ClanPack clanPack = await Proxy.GetClanPack(1);
            int pageMax = clanPack.meta.total / 100 + 1;
            Console.WriteLine("pageMax = {0}", pageMax);

            //get all clans via api
            for (int page = 1; page <= pageMax; page++)
            {
                Console.WriteLine("Start get ClanPack {0}...", page);
                clanPack = await Proxy.GetClanPack(page);
                Console.WriteLine("Get ClanPack {0} finished!", page);
                foreach (var clan in clanPack.data)
                {
                    clans.Add(clan);
                    //Console.WriteLine("Clan {0} {1} {2} added!", clan.clan_id, clan.tag, clan.name);
                }
                Console.WriteLine("Clans in ClanPack {0} added!");
                Console.WriteLine();
            }

            //connect to database
            MySqlConnection conn;
            try
            {
                conn = new MySqlConnection("server=59.110.222.86;User Id=deto;password=WOWSdr2018;Database=wows_detonation");
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            Console.WriteLine("Database connected!");

            //create new aisa_clan_player_tmp table
            MySqlCommand command = new MySqlCommand("CREATE TABLE `asia_clan_player_tmp` (`cid` int(11) NOT NULL,  `id` int(11) NOT NULL,  PRIMARY KEY(`cid`,`id`)) ENGINE = InnoDB DEFAULT CHARSET = latin1", conn);
            command.ExecuteReader();

            //deal whth each clan
            foreach (var clan in clans)
            {
                //Console.WriteLine("{0} {1} {2}", clan.clan_id, clan.tag, clan.name);

                //get clan detail
                ClanDetail clanDetail = await Proxy.GetClanDetail(clan.clan_id);

                //check new clan or not

                //get all clan menbers' account_id

                //insert into asia_cian_player_tmp

            }

            //drop old table
            command = new MySqlCommand("DROP TABLE `wows_detonation`.`asia_clan_player`;", conn);
            command.ExecuteReader();

            //alter tmp name to asia_clan_player
            command = new MySqlCommand("ALTER TABLE `wows_detonation`.`asia_clan_player_tmp` RENAME TO  `wows_detonation`.`asia_clan_player` ; ", conn);
            command.ExecuteReader();

            conn.Close();
        }
    }
}
