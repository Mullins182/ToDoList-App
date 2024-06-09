using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;


namespace ToDoList_App.Model
{
    public static class Buttons
    {
        #pragma warning disable CA2211
        public static Button FullscreenMode     = new();

        public static Button AutoSave           = new();

        public static Button DelEntry           = new();
        #pragma warning restore CA2211

        public static void SetDelEntryProps()
        {
            DelEntry.Width = 400;
            DelEntry.Height = 50;
            DelEntry.Background = Brushes.Red;
            DelEntry.Foreground = Brushes.White;
            DelEntry.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            DelEntry.Content = "Delete selected ToDo-Entry";
            DelEntry.FontSize = 30;
            DelEntry.Visibility = System.Windows.Visibility.Hidden;
        }

        public static void SetFullscreenModeProps()
        {
            FullscreenMode.Height       = 60;
            FullscreenMode.Content      = "Fullsceen Mode On/Off";
            FullscreenMode.FontFamily   = new FontFamily("Bahnschrift");
            FullscreenMode.FontSize     = 30;
            FullscreenMode.Background   = Brushes.Black;
            FullscreenMode.Foreground   = Brushes.Red;
        }

        public static void SetAutoSaveProps()
        {
            AutoSave.Height       = 60;
            AutoSave.FontFamily   = new FontFamily("Bahnschrift");
            AutoSave.FontSize     = 30;
            AutoSave.Background   = Brushes.Black;
        }

        public static void SetAutoSaveProps(TimeSpan timespan)
        {
            AutoSave.Content = (timespan.Minutes == 0) ? "Autosave Off" : (timespan.Minutes == 1) ? "Autosave every Minute" 
                : (timespan.Minutes == 3) ? "Autosave every 3 Minutes" : (timespan.Minutes == 5) ? "Autosave every 5 Minutes" 
                : (timespan.Minutes == 9) ? "Autosave every 9 Minutes" : "Autosave Off";

            AutoSave.Foreground = (timespan.Minutes == 0) ? Brushes.Red : Brushes.LawnGreen;
        }
    }
}
