using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Playnite.SDK;
using Playnite.SDK.Data;

namespace MindGame.Models
{
    public class MindGameData : ObservableObject
    {
        private Dictionary<string, List<Guid>> ignoredProperites = new Dictionary<string, List<Guid>>();
        public Dictionary<string, List<Guid>> IgnoredProperites { get => ignoredProperites; set => ignoredProperites = value; }

        [DontSerialize]
        private string Path => System.IO.Path.Combine(API.Instance.Addons.Plugins.OfType<MindGamePlugin>().FirstOrDefault().GetPluginUserDataPath(),"data.json");

        public void Load()
        {
            try
            {
                if (Serialization.TryFromJson(System.IO.File.ReadAllText(Path), out MindGameData loaded))
                {
                    ignoredProperites = loaded.ignoredProperites;
                }
            }
            catch { }
        }

        public void Save()
        {
            System.IO.File.WriteAllText(Path,Serialization.ToJson(this));
        }

        public bool Contains(string type,Guid id)
        {
            return ignoredProperites.ContainsKey(type) && ignoredProperites[type].Contains(id);
        }

    }
}
