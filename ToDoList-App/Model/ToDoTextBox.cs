using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ToDoList_App.Model
{
    public class ToDoTextBox
    {
        //public ImageBrush statusBoxBG = new();
        
        public ToDoTextBox() { }

        public TextBox StatusBox()
        {
            TextBox statusBox = new();

            statusBox.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            statusBox.Width             = 800;
            statusBox.FontFamily        = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Curlz MT");
            statusBox.FontWeight        = FontWeights.Bold;
            statusBox.FontSize          = 50;
            statusBox.Background        = Brushes.Transparent;
            statusBox.IsReadOnly        = true;
            statusBox.Focusable         = false;
            statusBox.BorderThickness   = new Thickness(0, 3, 0, 0);
            statusBox.BorderBrush       = Brushes.DarkSlateGray;

            statusBox.Text = $"{(char)1421} in the works {(char)1421}";    // 0x2610 = Ballot Box | 0x2713 = Check Mark

            return statusBox;
        }

        public TextBox NewToDo()
        {
            TextBox box = new();

            box.TextWrapping    = System.Windows.TextWrapping.Wrap;
            box.TextAlignment   = System.Windows.TextAlignment.Center;
            box.FontFamily      = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Bradley Hand ITC");
            box.FontWeight      = FontWeights.Bold;
            box.Width           = 800;
            box.Foreground      = Brushes.DarkSlateGray;
            box.Background      = Brushes.Transparent;
            box.BorderThickness = new Thickness(0, 0, 0, 0);
            box.BorderBrush     = Brushes.DarkSlateGray;
            box.ClipToBounds    = true;
            box.AcceptsReturn   = false;
            box.IsReadOnly      = true;
            box.Focusable       = true;
            box.FontSize        = 40;

            box.Text            = $"enter smth here !";    // 0x2610 = Ballot Box | 0x2713 = Check Mark

            return box;
        }

        public List<TextBox> ReadToDos(List<string> readCache)
        {
            List<TextBox> textBoxes = [];

            string buildBoxContent = "";

            for (int i = 0; i < readCache.Count; i++)
            {
                TextBox statusBox   = new();
                TextBox box         = new();

                //statusBoxBG.ImageSource     = new BitmapImage(new Uri("pack://application:,,,/Textures/gold2.jpg"));

                statusBox.HorizontalContentAlignment    = System.Windows.HorizontalAlignment.Center;
                statusBox.Width                         = 800;
                statusBox.FontFamily                    = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Curlz MT");
                statusBox.FontWeight                    = FontWeights.Bold;
                statusBox.FontSize                      = 50;
                //statusBox.Background                    = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x15, 0, 0, 0));
                statusBox.Background                    = Brushes.Transparent;
                statusBox.IsReadOnly                    = true;
                statusBox.Focusable                     = false;
                statusBox.BorderThickness               = new Thickness(0, 3, 0, 0);
                statusBox.BorderBrush                   = Brushes.DarkSlateGray;

                box.TextWrapping                        = System.Windows.TextWrapping.Wrap;
                box.TextAlignment                       = System.Windows.TextAlignment.Center;
                box.FontFamily                          = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Bradley Hand ITC");
                box.FontWeight                          = FontWeights.Bold;
                box.Width                               = 800;
                box.Foreground                          = Brushes.DarkSlateGray;
                //box.Background                          = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x15, 0, 0, 0));
                box.Background                          = Brushes.Transparent;
                box.BorderThickness                     = new Thickness(0, 0, 0, 0);
                box.BorderBrush                         = Brushes.DarkSlateGray;
                box.ClipToBounds                        = true;
                box.AcceptsReturn                       = false;
                box.IsReadOnly                          = true;
                box.Focusable                           = true;
                box.FontSize                            = 40;

                if (i == 0)
                {
                    statusBox.Text = readCache[i];
                    textBoxes.Add(statusBox);
                }
                else if (readCache[i].Contains($"{(char)1421}") || readCache[i].Contains($"{(char)0x2713}"))
                {
                    box.Text = buildBoxContent;
                    buildBoxContent = "";
                    textBoxes.Add(box);
                    statusBox.Text = readCache[i];
                    textBoxes.Add(statusBox);
                }
                else if (i == readCache.Count - 1)
                {
                    buildBoxContent += $"{readCache[i]}";
                    box.Text = buildBoxContent;
                    textBoxes.Add(box);
                }
                else
                {
                    buildBoxContent += $"{readCache[i]}";
                }
            }

            return textBoxes;
        }
    }
}
