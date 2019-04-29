using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BeveragePairer
{
    /// <summary>
    /// Interaction logic for DishViewer.xaml
    /// </summary>
    public partial class DishViewer : Window
    {
        public DishViewer(DishMetadata dishes)
        {
            InitializeComponent();

            this.Dishes = dishes.Dishes;
            this.DishList = new ObservableCollection<string>();
            foreach (Dish d in dishes.Dishes)
            {
                DishList.Add(d.title);                    
            }
            this.dishLv.ItemsSource = DishList;
            DataContext = this;
        }
        private List<Dish> Dishes { get; set; }
        public ObservableCollection<string> DishList { get; private set; }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            string selectedDishName = dishLv.SelectedItem.ToString();
            DishData.SelectedDish = Dishes.Find(d => d.title == selectedDishName);
            this.DialogResult = true;
            this.Close();
        }
    }
}
