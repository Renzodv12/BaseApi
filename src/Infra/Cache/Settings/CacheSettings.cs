using System;
namespace Infra.Cache.Settings
{
    public enum Period
    {
        s, m, h, d
    }
    public class EntitySetting
    {
        public string Name { get; set; }
        public long Expiry { get; set; }
        public Period Period { get; set; }

    }
    public class CacheSettings
    {
        public List<EntitySetting> EntitySettings { get; set; }
        public string Endpoint { get; set; }

    }
}

