using System.Collections.ObjectModel;


namespace PDFMerger.Models
{

    public class PDFMergerModel
    {

        public ObservableCollection<PDFFile> PdfFiles { get; } = new ObservableCollection<PDFFile>();
        public string DestinationPath { get; set; }

    }

}