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
        
        public ToDoTextBox() { }

        public TextBox NewToDo(string x)
        {
            TextBox box = new();

            box.TextWrapping    = System.Windows.TextWrapping.Wrap;
            box.TextAlignment   = System.Windows.TextAlignment.Center;
            //Box.TextDecorations = System.Windows.TextDecorations.Underline;

            box.FontFamily      = new FontFamily("Bradley Hand ITC");
            box.FontWeight      = FontWeights.Bold;
            box.Foreground      = Brushes.DarkSlateGray;
            box.Background      = Brushes.Transparent;
            box.BorderThickness = new Thickness(0, 1, 0, 1);
            box.BorderBrush     = Brushes.DarkSlateGray;
            box.AcceptsReturn   = true;
            box.ClipToBounds    = true;
            box.IsReadOnly      = false;
            box.Focusable       = true;
            box.FontSize        = 40;

            box.Text            = $"{(char)1421} in the works {(char)1421}\n{x}";    // 0x2610 = Ballot Box | 0x2713 = Check Mark

            return box;
        }

        public List<TextBox> ReadToDos(List<string> readCache)
        {
            List<TextBox> textBoxes = [];

            string buildTxtBoxContent = "";

            int readCacheIndex = 0;

            foreach (var item in readCache)
            {
                TextBox box = new();

                box.TextWrapping    = System.Windows.TextWrapping.Wrap;
                box.TextAlignment   = System.Windows.TextAlignment.Center;
                box.FontFamily      = new FontFamily("Bradley Hand ITC");
                box.FontWeight      = FontWeights.Bold;
                box.Foreground      = Brushes.DarkSlateGray;
                box.Background      = Brushes.Transparent;
                box.BorderThickness = new Thickness(0, 1, 0, 1);
                box.BorderBrush     = Brushes.DarkSlateGray;
                box.ClipToBounds    = true;
                box.AcceptsReturn   = true;
                box.IsReadOnly      = true;
                box.Focusable       = true;
                box.FontSize        = 40;

                if (item.Contains($"{(char)1421}") && readCacheIndex > 0)
                {
                    box.Text = buildTxtBoxContent;
                    textBoxes.Add(box);
                    buildTxtBoxContent = $"{(char)1421} in the works {(char)1421}";
                }
                else if (item.Contains($"{(char)0x2713}") && readCacheIndex > 0)
                {
                    box.Text = buildTxtBoxContent;
                    textBoxes.Add(box);
                    buildTxtBoxContent = $"Done {(char)0x2713}";
                }
                else
                {
                    if (item == readCache.Last())
                    {
                        buildTxtBoxContent += $"\n{item}";

                        box.Text = buildTxtBoxContent;
                        textBoxes.Add(box);
                    }
                    else
                    {
                        if (item == readCache.First())
                        {
                            buildTxtBoxContent += $"{item}";
                        }
                        else
                        {
                            buildTxtBoxContent += $"\n{item}";
                        }
                    }
                }

                readCacheIndex++;
            }

            return textBoxes;
        }
    }
}
