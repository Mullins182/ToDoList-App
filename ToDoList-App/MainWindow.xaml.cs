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
        private readonly DispatcherTimer saveTimer  = new();

        private List<TextBox> toDoEntrys            = [];

        private Label SaveInfo                      = new();

        private readonly string markInWorks         = $"{(char)1421} in the works {(char)1421}";    
        private readonly string markDone            = $"Done {(char)0x2713}";

        public MainWindow()
        {
            InitializeComponent();

            saveTimer.Interval = TimeSpan.FromMinutes(1);
            saveTimer.Tick += SaveRoutine;

            SaveInfo.Visibility = Visibility.Hidden;
            SaveInfo.Width = 300;
            SaveInfo.Height = 50;
            SaveInfo.Foreground = Brushes.OrangeRed;
            SaveInfo.FontFamily = new FontFamily("Chiller");
            SaveInfo.Content = "Data Saved !";
            SaveInfo.FontSize = 37;
            SaveInfo.FontWeight = FontWeights.Bold;

            Menu.Children.Add(SaveInfo);

            ToDoList.ItemsSource = toDoEntrys;

            if (File.Exists("Data.dat"))
            {
                ReadData();
            }

            ToDoList.Focus();

            saveTimer.Start();
        }

        private async void SaveRoutine(object? sender, EventArgs e)
        {
            SaveInfo.Visibility = Visibility.Visible;
            SaveData();

            await Task.Delay(4500);
            SaveInfo.Visibility = Visibility.Hidden;
        }

        private void SaveData()
        {
            List<string> writeCache = [];

            foreach (var item in toDoEntrys)
            {
                writeCache.Add(item.Text);
            }

            File.WriteAllLines("data.dat", writeCache);
        }

        private void ReadData()
        {
            List<string> readCache = [.. File.ReadAllLines("Data.dat")];     // [.. ] = simplifying .toList()

            ToDoTextBox ToDo = new();

            List<TextBox> dataCache = ToDo.ReadToDos(readCache);

            foreach (var item in dataCache)
            {
                toDoEntrys.Add(item);
            }

            ToDoList.Items.Refresh();
        }

        // UI-Elements Events
        private void NewEntry_Click(object sender, RoutedEventArgs e)
        {
            ToDoTextBox ToDo = new();
            toDoEntrys.Add(ToDo.NewToDo("Click here to enter smth !"));
            ToDoList.Items.Refresh();
        }

        private void ToDoList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToDoList.Focus();

            if (ToDoList.SelectedIndex >= 0)         // Funktioniert noch nicht !
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
            ToDoList.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void NewEntry_MouseEnter(object sender, MouseEventArgs e)
        {
            NewEntry.Foreground = Brushes.YellowGreen;
        }

        private void NewEntry_MouseLeave(object sender, MouseEventArgs e)
        {
            NewEntry.Foreground = Brushes.OrangeRed;
        }

        private void Save_MouseEnter(object sender, MouseEventArgs e)
        {
            Save.Foreground = Brushes.YellowGreen;
        }

        private void Save_MouseLeave(object sender, MouseEventArgs e)
        {
            Save.Foreground = Brushes.OrangeRed;
        }

        private void Exit_MouseEnter(object sender, MouseEventArgs e)
        {
            Exit.Foreground = Brushes.YellowGreen;
        }

        private void Exit_MouseLeave(object sender, MouseEventArgs e)
        {
            Exit.Foreground = Brushes.OrangeRed;
        }
    }
}