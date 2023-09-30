using MindGame.GameProperties;
using MindGame.Models;
using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MindGame
{
    public class MindGameSettings : ObservableObject
    {
        private bool useCategory = true;
        private bool useCompletionStatus = true;
        private bool useGenre = true;
        private bool useFeature = true;
        private bool useTag = true;

        // Cant be bothered to do the WPF dynamically
        [Obsolete]
        public bool UseCategory
        {
            get => useCategory; set
            {
                SetValue(ref useCategory, value);
                Settings["Category"] = value;
            }
        }
        [Obsolete]
        public bool UseCompletionStatus
        {
            get => useCompletionStatus; set
            {
                SetValue(ref useCompletionStatus, value);
                Settings["CompletionStatus"] = value;
            }
        }
        [Obsolete]
        public bool UseGenre
        {
            get => useGenre; set
            {
                SetValue(ref useGenre, value);
                Settings["Genre"] = value;
            }
        }
        [Obsolete]
        public bool UseFeature
        {
            get => useFeature; set
            {
                SetValue(ref useFeature, value);
                Settings["Feature"] = value;
            }
        }
        [Obsolete]
        public bool UseTag
        {
            get => useTag; set
            {
                SetValue(ref useTag, value);
                Settings["GameTag"] = value;
            }
        }

        public Dictionary<string, bool> Settings { get; set; } = new Dictionary<string, bool>();
        [DontSerialize]
        public ObservableCollection<MindGameCategorySettings> Categories { get; set; } = new ObservableCollection<MindGameCategorySettings>();
    }

    public class MindGameCategorySettings : ObservableObject
    {
        private bool useCategory;

        public string Label => Type.Label;
        public string Name => Type.Label;
        public bool UseCategory
        {
            get => useCategory; set
            {
                useCategory = value;
                UseChanged(value);
            }
        }
        public Action<bool> UseChanged { get; set; }
        public IMindGameProperty Type { get; set; }

        public ObservableCollection<MindGameCategoryItem> Items { get; set; } = new ObservableCollection<MindGameCategoryItem>();
    }

    public class MindGameCategoryItem
    {
        public string Label { get; set; }
        public Guid Id { get; set; }

        public string TypeName { get; set; }
    }

    public class MindGameSettingsViewModel : ObservableObject, ISettings
    {
        private readonly MindGamePlugin plugin;
        private MindGameSettings editingClone { get; set; }

        private MindGameSettings settings;

        public MindGameSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public ICommand ClearItem { get; set; }
        public ICommand ClearAll { get; set; }

        public MindGameSettingsViewModel(MindGamePlugin plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<MindGameSettings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new MindGameSettings();
            }

            Init();

            ClearAll = new RelayCommand<MindGameCategorySettings>(type =>
            {
                if (plugin.Data.IgnoredProperites.TryGetValue(type.Type.Name, out List<Guid> ignoredProperites))
                {
                    ignoredProperites.Clear();
                }
                Settings.Categories.First(c => c.Type.Name == type.Type.Name).Items.Clear();
            });
            ClearItem = new RelayCommand<MindGameCategoryItem>(item =>
            {
                if (plugin.Data.IgnoredProperites.TryGetValue(item.TypeName, out List<Guid> ignoredProperites))
                {
                    ignoredProperites.Remove(item.Id);
                }
                Settings.Categories.First(c => c.Type.Name == item.TypeName).Items.Remove(item);
            });

        }

        public void Init()
        {
            Settings.Categories.Clear();
            // TODO put into Data?
            IMindGameProperty[] propertyTypes = new IMindGameProperty[] {
                new MindGameGenre(),
                new MindGameCompletion(),
                new MindGameFeature(),
                new MindGameTag(),
                new MindGameCategory()
            };

            propertyTypes.ToList().ForEach(type =>
            {
                plugin.Data.IgnoredProperites.TryGetValue(type.Name, out List<Guid> ignoredProperites);


                MindGameCategorySettings category = new MindGameCategorySettings()
                {
                    Type = type,
                    UseChanged = (use) => Settings.Settings[type.Name] = use,
                    UseCategory = Settings.Settings[type.Name],
                };

                ignoredProperites
                .Select(ignored => new MindGameCategoryItem() { Id = ignored, Label = type.GetValue(ignored), TypeName = type.Name })
                .OrderBy(ignored => ignored.Label)
                .ForEach(item => category.Items.Add(item));



                Settings.Categories.Add(category);
            });
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            plugin.Data.Load();
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.Data.Save();
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}