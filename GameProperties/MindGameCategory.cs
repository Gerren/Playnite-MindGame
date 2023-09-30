using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindGame.GameProperties
{
    internal class MindGameCategory : IMindGameProperty
    {
        public string Name => "Category";

        public string Label => ResourceProvider.GetString("LOCCategoryLabel");

        public string GetValue(Guid id) => API.Instance.Database.Categories.Where(v =>v.Id == id).Select(v => v.Name).FirstOrDefault();

        public List<Guid> GetIds(Game game) => game.CategoryIds ?? new List<Guid>();

        public bool IsAllowed(MindGameSettings settings) => settings.Settings.ContainsKey(Name) ? settings.Settings[Name] : true;
    }
}
