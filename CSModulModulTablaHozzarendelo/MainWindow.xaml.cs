
using CSLibAc4yObjectDBCap;
using CSLibAc4yObjectObjectService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CSModulModulTablaHozzarendelo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<string> zoneList = new ObservableCollection<string>();
        List<Ac4yObjectHome> Ac4yObjectHomeList = new List<Ac4yObjectHome>();
        SqlConnection conn = new SqlConnection("Data Source=80.211.241.82;Integrated Security=False;uid=root;pwd=Sycompla9999*;Initial Catalog=ModulDb;");
        ListBox dragSource = null;

        public MainWindow()
        {
            InitializeComponent();
            conn.Open();
            ListInstanceByNameResponse listInstanceByNameResponse =
                new Ac4yObjectObjectService(conn).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = "Tábla" }
                );

            Ac4yObjectHomeList = listInstanceByNameResponse.Ac4yObjectHomeList;
            foreach (Ac4yObjectHome home in Ac4yObjectHomeList)
            {
                zoneList.Add(home.HumanId);
            }
            lbOne.ItemsSource = zoneList;
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            dragSource = parent;
            object data = GetDataFromListBox(dragSource, e.GetPosition(parent));

            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            object data = e.Data.GetData(typeof(string));
            ((IList)dragSource.ItemsSource).Remove(data);
            parent.Items.Add(data);
        }

        #region GetDataFromListBox(ListBox,Point)
        private static object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }

                    if (element == source)
                    {
                        return null;
                    }
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }

            return null;
        }
        #endregion
    }
}
