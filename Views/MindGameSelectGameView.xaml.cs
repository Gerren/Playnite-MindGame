using MindGame.Models;
using Playnite.SDK.Controls;
using Playnite.SDK.Models;
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

namespace MindGame.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MindGameSelectGameView : PluginUserControl
    {
        private MindGameSelectGameViewModel vm;

        public MindGameSelectGameView()
        {
            InitializeComponent();
        }

        internal void Init()
        {
            vm = new MindGameSelectGameViewModel(); 
            DataContext = vm;
            vm.Init();
        }

        private void StartAgain(object sender, RoutedEventArgs e)
        {
            vm.DoInit();
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            vm.OptionSelected(button.Name);
        }

        private void Ignore(object sender, RoutedEventArgs e)
        {
            vm.Ignore();
        }

        private void Another(object sender, RoutedEventArgs e)
        {
            vm.Next();
        }

        private void Filtered(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            vm.UseCurrentFilter = checkBox.IsChecked ?? true;
            vm.Init();
        }
    }
}
