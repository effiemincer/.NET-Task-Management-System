using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Engineer;

/// <summary>
/// Interaction logic for EngineerListWindow.xaml
/// </summary>
public partial class EngineerListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.Enums.EngineerExperience EngineerExperience { get; set; } = BO.Enums.EngineerExperience.None;
    public EngineerListWindow()
    {
        EngineerList = s_bl?.Engineer.ReadAll();
        InitializeComponent();
    }
    public IEnumerable<BO.Engineer> EngineerList 
    { 
        get { return (IEnumerable<BO.Engineer>)GetValue(EngineerListProperty); }
        set { SetValue(EngineerListProperty, value); }
    }

    public static readonly DependencyProperty EngineerListProperty =
        DependencyProperty.Register("EngineerList",
            typeof(IEnumerable<BO.Engineer>),
            typeof(EngineerListWindow),
            new PropertyMetadata(null)
        );

    private void cbEngineerExperience_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        EngineerList = (EngineerExperience == BO.Enums.EngineerExperience.None) ? s_bl?.Engineer.ReadAll()! : s_bl?.Engineer.ReadAll(item => item.ExperienceLevel == EngineerExperience)!;
    }

}
