using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace ItemsControlDemo
{
    /// <summary>
    /// TreeViewWin.xaml 的交互逻辑
    /// </summary>
    public partial class TreeViewWin : Window
    {
        public TreeViewWin()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath;
                txtFilePath.Text = folderPath;
                LoadTreeView(folderPath);
            }
        }
        private void LoadTreeView(string rootPath)
        {
            treeView.Items.Clear();
            // 设置根节点
            TreeViewItem rootNode = new TreeViewItem();
            rootNode.Header = "根目录";

            // 加载子文件夹和文件
            LoadSubDirectory(rootNode, rootPath);

            // 将根节点添加到TreeView中
            treeView.Items.Add(rootNode);
        }

        private void LoadSubDirectory(TreeViewItem node, string path)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);

                // 加载子文件夹
                foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
                {
                    TreeViewItem subNode = new TreeViewItem();
                    subNode.Header = subDirInfo.Name;

                    LoadSubDirectory(subNode, subDirInfo.FullName);

                    node.Items.Add(subNode);
                }

                // 加载文件
                foreach (FileInfo fileInfo in dirInfo.GetFiles())
                {
                    TreeViewItem subNode = new TreeViewItem();
                    subNode.Header = fileInfo.Name;

                    node.Items.Add(subNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
