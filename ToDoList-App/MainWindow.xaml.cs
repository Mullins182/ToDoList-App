﻿using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ToDoList_App.Model;
using Windows.UI.Composition;
using WinRT;

namespace ToDoList_App                                      //  ToDo-List App | @Mullins182
{
    public partial class MainWindow : Window
    {
        private readonly MediaPlayer floppyWrite            = new();
        private readonly MediaPlayer scribble1              = new();
        private readonly MediaPlayer scribble2              = new();

        private readonly Random genRdNr                     = new();

        private DispatcherTimer saveTimer                   = new();
        private readonly DispatcherTimer savingAnimTimer    = new();

        private DoubleAnimation InfoLabelAnim               = new();
        private DoubleAnimation InfoLabelAnimReverse        = new();

        private List<TextBox> toDoEntrys                    = [];

        private string[] options                            = ["Fullscreen Mode = 0", "Autosave Minutes = 0", "Save On Exit = 0", "Sounds_Mode = 3"];

        private int delEntryIndex                           = -1;
        private int floppySoundPos                          = 50;
        private int scribble1SoundPos                       = 0;
        private int scribble2SoundPos                       = 350;
        private int sound_Mode                              = 3;

        private double savingAnimRectPos                    = 0.00;

        private bool optionsWriteProcess                    = false;
        private bool savingAnimRectUp                       = true;
        private bool entryFinished                          = true;
        private bool saveFinished                           = true;
        private bool saveOnExit                             = false;

        private readonly string markInWorks                 = $"{(char)1421} in the works {(char)1421}";    
        private readonly string markDone                    = $"Done {(char)0x2713}";

        public MainWindow()
        {
            InitializeComponent();

            InitializePrg();
        }

