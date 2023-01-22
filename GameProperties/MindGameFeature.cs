using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindGame.GameProperties
{
    internal class MindGameFeature: IMindGameProperty
    {
        public string Name => "Feature";

        public string Label => ResourceProvider.GetString("LOCFeatureLabel");

        public string GetValue(Guid id) => API.Instance.Database.Features.Where(v =>v.Id == id).Select(v => v.Name).FirstOrDefault();

        public List<Guid> GetIds(Game game) => game.FeatureIds ?? new List<Guid>();

        public bool IsAllowed(MindGameSettings settings) => settings.UseFeature;
    }
}
