using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOWSClanCollector
{
    public class ClanPack
    {
        public string status { get; set; }
        public ClanPackMeta meta { get; set; }
        public List<Clan> data { get; set; }
    }

    public class ClanPackMeta
    {
        public int count { get; set; }
        public int total { get; set; }
        public int page { get; set; }
    }

    public class Clan
    {
        public int members_count { get; set; }
        public int created_at { get; set; }
        public long clan_id { get; set; }
        public string tag { get; set; }
        public string name { get; set; }
    }

    public class ClanDetailPack
    {
        public string status { get; set; }
        public ClanDetailMeta count { get; set; }
        public ClanDetailData data { get; set; }
    }

    public class ClanDetailMeta
    {
        public int count { get; set; }
    }

    public class ClanDetailData
    {
        public ClanDetail clan_detail { get; set; }
    }

    public class ClanDetail
    {
        public int members_count { get; set; }
        public string name { get; set; }
        public string creator_name { get; set; }
        public long clan_id { get; set; }
        public int created_at { get; set; }
        public int updated_at { get; set; }
        public string leader_name { get; set; }
        public List<long> members_ids { get; set; }
        public long creator_id { get; set; }
        public string tag { get; set; }
        //public string old_name { get; set; }
        public bool is_clan_disbanded { get; set; }
        //public int renamed_at { get; set; }
        //public string old_tag { get; set; }
        public long leader_id { get; set; }
        public string description { get; set; }
    }
}
