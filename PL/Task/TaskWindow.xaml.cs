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

        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("CurrentTask", typeof(BO.Task), typeof(TaskWindow), new PropertyMetadata(null));

        public BO.Task CurrentTask
        {
            get { return (BO.Task)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }

        private bool isAdd;

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public TaskWindow(BO.Task task_, bool isAdd_ = false)
        {
            InitializeComponent();
            CurrentTask = task_;
            isAdd = isAdd_;
        }

        //public IEnumerable<BO.TaskInList> TaskList
        //{
        //    get { return (IEnumerable<BO.TaskInList>)GetValue(TaskListProperty); }
        //    set { SetValue(TaskListProperty, value); }
        //}

        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            return;
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (isAdd)
            {
                s_bl?.Task.Create(CurrentTask);
            }
            else
            {
                s_bl?.Task.Update(CurrentTask);
            }
            Close();
        }

        private void cbTaskStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TaskList = (TaskStatus == BO.Enums.Status.None) ? s_bl?.Task.ReadAll()! : s_bl?.Task.ReadAll(task => task.Status == TaskStatus)!;
            return;
        }
    }
}
