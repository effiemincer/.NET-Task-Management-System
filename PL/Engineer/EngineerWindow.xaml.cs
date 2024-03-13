using Milestone;
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

namespace Engineer;

/// <summary>
/// Interaction logic for EngineerWindow.xaml
/// </summary>
public partial class EngineerWindow : Window
{

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public static readonly DependencyProperty EngineerProperty =
        DependencyProperty.Register("CurrentEngineer", typeof(BO.Engineer), typeof(EngineerWindow), new PropertyMetadata(null));

    public BO.Engineer CurrentEngineer
    {
        get { return (BO.Engineer)GetValue(EngineerProperty); }
        set { SetValue(EngineerProperty, value); }
    }
    private bool isAdd;

    public EngineerWindow(BO.Engineer CurrentEngineer_, bool isAdd_)
    {
        InitializeComponent();
        CurrentEngineer = CurrentEngineer_;
        isAdd = isAdd_;
    }
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        return;
    }

    private void cbEngineerStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        return;
    }

    private void btnAddUpdate_EngineerClick(object sender, RoutedEventArgs e)
    {
        if (isAdd)
        {
            s_bl?.Engineer.Create(CurrentEngineer);
        }
        else
        {
            s_bl?.Engineer.Update(CurrentEngineer);
        }
        Close();
    }

}
