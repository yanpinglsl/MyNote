using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace CommandBindingDemo
{
    /// <summary>
    /// ApplicationCommandsWin.xaml 的交互逻辑
    /// </summary>
    public partial class ApplicationCommandsWin : Window
    {
        public ApplicationCommandsWin()
        {
            InitializeComponent();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "文本文档(.txt)|*.txt",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog().Value)
            {
                textbox.Text = File.ReadAllText(openFileDialog.FileName);
            }

        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = textbox != null && textbox.Text.Length > 0;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "文本文档 (.txt)|*.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, textbox.Text);
            }

        }

        private void CutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = textbox != null && textbox.Text.Length > 0;
        }

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            textbox.Cut();
        }

        private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsText(); 
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            textbox.Paste();
        }
    }
}
