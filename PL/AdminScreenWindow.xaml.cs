using BO;
using Engineer;
using Milestone;
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
    /// Interaction logic for AdminScreenWindow.xaml
    /// </summary>
    public partial class AdminScreenWindow : Window
    {
        public AdminScreenWindow()
        {
            InitializeComponent();
        }

        private void Manage_Tasks_Click(object sender, RoutedEventArgs e)
        {
            // send IsAdmin is true
            new TaskListWindow(true).Show();
        }

        private void Manage_Engineers_Click(object sender, RoutedEventArgs e)
        {
            new EngineerListWindow().Show();
        }

        private void Manage_Milestones_Click(object sender, RoutedEventArgs e)
        {
            new MilestoneListWindow().Show();
        }
    }
}
