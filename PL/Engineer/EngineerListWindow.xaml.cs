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

    // Constructor for EngineerListWindow class.
    public EngineerListWindow()
    {
        // Reads all engineers from the data source and initializes the window.
        InitializeComponent();
        EngineerList = s_bl?.Engineer.ReadAll();
    }

    // Property to bind a list of engineers to a UI element.
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

    // Event handler for selection change in engineer experience combo box.
    private void cbEngineerExperience_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Updates the EngineerList based on selected experience level.
        EngineerList = (EngineerExperience == BO.Enums.EngineerExperience.None) ? s_bl?.Engineer.ReadAll()! : s_bl?.Engineer.ReadAll(item => item.ExperienceLevel == EngineerExperience);
        return;
    }

    // Event handler for adding a new engineer.
    private void btnAddEngineer_Click(object sender, RoutedEventArgs e)
    {
        // Closes the current window and opens a new EngineerWindow to add a new engineer.
        new EngineerWindow(new BO.Engineer(), true).ShowDialog();
        EngineerList = null;
        EngineerList = s_bl?.Engineer.ReadAll();
    }

    // Event handler for double-click event on the engineer list to update an engineer's details.
    private void doubleClickEvent_UpdateEngineer(object sender, MouseButtonEventArgs e)
    {
        // Gets the selected engineer and opens a new EngineerWindow to update their details.
        BO.Engineer engineer = (sender as ListView)?.SelectedItem as BO.Engineer;
        new EngineerWindow(s_bl.Engineer.Read(engineer!.Id)!, false).ShowDialog();
        EngineerList = null;
        EngineerList = s_bl?.Engineer.ReadAll();
    }
}
