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
        private readonly DispatcherTimer PrgRoutineTimer = new();

        private List<TextBox> toDoEntrys = [];

        private Label DebugLabel = new();

        private string x = "Hi, I just want to know how to change a WPF label FontFamily in code-behind, since I have seen a lot of examples in C# but not In Basic. Thank you!";
        private string y = "PS: I can't understand why these things were so much simpler in Windows Forms, and basic things like this are so hard to do in WPF.";
        private string z = "Change Label FontFamily in code behind WPF Basic.PS: I can't understand why these things were so much simpler in Windows Forms, and basic things like this are so hard";

        public MainWindow()
        {
            InitializeComponent();

            PrgRoutineTimer.Interval = TimeSpan.FromMilliseconds(200);
            PrgRoutineTimer.Tick += PrgRoutine;

            //ToDoTextBox a = new();
            //ToDoTextBox b = new();
            //ToDoTextBox c = new();
            //ToDoTextBox d = new();
            //ToDoTextBox e = new();
            //ToDoTextBox f = new();
            //ToDoTextBox g = new();
            //ToDoTextBox h = new();
            //ToDoTextBox i = new();


            //ToDoList.Items.Add(a.ToDo(x));
            //ToDoList.Items.Add(b.ToDo(y));
            //ToDoList.Items.Add(c.ToDo(z));
            //ToDoList.Items.Add(d.ToDo(x));
            //ToDoList.Items.Add(e.ToDo(y));
            //ToDoList.Items.Add(f.ToDo(z));
            //ToDoList.Items.Add(g.ToDo(x));
            //ToDoList.Items.Add(h.ToDo(y));
            //ToDoList.Items.Add(i.ToDo(z));

            //ToDoList.SelectionMode = SelectionMode.Extended;
            //ToDoList.Focus();

            DebugLabel.Width = 1000;
            DebugLabel.Height = 60;
            DebugLabel.Foreground = Brushes.Black;
            DebugLabel.FontSize = 20;
            Menu.Children.Add(DebugLabel);

            ToDoList.ItemsSource = toDoEntrys;

            ToDoList.Focus();

            PrgRoutineTimer.Start();
        }

        private void PrgRoutine(object? sender, EventArgs e)
        {
            //if (ToDoList.SelectedIndex == 1)
            //{
            //    ToDoList.Items.RemoveAt(ToDoList.SelectedIndex);
            //}


            DebugLabel.Content = ToDoList.SelectedIndex.ToString();
        }

        // UI-Elements Events
        private void NewEntry_Click(object sender, RoutedEventArgs e)
        {
            ToDoTextBox ToDo = new();
            toDoEntrys.Add(ToDo.ToDo("Click here to enter smth !"));
            ToDoList.Items.Refresh();
        }

        private void NewEntry_MouseEnter(object sender, MouseEventArgs e)
        {
            NewEntry.Foreground = Brushes.YellowGreen;
        }

        private void NewEntry_MouseLeave(object sender, MouseEventArgs e)
        {
            NewEntry.Foreground = Brushes.OrangeRed;
        }

        private void ToDoList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ToDoList.IsFocused == true)         // Funktioniert noch nicht !
            {
                if (toDoEntrys[ToDoList.SelectedIndex].Text.First() == (char)1421)
                {
                    toDoEntrys[ToDoList.SelectedIndex].Text = toDoEntrys[ToDoList.SelectedIndex].Text.Replace((char)1421, (char)0x2713);
                    ToDoList.Items.Refresh();
                }
                else if (toDoEntrys[ToDoList.SelectedIndex].Text.First() == (char)0x2713)
                {
                    toDoEntrys[ToDoList.SelectedIndex].Text = toDoEntrys[ToDoList.SelectedIndex].Text.Replace((char)0x2713, (char)1421);
                    ToDoList.Items.Refresh();
                }
            }
        }

        private void ToDoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToDoList.Focus();
        }
    }
}