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

        public TextBox StatusBox()
        {
            TextBox box = new();

            box.TextAlignment = System.Windows.TextAlignment.Center;
            //Box.TextDecorations = System.Windows.TextDecorations.Underline;

            box.FontFamily = new FontFamily("Bradley Hand ITC");
            box.FontWeight = FontWeights.Bold;
            box.Foreground = Brushes.DarkSlateGray;
            box.Background = Brushes.Transparent;
            box.BorderThickness = new Thickness(0, 2, 0, 0);
            box.BorderBrush = Brushes.DarkSlateGray;
            box.Name        = "statusBox";
            box.AcceptsReturn = false;
            box.ClipToBounds = true;
            box.IsReadOnly = true;
            box.Focusable = false;
            box.FontSize = 40;

            box.Text = $"{(char)1421} in the works {(char)1421}";    // 0x2610 = Ballot Box | 0x2713 = Check Mark

            return box;
        }

        public TextBox NewToDo()
        {
            TextBox box = new();

            box.TextWrapping    = System.Windows.TextWrapping.Wrap;
            box.TextAlignment   = System.Windows.TextAlignment.Center;
            //Box.TextDecorations = System.Windows.TextDecorations.Underline;

            box.FontFamily      = new FontFamily("Bradley Hand ITC");
            box.FontWeight      = FontWeights.Normal;
            box.Foreground      = Brushes.DarkSlateGray;
            box.Background      = Brushes.Transparent;
            box.BorderThickness = new Thickness(0, 0, 0, 2);
            box.BorderBrush     = Brushes.DarkSlateGray;
            box.AcceptsReturn   = true;
            box.ClipToBounds    = true;
            box.IsReadOnly      = true;
            box.Focusable       = true;
            box.FontSize        = 40;

            box.Text            = $"enter smth here !";    // 0x2610 = Ballot Box | 0x2713 = Check Mark

            return box;
        }

        public List<TextBox> ReadToDos(List<string> readCache)
        {
            List<TextBox> textBoxes = [];

            string buildTxtBoxContent = "";

            for (int i = 0; i < readCache.Count; i++)
            {
                TextBox box = new();

                box.TextWrapping    = System.Windows.TextWrapping.Wrap;
                box.TextAlignment   = System.Windows.TextAlignment.Center;
                box.FontFamily      = new FontFamily("Bradley Hand ITC");
                box.FontWeight      = FontWeights.Bold;
                box.Width           = 800;
                box.Foreground      = Brushes.DarkSlateGray;
                box.Background      = Brushes.Transparent;
                box.BorderThickness = new Thickness(0, 0, 0, 2);
                box.BorderBrush     = Brushes.DarkSlateGray;
                box.ClipToBounds    = true;
                box.AcceptsReturn   = true;
                box.IsReadOnly      = true;
                box.Focusable       = true;
                box.FontSize        = 40;

                if (readCache[i].Contains($"{(char)1421}") && i != 0)
                {
                    box.Text = buildTxtBoxContent;
                    textBoxes.Add(box);
                    buildTxtBoxContent = $"{(char)1421} in the works {(char)1421}";
                }
                else if (readCache[i].Contains($"{(char)0x2713}") && i != 0)
                {
                    box.Text = buildTxtBoxContent;
                    textBoxes.Add(box);
                    buildTxtBoxContent = $"Done {(char)0x2713}";
                }
                else
                {
                    if (i == readCache.Count - 1)
                    {
                        buildTxtBoxContent += $"\n{readCache[i]}";

                        box.Text = buildTxtBoxContent;
                        textBoxes.Add(box);
                    }
                    else
                    {
                        if (i == 0)
                        {
                            buildTxtBoxContent += $"{readCache[i]}";
                        }
                        else
                        {
                            buildTxtBoxContent += $"\n{readCache[i]}";
                        }
                    }
                }
            }

            return textBoxes;
        }
    }
}
