using BO;
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

        private bool? scheduleCreated = s_bl.Config.GetIsScheduleGenerated();

        // Constructor
        public TaskWindow(BO.Task task_, bool isAdd_ = false)
        {
            InitializeComponent();
            CurrentTask = task_; // Initialize current task
            isAdd = isAdd_; // Initialize flag for adding a task
            isDeliverableFirstRun = task_.Deliverable; // Initialize flag for deliverable property
            _dependenciesListDisplayTextBlock.Text = DependenciesListDisplay;

            //allowing adding depenedencies
            if (isAdd)
            {
                _dateCreated.Text = DateTime.Now.ToString("dd/MM/yyyy");
                if (scheduleCreated is null || !(bool)scheduleCreated)
                {
                    _dependenciesListDisplayTextBlock.IsEnabled = true;
                    _deadline.IsEnabled = true;
                    _projectedStart.IsEnabled = true;
                    _requiredEffort.IsEnabled = true;
                }
                else
                {
                    _dependenciesListDisplayTextBlock.IsEnabled = false;
                    _deadline.IsEnabled = false;
                    _projectedStart.IsEnabled = false;
                    _requiredEffort.IsEnabled = false;
                }
            }
            //changing the screen layout depending on if the scheudle has been created or not
            else
            {
                if (scheduleCreated is null || !(bool)scheduleCreated)
                {
                    _dependenciesListDisplayTextBlock.IsEnabled = false;
                    _deadline.IsEnabled = true;
                    _projectedStart.IsEnabled = true;
                    _requiredEffort.IsEnabled = true;
                }
                else
                {
                    _dependenciesListDisplayTextBlock.IsEnabled = false;
                    _deadline.IsEnabled = false;
                    _projectedStart.IsEnabled = false;
                    _requiredEffort.IsEnabled = false;
                }
            }

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

            //add
            if (isAdd)
            {
                try { 
                    //allow for adding dependencies
                    s_bl?.Task.Create(CurrentTask); 
                
                }
                catch(BlNullPropertyException ex)
                {
                    MessageBox.Show(ex.Message, "BadDataInput", MessageBoxButton.OK, MessageBoxImage.Error); 
                    return;
                }
                
            }

            //update
            else
            {

                //update with the schedule not yet created
                bool? isScheduleGenerated = s_bl.Config.GetIsScheduleGenerated();
                if (isScheduleGenerated is null || !(bool)isScheduleGenerated)
                {
                    try
                    {
                        BO.Task updatedTask = new BO.Task
                        {
                            Id = CurrentTask!.Id,
                            Alias = _alias.Text,
                            Description = _description.Text,
                            Deadline = _deadline.SelectedDate,
                            Status = CurrentTask!.Status,
                            Engineer = CurrentTask!.Engineer,
                            DateCreated = CurrentTask!.DateCreated,
                            ActualEndDate = _actualEndDate.SelectedDate,
                            ActualStartDate = _actualStartDate.SelectedDate,
                            Complexity = (BO.Enums.EngineerExperience?)Complexity_ComboBox.SelectedValue,
                            Deliverable = (bool)_deliverable.IsChecked!,
                            Dependencies = CurrentTask!.Dependencies,
                            Milestone = CurrentTask!.Milestone,
                            ProjectedStartDate = CurrentTask!.ProjectedStartDate,
                            Remarks = _remarks.Text,
                            RequiredEffortTime = CurrentTask!.RequiredEffortTime
                        };
                        s_bl?.Task.Update(updatedTask);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Invalid time format. Please enter time in the format HH:MM:SS", "Invalid Time Format", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                //update after the schedule was created
                else
                {
                    try
                    {
                        TimeSpan? timeSpan = TimeSpan.Parse(_requiredEffort.Text);
                        BO.Task updatedTask = new BO.Task
                        {
                            Id = CurrentTask!.Id,
                            Alias = _alias.Text,
                            Description = _description.Text,
                            Deadline = CurrentTask!.Deadline,
                            Status = CurrentTask!.Status,
                            Engineer = CurrentTask!.Engineer,
                            DateCreated = CurrentTask!.DateCreated,
                            ActualEndDate = _actualEndDate.SelectedDate,
                            ActualStartDate = _actualStartDate.SelectedDate,
                            Complexity = (BO.Enums.EngineerExperience?)Complexity_ComboBox.SelectedValue,
                            Deliverable = (bool)_deliverable.IsChecked!,
                            Dependencies = CurrentTask!.Dependencies,
                            Milestone = CurrentTask!.Milestone,
                            ProjectedStartDate = _projectedStart.SelectedDate,
                            Remarks = _remarks.Text,
                            RequiredEffortTime = timeSpan
                        };
                        s_bl?.Task.Update(updatedTask);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Invalid time format. Please enter time in the format HH:MM:SS", "Invalid Time Format", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                
            }
            Close();
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

        private string DependenciesListDisplay
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (CurrentTask.Dependencies is null)
                    return "";
                int count = CurrentTask.Dependencies.Count;
                foreach (BO.TaskInList task in CurrentTask.Dependencies)
                {
                    sb.Append(task.Id.ToString());
                    if (--count > 0)
                        sb.Append(", ");
                }
                return sb.ToString();

            }
        }
    }
}
