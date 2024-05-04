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

        public TextBox ToDo(string x)
        {
            Box.TextWrapping = System.Windows.TextWrapping.Wrap;
            Box.TextAlignment = System.Windows.TextAlignment.Center;
            //Box.TextDecorations = System.Windows.TextDecorations.Underline;

            Box.FontFamily      = new FontFamily("Bradley Hand ITC");
            Box.FontWeight      = FontWeights.Bold;
            Box.Foreground      = Brushes.DarkSlateGray;
            Box.Background      = Brushes.Transparent;
            Box.BorderThickness = new Thickness(0, 0, 0, 5);
            Box.IsReadOnly      = true;
            Box.Focusable       = false;
            Box.FontSize        = 40;

            Box.Text            = $"{(char)1421}\n{x}";    // 1421 | 0x2610 = Ballot Box | 0x2713 = Check Mark

            return Box;
        }
    }
}
