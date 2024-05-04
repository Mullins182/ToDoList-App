using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ToDoList_App.Model
{
    public class ToDoTextBox
    {
        private TextBox Box = new();

        public ToDoTextBox() { }

        public TextBox ToDo(string x, string boxName)
        {
            Box.TextWrapping = System.Windows.TextWrapping.Wrap;
            Box.TextAlignment = System.Windows.TextAlignment.Center;
            //Box.TextDecorations = System.Windows.TextDecorations.Underline;

            Box.FontFamily      = new FontFamily("Bradley Hand ITC");
            Box.Name            = boxName;
            Box.Foreground      = Brushes.DarkSlateGray;
            Box.Background      = Brushes.Transparent;
            Box.BorderThickness = new Thickness(0, 0, 0, 5);
            Box.FontSize        = 40;
            Box.FontWeight      = FontWeights.Bold;

            Box.Text            = $"{(char)1421}\n{x}";    // 1421 | 0x2610 = Ballot Box | 0x2713 = Check Mark

            return Box;
        }
    }
}
