namespace HSE_financial_accounting.DataImport
{
    using Facades;
    using DataTransferObjects;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    public class YamlDataImporter : DataImporter
    {
        public YamlDataImporter(
            IBankAccountFacade accountFacade,
            ICategoryFacade categoryFacade,
            IOperationFacade operationFacade)
            : base(accountFacade, categoryFacade, operationFacade)
        {
        }
        
        protected override FinancialData ParseFile(string filePath)
        {
            string yamlString = File.ReadAllText(filePath);
            
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
                
            return deserializer.Deserialize<FinancialData>(yamlString) ?? new FinancialData();
        }
    }
}