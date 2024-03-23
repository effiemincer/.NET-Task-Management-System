using Milestone; // Importing the Milestone namespace for access to its functionalities.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
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

namespace Engineer; // Declaring the namespace for the Engineer class.

/// <summary>
/// Interaction logic for EngineerListWindow.xaml
/// </summary>
public partial class EngineerListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.

    // Property to get or set the engineer's experience level.
    public BO.Enums.EngineerExperience EngineerExperience { get; set; } = BO.Enums.EngineerExperience.None;

    /// <summary>
    /// EngineerListWindow constructor
    /// </summary>
    public EngineerListWindow()
    {
        // Reads all engineers from the data source and initializes the window.
        InitializeComponent();
        EngineerList = s_bl?.Engineer.ReadAll();
    }

    /// <summary>
    /// EngineerList property to get or set the list of engineers.
    /// </summary>
    public IEnumerable<BO.Engineer> EngineerList
    {
        get { return (IEnumerable<BO.Engineer>)GetValue(EngineerListProperty); }
        set {  SetValue(EngineerListProperty, value); }
    }

    // Dependency property for EngineerList.
    public static readonly DependencyProperty EngineerListProperty =
        DependencyProperty.Register("EngineerList",
            typeof(IEnumerable<BO.Engineer>),
            typeof(EngineerListWindow),
            new PropertyMetadata(null)
        );

    /// <summary>
    /// Event handler for the EngineerExperience selection changed event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cbEngineerExperience_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Updates the EngineerList based on selected experience level.
        EngineerList = (EngineerExperience == BO.Enums.EngineerExperience.None) ? s_bl?.Engineer.ReadAll()! : s_bl?.Engineer.ReadAll(item => item.ExperienceLevel == EngineerExperience);
        return;
    }

    /// <summary>
    /// Event handler for the Add Engineer button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddEngineer_Click(object sender, RoutedEventArgs e)
    {
        // Closes the current window and opens a new EngineerWindow to add a new engineer.
        new EngineerWindow(new BO.Engineer(), true).ShowDialog();
        EngineerList = null;
        EngineerList = s_bl?.Engineer.ReadAll();
    }

    /// <summary>
    /// Event handler for the Delete Engineer button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void doubleClickEvent_UpdateEngineer(object sender, MouseButtonEventArgs e)
    {
        // Gets the selected engineer and opens a new EngineerWindow to update their details.
        BO.Engineer engineer = (sender as ListView)?.SelectedItem as BO.Engineer;
        if (engineer is null)
            return;
        new EngineerWindow(s_bl.Engineer.Read(engineer!.Id)!, false).ShowDialog();
        EngineerList = null;
        EngineerList = s_bl?.Engineer.ReadAll();
    }
}
