using System;
using System.Collections.Generic;
using System.IO;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace ST10121162_PROG_Task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //initialise the correct combination array:
            correctCombinations.Add(000, "General Works");
            correctCombinations.Add(100, "Philosophy");
            correctCombinations.Add(200, "Religion");
            correctCombinations.Add(300, "Social Sciences");
            correctCombinations.Add(400, "Language");
            correctCombinations.Add(500, "Pure Science");
            correctCombinations.Add(600, "Technology (Applied Science)");
            correctCombinations.Add(700, "The Arts");
            correctCombinations.Add(800, "Literature");
            correctCombinations.Add(900, "History and Geography");

            PrgssBr.Value = 0;
            prgssbrIdenArea.Value = 0;
            PrgrssbrMainMenu.Value = 0;

            initTree();
            
        }
        //These are the two lists needed for storing the correctly sorted list and the user sorted list.
        List<Book> bkList = new List<Book>();
        List<Book> bkUserList = new List<Book>();

        //Creating the tree data strucure
        Tree booktree = new Tree();

        //Initilising the target answer values
        Node targetAns = null;
        Node targetCat = null;
        Node targetSemiCat = null;

        //initialising the temp node value
        Node tempNode;

        //initialising the int value for tracking user progress in the call number drilling
        int userPrgss = 0;

        public void initTree()
        {
            using (StreamReader reader = new StreamReader("CallNumbers.txt"))
            {
                //creating root of tree
                booktree.OpenBranch(-001, "Books");
                string line;
                bool flag = false;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] ar = line.Split(new char[] { '#' });
                    int number = Int32.Parse(ar[0]);
                    string category = ar[1];
                    if (number % 100 == 0)
                    {
                        if (flag)
                        {
                            booktree.CloseBranch();
                            booktree.CloseBranch();
                        }
                        booktree.OpenBranch(number, category);
                    }
                    else if (number % 10 == 0)
                    {
                        booktree.OpenBranch(number, category);
                    }
                    else 
                    {
                        booktree.AddChild(number, category);
                    }

                }
                reader.Close();
            }
            //foreach (Node n in booktree.Nodes)
            //{
            //    lstboxFindCallNumbers.Items.Add("Parent: " + n.Key + " Value: " + n.strValue);

            //    foreach (Node child in n.Children)
            //    {
            //        lstboxFindCallNumbers.Items.Add("Key: " + child.Key + " Value: " + child.strValue);
            //    }

            //}

        }

        public void setfindCallnumAns() 
        {
            Random rnd = new Random();
            int r = rnd.Next(999);
            bool flag = true;

            //setting target answer
            while (flag) 
            {
                targetAns = booktree.FindNode(booktree.Root, r);
                
                if (targetAns != null && targetAns.Key % 100 !=0 && targetAns.Key % 10 !=0) 
                {
                    flag = false;
                }
                else 
                {
                    r = rnd.Next(999);
                }
            }

            //finding category of the answer
            int firstDigit = (int)(targetAns.Key.ToString()[0]) - 48;
            targetCat = booktree.FindNode(booktree.Root, firstDigit*100);



            //finding Sub category of the answer
            int twoDigits = Convert.ToInt32(targetAns.Key.ToString().Substring(0, 2));
            targetSemiCat = booktree.FindNode(booktree.Root, twoDigits * 10);

            lblTargetCallNum.Content = "Target Genre: " + targetAns.strValue;
        }

        //bool value to track type of question
        bool typeOfQuestion = true;

        //The two dictionaries that will be used
        Dictionary<int, string> correctCombinations = new Dictionary<int, string>();
        Dictionary<int, string> userCombinations = new Dictionary<int, string>();

        int wincount;


        //this initialises the notifier
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });


        //Class to create object of type book and also
        //a custom method to compare two book objects
        class Book : IComparable<Book>
        {
            public int Number { get; set; }
            public string Author { get; set; }

            /// <summary>
            /// initialises the book info values.
            /// </summary>
            /// <param name="theNumber"></param>
            /// <param name="theAuthor"></param>
            public Book(int theNumber, string theAuthor)
            {
                this.Number = theNumber;
                this.Author = theAuthor;
            }

            /// <summary>
            /// Compares a books number and author and then
            /// returns the results
            /// </summary>
            /// <param name="bk"></param>
            /// <returns></returns>
            public int CompareTo(Book bk)
            {
                int results = Number.CompareTo(bk.Number);

                if (results == 0)
                {
                    results = Author.CompareTo(bk.Author);
                }

                return results;
            }
        }

        //This button initialises the two lists and sorts the sorted list into ascending order
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            bkList.Clear();
            bkUserList.Clear();
            lstSorted.Items.Clear();
            lstUnsorted.Items.Clear();
            

            string[] arNames = new string[] { "Jam","Smi", "Bon",
                                            "Ler", "Kab", "Car",
                                            "Tha", "Kel", "Jon",
                                            "Jes", "Kar", "Mic" };

            Random rnd = new Random();

            for (int i = 1; i <= 10; i++)
            {
                bkList.Add(new Book(rnd.Next(1, 1000), arNames[rnd.Next(0, arNames.Length - 1)]));
            }


            foreach (Book x in bkList)
            {
                lstUnsorted.Items.Add(string.Join(" ", x.Number, x.Author));

            }
            bubbleSort(bkList);
        }

        //this method is used to sort the sorted array
        private void bubbleSort(List<Book> bookList)
        {
            for (int i = 0; i < bookList.Count - 1; i++)
            {   
                for (int k = (i + 1); k < bookList.Count; k++)
                {
                    if (bookList[i].CompareTo(bookList[k]) == 1)
                    {
                        Book temp = bookList[i];
                        bookList[i] = bookList[k];
                        bookList[k] = temp;
                    }
                }
            }
        }

        //this code checks if the users list is identical to the sorted list
        //and if it is it will tell the user they are correct
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < lstSorted.Items.Count; i++)
            {
                String item = lstSorted.Items[i].ToString();
                int spaceIndex = item.IndexOf(" ");
                int grade = int.Parse(item.Substring(0, spaceIndex));
                string name = item.Substring(spaceIndex + 1);

                bkUserList.Add(new Book(grade, name));
            }

            int corrcnt = 0;
            if (bkUserList.Count == bkList.Count)
            {
                for (int i = 0; i < bkList.Count; i++)
                {
                        if (bkList[i].CompareTo(bkUserList[i]) == 0)
                        {
                            corrcnt++;
                        }
                }
            }

            if (corrcnt == bkList.Count())
            {
                notifier.ShowSuccess("Correct!\nIf you would like to play again press Start!");
                UpdateProgressBars();

            }
            else
            {
                notifier.ShowError("Incorrect, please try again");
            }
        }

        //this button goes to the book sorting screen
        private void BtnPlcBk_Click(object sender, RoutedEventArgs e)
        {
            tabctrlLibrary.SelectedIndex = 1;
        }
        //this button goes back to the main menu
        private void btnReturnToMain_Click(object sender, RoutedEventArgs e)
        {
            tabctrlLibrary.SelectedIndex = 0;
        }

        //this button exits the application
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //The next section of code is responsible for the the drag and drop between the lists
        //the code was based off of the code from WPFapp2 provided by our lecturer and was further modified to fix bugs and
        //make it work the way I needed it to
        //I used this site as guidance for this:
        //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/drag-and-drop-overview?view=netframeworkdesktop-4.8
        private void lstUnsorted_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, lstUnsorted.SelectedItem.ToString(), DragDropEffects.Move);
            }
        }

        private void lstSorted_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, lstSorted.SelectedItem.ToString(), DragDropEffects.Move);
            }
        }

        private void lstSorted_Drop(object sender, DragEventArgs e)
        {
            var myObj = e.Data.GetData(DataFormats.Text);
            lstUnsorted.Items.Remove(lstUnsorted.SelectedItem);
            lstSorted.Items.Add(myObj);

            if (lstSorted.Items.Contains(lstSorted.SelectedItem))
            {
                lstSorted.Items.Remove(lstUnsorted.SelectedItem);
            }

            PrgssBr.Value = PrgssBr.Value+10;

            if (PrgssBr.Value == 100)
            {
                btnCheck.IsEnabled = true;
            }
            else
            {
                btnCheck.IsEnabled = false;
            }
        }

        private void lstUnsorted_Drop(object sender, DragEventArgs e)
        {
            var myObj = e.Data.GetData(DataFormats.Text);
            lstSorted.Items.Remove(lstSorted.SelectedItem);
            lstUnsorted.Items.Add(myObj);

            if (lstUnsorted.Items.Contains(lstUnsorted.SelectedItem))
            {
                lstUnsorted.Items.Remove(lstSorted.SelectedItem);
            }

            PrgssBr.Value = PrgssBr.Value - 10;

            //lstSorted.Items.Count > 0

            if (PrgssBr.Value == 100)
            {
                btnCheck.IsEnabled = true;
            }
            else
            {
                btnCheck.IsEnabled = false;
            }
        }

        private void btn_Identify_Area_Click(object sender, RoutedEventArgs e)
        {
            tabctrlLibrary.SelectedIndex = 2;
        }

        private void btnBackToMainMenuIA_Click(object sender, RoutedEventArgs e)
        {
            tabctrlLibrary.SelectedIndex = 0;
        }

        private void BtnSubmitIdenArea_Click(object sender, RoutedEventArgs e)
        {
            //lblIdentifyAreadTitle.Content = "Content:" + correctCombinations.ElementAt(0).Value;
            //numbers first
            if (lstvwList1.SelectedItem != null && lstvwList2.SelectedItem != null)
            {
                if (typeOfQuestion)
                {

                    int id = (int)lstvwList1.SelectedItem;
                    string desc = lstvwList2.SelectedItem.ToString();
                    string temp;
                    correctCombinations.TryGetValue(id, out temp);
                    if (desc.Equals(temp))
                    {
                        wincount++;
                        notifier.ShowSuccess("Correct!");
                        lstvwList1.Items.Remove(lstvwList1.SelectedItem);
                        lstvwList2.Items.Remove(lstvwList2.SelectedItem);
                        if (wincount == 4)
                        {
                            lstvwList1.Items.Clear();
                            lstvwList2.Items.Clear();
                            typeOfQuestion = false;
                            UpdateProgressBars();
                            wincount = 0;
                        }
                    }
                    else
                    {
                        notifier.ShowWarning("Incorrect, please Try Again");
                    }
                }
                else
                {
                    int id = (int)lstvwList2.SelectedItem;
                    string desc = lstvwList1.SelectedItem.ToString();
                    string temp;
                    correctCombinations.TryGetValue(id, out temp);
                    if (desc.Equals(temp))
                    {
                        wincount++;
                        notifier.ShowSuccess("Correct!");
                        lstvwList1.Items.Remove(lstvwList1.SelectedItem);
                        lstvwList2.Items.Remove(lstvwList2.SelectedItem);
                        if (wincount == 4)
                        {
                            lstvwList1.Items.Clear();
                            lstvwList2.Items.Clear();
                            typeOfQuestion = true;
                            UpdateProgressBars();
                            wincount = 0;
                        }
                    }
                    else
                    {
                        notifier.ShowWarning("Incorrect, please Try Again");
                    }
                }
            }
            else
            {
                notifier.ShowWarning("Please select a item from both lists and try again");
            }

        }

        private void btnBeginMatching_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            int temp;
            lstvwList1.Items.Clear();
            lstvwList2.Items.Clear();
            List<int> idList = new List<int>();
            List<string> valList = new List<string>();

            //numbers first
            if (typeOfQuestion)
            {
                //adding the three correct values to the lists
                int x =0;
                while (x < 4)
                {
                    temp = rnd.Next(0, correctCombinations.Count);
                    int id = correctCombinations.ElementAt(temp).Key;
                    string val = correctCombinations.ElementAt(temp).Value;
                    if (!idList.Contains(id)) 
                    {
                        idList.Add(id);
                        valList.Add(val);
                        x++;
                    }
                }
                

                //adding two red herring values
                int y = 0;
                while (y < 3) 
                {
                    temp = rnd.Next(0, correctCombinations.Count);
                    string val = correctCombinations.ElementAt(temp).Value;
                    if (!valList.Contains(val))
                    {
                        valList.Add(val);
                        y++;
                    }
                }
                //randomise lists to ensure that they are not displayed in order
                idList.Shuffle();
                valList.Shuffle();

                foreach (int i in idList) 
                {
                    lstvwList1.Items.Add(i);
                }
                foreach (string i in valList) 
                {
                    lstvwList2.Items.Add(i);
                }
            }
            else //numbers second
            {
                //adding the three correct values to the lists
                int x = 0;
                while (x < 4)
                {
                    temp = rnd.Next(0, correctCombinations.Count);
                    int id = correctCombinations.ElementAt(temp).Key;
                    string val = correctCombinations.ElementAt(temp).Value;
                    if (!idList.Contains(id))
                    {
                        idList.Add(id);
                        valList.Add(val);
                        x++;
                    }
                }

                //adding two red herring values
                int y = 0;
                while (y < 3)
                {
                    temp = rnd.Next(0, correctCombinations.Count);
                    int val = correctCombinations.ElementAt(temp).Key;
                    if (!idList.Contains(val))
                    {
                        idList.Add(val);
                        y++;
                    }
                }
                //randomise lists to ensure that they are not displayed in order
                idList.Shuffle();
                valList.Shuffle();

                foreach (int i in idList)
                {
                    lstvwList2.Items.Add(i);
                }
                foreach (string i in valList)
                {
                    lstvwList1.Items.Add(i);
                }
            }
        }

        /// <summary>
        /// Updates the daily goal progress bars
        /// </summary>
        public void UpdateProgressBars() 
        {
            prgssbrIdenArea.Value = prgssbrIdenArea.Value + 10;
            PrgrssbrMainMenu.Value = PrgrssbrMainMenu.Value + 10;
            prgssbrFindingCallNumbers.Value = prgssbrFindingCallNumbers.Value + 10;
            if (PrgrssbrMainMenu.Value == 100) 
            {
                notifier.ShowSuccess("Daily Goal Complete!\n You have completed your ten questions.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btn_Find_Call_Numbers_Click(object sender, RoutedEventArgs e)
        {
            //finding call numbers
            tabctrlLibrary.SelectedIndex = 3;
        }

        private void btnBackToMainFindCallNum_Click(object sender, RoutedEventArgs e)
        {
            tabctrlLibrary.SelectedIndex = 0;
        }

        private void btnBeginCallNumbers_Click(object sender, RoutedEventArgs e)
        {
            generateCallNumQuestion();

        }

        public void generateCallNumQuestion() 
        {
            setfindCallnumAns();
            lstboxFindCallNumbers.Items.Clear();
            Random rnd = new Random();
            int r = rnd.Next(999);
            tempNode = booktree.FindNode(booktree.Root, r);
            List<Node> CatList = new List<Node>();

            //adding correct answer to list
            CatList.Add(targetCat);
            int y = 0;
            while (y < 3)
            {
                tempNode = booktree.FindNode(booktree.Root, r);
                if (tempNode != null && !CatList.Contains(tempNode) && tempNode.Key % 100 == 0)
                {
                    CatList.Add(tempNode);
                    y++;
                }
                else
                {
                    r = rnd.Next(999);
                }
            }
            CatList.Shuffle();
            foreach (Node n in CatList)
            {
                lstboxFindCallNumbers.Items.Add(n.Key + ": " + n.strValue);
            }

        }

        private void btnSubmitCallNumberCh_Click(object sender, RoutedEventArgs e)
        {
            //getting user answer
            string usrch = lstboxFindCallNumbers.SelectedItem.ToString();
            int usrnum = Convert.ToInt32(usrch.Substring(0, usrch.IndexOf(':')));


            //checking user answer
            if (userPrgss == 0)
            {
                int tempusr = (int)(usrch.ToString()[0]) - 48;
                int tempans = (int)(targetCat.Key.ToString()[0]) - 48;
                if (tempusr == tempans)
                {
                    userPrgss++;
                    notifier.ShowSuccess("Correct!\nPlease select the correct sub-category");
                    Random rnd = new Random();
                    int r = rnd.Next(999);
                    tempNode = booktree.FindNode(booktree.Root, r);
                    List<Node> CatList = new List<Node>();

                    //adding correct category to list
                    CatList.Add(targetSemiCat);
                    int y = 0;
                    while (y < 3)
                    {
                        tempNode = booktree.FindNode(booktree.Root, r);
                        if (tempNode != null && !CatList.Contains(tempNode) && tempNode.Key % 10 == 0 && tempNode.Key %100 !=0)
                        {
                            CatList.Add(tempNode);
                            y++;
                        }
                        else
                        {
                            r = rnd.Next(999);
                        }
                    }
                    CatList.Shuffle();
                    lstboxFindCallNumbers.Items.Clear();
                    foreach (Node n in CatList)
                    {
                        lstboxFindCallNumbers.Items.Add(n.Key + ": " + n.strValue);
                    }
                }
                else
                {
                    userPrgss = 0;
                    generateCallNumQuestion();
                    notifier.ShowError("Incorrect!\nPlease try again!");
                }
            }
            else if (userPrgss == 1) 
            {
                int tempusr = Convert.ToInt32(usrch.ToString().Substring(0, 2));
                int tempans = Convert.ToInt32(targetSemiCat.Key.ToString().Substring(0, 2));
                if (tempusr == tempans)
                {
                    notifier.ShowSuccess("Congratulations!\nYou got the correct sub category!");
                    UpdateProgressBars();
                    userPrgss = 0;
                    generateCallNumQuestion();
                }
                else 
                {
                    userPrgss = 0;
                    generateCallNumQuestion();
                    notifier.ShowError("Incorrect!\nPlease try again!");
                }
            }
        }   
    }
    //code taken from the answer to this question:https://www.techiedelight.com/randomize-a-list-in-csharp/
    /// <summary>
    /// Shuffles the List that the method recieves
    /// </summary>
    public static class Extensions
    {
        private static Random rand = new Random();

        public static void Shuffle<T>(this IList<T> values)
        {
            for (int i = values.Count - 1; i > 0; i--)
            {
                int k = rand.Next(i + 1);
                T value = values[k];
                values[k] = values[i];
                values[i] = value;
            }
        }
    }
    public class Tree
    {
        //A tree will store Nodes 
        public Node Root { get; set; }
        public List<Node> Nodes { get; } = new List<Node>();
        private Stack<Node> temp_st = new Stack<Node>();


        public Tree CloseBranch()
        {
            temp_st.Pop();
            return this;
        }

        public Tree OpenBranch(int key, string value)
        {
            if (temp_st.Count == 0)
            {
                Root = new Node(key, value, null);
                Nodes.Add(Root);
                temp_st.Push(Root);
            }
            else
            {
                Node child = temp_st.Peek().Insert(key, value);
                Nodes.Add(child);
                temp_st.Push(child);
            }
            return this;
        }

        public Tree AddChild(int key, string value)
        {
            Node child = temp_st.Peek().Insert(key, value);
            return this;
        }

        public Node FindNode(Node rt, int x)
        {
            if (rt.Key == x) return rt;//dont need the {} because there is only one statement
            foreach (Node child in rt.Children)
            {
                Node ch = FindNode(child, x);
                if (ch != null) return ch;
            }
            return null;
        }
    }
    public class Node
    {
        // a node to store integer values
        public int Key { get; set; }
        public string strValue { get; set; }
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }

        public Node(int data, string value, Node _parent)
        {
            this.Key = data;
            this.strValue = value;
            this.Parent = _parent;
            this.Children = new List<Node>();
        }

        //add a new value
        public Node Insert(int key, string value)
        {
            //this = the current object that is calling the instert method from!
            Node newNode = new Node(key, value, Parent = this);
            this.Children.Add(newNode);
            return newNode;
        }
    }


}
