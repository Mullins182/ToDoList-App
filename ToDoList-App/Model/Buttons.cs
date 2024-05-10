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
        public static Button FullscreenMode = new();
        #pragma warning restore CA2211

        #pragma warning disable CA2211
        public static Button AutoSave = new();
        #pragma warning restore CA2211

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
            AutoSave.Content      = "Autosave every Minute";
            AutoSave.FontFamily   = new FontFamily("Bahnschrift");
            AutoSave.FontSize     = 30;
            AutoSave.Background   = Brushes.Black;
            AutoSave.Foreground   = Brushes.Green;
        }
    }
}
