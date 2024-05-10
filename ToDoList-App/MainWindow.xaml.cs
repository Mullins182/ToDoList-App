using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ToDoList_App.Model;
using WinRT;

namespace ToDoList_App          // The name says everything :)
{
    public partial class MainWindow : Window
    {
        private readonly MediaPlayer floppyWrite            = new();

        private DispatcherTimer saveTimer                   = new();
        private readonly DispatcherTimer savingAnimTimer    = new();

        private List<TextBox> toDoEntrys            = [];

        private double savingAnimRectPos            = 0.00;
        private bool savingAnimRectUp               = true;
        private bool entryFinished                  = true;
        private bool saveFinished                   = true;

        private readonly string markInWorks         = $"{(char)1421} in the works {(char)1421}";    
        private readonly string markDone            = $"Done {(char)0x2713}";

        public MainWindow()
        {
            floppyWrite.IsMuted = true;

            InitializeComponent();

            InitializePrg();
        }

        public async void InitializePrg()
        {
            await Task.Delay(800);

            floppyWrite.Open(new Uri("Sounds/readingFloppyDisc.mp3", UriKind.Relative));
            floppyWrite.Position        = TimeSpan.FromMilliseconds(150);

            saveTimer.Interval          = TimeSpan.FromMinutes(1);
            savingAnimTimer.Interval    = TimeSpan.FromMilliseconds(25);
            saveTimer.Tick              += SaveRoutine;
            savingAnimTimer.Tick        += SavingAnimationRoutine;

            Canvas.SetLeft(SavingAnim, (SavingCanvas.Width / 2) - SavingAnim.Width / 2);
            Canvas.SetTop(SavingAnim, (SavingCanvas.Height / 2) - SavingAnim.Height / 2);
            savingAnimRectPos = Canvas.GetTop(SavingAnim);

            ToDoList.ItemsSource = toDoEntrys;

            if (File.Exists("data.dat"))
            {
                ReadData();
            }

            Buttons.SetFullscreenModeProps();
            Buttons.SetAutoSaveProps();
            Buttons.FullscreenMode.Template     = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.AutoSave.Template           = (ControlTemplate)FindResource("NoMouseOverButtonTemplate"); // Suggestion from Bing Co-Pilot
            Buttons.FullscreenMode.Click        += FullscreenMode_Click;
            Buttons.AutoSave.Click              += AutoSave_Click;

            OptionsStack.Children.Add(Buttons.FullscreenMode);
            OptionsStack.Children.Add(Buttons.AutoSave);

            saveTimer.Start();

            floppyWrite.IsMuted = false;
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
            List<string> readCache  = [.. System.IO.File.ReadAllLines("data.dat")];     // [.. ] = simplifying .toList()

            ToDoTextBox ToDo        = new();

            ToDoList.ItemsSource = toDoEntrys;

            foreach (var item in ToDo.ReadToDos(readCache))
            {
                toDoEntrys.Add(item);
            }

            toDoEntrys.ForEach(item => { item.MouseDoubleClick += ToDoBoxMouseDoubleClick; });

            ToDoList.Items.Refresh();
        }

        private void ToDoBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToDoList.SelectedItem = e.Source;
        }

        // UI-Elements Click Events

        // Code-Behind Elements
        private void FullscreenMode_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                Buttons.FullscreenMode.Foreground = Brushes.Red;
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                Buttons.FullscreenMode.Foreground = Brushes.LawnGreen;
            }
        }

        private void AutoSave_Click(object sender, RoutedEventArgs e)
        {
            saveTimer.Stop();

            saveTimer.Interval = (saveTimer.Interval == TimeSpan.FromMinutes(1)) ? TimeSpan.FromMinutes(3) : (saveTimer.Interval == TimeSpan.FromMinutes(3)) 
                ? TimeSpan.FromMinutes(5) : (saveTimer.Interval == TimeSpan.FromMinutes(5)) ? TimeSpan.FromMinutes(10) 
                : (saveTimer.Interval == TimeSpan.FromMinutes(10)) ? TimeSpan.FromMinutes(0) : TimeSpan.FromMinutes(1);

            Buttons.AutoSave.Content = (saveTimer.Interval == TimeSpan.FromMinutes(1)) ? "Autosave every Minute" 
                : (saveTimer.Interval == TimeSpan.FromMinutes(3)) ? "Autosave every 3 Minutes" : (saveTimer.Interval == TimeSpan.FromMinutes(5))
                ? "Autosave every 5 Minutes" : (saveTimer.Interval == TimeSpan.FromMinutes(10)) ? "Autosave every 10 Minutes" : "Autosave Off";

            Buttons.AutoSave.Foreground = (saveTimer.Interval == TimeSpan.FromMinutes(0)) ? Brushes.Red : Brushes.Green;

            if (saveTimer.Interval != TimeSpan.FromMinutes(0))
            {
                saveTimer.Start();
            }
        }

        // Code-Behind Elements END 

        private void NewEntry_Click(object sender, RoutedEventArgs e)
        {
            if (entryFinished)
            {
                entryFinished = false;

                ToDoTextBox ToDo = new();
                toDoEntrys.Add(ToDo.NewToDo("Click here to enter smth !"));
                ToDoList.Items.Refresh();

                entryFinished = true;
            }
        }

        private void ToDoList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ToDoList.SelectedIndex >= 0)
            {
                if (toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markInWorks))
                {
                    toDoEntrys[ToDoList.SelectedIndex].Text = 
                    toDoEntrys[ToDoList.SelectedIndex].Text.Replace(markInWorks, markDone);
                    ToDoList.Items.Refresh();
                }
                else if (toDoEntrys[ToDoList.SelectedIndex].Text.Contains(markDone))
                {
                    toDoEntrys[ToDoList.SelectedIndex].Text = 
                    toDoEntrys[ToDoList.SelectedIndex].Text.Replace(markDone, markInWorks);
                    ToDoList.Items.Refresh();
                }
            }
        }

        private void ToDoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            toDoEntrys.ForEach(item => { item.IsReadOnly = true; });

            ToDoList.Items.Refresh();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (saveFinished)
            {
                saveFinished = false;

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

                saveFinished = true;
            }
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (saveFinished)
            {
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = saveFinished ? false : true;     // Cancels Closing of Mainwindow if SaveFinished not true !
        }

        // MouseEnter / Leave Events

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
            Save.Foreground = Brushes.DarkRed;
        }

        private void Save_MouseLeave(object sender, MouseEventArgs e)
        {
            Save.Foreground = Brushes.Black;
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