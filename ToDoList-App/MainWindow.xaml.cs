using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ToDoList_App.Model;

namespace ToDoList_App          // The name says everything :)
{
    public partial class MainWindow : Window
    {
        private readonly MediaPlayer floppyWrite            = new();

        private readonly DispatcherTimer saveTimer          = new();
        private readonly DispatcherTimer savingAnimTimer    = new();

        private List<TextBox> toDoEntrys            = [];

        private double savingAnimRectPos            = 0.00;
        private bool savingAnimRectUp               = true;
        private bool playbackEnded                  = true;
        private bool entryFinished                  = true;

        private readonly string markInWorks         = $"{(char)1421} in the works {(char)1421}";    
        private readonly string markDone            = $"Done {(char)0x2713}";

        public MainWindow()
        {
            floppyWrite.IsMuted = true;

            InitializeComponent();

            InitializePrg();
        }

        private async void InitializePrg()
        {
            await Task.Delay(500);

            floppyWrite.Open(new Uri("Sounds/readingFloppyDisc.mp3", UriKind.Relative));
            floppyWrite.Position = TimeSpan.FromMilliseconds(50);

            saveTimer.Interval = TimeSpan.FromMinutes(1);
            savingAnimTimer.Interval = TimeSpan.FromMilliseconds(25);
            saveTimer.Tick += SaveRoutine;
            savingAnimTimer.Tick += SavingAnimationRoutine;

            Canvas.SetLeft(SavingAnim, (SavingCanvas.Width / 2) - SavingAnim.Width / 2);
            Canvas.SetTop(SavingAnim, (SavingCanvas.Height / 2) - SavingAnim.Height / 2);
            savingAnimRectPos = Canvas.GetTop(SavingAnim);

            ToDoList.ItemsSource = toDoEntrys;

            if (File.Exists("Data.dat"))
            {
                ReadData();
            }

            ToDoList.Focus();

            saveTimer.Start();

            floppyWrite.IsMuted       = false;
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
            savingAnimTimer.Start();
            SavingAnim.Visibility = Visibility.Visible;

            SaveData();

            floppyWrite.Position = TimeSpan.FromMilliseconds(50);
            floppyWrite.Play();

            await Task.Delay(4500);

            floppyWrite.Stop();
            savingAnimTimer.Stop();
            SavingAnim.Visibility = Visibility.Hidden;
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
            List<string> readCache  = [.. File.ReadAllLines("Data.dat")];     // [.. ] = simplifying .toList()

            ToDoTextBox ToDo        = new();

            foreach (var item in ToDo.ReadToDos(readCache))
            {
                toDoEntrys.Add(item);
            }

            toDoEntrys.ForEach(item => { item.MouseDoubleClick += ToDoBox; });

            readCache.Clear();
            ToDoList.Items.Refresh();
        }

        private void ToDoBox(object sender, MouseButtonEventArgs e)
        {
            ToDoList.SelectedItem = e.Source;
        }

        // UI-Elements Events
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
            ToDoList.Focus();

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
            if (playbackEnded)
            {
                playbackEnded = false;

                saveTimer.Stop();
                savingAnimTimer.Start();
                SavingAnim.Visibility = Visibility.Visible;

                floppyWrite.Position = TimeSpan.FromMilliseconds(50);
                floppyWrite.Play();
                SaveData();

                await Task.Delay(4500);

                floppyWrite.Stop();

                savingAnimTimer.Stop();

                SavingAnim.Visibility = Visibility.Hidden;

                saveTimer.Start();

                playbackEnded = true;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        // MouseEnter / Leave Events END !
    }
}