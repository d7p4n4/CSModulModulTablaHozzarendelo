
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
using static CSLibAc4yObjectObjectService.Ac4yAssociationObjectService;

namespace CSModulModulTablaHozzarendelo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<string> zoneList = new ObservableCollection<string>();
        ObservableCollection<string> secondSource = new ObservableCollection<string>();
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
            ListInstanceByNameResponse listInstanceByNameResponseModul =
                new Ac4yObjectObjectService(conn).ListInstanceByName(
                    new ListInstanceByNameRequest() { TemplateName = "Modul" }
                );

            Ac4yObjectHomeList = listInstanceByNameResponse.Ac4yObjectHomeList;
            foreach (Ac4yObjectHome home in Ac4yObjectHomeList)
            {
                zoneList.Add(home.HumanId);
            }
            lbOne.ItemsSource = zoneList;
            lbTwo.ItemsSource = secondSource;
            /*
            foreach(var name in zoneList)
            {
                lbOne.Items.Add(name);
            }
            */
            foreach (var modul in listInstanceByNameResponseModul.Ac4yObjectHomeList)
            {
                cBox.Items.Add(modul.HumanId);
            }
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

        private void Upload(object sender, RoutedEventArgs e)
        {
            
            string modul = cBox.SelectedItem.ToString();
            List<string> tablak = new List<string>();

            new Ac4yAssociationObjectService(conn).DeleteTargetOnPathByNames(
                new DeleteTargetOnPathByNamesRequest() { templateName = "Modul", name = modul, pathName = "Modul.Tábla"}
                );

            for (int i = 0; i < lbTwo.Items.Count; i++)
            {
                ListBoxItem lb = (ListBoxItem)lbTwo.ItemContainerGenerator.ContainerFromIndex(i);
                tablak.Add(lb.Content.ToString());
            }

            foreach(var tabla in tablak)
            {
                try
                {
                    Ac4yAssociationObjectService.SetByNamesResponse setByNamesResponse =
                        new Ac4yAssociationObjectService(conn).SetByNames(
                            new Ac4yAssociationObjectService.SetByNamesRequest()
                            {
                                originTemplateName = "Modul",
                                originName = modul,
                                targetTemplateName = "Tábla",
                                targetName = tabla,
                                associationPathName = "Modul.Tábla"
                            }
                        );


                    MessageBox.Show("mentve: " + tabla);
                }
                catch(Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string modul = cBox.SelectedItem.ToString();
            List<string> tablak = new List<string>();

            ListTargetOnPathNamesResponse listTargetOnPathNamesResponse =
                new Ac4yAssociationObjectService(conn).ListTargetOnPathNames(
                    new ListTargetOnPathNamesRequest() { templateName = "Modul", name = modul, pathName = "Modul.Tábla"}
                    );

            foreach(Ac4yAssociationTargetHome tabla in listTargetOnPathNamesResponse.Ac4yAssociationTargetHomeList)
            {
                tablak.Add(tabla.humanId);
            }

            lbTwo.Items.Clear();
            foreach (var name in tablak)
            {
                lbTwo.Items.Add(name);
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
