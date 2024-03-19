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
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Admin_Button_Click(object sender, RoutedEventArgs e)
    {
        new AdminScreenWindow().Show();
    }

    private void Engineer_Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(engineerID.Text))
            {
                MessageBox.Show("Please enter an ID");
                return;
            }
            if (!int.TryParse(engineerID.Text, out _))
            {
                MessageBox.Show("ID must be a number");
                return;
            }
            BO.Engineer eng = s_bl?.Engineer.Read(int.Parse(engineerID.Text));
            if (eng == null)
            {
                MessageBox.Show("Engineer not found");
                return;
            }
            else
            {
                MessageBox.Show("Welcome " + eng.Name);
                new EngineerScreenWindow(eng.Id).Show();
            }
        }
        catch (BO.BlDoesNotExistException)
        {
            MessageBox.Show("Engineer not found");
            return;
        }
    
    }

    private void Initialize_Data_Button_Click(object sender, RoutedEventArgs e)
    {
        DalTest.DalTest.Initialization.Do();
        MessageBox.Show("Data initialized");
        // Cast the sender to Button to access its properties
        Button button = sender as Button;

        // Set the visibility of the button to Collapsed
        button.Visibility = Visibility.Collapsed;
    }
}