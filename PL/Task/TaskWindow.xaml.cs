using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace Task
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        //public static readonly DependencyProperty TaskListProperty =
        //    DependencyProperty.Register("TaskList",
        //        typeof(IEnumerable<BO.TaskInList>),
        //        typeof(TaskListWindow),
        //        new PropertyMetadata(null)
        //    );

        // Dependency property to bind a Task object
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("CurrentTask", typeof(BO.Task), typeof(TaskWindow), new PropertyMetadata(null));

        // Property to get or set the current task
        public BO.Task CurrentTask
        {
            get { return (BO.Task)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }

        private bool isAdd; // Flag indicating if it's adding a new task

        private bool isDeliverableFirstRun; // Flag indicating if the deliverable property has been set for the first time

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Reference to business logic instance

        // Constructor
        public TaskWindow(BO.Task task_, bool isAdd_ = false)
        {
            InitializeComponent();
            CurrentTask = task_; // Initialize current task
            isAdd = isAdd_; // Initialize flag for adding a task
            isDeliverableFirstRun = task_.Deliverable; // Initialize flag for deliverable property
        }

        // TextChanged event handler for TextBox (currently not used)
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            return;
        }

        // Click event handler for Add/Update button
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Add or update task based on isAdd flag
            if (isAdd)
            {
                s_bl?.Task.Create(CurrentTask);
            }
            else
            {
                s_bl?.Task.Update(CurrentTask);
            }
            Close(); // Close the window
        }

        // SelectionChanged event handler for Task Status ComboBox (currently not used)
        private void cbTaskStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update TaskList based on selected task status
            //TaskList = (TaskStatus == BO.Enums.Status.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Status == TaskStatus)!;
            return;
        }

        // Flag to indicate if RadioButton was just checked
        private bool JustChecked;

        // Checked event handler for RadioButton
        private void RB_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton s = sender as RadioButton;
            // Action on Check...

            JustChecked = true;
            if (isDeliverableFirstRun)
            {
                JustChecked = false;
                isDeliverableFirstRun = false;
            }
        }

        // Clicked event handler for RadioButton
        private void RB_Clicked(object sender, RoutedEventArgs e)
        {
            if (JustChecked)
            {
                JustChecked = false;
                e.Handled = true;
                return;
            }
            RadioButton s = sender as RadioButton;
            if ((bool)s.IsChecked)
                s.IsChecked = false;

        }
    }
}
