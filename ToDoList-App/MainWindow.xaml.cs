using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

namespace ToDoList_App          // AI Helper for programmers => https://www.phind.com/ :)
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

        private string[] options                            = ["Fullscreen Mode = 0", "Autosave Minutes = 0", "Save On Exit = 0"];

        private int delEntryIndex                           = -1;

        private double savingAnimRectPos                    = 0.00;
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
            Buttons.SetAutoSaveProps();
            Buttons.SetDelEntryProps();
            Buttons.SetSaveOnPrgExitProps();
            Buttons.FullscreenMode.Template     = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.DelEntry.Template           = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.AutoSave.Template           = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.saveOnPrgExit.Template      = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.FullscreenMode.Click        += FullscreenMode_Click;
            Buttons.AutoSave.Click              += AutoSave_Click;
            Buttons.DelEntry.Click              += DelEntry_Click;
            Buttons.saveOnPrgExit.Click         += SaveOnPrgExit_Click;
            Grid.SetRow(Buttons.DelEntry, 0);
            Grid.SetColumn(Buttons.DelEntry, 1);

            floppyWrite.IsMuted = true;
            scribble1.IsMuted   = true;
            scribble2.IsMuted   = true;
            scribble1.Play();
            scribble2.Play();

            await Task.Delay(1000);

            floppyWrite.Open(new Uri("Sounds/readingFloppyDisc.mp3", UriKind.Relative));
            floppyWrite.Position = TimeSpan.FromMilliseconds(150);
            scribble1.Open(new Uri("Sounds/scribble1.wav", UriKind.Relative));
            scribble2.Open(new Uri("Sounds/scribble2.wav", UriKind.Relative));

            if (File.Exists("options.ini"))
            {
                options = [];
                options = File.ReadAllLines("options.ini");

                if (options[0].Last() - 48 == 0)
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Normal;
                    Buttons.FullscreenMode.Foreground = Brushes.Red;


                }
                else if (options[0].Last() - 48 == 1)
                {
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                    Buttons.FullscreenMode.Foreground = Brushes.LawnGreen;
                }

                if (options[2].Last() - 48 == 0)
                {
                    Buttons.saveOnPrgExit.Foreground = Brushes.Red;

                    saveOnExit = false;
                }
                else if (options[2].Last() - 48 == 1)
                {
                    Buttons.saveOnPrgExit.Foreground = Brushes.LawnGreen;

                    saveOnExit = true;
                }

                saveTimer.Interval = TimeSpan.FromMinutes(((double)options[1].Last()) -48);

                Buttons.SetAutoSaveProps(saveTimer.Interval);
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

            Canvas.SetLeft(SavingAnim, (SavingCanvas.Width / 2) - SavingAnim.Width / 2);
            Canvas.SetTop(SavingAnim, (SavingCanvas.Height / 2) - SavingAnim.Height / 2);
            savingAnimRectPos = Canvas.GetTop(SavingAnim);

            ToDoList.ItemsSource = toDoEntrys;

            InfoLabel.Background = Brushes.Black;
            InfoLabel.Content = "Left Click again to finish editing !";

            if (File.Exists("data.dat"))
            {
                ReadData();
            }

            MainGrid.Children.Add(Buttons.DelEntry);

            OptionsStack.Children.Add(Buttons.FullscreenMode);
            OptionsStack.Children.Add(Buttons.AutoSave);
            OptionsStack.Children.Add(Buttons.saveOnPrgExit);

            if (saveTimer.Interval != TimeSpan.FromMinutes(0)) { saveTimer.Start(); }

            floppyWrite.IsMuted = false;
            scribble1.IsMuted = false;
            scribble2.IsMuted = false;
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
                Canvas.SetTop(SavingAnim, savingAnimRectPos -= 1.5);
            }
            else
            {
                Canvas.SetTop(SavingAnim, savingAnimRectPos += 1.5);
            }
        }

        private async void SaveRoutine(object? sender, EventArgs e)
        {
            saveFinished = false;

            savingAnimTimer.Start();
            SavingAnim.Visibility = Visibility.Visible;

            SaveData();

            floppyWrite.Position = TimeSpan.FromMilliseconds(50);
            floppyWrite.Play();

            await Task.Delay(3000);

            floppyWrite.Stop();
            savingAnimTimer.Stop();
            SavingAnim.Visibility = Visibility.Hidden;

            saveFinished = true;
        }

        private void SaveData()
        {
            List<string> writeCache = [];

            foreach (var item in toDoEntrys)
            {
                writeCache.Add(item.Text);
            }

            File.WriteAllLines("data.dat", writeCache);

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
                    toDoEntrys[i].Foreground    = Brushes.DarkRed;
                    toDoEntrys[i+1].Name        = "EntryWork";
                }
                else if (toDoEntrys[i].Text.Contains($"{(char)0x2713}"))
                {
                    toDoEntrys[i].Foreground    = Brushes.Green;
                    toDoEntrys[i+1].Name        = "EntryDone";
                }
            }

            toDoEntrys.ForEach(item => { item.MouseDoubleClick += ToDoBoxMouseDoubleClick; });
            toDoEntrys.ForEach(item => { item.MouseEnter += ToDoBoxMouseEnter; });
            toDoEntrys.ForEach(item => { item.TextDecorations = item.Name == "EntryDone" ? TextDecorations.Strikethrough : null; });

            ToDoList.Items.Refresh();
        }

        private async Task SaveListBoxData()
        {
            saveFinished = false;

            Save.Foreground = Brushes.Green;

            saveTimer.Stop();
            savingAnimTimer.Start();
            SavingAnim.Visibility = Visibility.Visible;

            floppyWrite.Position = TimeSpan.FromMilliseconds(50);
            floppyWrite.Play();
            SaveData();

            await Task.Delay(3000);

            floppyWrite.Stop();

            savingAnimTimer.Stop();

            SavingAnim.Visibility = Visibility.Hidden;

            if (saveTimer.Interval != TimeSpan.FromMinutes(0))
            {
                saveTimer.Start();
            }

            Save.Foreground = Brushes.Black;
            saveFinished = true;
        }

        private void ToDoBoxMouseLeave(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        // UI-Elements Click Events

        // Code-Behind Elements
        private async void FullscreenMode_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                Buttons.FullscreenMode.Foreground = Brushes.Red;

                options[0] = "Fullscreen Mode = 0";

                await File.WriteAllLinesAsync("options.ini", options, UTF8Encoding.Unicode);
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                Buttons.FullscreenMode.Foreground = Brushes.LawnGreen;

                options[0] = "Fullscreen Mode = 1";

                await File.WriteAllLinesAsync("options.ini", options, UTF8Encoding.Unicode);
            }
        }

        private async void AutoSave_Click(object sender, RoutedEventArgs e)
        {
            saveTimer.Stop();

            saveTimer.Interval = (saveTimer.Interval == TimeSpan.FromMinutes(1)) ? TimeSpan.FromMinutes(3) : (saveTimer.Interval == TimeSpan.FromMinutes(3)) 
                ? TimeSpan.FromMinutes(5) : (saveTimer.Interval == TimeSpan.FromMinutes(5)) ? TimeSpan.FromMinutes(9) 
                : (saveTimer.Interval == TimeSpan.FromMinutes(9)) ? TimeSpan.FromMinutes(0) : TimeSpan.FromMinutes(1);

            Buttons.SetAutoSaveProps(saveTimer.Interval);

            options[1] = options[1].Replace(options[1].Last() , (char)(saveTimer.Interval.Minutes + 48));

            await File.WriteAllLinesAsync("options.ini", options);

            if (saveTimer.Interval != TimeSpan.FromMinutes(0))
            {
                saveTimer.Start();
            }
        }

        private async void SaveOnPrgExit_Click(object sender, RoutedEventArgs e)
        {
            saveOnExit = saveOnExit ? false : true;

            Buttons.saveOnPrgExit.Foreground = saveOnExit ? Brushes.LawnGreen : Brushes.Red;

            options[2] = saveOnExit ? "Save On Exit = 1" : "Save On Exit = 0";

            await File.WriteAllLinesAsync("options.ini", options, UTF8Encoding.Unicode);
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

                scribble1.Position = TimeSpan.Zero;
                scribble2.Position = TimeSpan.Zero;
                scribble1.Stop();
                scribble2.Stop();

                if (toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markInWorks))
                {
                    toDoEntrys[ToDoList.SelectedIndex].Text = 
                    toDoEntrys[ToDoList.SelectedIndex].Text.Replace(markInWorks, markDone);
                    toDoEntrys[ToDoList.SelectedIndex].Foreground = Brushes.Green;
                    toDoEntrys[ToDoList.SelectedIndex + 1].TextDecorations = TextDecorations.Strikethrough;
                    toDoEntrys[ToDoList.SelectedIndex + 1].Name = "EntryDone";

                    scribble2.Play();

                    entryFinished = true;

                    return;
                }                
                else if (toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markDone))
                {
                    toDoEntrys[ToDoList.SelectedIndex].Text = 
                    toDoEntrys[ToDoList.SelectedIndex].Text.Replace(markDone, markInWorks);
                    toDoEntrys[ToDoList.SelectedIndex].Foreground = Brushes.DarkRed;
                    toDoEntrys[ToDoList.SelectedIndex + 1].TextDecorations = null;
                    toDoEntrys[ToDoList.SelectedIndex + 1].Name = "EntryWork";
                    ToDoList.Items.Refresh();

                    scribble1.Play();

                    entryFinished = true;

                    return;
                }
            }
        }

        private void ToDoList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ToDoList.SelectedIndex < 0 || toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markDone) 
                || toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markInWorks))
            {

            }
            else
            {
                toDoEntrys[ToDoList.SelectedIndex].IsReadOnly = toDoEntrys[ToDoList.SelectedIndex].IsReadOnly == true ? false : true;
                                                
                if (toDoEntrys[ToDoList.SelectedIndex].IsReadOnly == true)
                {
                    if (toDoEntrys[ToDoList.SelectedIndex].Text == "") { toDoEntrys[ToDoList.SelectedIndex].Text = "enter smth here !"; }

                    InfoLabel.BeginAnimation(OpacityProperty, InfoLabelAnimReverse);
                    Buttons.DelEntry.Visibility = Visibility.Hidden;
                    delEntryIndex = -1;
                }
                else if (toDoEntrys[ToDoList.SelectedIndex].IsReadOnly == false)
                {
                    delEntryIndex = ToDoList.SelectedIndex;
                    InfoLabel.BeginAnimation(OpacityProperty, InfoLabelAnim);
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

        // MouseEnter / Leave Events END !
    }
}