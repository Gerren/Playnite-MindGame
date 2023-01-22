using MindGame.Models;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MindGame
{
    public partial class MindGameSettingsView : UserControl
    {
        public MindGameSettingsView()
        {
            InitializeComponent();
        }

        private void ClearList(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            MindGameSettingsViewModel settings = (MindGameSettingsViewModel)this.DataContext;
            settings.Clear(button.Name);
        }
    }
}