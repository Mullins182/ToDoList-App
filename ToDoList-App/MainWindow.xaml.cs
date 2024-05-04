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
using ToDoList_App.Model;

namespace ToDoList_App          // The name says everything :)
{
    public partial class MainWindow : Window
    {
        private string x = "Hi, I just want to know how to change a WPF label FontFamily in code-behind, since I have seen a lot of examples in C# but not In Basic. Thank you!";
        private string y = "PS: I can't understand why these things were so much simpler in Windows Forms, and basic things like this are so hard to do in WPF.";
        private string z = "Change Label FontFamily in code behind WPF Basic.PS: I can't understand why these things were so much simpler in Windows Forms, and basic things like this are so hard";

        public MainWindow()
        {
            InitializeComponent();

            ToDoTextBox a = new();
            ToDoTextBox b = new();
            ToDoTextBox c = new();
            ToDoTextBox d = new();
            ToDoTextBox e = new();
            ToDoTextBox f = new();
            ToDoTextBox g = new();
            ToDoTextBox h = new();
            ToDoTextBox i = new();


            ToDoList.Items.Add(a.ToDo(x, "Box1"));
            ToDoList.Items.Add(b.ToDo(y, "Box2"));
            ToDoList.Items.Add(c.ToDo(z, "Box3"));
            ToDoList.Items.Add(d.ToDo(x, "Box1"));
            ToDoList.Items.Add(e.ToDo(y, "Box2"));
            ToDoList.Items.Add(f.ToDo(z, "Box3"));
            ToDoList.Items.Add(g.ToDo(x, "Box1"));
            ToDoList.Items.Add(h.ToDo(y, "Box2"));
            ToDoList.Items.Add(i.ToDo(z, "Box3"));
            ToDoList.Focus();

        }



        // UI-Elements Events
        private void NewEntry_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NewEntry_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void NewEntry_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}