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
        public TextBox Box = new();
        
        public ToDoTextBox() { }

        public TextBox NewToDo(string x)
        {
            Box.TextWrapping    = System.Windows.TextWrapping.Wrap;
            Box.TextAlignment   = System.Windows.TextAlignment.Center;
            //Box.TextDecorations = System.Windows.TextDecorations.Underline;

            Box.FontFamily      = new FontFamily("Bradley Hand ITC");
            Box.FontWeight      = FontWeights.Bold;
            Box.Foreground      = Brushes.DarkSlateGray;
            Box.Background      = Brushes.Transparent;
            Box.BorderThickness = new Thickness(0, 0, 0, 0);
            Box.AcceptsReturn   = true;
            Box.IsReadOnly      = false;
            Box.Focusable       = true;
            Box.FontSize        = 40;

            Box.Text            = $"{(char)1421} in the works {(char)1421}\n{x}";    // 0x2610 = Ballot Box | 0x2713 = Check Mark

            return Box;
        }

        public List<TextBox> ReadToDos(List<string> readCache)
        {

            this.Box.TextWrapping = System.Windows.TextWrapping.Wrap;
            this.Box.TextAlignment = System.Windows.TextAlignment.Center;
            //Box.TextDecorations = System.Windows.TextDecorations.Underline;
            this.Box.FontFamily = new FontFamily("Bradley Hand ITC");
            this.Box.FontWeight = FontWeights.Bold;
            this.Box.Foreground = Brushes.DarkSlateGray;
            this.Box.Background = Brushes.Transparent;
            this.Box.BorderThickness = new Thickness(0, 0, 0, 0);
            this.Box.AcceptsReturn = true;
            this.Box.IsReadOnly = false;
            this.Box.Focusable = true;
            this.Box.FontSize = 40;

            List<TextBox> textBoxes = [];

            string buildTextBox     = "";

            int readCacheIndex = 0;

            foreach (var item in readCache)
            {
                TextBox xyz = new();

                xyz.TextWrapping = System.Windows.TextWrapping.Wrap;
                xyz.TextAlignment = System.Windows.TextAlignment.Center;
                xyz.FontFamily = new FontFamily("Bradley Hand ITC");
                xyz.FontWeight = FontWeights.Bold;
                xyz.Foreground = Brushes.DarkSlateGray;
                xyz.Background = Brushes.Transparent;
                xyz.BorderThickness = new Thickness(0, 0, 0, 0);
                xyz.AcceptsReturn = true;
                xyz.IsReadOnly = false;
                xyz.Focusable = true;
                xyz.FontSize = 40;

                if (item.Contains($"{(char)1421}") && readCacheIndex > 0)
                {
                    xyz.Text = buildTextBox;
                    textBoxes.Add(xyz);
                    buildTextBox = $"{(char)1421} in the works {(char)1421}";
                }
                else if (item.Contains($"{(char)0x2713}") && readCacheIndex > 0)
                {
                    xyz.Text = buildTextBox;
                    textBoxes.Add(xyz);
                    buildTextBox = $"Done {(char)0x2713}";
                }
                else
                {
                    if (item == readCache.Last())
                    {
                        buildTextBox += $"\n{item}";

                        xyz.Text = buildTextBox;
                        textBoxes.Add(xyz);
                    }
                    else
                    {
                        if (item == readCache.First())
                        {
                            buildTextBox += $"{item}";
                        }
                        else
                        {
                            buildTextBox += $"\n{item}";
                        }
                    }
                }

                readCacheIndex++;
            }

            return textBoxes;
        }
    }
}
