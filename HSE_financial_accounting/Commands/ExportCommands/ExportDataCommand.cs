using HSE_financial_accounting.DataExport;

namespace HSE_financial_accounting.Commands.ExportCommands
{
    public class ExportDataCommand : ICommand
    {
        private readonly DataExporter _exporter;
        private readonly IExportVisitor _visitor;
        private readonly string _filePath;
        
        public ExportDataCommand(
            DataExporter exporter,
            IExportVisitor visitor,
            string filePath)
        {
            _exporter = exporter;
            _visitor = visitor;
            _filePath = filePath;
        }
        
        public void Execute()
        {
            _exporter.ExportData(_visitor, _filePath);
        }
    }
}