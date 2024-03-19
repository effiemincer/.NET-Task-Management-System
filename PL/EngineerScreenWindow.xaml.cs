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
using Task; // Importing the Task namespace for access to its functionalities.

namespace PL
{
    /// <summary>
    /// Interaction logic for EngineerScreenWindow.xaml
    /// </summary>
    public partial class EngineerScreenWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.

        private int engID; // Field to store the ID of the user.
        private BO.Engineer currentEng;

        // Constructor for EngineerScreenWindow class.
        public EngineerScreenWindow(int _id)
        {
            InitializeComponent(); // Initializes the window components.
            engID = _id; // Sets the ID field to the given ID.
            currentEng = s_bl?.Engineer.Read(_id)!;
        }

        // Event handler for "Tasks" button click.
        private void Tasks_Button_Click(object sender, RoutedEventArgs e)
        {
            // Opens the TaskListWindow with isAdmin set to false, indicating non-admin access.
            new TaskListWindow(false).Show();
        }

        // Event handler for "Current Task" button click.
        private void Current_Task_Button_Click(object sender, RoutedEventArgs e)
        {
            // Opens the TaskWindow for a specific task 
            if (currentEng.Task is null)
            {
                MessageBox.Show(currentEng.Name + " does not have any currently assigned Tasks.");
            }
            else
            {
                new TaskWindow(s_bl!.Task.Read((int)currentEng.Task!.Id!)!).ShowDialog();
            }
           
        }
    }
}
