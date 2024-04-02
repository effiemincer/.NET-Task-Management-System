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

namespace PL;

/// <summary>
/// Interaction logic for ProjectSizeWindow.xaml
/// </summary>
public partial class ProjectSizeWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.
    public ProjectSizeWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Event handler for the Initialize Data Small button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Initialize_Data_Small(object sender, RoutedEventArgs e)
    {
        DalTest.DalTest.Initialization.Do(0);
        MessageBox.Show("Data initialized", "Data initialized", MessageBoxButton.OK, MessageBoxImage.Information);
        s_bl.Milestone.Reset();
        this.Close();
    }

    /// <summary>
    /// Event handler for the Initialize Data Medium button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Initialize_Data_Medium(object sender, RoutedEventArgs e)
    {
        DalTest.DalTest.Initialization.Do(1);
        MessageBox.Show("Data initialized", "Data initialized", MessageBoxButton.OK, MessageBoxImage.Information);
        s_bl.Milestone.Reset();
        this.Close();
    }

    /// <summary>
    /// Event handler for the Initialize Data Large button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Initialize_Data_Large(object sender, RoutedEventArgs e)
    {
        DalTest.DalTest.Initialization.Do(2);
        MessageBox.Show("Data initialized", "Data initialized", MessageBoxButton.OK, MessageBoxImage.Information);
        s_bl.Milestone.Reset();
        this.Close();
    }


}
