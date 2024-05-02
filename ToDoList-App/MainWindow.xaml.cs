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

namespace ToDoList_App          // The name says everything :)
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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