using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindGame.GameProperties;

namespace MindGame.Models
{
    public class MindGameSelectGameViewModel : ObservableObject
    {
        private bool useCurrentFilter = true; //API.Instance.MainView.FilteredGames
        private ObservableCollection<Game> games = new ObservableCollection<Game>();
        private ObservableCollection<MindGameCondition> conditions = new ObservableCollection<MindGameCondition>();
        private MindGameCondition currentCondition = new MindGameCondition();
        private Game game;
        private RelayCommand<Guid> _GoToGame;
        private readonly MindGame app;
        private readonly Random random = new Random();

        public bool UseCurrentFilter { get => useCurrentFilter; set => SetValue(ref useCurrentFilter, value); } //LOCRandomGameLimistToFilter
        public ObservableCollection<MindGameCondition> Conditions { get => conditions; set => SetValue(ref conditions, value); }
        public MindGameCondition CurrentCondition { get => currentCondition; set => SetValue(ref currentCondition, value); }

        public ObservableCollection<Game> Games {get => games; set =>SetValue(ref games, value);}
        public Game Game { get => game; set => SetValue(ref game,value); }

        public bool HasGame => games.Any();

        internal void Init()
        {
            ReadGames();
            conditions.Clear();
            Next();
        }


        public MindGameSelectGameViewModel()
        {
            app = (MindGame)API.Instance.Addons.Plugins.FirstOrDefault(p => p.GetType() == typeof(MindGame));
            _GoToGame = new RelayCommand<Guid>((Id) =>
            {
                API.Instance.MainView.SelectGame(Id);
                API.Instance.MainView.SwitchToLibraryView();
            });
        }

        public void ReadGames()
        {
            Games.Clear();
            if (UseCurrentFilter)
            {
                API.Instance.MainView.FilteredGames.OrderBy(g => Guid.NewGuid()).ForEach(game => Games.Add(game));
            }
            else
            {
                API.Instance.Database.Games.OrderBy(g => Guid.NewGuid()).ForEach(game => Games.Add(game));
            }
            Game = Games[0];
        }
        public void Ignore()
        {
            if (CurrentCondition.Id == null) return;
            MindGameData data = app.Data;
            if (!data.IgnoredProperites.TryGetValue(CurrentCondition.Type.Name, out List<Guid> ignored))
            {
                ignored = new List<Guid>();
                data.IgnoredProperites.Add(CurrentCondition.Type.Name, ignored);
            }
            ignored.Add(CurrentCondition.Id.Value);
            data.Save();
            Next();
        }

        public void Next()
        {
            int retries = 20;
            Guid? value = null;
            IMindGameProperty type = null;
            Game game = null;
            while (retries-- > 0)
            {
                if (Games.Count == 0) break;
                game = Games[random.Next(0, Games.Count)];

                type = 
                    app.PropertyTypes
                    .Where(p => p.IsAllowed(app.Settings))
                    .OrderBy(x => Guid.NewGuid())
                    .FirstOrDefault();

                if (type == null)
                {
                    game = null;
                    continue;
                }

                value = type
                    .GetIds(game)
                    .Distinct()
                    .Where(v => !conditions.Any((c) => c.Type.Name == type.Name && c.Id == v)
                            && !app.Data.Contains(type.Name, v))
                    .OrderBy(v => Guid.NewGuid())
                    .FirstOrDefault();

                if (value == null || value == Guid.Empty)
                {
                    value = null;
                    game = null;
                    type = null;
                    continue;
                }

                if (string.IsNullOrEmpty(type.GetValue(value.Value))) continue;

                break;
            }
            Game = game;
            CurrentCondition.Type= type;
            CurrentCondition.Id = value;
        }

        internal void OptionSelected(string name)
        {
            if (games.Count <= 1) return;
            switch (name)
            {
                case "Yes":
                    currentCondition.ConditionOperator = MindGameCondition.ConditionOperatorType.Has;
                    break;
                case "Maybe":
                    currentCondition.ConditionOperator = MindGameCondition.ConditionOperatorType.DoesNotMatter;
                    break;
                case "No":
                    currentCondition.ConditionOperator = MindGameCondition.ConditionOperatorType.HasNot;
                    break;
            }
            Conditions.Add(CurrentCondition);
            for (int i = 0; i < Games.Count;)
            {
                bool hasit = CurrentCondition.Type.GetIds(Games[i]).Contains(currentCondition.Id.Value);
                if (hasit && CurrentCondition.ConditionOperator == MindGameCondition.ConditionOperatorType.HasNot)
                {
                    Games.RemoveAt(i);
                    continue;
                }
                if (!hasit && CurrentCondition.ConditionOperator == MindGameCondition.ConditionOperatorType.Has)
                {
                    Games.RemoveAt(i);
                    continue;
                }
                i++;
            }
            CurrentCondition = new MindGameCondition();
            Next();
        }

        public RelayCommand<Guid> GoToGame
        {
            get => _GoToGame;
            set => _GoToGame = value;
        }
    }
}