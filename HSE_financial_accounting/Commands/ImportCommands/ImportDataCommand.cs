using HSE_financial_accounting.DataImport;

namespace HSE_financial_accounting.Commands.ImportCommands
{
    public class ImportDataCommand : ICommand
    {
        private readonly DataImporter _importer;
        private readonly string _filePath;
        
        public ImportDataCommand(DataImporter importer, string filePath)
        {
            _importer = importer;
            _filePath = filePath;
        }
        
        public void Execute()
        {
            _importer.ImportData(_filePath);
        }
    }
}