        public async void InitializePrg()
        {
            Buttons.SetFullscreenModeProps();
            Buttons.SetSoundsProps(sound_Mode);
            Buttons.SetAutoSaveProps();
            Buttons.SetDelEntryProps();
            Buttons.SetSaveOnPrgExitProps();

            Buttons.FullscreenMode.Template     = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.DelEntry.Template           = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.AutoSave.Template           = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.saveOnPrgExit.Template      = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.Sounds.Template             = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot

            Buttons.FullscreenMode.Click        += FullscreenMode_Click;
            Buttons.AutoSave.Click              += AutoSave_Click;
            Buttons.saveOnPrgExit.Click         += SaveOnPrgExit_Click;
            Buttons.DelEntry.Click              += DelEntry_Click;
            Buttons.DelEntry.MouseEnter         += DelEntry_MouseEnter;
            Buttons.DelEntry.MouseLeave         += DelEntry_MouseLeave;
            Buttons.Sounds.Click                += Sounds_Click;
            Buttons.DelEntry.Style              = (Style)FindResource("MenuStyleButtons");

            Grid.SetRow(Buttons.DelEntry, 0);
            Grid.SetColumn(Buttons.DelEntry, 1);

            floppyWrite.Open(new Uri("Sounds/readingFloppyDisc.mp3", UriKind.Relative));
            scribble1.Open(new Uri("Sounds/scribble1.wav", UriKind.Relative));
            scribble2.Open(new Uri("Sounds/scribble2.wav", UriKind.Relative));

            await Task.Delay(450);

            if (File.Exists("options.ini"))
            {
                options = [];
                options = File.ReadAllLines("options.ini");

                if (options[0].Last() - 48 == 0)
                {
                    this.WindowStyle                    = WindowStyle.SingleBorderWindow;
                    this.WindowState                    = WindowState.Normal;

                    AppLabel.FontSize                   = 100;

                    Ornament_left.Width                 = 135.3;
                    Ornament_left.Height                = 192;
                    Ornament_left.Margin                = new Thickness(30, 20, 0, 0);
                    Ornament_right.Width                = 135.3;
                    Ornament_right.Height               = 192;
                    Ornament_right.Margin               = new Thickness(0, 20, 30, 0);

                    Buttons.FullscreenMode.Foreground   = Brushes.Red;
                }
                else if (options[0].Last() - 48 == 1)
                {
                    this.WindowStyle                    = WindowStyle.None;
                    this.WindowState                    = WindowState.Maximized;

                    AppLabel.FontSize                   = 150;

                    Ornament_left.Width                 = 236.775;
                    Ornament_left.Height                = 336;
                    Ornament_left.Margin                = new Thickness(120, 20, 0, 0);
                    Ornament_right.Width                = 236.775;
                    Ornament_right.Height               = 336;
                    Ornament_right.Margin               = new Thickness(0, 20, 120, 0);

                    Buttons.FullscreenMode.Foreground   = Brushes.LawnGreen;
                }

                if (options[2].Last() - 48 == 0)
                {
                    Buttons.saveOnPrgExit.Foreground    = Brushes.Red;

                    saveOnExit                          = false;
                }
                else if (options[2].Last() - 48 == 1)
                {
                    Buttons.saveOnPrgExit.Foreground    = Brushes.LawnGreen;

                    saveOnExit                          = true;
                }

                sound_Mode = (options[3].Last() - 48 == 1) ? 1 : (options[3].Last() - 48 == 2) ? 2 : (options[3].Last() - 48 == 3) ? 3 : 0;

                saveTimer.Interval = TimeSpan.FromMinutes(options[1].Last() == 'T' ? 10 : (double)options[1].Last() -48);

                Buttons.SetAutoSaveProps(saveTimer.Interval);
                Buttons.SetSoundsProps(sound_Mode);
            }
            else
            {
                await File.WriteAllLinesAsync("options.ini", options, UTF8Encoding.Unicode);

                saveTimer.Interval = TimeSpan.FromMinutes(((double)options[1].Last()) - 48);

                Buttons.SetAutoSaveProps(saveTimer.Interval);
            }

            savingAnimTimer.Interval            = TimeSpan.FromMilliseconds(25);
            saveTimer.Tick                      += SaveRoutine;
            savingAnimTimer.Tick                += SavingAnimationRoutine;
            ToDoList.PreviewMouseLeftButtonDown += ToDoList_PreviewMouseLeftButtonDown;

            InfoLabelAnim.Duration              = TimeSpan.FromSeconds(0.44);
            InfoLabelAnim.From                  = 0.00;
            InfoLabelAnim.To                    = 0.85;
            InfoLabelAnimReverse.Duration       = TimeSpan.FromSeconds(0.44);
            InfoLabelAnimReverse.From           = 0.85;
            InfoLabelAnimReverse.To             = 0.00;

            Canvas.SetLeft(SavingRectangle, (SavingCanvas.Width / 2) - SavingRectangle.Width / 2);
            Canvas.SetTop(SavingRectangle, (SavingCanvas.Height / 2) - SavingRectangle.Height / 2);

            savingAnimRectPos                   = Canvas.GetTop(SavingRectangle);
            ToDoList.ItemsSource                = toDoEntrys;

            //ToDoList.Background                 = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x10, 0, 0, 0));

            InfoLabel.Background                = Brushes.Black;
            InfoLabel.Content                   = "Left Click again to finish editing !";

            if (File.Exists("data.dat"))
            {
                ReadData();
            }

            MainGrid.Children.Add(Buttons.DelEntry);

            OptionsStack.Children.Add(Buttons.FullscreenMode);
            OptionsStack.Children.Add(Buttons.Sounds);
            OptionsStack.Children.Add(Buttons.AutoSave);
            OptionsStack.Children.Add(Buttons.saveOnPrgExit);

            if (saveTimer.Interval != TimeSpan.FromMinutes(0)) { saveTimer.Start(); }
        }

        private void SavingAnimationRoutine(object? sender, EventArgs e)
        {
            if (savingAnimRectPos < 30)
            {
                savingAnimRectUp = false;
            }

            if (savingAnimRectPos > 50)
            {
                savingAnimRectUp = true;
            }

            if (savingAnimRectUp)
            {
                Canvas.SetTop(SavingRectangle, savingAnimRectPos -= 1.5);
            }
            else
            {
                Canvas.SetTop(SavingRectangle, savingAnimRectPos += 1.5);
            }
        }

        private async void SaveRoutine(object? sender, EventArgs e)
        {
            saveFinished = false;

            savingAnimTimer.Start();
            SavingRectangle.Visibility = Visibility.Visible;

            SaveData();

            if (sound_Mode == 2 || sound_Mode == 3)
            {
                floppyWrite.Position = TimeSpan.FromMilliseconds(floppySoundPos);
                floppyWrite.Play();
            }

            await Task.Delay(3000);

            floppyWrite.Stop();
            savingAnimTimer.Stop();
            SavingRectangle.Visibility = Visibility.Hidden;

            saveFinished = true;
        }

        private void SaveData()
        {
            List<string> writeCache = [];

            foreach (var item in toDoEntrys)
            {
                writeCache.Add(item.Text);
            }

            File.WriteAllLines("data.dat", writeCache, UTF8Encoding.BigEndianUnicode);

            writeCache.Clear();
        }

