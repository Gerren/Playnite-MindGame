using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindGame.GameProperties
{
    internal class MindGameCompletion : IMindGameProperty
    {
        public string Name => "CompletionStatus";

        public string Label => ResourceProvider.GetString("LOCCompletionStatus");

        public string GetValue(Guid id) => API.Instance.Database.CompletionStatuses.Where(v =>v.Id == id).Select(v => v.Name).FirstOrDefault();

        public List<Guid> GetIds(Game game) => new List<Guid>() { game.CompletionStatusId } ?? new List<Guid>();

        public bool IsAllowed(MindGameSettings settings) => settings.Settings.ContainsKey(Name) ? settings.Settings[Name] : true;
    }
}
