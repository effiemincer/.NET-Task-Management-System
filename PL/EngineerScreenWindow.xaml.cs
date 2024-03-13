using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Task;

namespace PL
{
    /// <summary>
    /// Interaction logic for EngineerScreenWindow.xaml
    /// </summary>
    public partial class EngineerScreenWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public EngineerScreenWindow()
        {
            InitializeComponent();
        }

        private void Tasks_Button_Click(object sender, RoutedEventArgs e)
        {
            // IsAdmin is false
            new TaskListWindow(false).Show();
        }

        private void Current_Task_Button_Click(object sender, RoutedEventArgs e)
        {
            new TaskWindow(s_bl.Task.Read(8001), false).Show();
        }
    }
}