        private void ReadData()
        {
            List<string> readCache  = [.. File.ReadAllLines("data.dat")];     // [.. ] = simplifying .toList()

            ToDoTextBox ToDo        = new();

            foreach (var item in ToDo.ReadToDos(readCache))
            {
                toDoEntrys.Add(item);
            }

            for (int i = 0; i < toDoEntrys.Count; i++)
            {
                if (toDoEntrys[i].Text.Contains($"{(char)1421}"))
                {
                    toDoEntrys[i].Foreground        = Brushes.DarkRed;
                    toDoEntrys[i+1].Name            = "EntryWork";
                }
                else if (toDoEntrys[i].Text.Contains($"{(char)0x2713}"))
                {
                    toDoEntrys[i].Foreground        = Brushes.DarkSlateGray;
                    toDoEntrys[i + 1].Foreground    = Brushes.DarkSlateGray;
                    toDoEntrys[i + 1].Name          = "EntryDone";
                }
            }

            toDoEntrys.ForEach(item => { item.MouseDoubleClick += ToDoBoxMouseDoubleClick; });
            toDoEntrys.ForEach(item => { item.MouseEnter += ToDoBoxMouseEnter; });
            toDoEntrys.ForEach(item => { item.TextDecorations = item.Name == "EntryDone" ? TextDecorations.Strikethrough : null; });

            ToDoList.Items.Refresh();
        }

        private async Task WriteOptionsFile(int optionsIndex, string optionsIndexValue)
        {
            if (optionsWriteProcess) { return; }

            optionsWriteProcess = true;

            options[optionsIndex] = optionsIndexValue;

            await File.WriteAllLinesAsync("options.ini", options, UTF8Encoding.ASCII);

            optionsWriteProcess = false;
        }

        private async Task SaveListBoxData()
        {
            saveFinished = false;

            Save.Foreground = Brushes.Green;

            saveTimer.Stop();
            savingAnimTimer.Start();
            SavingRectangle.Visibility = Visibility.Visible;

            if (sound_Mode == 2 || sound_Mode == 3)
            {
                floppyWrite.Position = TimeSpan.FromMilliseconds(floppySoundPos);
                floppyWrite.Play();
            }

            SaveData();

            await Task.Delay(3000);

            floppyWrite.Stop();

            savingAnimTimer.Stop();

            SavingRectangle.Visibility = Visibility.Hidden;

            if (saveTimer.Interval != TimeSpan.FromMinutes(0))
            {
                saveTimer.Start();
            }

            Save.Foreground = Brushes.Black;
            saveFinished = true;
        }

        // UI-Elements Click Events

        // Code-Behind Elements
        private async void FullscreenMode_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Buttons.FullscreenMode.Foreground = Brushes.Red;

                this.WindowStyle        = WindowStyle.SingleBorderWindow;
                this.WindowState        = WindowState.Normal;

                AppLabel.FontSize       = 100;

                Ornament_left.Width     = 135.3;
                Ornament_left.Height    = 192;
                Ornament_left.Margin    = new Thickness(30, 20, 0, 0);
                Ornament_right.Width    = 135.3;
                Ornament_right.Height   = 192;
                Ornament_right.Margin   = new Thickness(0, 20, 30, 0);

