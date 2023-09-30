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
        public bool UseCategory { get => useCategory; set => SetValue(ref useCategory, value); }
        public bool UseCompletionStatus { get => useCompletionStatus; set => SetValue(ref useCompletionStatus, value); }
        public bool UseGenre { get => useGenre; set => SetValue(ref useGenre, value); }
        public bool UseFeature { get => useFeature; set => SetValue(ref useFeature, value); }
        public bool UseTag { get => useTag; set => SetValue(ref useTag, value); }
    }

    public class MindGameCategorySettings : ObservableObject
    {
        public string LabelHandle { get; set; }
        public bool UseCategory { get; set; }
        public IMindGameProperty Type { get; set; }
        public Guid Guid { get; set; }
    }

    public class MindGameSettingsViewModel : ObservableObject, ISettings
    {
        private readonly MindGame plugin;
        private MindGameSettings editingClone { get; set; }

        private MindGameSettings settings;
        private ObservableCollection<MindGameCategorySettings> categories;

        public MindGameSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MindGameCategorySettings> Categories { get => categories; set => categories = value; }

        internal void Clear(string property)
        {
            if (plugin.Data.IgnoredProperites.TryGetValue(property, out List<Guid> ignoredProperites))
            {
                ignoredProperites.Clear();
            }
        }

        public MindGameSettingsViewModel(MindGame plugin)
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

            IMindGameProperty[] propertyTypes = new IMindGameProperty[] {
                new MindGameGenre(),
                new MindGameCompletion(),
                new MindGameFeature(),
                new MindGameTag(),
                new MindGameCategory()
            };
            
            categories = new ObservableCollection<MindGameCategorySettings>();
            propertyTypes.ToList().ForEach(type =>
            {
                plugin.Data.IgnoredProperites.TryGetValue(type, out List<Guid> ignoredProperites);

                MindGameCategorySettings category = new MindGameCategorySettings()
                {
                    Type = type,
                };

                

                categories.Add(category)
            })

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