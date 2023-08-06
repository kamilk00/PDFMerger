using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using PDFMerger.Models;

namespace PDFMerger.ViewModels
{

    public class PDFMergerViewModel : INotifyPropertyChanged
    {

        private PDFMergerModel _model = new PDFMergerModel();


        public PDFMergerViewModel()
        {

            AddFileCommand = new RelayCommand(AddFile);
            RemoveFileCommand = new RelayCommand(RemoveFile);
            ChangePathCommand = new RelayCommand(ChangePath);
            MoveUpCommand = new RelayCommand(MoveUp);
            MoveDownCommand = new RelayCommand(MoveDown);
            MergeFilesCommand = new RelayCommand(MergeFiles);

        }

        public ICommand AddFileCommand { get; }
        public ICommand RemoveFileCommand { get; }
        public ICommand ChangePathCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand MergeFilesCommand { get; }

        public ObservableCollection<PDFFile> PdfFiles => _model.PdfFiles;

        public string DestinationPath
        {
            get => _model.DestinationPath;
            set
            {
                _model.DestinationPath = value;
                OnPropertyChanged(nameof(DestinationPath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private PDFFile _selectedPdfFile;


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //add files to the list
        private void AddFile()
        {

            var openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
                foreach (var fileName in openFileDialog.FileNames)
                    PdfFiles.Add(new PDFFile { FilePath = fileName });

        }


        //remove selected files from the list
        private void RemoveFile()
        {

            if (SelectedPdfFile == null)
                return;
            var selectedFiles = PdfFiles.Where(file => file.FilePath == SelectedPdfFile.FilePath).ToList();
            foreach (var file in selectedFiles)
                PdfFiles.Remove(file);
            
        }


        //change the destination path
        private void ChangePath()
        {

            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {

                DestinationPath = dialog.FileName;
                OnPropertyChanged(nameof(DestinationPath));

            }

        }


        //change the order of files -> selected file is moved up (if it's possible)
        private void MoveUp()
        {

            if (SelectedPdfFile == null)
                return;
            int selectedIndex = PdfFiles.IndexOf(PdfFiles.FirstOrDefault(file => file.FilePath == SelectedPdfFile.FilePath));
            if (selectedIndex > 0)
                PdfFiles.Move(selectedIndex, selectedIndex - 1);

        }


        //change the order of files -> selected file is moved down (if it's possible)
        private void MoveDown()
        {

            if (SelectedPdfFile == null)
                return;
            int selectedIndex = PdfFiles.IndexOf(PdfFiles.FirstOrDefault(file => file.FilePath == SelectedPdfFile.FilePath));
            if (selectedIndex >= 0 && selectedIndex < PdfFiles.Count - 1)
                PdfFiles.Move(selectedIndex, selectedIndex + 1);
            
        }


        //merge files if the destination path is given and pdf files are added to the collection
        private void MergeFiles()
        {

            if (string.IsNullOrWhiteSpace(DestinationPath) || PdfFiles.Count == 0)
            {
                MessageBox.Show("Please enter a destination path and add PDF files.");
                return;
            }

            try
            {

                var merged = new PdfDocument();

                foreach (var pdfFile in PdfFiles)
                {

                    var inputDocument = PdfReader.Open(pdfFile.FilePath, PdfDocumentOpenMode.Import);
                    foreach (var page in inputDocument.Pages)
                        merged.AddPage(page);
                    
                }

                string filename = Path.Combine(DestinationPath, "merged.pdf");
                merged.Save(filename);

                PdfFiles.Clear();
                MessageBox.Show("Merged file was saved!");

            }

            catch
            {
                MessageBox.Show("Error during merging files!");
            }

        }


        public PDFFile SelectedPdfFile
        {

            get { return _selectedPdfFile; }
            set
            {
                _selectedPdfFile = value;
                OnPropertyChanged(nameof(SelectedPdfFile));
            }

        }

    }

    public class RelayCommand : ICommand
    {

        private Action _execute;

        public event EventHandler CanExecuteChanged;


        public RelayCommand(Action execute)
        {
            _execute = execute;
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke();
        }

    }

}