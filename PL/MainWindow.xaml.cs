using DalApi;
using PL.Admin;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Static reference to the business logic layer.
    private bool dataInitialized = false; // Flag to track if data is initialized.

    // Dependency property for binding the current time.
    public static readonly DependencyProperty TimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(null));

   /// <summary>
   /// DateTime property to get or set the current time.
   /// </summary>
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(TimeProperty); }
        set { SetValue(TimeProperty, s_bl.Clock); }
    }

    /// <summary>
    /// Constructor for the MainWindow class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        CurrentTime = s_bl.Clock; // Sets the current time.
    }

    /// <summary>
    /// Event handler for the Admin button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Admin_Button_Click(object sender, RoutedEventArgs e)
    {
        // Opens the ProjectDatesWindow if project start and end dates are not set.
        if (s_bl.Config.GetProjectStartDate() is null || s_bl.Config.GetProjectEndDate() is null)
        {
            new ProjectDatesWindow().ShowDialog();
        }

        // Checks if both start and end dates are set and opens the AdminScreenWindow.
        if (s_bl.Config.GetProjectStartDate() is not null && s_bl.Config.GetProjectEndDate() is not null)
        {
            new AdminScreenWindow().Show();
            Close();
        }
        else
        {
            MessageBox.Show("Please set the project start and end dates first", "DatesNotSet", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

   /// <summary>
   /// Event handler for the Engineer button click event.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    private void Engineer_Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Validates engineer ID input.
            if (string.IsNullOrEmpty(engineerID.Text))
            {
                MessageBox.Show("Please enter an ID", "EngNotFound", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(engineerID.Text, out _))
            {
                MessageBox.Show("ID must be a number", "EngNotFound", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Retrieves engineer information and opens EngineerScreenWindow.
            BO.Engineer eng = s_bl?.Engineer.Read(int.Parse(engineerID.Text));
            if (eng == null)
            {
                MessageBox.Show("Engineer not found", "EngNotFound", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else
            {
                new EngineerScreenWindow(eng.Id).Show();
                Close();
            }
        }
        catch (BO.BlDoesNotExistException)
        {
            MessageBox.Show("Engineer not found", "EngNotFound", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return;
        }
    }

    /// <summary>
    /// Event handler for the Initialize Data button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Initialize_Data_Button_Click(object sender, RoutedEventArgs e)
    {
        // Initializes data if not already initialized.
        if (!dataInitialized)
        {
            DalTest.DalTest.Initialization.Do();
            MessageBox.Show("Data initialized", "Data initialized", MessageBoxButton.OK, MessageBoxImage.Information);
            dataInitialized = true;
        }
        else
        {
            MessageBox.Show("Data was already initialized.\n" +
                "Reset all data in order to initialize it again.", "DataAlreadyInitialized", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    /// <summary>
    /// Event handler for the Reset Data button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Reset_Data_Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult res = MessageBox.Show("Are you sure you want to reset all the data?", "Reset Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        // Resets all data if confirmed by the user.
        if (res == MessageBoxResult.Yes)
        {
            s_bl!.Config.Reset();
            s_bl.Milestone.Reset();
            dataInitialized = false;
            MessageBox.Show("Reset complete!", "ResetSuccessful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

   /// <summary>
   /// Event handler for the Travel Forwards Day button click event.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    private void Travel_Forwards_Day_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelForwardsDay();
        CurrentTime = s_bl.Clock;
    }

    /// <summary>
    /// Event handler for the Travel Forwards Hour button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Travel_Forwards_Hour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelForwardsHour();
        CurrentTime = s_bl.Clock;
    }


    /// <summary>
    /// Event handler for the Travel Backwards Day button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Travel_Backwards_Day_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelBackwardDay();
        CurrentTime = s_bl.Clock;
    }

    /// <summary>
    /// Event handler for the Travel Backwards Hour button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Travel_Backwards_Hour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelBackwardHour();
        CurrentTime = s_bl.Clock;
    }

    /// <summary>
    /// Event handler for the Reset Clock button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Reset_Clock_Click(object sender, RoutedEventArgs e)
    {
        s_bl.resetClock();
        CurrentTime = s_bl.Clock;
    }
}
