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
using System.IO;

namespace BeveragePairer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string appfolder = appdata + @"\BeveragePairer";
        private static string saveFile = appfolder + @"\history.txt";
        private static string errorFile = appfolder + @"\error.txt";
        public MainWindow()
        {
            InitializeComponent();
            var d = new Directories();
            if (d.ExistsIO(appfolder))
            {
                d.CreateIO(appfolder);
                var f = new Files();
                f.CreateIO(saveFile);
                f.CreateIO(errorFile);
            }
            numberOfTimesRefreshed = 0;
            DisplaySaved();
        }
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if(searchItem.Text.All(char.IsDigit))
            {
                MessageBox.Show("You cannot have any integers in your search term");
                return;
            }
            if (searchItem.Text.All(char.IsSymbol) || searchItem.Text.All(char.IsPunctuation))
            {
                MessageBox.Show("You cannot have any punctuation in your search term.");
                return;
            }

            string request = HttpUtility.UrlEncode(searchItem.Text);

            DishMetadata dishes = DishData.GetDishList(request);
            SelectDish(dishes);
            var beverages = FindBeverage.GetBeverage();
            if(beverages == null)
            {
                return;
            }
            Beverages = beverages;
            var mostUsed = RankBeverage();
            SavePairing();
            SetBeverage(mostUsed);
            numberOfTimesRefreshed = 0;
        }

       private void SetBeverage(string mostUsed)
       {
            string imageFileName = "";
            bevLabel.Content = mostUsed;
            switch (mostUsed)
            {
                case "Red Ale":
                    imageFileName = "red ale.jpg";
                    break;
                case "Merlot":
                    imageFileName = "merlot.jpg";
                    break;
                case "Champagne":
                    imageFileName = "champagne.jpg";
                    break;
                case "Cabernet Sauvignon":
                    imageFileName = "cabernet.jpg";
                    break;
                case "Chardonnay":
                    imageFileName = "chardonnay.jpg";
                    break;
                case "Hefeweizen":
                    imageFileName = "hefeweisen.jpg";
                    break;
                case "IPA":
                    imageFileName = "ipa.png";
                    break;
                case "Lager":
                    imageFileName = "lager.jpg";
                    break;
                case "Pinot Grigio":
                    imageFileName = "pinot grigio.jpg";
                    break;
                case "Pinot Noir":
                    imageFileName = "pinot noir.jpg";
                    break;
                case "Stout":
                    imageFileName = "stout.jpg";
                    break;
                default:
                    imageFileName = "error.png";
                    break;
            }
            BitmapImage beverage = new BitmapImage();
            beverage.BeginInit();
            beverage.UriSource = new Uri("pack://application:,,,/BeveragePairer;component/Images/" + imageFileName);
            beverage.EndInit();
            bevImage.Source = beverage;
            numberOfTimesRefreshed = 1;
        }

        private List<string> Beverages { get; set; }
        private int numberOfTimesRefreshed { get; set; }

        private void SelectDish(DishMetadata dishes)
        {
            DishViewer dw = new DishViewer(dishes);
            var result = dw.ShowDialog();
            if(result != true)
            {
                MessageBox.Show("No dish was selected.");
            }
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            numberOfTimesRefreshed += 1;
            string used = RankBeverage(numberOfTimesRefreshed);
            SetBeverage(used);
        }

        private string RankBeverage()
        {
            var beverageGroup = Beverages.GroupBy(x => x);
            var maxCount = beverageGroup.Max(g => g.Count());
            var mostUsed = beverageGroup.Where(x => x.Count() == maxCount).Select(x => x.Key).First();
            return mostUsed;
        }

        private string RankBeverage(int times)
        {
            var beverageGroup = Beverages.GroupBy(x => x);
            var maxCount = beverageGroup.Max(g => g.Count());
            var mostUsed = beverageGroup.Where(x => x.Count() == maxCount).Select(x => x.Key).ToArray();
            if (mostUsed.Length <= times)
            {
                //if there are no other options, just use merlot because it is pretty universal
                return "Merlot";
            }
            else
            {
                return mostUsed[times];
            }
        }

        private void SavePairing()
        {
            string text = DishData.SelectedDish.title + " -> " + RankBeverage() + Environment.NewLine;
            File.AppendAllText(saveFile, text);
            DisplaySaved();
        }

        private void DisplaySaved()
        {
            var lines = File.ReadLines(saveFile);
            foreach (var line in lines)
            {
                historyLb.Items.Add(line);
            }
        }
    }
}
