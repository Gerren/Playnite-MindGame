using MindGame.GameProperties;
using MindGame.Models;
using MindGame.Views;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MindGame
{
    public class MindGame : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private MindGameData data;
        private readonly IMindGameProperty[] propertyTypes;
        private readonly MindGameViewSidebar side;

        private MindGameSettingsViewModel settings { get; set; }
        internal MindGameSettings Settings => settings.Settings;

        public override Guid Id { get; } = Guid.Parse("1f0c2867-28cf-4960-a2fc-869ee018664a");

        public IMindGameProperty[] PropertyTypes { get => propertyTypes; }

        public MindGame(IPlayniteAPI api) : base(api)
        {
            settings = new MindGameSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
            // Implement class, edit settings, edit settings WPF, that's all
            propertyTypes = new IMindGameProperty[] { 
                new MindGameGenre(),
                new MindGameCompletion(),
                new MindGameFeature(),
                new MindGameTag(),
                new MindGameCategory()
            };

            side = new MindGameViewSidebar();

        }

        public class MindGameViewSidebar : SidebarItem
        {
            public MindGameViewSidebar()
            {
                Type = SiderbarItemType.View;
                Title = ResourceProvider.GetString("LOCMindGameTitle");
                Icon = new TextBlock
                {
                    Text = "🎲",
                    FontSize = 22,
                };
                Opened = () =>
                {
                    MindGameSelectGameView ViewExtension = new MindGameSelectGameView();
                    ViewExtension.Init();
                    return ViewExtension;
                };
                Visible = true;
            }
        }
        public override IEnumerable<SidebarItem> GetSidebarItems()
        {
            return new List<SidebarItem>
            {
                side
            };
        }

        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(OnGameStartedEventArgs args)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
        }

        internal MindGameData Data
        {
            get
            {
                if (data == null)
                {
                    data = new MindGameData();
                    data.Load();
                }
                return data;
            }
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new MindGameSettingsView();
        }

    }
}