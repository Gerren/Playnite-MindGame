using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindGame.GameProperties
{
    internal class MindGameTag : IMindGameProperty
    {
        public string Name => "GameTag";

        public string Label => ResourceProvider.GetString("LOCTagLabel");

        public string GetValue(Guid id) => API.Instance.Database.Tags.Where(v =>v.Id == id).Select(v => v.Name).FirstOrDefault();

        public List<Guid> GetIds(Game game) => game.TagIds ?? new List<Guid>();

        public bool IsAllowed(MindGameSettings settings) => settings.UseTag;
    }
}
