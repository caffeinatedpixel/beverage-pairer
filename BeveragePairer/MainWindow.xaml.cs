using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
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
using System.Collections;

namespace BeveragePairer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            string request = HttpUtility.UrlEncode(searchItem.Text);

            DishMetadata dishes = DishData.GetDishList(request);
            //Console.WriteLine(dishes);
            var poo = SelectDish(dishes);
            Console.WriteLine(poo.title);
            var beverages = FindBeverage.GetBeverage();
            var beverageGroup = beverages.GroupBy(x => x);
            var maxCount = beverageGroup.Max(g => g.Count());
            var mostUsed = beverageGroup.Where(x => x.Count() == maxCount).Select(x => x.Key).First();
            Console.WriteLine(mostUsed);

        }

       

        private Dish SelectDish(DishMetadata dishes)
        {
            DishViewer dw = new DishViewer(dishes);
            var result = dw.ShowDialog();
            if(result == true)
            {
                return DishData.SelectedDish;

            }
            return null;
        }
    }
}
