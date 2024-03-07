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
        private bool isAdd;

        private BO.Task task;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Enums.Status TaskStatus { get; set; } = BO.Enums.Status.None;

        public TaskWindow(BO.Task task_, bool isAdd_ = false)
        {
            TaskList = s_bl?.Task.ReadAll();
            InitializeComponent();
            isAdd = isAdd_;
            task = task_;
        }

        public IEnumerable<BO.TaskInList> TaskList
        {
            get { return (IEnumerable<BO.TaskInList>)GetValue(TaskListProperty); }
            set { SetValue(TaskListProperty, value); }
        }

        public static readonly DependencyProperty TaskListProperty =
    DependencyProperty.Register("TaskList",
        typeof(IEnumerable<BO.TaskInList>),
        typeof(TaskListWindow),
        new PropertyMetadata(null)
    );
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            return;
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (isAdd)
            {
                s_bl?.Task.Create(task);
            }
            else
            {
                s_bl?.Task.Update(task);
            }
            Close();
        }

        private void cbTaskStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TaskList = (TaskStatus == BO.Enums.Status.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Status == TaskStatus)!;
        }

    }
}
