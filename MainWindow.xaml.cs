using Microsoft.Win32;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PDFMerger
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

        //add files to the list
        private void addFile_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
                foreach (string fileName in openFileDialog.FileNames)
                    pdfListBox.Items.Add(fileName);

        }

        //remove selected files from the list
        private void removeFile_Click(object sender, RoutedEventArgs e)
        {

            var selectedItems = new List<object>();

            foreach (var item in pdfListBox.SelectedItems)
                pdfListBox.Items.Remove(item);

            foreach (var item in selectedItems)
                pdfListBox.Items.Remove(item);

        }

        //change the destination path
        private void changePath_Click(object sender, RoutedEventArgs e)
        {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedPath = dialog.FileName;
                destinationPath.Text = selectedPath;
            }

        }

        //change the order of files -> selected file is moved up (if it's possible)
        private void moveUp_Click(object sender, RoutedEventArgs e)
        {

            int selectedIndex = pdfListBox.SelectedIndex;

            if (selectedIndex > 0)
            {

                var selectedItem = pdfListBox.SelectedItem;
                pdfListBox.Items.RemoveAt(selectedIndex);
                pdfListBox.Items.Insert(selectedIndex - 1, selectedItem);
                pdfListBox.SelectedIndex = selectedIndex - 1;

            }

        }

        //change the order of files -> selected file is moved down (if it's possible)
        private void moveDown_Click(object sender, RoutedEventArgs e)
        {
 
            int selectedIndex = pdfListBox.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < pdfListBox.Items.Count - 1)
            {

                var selectedItem = pdfListBox.SelectedItem;
                pdfListBox.Items.RemoveAt(selectedIndex);
                pdfListBox.Items.Insert(selectedIndex + 1, selectedItem);
                pdfListBox.SelectedIndex = selectedIndex + 1;

            }

        }

        //merge files
        private void mergeFiles_Click(object sender, RoutedEventArgs e)
        {

            if (destinationPath.Text != "" && pdfListBox.Items.Count > 0)
            {

                try
                {

                    PdfDocument merged = new PdfDocument();
                    foreach (string file in pdfListBox.Items)
                    {

                        //open PDF and import pages from it.
                        PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                        for (int i = 0; i < inputDocument.PageCount; i++)
                            merged.AddPage(inputDocument.Pages[i]);

                    }

                    // Save the merged PDF
                    string filename = System.IO.Path.Combine(destinationPath.Text, "merged.pdf");
                    merged.Save(filename);
                    pdfListBox.Items.Clear();
                    MessageBox.Show("Merged file was saved!");


                }

                catch 
                {
                    MessageBox.Show("Error during merging files!");
                }

            }

            else if (pdfListBox.Items.Count == 0)
                MessageBox.Show("The list is empty!");

            else
                MessageBox.Show("Enter a destination path!");

        }

    }

}
