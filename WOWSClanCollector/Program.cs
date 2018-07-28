using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace WOWSClanCollector
{
    class Program
    {
        private static string result;

        static void Main(string[] args)
        {
            Console.WriteLine("WOWS Clan Collector");
            Console.WriteLine("By.bunnyxt 2018-7-28");
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

            //debug
            //pageMax = 1;

            //get all clans via api
            for (int page = 1; page <= pageMax; page++)
            {
                Console.WriteLine("Start get ClanPack {0}...", page);
                clanPack = await Proxy.GetClanPack(page);
                Console.WriteLine("Get ClanPack {0} finished!", page);
                foreach (var clan in clanPack.data)
                {
                    clans.Add(clan);
                    Console.WriteLine("Clan {0} {1} {2} added!", clan.clan_id, clan.tag, clan.name);
                }
                Console.WriteLine("Clans in ClanPack {0} added!", page);
                Console.WriteLine();
            }

            //connect to database
            MySqlConnection conn;
            MySqlCommand command;
            MySqlDataReader reader;
            try
            {
                conn = new MySqlConnection("server=59.110.222.86;User Id=deto;password=WOWSdr2018;Database=wows_detonation;SslMode=None;charset=utf8");
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            Console.WriteLine("Database connected!");

            //create new aisa_clan_player_tmp table
            try
            {
                command = new MySqlCommand("CREATE TABLE `asia_clan_player_tmp` (  `cid` int(11) NOT NULL,  `id` int(11) NOT NULL,  PRIMARY KEY(`cid`,`id`),  KEY `id` (`id`),  CONSTRAINT FOREIGN KEY(`id`) REFERENCES `asia_player` (`id`),  CONSTRAINT FOREIGN KEY(`cid`) REFERENCES `asia_clan` (`cid`)) ENGINE = InnoDB DEFAULT CHARSET = latin1;", conn);
                reader = command.ExecuteReader();
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            //deal whth each clan
            foreach (var clan in clans)
            {
                //Console.WriteLine("{0} {1} {2}", clan.clan_id, clan.tag, clan.name);

                //get clan detail
                ClanDetail clanDetail = await Proxy.GetClanDetail(clan.clan_id);

                int cid = -1;

                //check new clan or not
                command = new MySqlCommand("SELECT * FROM wows_detonation.asia_clan where clan_id = " + clanDetail.clan_id, conn);
                reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    //new clan
                    reader.Close();

                    //insert new clan
                    command = new MySqlCommand("INSERT INTO `wows_detonation`.`asia_clan` (`clan_id`, `created_at`, `tag`, `name`) VALUES (" + clanDetail.clan_id + ", " + clanDetail.created_at + ", '" + clanDetail.tag + "', '" + Modify(clanDetail.name) + "');", conn);
                    reader = command.ExecuteReader();
                    reader.Close();
                    Console.WriteLine("new clan " + clanDetail.tag);

                    //get cid
                    command = new MySqlCommand("SELECT * FROM wows_detonation.asia_clan where clan_id = " + clanDetail.clan_id, conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        cid = reader.GetInt32(0);
                    }
                    reader.Close();
                }
                if (!reader.IsClosed)
                {
                    //old clan
                    cid = reader.GetInt32(0);
                    string oldTag = reader.GetString(3);
                    string oldName = reader.GetString(4);
                    reader.Close();
                    Console.WriteLine("old clan " + clanDetail.tag);

                    //update old clan tag and name
                    if (oldTag != clanDetail.tag)
                    {
                        Console.WriteLine("clan tag changed from " + oldTag + " to " + clanDetail.tag + " !");
                        Console.WriteWarning("clan tag changed from " + oldTag + " to " + clanDetail.tag + " !");

                        command = new MySqlCommand("UPDATE `wows_detonation`.`asia_clan` SET `tag`='" + clanDetail.tag + "' WHERE `cid`='" + cid + "';", conn);
                        reader = command.ExecuteReader();
                        reader.Close();
                    }

                    if (oldName != clanDetail.name)
                    {
                        Console.WriteLine("clan name changed from " + oldName + " to " + clanDetail.name + " !");
                        Console.WriteWarning("clan name changed from " + oldName + " to " + clanDetail.name + " !");

                        command = new MySqlCommand("UPDATE `wows_detonation`.`asia_clan` SET `name`='" + clanDetail.name + "' WHERE `cid`='" + cid + "';", conn);
                        reader = command.ExecuteReader();
                        reader.Close();
                    }

                }

                //insert into asia_cian_player_tmp
                Console.WriteLine("now insert players in clan " + clanDetail.tag);
                foreach (var account_id in clanDetail.members_ids)
                {
                    //get player id
                    command = new MySqlCommand("SELECT * FROM wows_detonation.asia_player where account_id = " + account_id, conn);
                    reader = command.ExecuteReader();
                    int id = -1;
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                    reader.Close();

                    //check if player exist
                    if (id != -1)
                    {
                        //player exist
                        command = new MySqlCommand("INSERT INTO `wows_detonation`.`asia_clan_player_tmp` (`cid`, `id`) VALUES (" + cid + ", " + id + ");", conn);
                        reader = command.ExecuteReader();
                        reader.Close();
                        Console.WriteLine(cid + "," + id + " inserted!");
                    }
                    else
                    {
                        //player not exist cause special reasons
                        Console.WriteLine("special account_id " + account_id + " detected!");
                        Console.WriteWarning("special account_id " + account_id);
                    }
                }

                Console.WriteLine();

            }

            //drop old table
            command = new MySqlCommand("DROP TABLE `wows_detonation`.`asia_clan_player`;", conn);
            reader = command.ExecuteReader();
            Console.WriteLine("Drop old table succeed!");
            reader.Close();

            //alter tmp name to asia_clan_player
            command = new MySqlCommand("ALTER TABLE `wows_detonation`.`asia_clan_player_tmp` RENAME TO  `wows_detonation`.`asia_clan_player` ; ", conn);
            command.ExecuteReader();
            reader.Close();
            Console.WriteLine("Alter tmp name succeed!");

            conn.Close();
            Console.WriteLine("Database connection closed!");
        }

        static string Modify(string str)
        {
            string pattern = "'";
            Regex regex = new Regex(pattern);
            string result = regex.Replace(str, "''");
            return result;
        }
    }
}
