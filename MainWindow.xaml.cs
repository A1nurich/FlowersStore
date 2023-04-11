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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlowersStore
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        FlowersStoreBDEntities BD = new FlowersStoreBDEntities();
        int PagesNumber = 0;
        int count = 15;
        public MainWindow()
        {
            InitializeComponent();
            PagesView(0);
        }
        /// <summary>
        /// Метод Window_IsVisibleChanged используется для того, чтобы при открытии страницы, автоматический выводились данные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PagesView(0);
        }
        /// <summary>
        /// Созданный метод PagesView используется в основном для вывода данных в ListView, а также для разметки страницы и такого метода как DiscountComboBox_DropDownClosed
        /// </summary>
        /// <param name="number">Количество записи, с которой начинается отчет</param>
        private void PagesView(int number)
        {
            ListViewData.Items.Clear();
            List<Product> products = BD.Product.ToList();

            var list = (from a in products
                        join b in BD.Manufacture on a.Manufacture equals b.ID
                        select new
                        {
                            Image = a.Image,
                            Name = a.Name,
                            Description = a.Description,
                            Manufacture = b.Manufacture1,
                            CurrentDiscount = a.CurrentDiscount,
                            Price = a.Price,
                            BackgroundColor = a.CurrentDiscount >= 15 ? "#7fff00" : "Transparent",
                            DiscountPrice = a.CurrentDiscount == 0 ? "" : Math.Round((a.Price * (100 - Convert.ToDouble(a.CurrentDiscount)) / 100), 3).ToString(),
                            TextDecoration = a.CurrentDiscount != 0 ? "Strikethrough" : "None"
                        }).ToList();
            //Условие, чтобы индекс не улетал за границу диапазона записей
            if(list.Count() - number <15)
            {
                count = list.Count();
            }
            else
            {
                count += number;
            }

            if(list.Count() >= number)
            {
                try
                {
                    ListViewData.Items.Clear();
                    for (int i = number; i < count; i++)
                    {
                        ListViewData.Items.Add(list[i]);
                    }
                }
                catch
                {
                    count = 15;
                    number = 0;
                    ListViewData.Items.Clear();
                    for (int i = number; i < count; i++)
                    {
                        ListViewData.Items.Add(list[i]);
                    }
                }
            }
            //Номера записей
            int Second = Convert.ToInt32(SecondLabel.Content);
            SecondLabel.Content = list.Count().ToString();
            if(Second == count)
            {
                FirstLabel.Content = list.Count().ToString();
            }
            else
            {
                FirstLabel.Content = count.ToString();
            }
            //Филтрация по диапазонам
            if(DiscountComboBox.Text != null)
            {
                switch(DiscountComboBox.Text)
                {
                    case "Все диапазоны":
                        ListViewData.Items.Clear();
                        if (list.Count() - number < 15)
                        {
                            count = list.Count();
                        }
                        else
                        {
                            count += number;
                        }

                        if (list.Count() >= number)
                        {
                            try
                            {
                                ListViewData.Items.Clear();
                                for (int i = number; i < count; i++)
                                {
                                    ListViewData.Items.Add(list[i]);
                                }
                            }
                            catch
                            {
                                count = 15;
                                number = 0;
                                ListViewData.Items.Clear();
                                for (int i = number; i < count; i++)
                                {
                                    ListViewData.Items.Add(list[i]);
                                }
                            }
                        }
                        //Номера записей
                        Second = Convert.ToInt32(SecondLabel.Content);
                        SecondLabel.Content = list.Count().ToString();
                        if (Second == count)
                        {
                            FirstLabel.Content = list.Count().ToString();
                        }
                        else
                        {
                            FirstLabel.Content = count.ToString();
                        }
                        break;
                    case "0-9,99%":
                        list = list.Where(a => a.CurrentDiscount >= 0 && a.CurrentDiscount <= 9.99).ToList();
                        ListViewData.Items.Clear();
                        for (int i = 0; i < list.Count(); i++)
                            ListViewData.Items.Add(list[i]);
                        Second = Convert.ToInt32(SecondLabel.Content);
                        SecondLabel.Content = list.Count().ToString();
                        FirstLabel.Content = list.Count().ToString();
                        break;
                    case "10-14,99%":
                        list = list.Where(a => a.CurrentDiscount >= 10 && a.CurrentDiscount <= 14.99).ToList();
                        ListViewData.Items.Clear();
                        for (int i = 0; i < list.Count(); i++)
                            ListViewData.Items.Add(list[i]);
                        Second = Convert.ToInt32(SecondLabel.Content);
                        SecondLabel.Content = list.Count().ToString();
                        FirstLabel.Content = list.Count().ToString();
                        break;
                    case "15% и более":
                        list = list.Where(a => a.CurrentDiscount >= 15).ToList();
                        ListViewData.Items.Clear();
                        for (int i = 0; i < list.Count(); i++)
                            ListViewData.Items.Add(list[i]);
                        Second = Convert.ToInt32(SecondLabel.Content);
                        SecondLabel.Content = list.Count().ToString();
                        FirstLabel.Content = list.Count().ToString();
                        break;
                }
            }
        }

        /// <summary>
        /// Метод DiscountComboBox_DropDownClosed необходим для фильтрации записей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscountComboBox_DropDownClosed(object sender, EventArgs e)
        {
            PagesView(0);
        }
        /// <summary>
        /// Метод PriceComboBox_DropDownClosed используется для сортировки данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PriceComboBox_DropDownClosed(object sender, EventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewData.Items);

            switch(PriceComboBox.Text)
            {
                case "По возрастанию":
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(Product.Price), System.ComponentModel.ListSortDirection.Ascending));
                    break;
                case "По убыванию":
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(Product.Price), System.ComponentModel.ListSortDirection.Descending));
                    break;
            }
        }
        /// <summary>
        /// Метод BackButton_Click необходим для отката записей назад
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if(PagesNumber != 0)
            {
                PagesNumber -= 15;
                PagesView(PagesNumber);
            }
        }

        /// <summary>
        /// Метод NextButton_Click необходим для отката записей вперед
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int First = Convert.ToInt32(FirstLabel.Content);
            int Second = Convert.ToInt32(SecondLabel.Content);
            if (First == Second)
            {
                
            }
            else
            {
                PagesNumber += 15;
                PagesView(PagesNumber);
                count += 15;
            }
        }
    }
}
