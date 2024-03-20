using DalApi;
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
    private bool dataInitialized = false;

    // Dependency property for binding the current engineer.
    public static readonly DependencyProperty TimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(null));

    // Property to get or set the current engineer.
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(TimeProperty); }
        set { SetValue(TimeProperty, s_bl.Clock); }
    }
    public MainWindow()
    {
        InitializeComponent();
        CurrentTime = s_bl.Clock;
    }

    private void Admin_Button_Click(object sender, RoutedEventArgs e)
    { 
        new AdminScreenWindow().Show();
        Close();
    }

    private void Engineer_Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
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
            BO.Engineer eng = s_bl?.Engineer.Read(int.Parse(engineerID.Text));
            if (eng == null)
            {
                MessageBox.Show("Engineer not found", "EngNotFound", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else
            {
               // MessageBox.Show("Welcome " + eng.Name + "!", "Welcome", MessageBoxButton.OK);
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

    private void Initialize_Data_Button_Click(object sender, RoutedEventArgs e)
    {
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
        // Cast the sender to Button to access its properties
        //Button button = sender as Button;

        //// Set the visibility of the button to Collapsed
        //button.Visibility = Visibility.Collapsed;
    }

    private void Reset_Data_Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult res = MessageBox.Show("Are you sure you want to reset all the data?", "Reset Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        
        if (res == MessageBoxResult.Yes)
        {
            s_bl!.Config.Reset();
            dataInitialized = false;
            MessageBox.Show("Reset complete!", "Reset Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void Travel_Forwards_Day_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelForwardsDay();
        CurrentTime = s_bl.Clock;
    }

    private void Travel_Forwards_Hour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelForwardsHour();
        CurrentTime = s_bl.Clock;
    }

    private void Travel_Backwards_Day_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelBackwardDay();
        CurrentTime = s_bl.Clock;
    }

    private void Travel_Backwards_Hour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.travelBackwardHour();
        CurrentTime = s_bl.Clock;
    }

    private void Reset_Clock_Click(object sender, RoutedEventArgs e)
    {
        s_bl.resetClock();
        CurrentTime = s_bl.Clock;
    }
}