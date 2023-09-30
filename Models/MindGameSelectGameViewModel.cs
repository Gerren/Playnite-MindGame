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
        public static MindGameSelectGameViewModel Instance { get; set; }

        private bool useCurrentFilter = true; //API.Instance.MainView.FilteredGames
        private ObservableCollection<Game> games = new ObservableCollection<Game>();
        private ObservableCollection<MindGameCondition> conditions = new ObservableCollection<MindGameCondition>();
        private MindGameCondition currentCondition = new MindGameCondition();
        private Game game;
        private RelayCommand<Guid> _GoToGame;
        private readonly MindGamePlugin app;
        private readonly Random random = new Random();

        public bool UseCurrentFilter { get => useCurrentFilter; set => SetValue(ref useCurrentFilter, value); } //LOCRandomGameLimistToFilter
        public ObservableCollection<MindGameCondition> Conditions { get => conditions; set => SetValue(ref conditions, value); }
        public MindGameCondition CurrentCondition { get => currentCondition; set => SetValue(ref currentCondition, value); }

        public ObservableCollection<Game> Games { get => games; set => SetValue(ref games, value); }
        public Game Game { get => game; set => SetValue(ref game, value); }

        public bool HasGame => games.Any();

        internal void DoInit()
        {
            NoMore = false;
            ReadGames();
            conditions.Clear();
            Next();
        }

        internal void Init()
        {
            if (Instance != null)
            {
                Conditions = Instance.Conditions;
                CurrentCondition = Instance.CurrentCondition;
                Game = Instance.Game;
                Games = Instance.Games;
                NoMore = Instance.NoMore;
                Prompt = Instance.Prompt;
                UseCurrentFilter = Instance.UseCurrentFilter;
            }
            else
            {
                DoInit();
            }
            Instance = this;
        }


        public MindGameSelectGameViewModel()
        {
            app = (MindGamePlugin)API.Instance.Addons.Plugins.FirstOrDefault(p => p.GetType() == typeof(MindGamePlugin));
            _GoToGame = new RelayCommand<Guid>((Id) =>
            {
                API.Instance.MainView.SelectGame(Id);
                API.Instance.MainView.SwitchToLibraryView();
            });
            _RemoveCondition = new RelayCommand<Guid>((Id) =>
            {
                MindGameCondition condition = conditions.First(cond => cond.Id == Id);
                conditions.Remove(condition);
                Next();
            });

            prompts = new string[] {ResourceProvider.GetString("LOCMindGamePrompt1"),
                ResourceProvider.GetString("LOCMindGamePrompt2"),
                ResourceProvider.GetString("LOCMindGamePrompt3"),
                ResourceProvider.GetString("LOCMindGamePrompt4"),
                ResourceProvider.GetString("LOCMindGamePrompt5"),
                ResourceProvider.GetString("LOCMindGamePrompt6"),
                ResourceProvider.GetString("LOCMindGamePrompt7"),
            };

        }

        private void RollPrompt() => Prompt = prompts[random.Next(prompts.Length)];

        private string[] prompts;
        private RelayCommand<Guid> _RemoveCondition;
        private bool noMore;

        public string Prompt { get; set; }

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

        private class LeaderType
        {
            public Guid id;
            public int count = 0;
            public IMindGameProperty type;
            public Game game;
        }

        public void Next()
        {
            if (games.Count == 0) return;

            RollPrompt();
            OnPropertyChanged(nameof(Prompt));

            Dictionary<Guid, LeaderType> leaders = new Dictionary<Guid, LeaderType>();

            foreach (Game game in Games.OrderBy(g => Guid.NewGuid()))
            {
                app.PropertyTypes
                .Where(p => p.IsAllowed(app.Settings))
                .ForEach(p => p.GetIds(game).ForEach(id =>
                {
                    if (id == null) return;
                    if (p.GetValue(id) == null) return;
                    if (conditions.Any(c => c.Id == id)) return;
                    if (app.Data.Contains(p.Name, id)) return;
                    LeaderType leader = null;
                    if (leaders.TryGetValue(id, out LeaderType found)) leader = found;
                    if (leader == null)
                    {
                        leader = new LeaderType
                        {
                            id = id,
                            game = game,
                            type = p
                        };
                        leaders.Add(id, leader);
                    }
                    leader.count++;
                }
                ));
            }

            int negative = 1;
            foreach (MindGameCondition condition in conditions.Reverse())
            {
                switch (condition.ConditionOperator.Value)
                {
                    case MindGameCondition.ConditionOperatorType.Has:
                        break;
                    default:
                        negative++;
                        continue;
                }
            }

            int len = games.Count / 5; // take inversely many, as is percentage of negative or tenative answers.
            if (len > 20) len = 20;
            len /= negative;
            if (len < 5) len = 5;


            LeaderType topLeader =
                leaders.Values
                    .OrderBy((_) => Guid.NewGuid())
                    .OrderBy(leader => Math.Abs(leader.count - games.Count / 2)) // the closer to middle, the better
                    .Take(len)
                    .OrderBy((_) => Guid.NewGuid())
                    .FirstOrDefault();

            if (topLeader == null)
            {
                NoMore = true;
                return;
            }

            Game = topLeader.game;
            CurrentCondition.Type = topLeader.type;
            CurrentCondition.Id = topLeader.id;
        }

        internal void OptionSelected(string name)
        {
            if (games.Count <= 1)
            {
                NoMore = true;
                return;
            }

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
            if (games.Count == 1)
            {
                Game=games[0];
                NoMore = true;
                return;
            }
            CurrentCondition = new MindGameCondition();
            Next();
        }

        public RelayCommand<Guid> GoToGame
        {
            get => _GoToGame;
            set => _GoToGame = value;
        }
        public RelayCommand<Guid> RemoveCondition
        {
            get => _RemoveCondition;
            set => _RemoveCondition = value;
        }
        public bool NoMore { get => noMore; private set => SetValue(ref noMore, value); }
    }
}