                await WriteOptionsFile(0, "Fullscreen Mode = 0");
            }
            else
            {
                Buttons.FullscreenMode.Foreground = Brushes.LawnGreen;

                this.WindowStyle        = WindowStyle.None;
                this.WindowState        = WindowState.Maximized;

                AppLabel.FontSize       = 150;

                Ornament_left.Width     = 236.775;
                Ornament_left.Height    = 336;
                Ornament_left.Margin    = new Thickness(120,20,0,0);
                Ornament_right.Width    = 236.775;
                Ornament_right.Height   = 336;
                Ornament_right.Margin   = new Thickness(0, 20, 120, 0);

                await WriteOptionsFile(0, "Fullscreen Mode = 1");
            }
        }

        private async void AutoSave_Click(object sender, RoutedEventArgs e)
        {
            saveTimer.Stop();

            saveTimer.Interval = (saveTimer.Interval == TimeSpan.FromMinutes(1)) ? TimeSpan.FromMinutes(3) : (saveTimer.Interval == TimeSpan.FromMinutes(3)) 
                ? TimeSpan.FromMinutes(5) : (saveTimer.Interval == TimeSpan.FromMinutes(5)) ? TimeSpan.FromMinutes(10) 
                : (saveTimer.Interval == TimeSpan.FromMinutes(10)) ? TimeSpan.FromMinutes(0) : TimeSpan.FromMinutes(1);

            Buttons.SetAutoSaveProps(saveTimer.Interval);

            await WriteOptionsFile(1, options[1].Replace(options[1].Last(), saveTimer.Interval.Minutes == 10 ? 'T' : (char)(saveTimer.Interval.Minutes + 48)));

            if (saveTimer.Interval != TimeSpan.FromMinutes(0))
            {
                saveTimer.Start();
            }
        }

        private async void SaveOnPrgExit_Click(object sender, RoutedEventArgs e)
        {
            saveOnExit = saveOnExit ? false : true;

            Buttons.saveOnPrgExit.Foreground = saveOnExit ? Brushes.LawnGreen : Brushes.Red;

            await WriteOptionsFile(2, saveOnExit ? "Save On Exit = 1" : "Save On Exit = 0");
        }

        private async void Sounds_Click(object sender, RoutedEventArgs e)
        {
            sound_Mode = sound_Mode == 1 ? 2 : sound_Mode == 2 ? 3 : sound_Mode == 3 ? 0 : 1;

            Buttons.SetSoundsProps(sound_Mode);

            await WriteOptionsFile(3, options[3].Replace(options[3].Last(), (char)(sound_Mode + 48)));
        }

        // Code-Behind Elements END 

        private void NewEntry_Click(object sender, RoutedEventArgs e)
        {
            if (entryFinished)
            {
                entryFinished       = false;

                ToDoTextBox ToDo    = new();
                toDoEntrys.Add(ToDo.StatusBox());

                toDoEntrys[toDoEntrys.Count - 1].MouseEnter += ToDoBoxMouseEnter;
                toDoEntrys[toDoEntrys.Count - 1].Foreground = Brushes.DarkRed;

                toDoEntrys.Add(ToDo.NewToDo());

                toDoEntrys[toDoEntrys.Count - 1].MouseEnter += ToDoBoxMouseEnter;

                ToDoList.Items.Refresh();

                entryFinished       = true;
            }
        }

        private void ToDoList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ToDoList.SelectedIndex >= 0 && entryFinished)
            {
                entryFinished = false;

                if (toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markInWorks))
                {
                    toDoEntrys[ToDoList.SelectedIndex].Text = 
                    toDoEntrys[ToDoList.SelectedIndex].Text.Replace(markInWorks, markDone);
                    toDoEntrys[ToDoList.SelectedIndex].Foreground = Brushes.DarkSlateGray;
                    toDoEntrys[ToDoList.SelectedIndex + 1].Foreground = Brushes.DarkSlateGray;
                    toDoEntrys[ToDoList.SelectedIndex + 1].TextDecorations = TextDecorations.Strikethrough;
                    toDoEntrys[ToDoList.SelectedIndex + 1].Name = "EntryDone";

                    if (sound_Mode == 1 || sound_Mode == 3)
                    {
                        scribble2.Position = TimeSpan.FromMilliseconds(scribble2SoundPos);
                        scribble2.Play();
                    }

                    entryFinished = true;

                    return;
                }                
                else if (toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markDone))
                {
                    toDoEntrys[ToDoList.SelectedIndex].Text = 
                    toDoEntrys[ToDoList.SelectedIndex].Text.Replace(markDone, markInWorks);
                    toDoEntrys[ToDoList.SelectedIndex].Foreground = Brushes.DarkRed;
                    toDoEntrys[ToDoList.SelectedIndex + 1].Foreground = Brushes.Black;
                    toDoEntrys[ToDoList.SelectedIndex + 1].TextDecorations = null;
                    toDoEntrys[ToDoList.SelectedIndex + 1].Name = "EntryWork";

                    if (sound_Mode == 1 || sound_Mode == 3)
                    {
                        scribble1.Position = TimeSpan.FromMilliseconds(scribble1SoundPos);
                        scribble1.Play();
                    }

                    entryFinished = true;

                    return;
                }
            }
        }

        private void ToDoList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ToDoList.SelectedIndex < 0 || toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markDone)
                || toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markInWorks) || InfoLabelAnim.IsFrozen || InfoLabelAnimReverse.IsFrozen)
            {

            }
            else
            {
                toDoEntrys[ToDoList.SelectedIndex].IsReadOnly = toDoEntrys[ToDoList.SelectedIndex].Name == "EntryDone" ? true 
                    : toDoEntrys[ToDoList.SelectedIndex].IsReadOnly == true ? false : true;
                                                
                if (toDoEntrys[ToDoList.SelectedIndex].IsReadOnly == true)
                {
                    if (toDoEntrys[ToDoList.SelectedIndex].Text == "") { toDoEntrys[ToDoList.SelectedIndex].Text = "enter smth here !"; }

                    if (InfoLabel.Opacity == 0.85) { InfoLabel.BeginAnimation(OpacityProperty, InfoLabelAnimReverse); }
                    Buttons.DelEntry.Visibility = Visibility.Hidden;
                    delEntryIndex = -1;
                }
                else if (toDoEntrys[ToDoList.SelectedIndex].IsReadOnly == false)
                {
                    delEntryIndex = ToDoList.SelectedIndex;

                    if (InfoLabel.Opacity == 0.00) { InfoLabel.BeginAnimation(OpacityProperty, InfoLabelAnim); }

                    Buttons.DelEntry.Visibility = Visibility.Visible;

                    if (toDoEntrys[ToDoList.SelectedIndex].Text == "enter smth here !") { toDoEntrys[ToDoList.SelectedIndex].Text = ""; }
                }
            }
        }

        private void DelEntry_Click(object sender, RoutedEventArgs e)
        {
            if (delEntryIndex >= 0)
            {
                toDoEntrys.RemoveAt(delEntryIndex - 1);
                toDoEntrys.RemoveAt(delEntryIndex - 1);
                InfoLabel.BeginAnimation(OpacityProperty, InfoLabelAnimReverse);
                Buttons.DelEntry.Visibility = Visibility.Hidden;
                delEntryIndex = -1;
            }
            ToDoList.Items.Refresh();
        }

        private void ToDoBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (toDoEntrys[ToDoList.SelectedIndex].Name == "statusBox")
            //{
            
            //}
            //else
            //{
            //    ToDoList.SelectedItem = e.Source;
            //}
        }

        private void ToDoList_KeyDown(object sender, KeyEventArgs e)
        {
            //toDoEntrys[ToDoList.SelectedIndex].IsReadOnly = false;
            //toDoEntrys[ToDoList.SelectedIndex].Focus();
            //toDoEntrys[ToDoList.SelectedIndex].CaretIndex = toDoEntrys[ToDoList.SelectedIndex].Text.Length;
        }

        private void ToDoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //toDoEntrys.ForEach(item => { item.IsReadOnly = true; });
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (saveFinished) { await SaveListBoxData(); }
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsStack.Visibility == Visibility.Visible)
            {
                OptionsStack.Visibility = Visibility.Collapsed;
                Options.Foreground      = Brushes.Black;
            }
            else
            {
                OptionsStack.Visibility = Visibility.Visible;
                Options.Foreground      = Brushes.Green;
            }
        }

        private async void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (saveOnExit) { await SaveListBoxData(); this.Close(); } else { if (saveFinished) { this.Close(); }}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = saveFinished ? false : true;     // Cancels Closing of Mainwindow if SaveFinished not true !
        }

        // MouseEnter / Leave Events

        private void ToDoBoxMouseEnter(object sender, MouseEventArgs e)
        {
            if (ToDoList.HasItems)
            {
                ToDoList.SelectedItem = e.Source;
            }
        }

        private void NewEntry_MouseEnter(object sender, MouseEventArgs e)
        {
            NewEntry.Foreground = Brushes.DarkRed;
        }

        private void NewEntry_MouseLeave(object sender, MouseEventArgs e)
        {
            NewEntry.Foreground = Brushes.Black;
        }

        private void Save_MouseEnter(object sender, MouseEventArgs e)
        {
            Save.Foreground = saveFinished ? Brushes.DarkRed : Save.Foreground;
        }

        private void Save_MouseLeave(object sender, MouseEventArgs e)
        {
            Save.Foreground = saveFinished ? Brushes.Black : Save.Foreground;
        }

        private void Exit_MouseEnter(object sender, MouseEventArgs e)
        {
            Exit.Foreground = Brushes.DarkRed;
        }

        private void Exit_MouseLeave(object sender, MouseEventArgs e)
        {
            Exit.Foreground = Brushes.Black;
        }

        private void Options_MouseEnter(object sender, MouseEventArgs e)
        {
            Options.Foreground = (OptionsStack.Visibility == Visibility.Visible) ? Brushes.Green : Brushes.DarkRed;
        }

        private void Options_MouseLeave(object sender, MouseEventArgs e)
        {
            Options.Foreground = (OptionsStack.Visibility == Visibility.Visible) ? Brushes.Green : Brushes.Black;
        }
        private void DelEntry_MouseEnter(object sender, MouseEventArgs e)
        {
            Buttons.DelEntry.Background = Brushes.DarkRed;
        }

        private void DelEntry_MouseLeave(object sender, MouseEventArgs e)
        {
            Buttons.DelEntry.Background = Brushes.Black;
        }

        // MouseEnter / Leave Events END !
    }
}