using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MindGame
{
    internal class PlayniteImageTaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            Task<BitmapImage> task = Task.Run(() =>
            {

                try
                {
                    if (value == null || (value is string path && string.IsNullOrEmpty(path)))
                    {
                        if (ResourceProvider.GetResource("DefaultGameIcon") != null)
                        {
                            return (BitmapImage)  ResourceProvider.GetResource("DefaultGameIcon");
                        }
                    }
                    else
                    {
                        if (!(value is string))
                        {
                            return (BitmapImage) value;
                        }

                        if (!File.Exists((string)value))
                        {
                            if (File.Exists(API.Instance.Database.GetFullFilePath((string)value)))
                            { 
                                return new BitmapImage(new Uri(API.Instance.Database.GetFullFilePath((string)value)));
                            }
                            else
                            {
                                if (ResourceProvider.GetResource("DefaultGameIcon") != null)
                                {
                                    return (BitmapImage)ResourceProvider.GetResource("DefaultGameIcon");
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                return null;
            });

            return task;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
