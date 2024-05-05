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
            List<TextBox> textBoxes = [];

            string buildTextBox     = "";

            foreach (var item in readCache)
            {
                if (item == $"{(char)1421} in the works {(char)1421}" && item != readCache[0])
                {
                    TextBox Box2 = new();
                    Box2.TextWrapping = System.Windows.TextWrapping.Wrap;
                    Box2.TextAlignment = System.Windows.TextAlignment.Center;
                    //Box.TextDecorations = System.Windows.TextDecorations.Underline;

                    Box2.FontFamily = new FontFamily("Bradley Hand ITC");
                    Box2.FontWeight = FontWeights.Bold;
                    Box2.Foreground = Brushes.DarkSlateGray;
                    Box2.Background = Brushes.Transparent;
                    Box2.BorderThickness = new Thickness(0, 0, 0, 0);
                    Box2.AcceptsReturn = true;
                    Box2.IsReadOnly = false;
                    Box2.Focusable = true;
                    Box2.FontSize = 40;
                    Box2.Text = buildTextBox;
                    textBoxes.Add(Box2);
                    buildTextBox = $"{(char)1421} in the works {(char)1421}";
                }
                else if (item == $"Done {(char)0x2713}" && item != readCache[0])
                {
                    TextBox Box2 = new();
                    Box2.TextWrapping = System.Windows.TextWrapping.Wrap;
                    Box2.TextAlignment = System.Windows.TextAlignment.Center;
                    //Box.TextDecorations = System.Windows.TextDecorations.Underline;

                    Box2.FontFamily = new FontFamily("Bradley Hand ITC");
                    Box2.FontWeight = FontWeights.Bold;
                    Box2.Foreground = Brushes.DarkSlateGray;
                    Box2.Background = Brushes.Transparent;
                    Box2.BorderThickness = new Thickness(0, 0, 0, 0);
                    Box2.AcceptsReturn = true;
                    Box2.IsReadOnly = false;
                    Box2.Focusable = true;
                    Box2.FontSize = 40;
                    Box2.Text = buildTextBox;
                    textBoxes.Add(Box2);
                    buildTextBox = $"Done {(char)0x2713}";
                }
                else
                {
                    if (item == readCache.Last())
                    {
                        buildTextBox += $"\n{item}";

                        TextBox Box2 = new();
                        Box2.TextWrapping = System.Windows.TextWrapping.Wrap;
                        Box2.TextAlignment = System.Windows.TextAlignment.Center;
                        //Box.TextDecorations = System.Windows.TextDecorations.Underline;

                        Box2.FontFamily = new FontFamily("Bradley Hand ITC");
                        Box2.FontWeight = FontWeights.Bold;
                        Box2.Foreground = Brushes.DarkSlateGray;
                        Box2.Background = Brushes.Transparent;
                        Box2.BorderThickness = new Thickness(0, 0, 0, 0);
                        Box2.AcceptsReturn = true;
                        Box2.IsReadOnly = false;
                        Box2.Focusable = true;
                        Box2.FontSize = 40;
                        Box2.Text = buildTextBox;
                        textBoxes.Add(Box2);
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
            }

            return textBoxes;
        }
    }
}
