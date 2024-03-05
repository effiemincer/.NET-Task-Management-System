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
        public TaskWindow(BO.Task task_, bool isAdd_ = false)
        {
            InitializeComponent();
            isAdd = isAdd_;
            task = task_;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            return;
        }

    }
}
