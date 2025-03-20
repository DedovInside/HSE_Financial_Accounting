using HSE_financial_accounting.Facades;
using HSE_financial_accounting.DataTransferObjects;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace HSE_financial_accounting.DataImport
{
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