using System.IO;

namespace FinancialAccounting.Patterns.TemplateMethod
{
    public abstract class DataImporter
    {
        public void ImportData(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var rawData = ReadFile(filePath);
            ParseData(rawData);
        }

        protected virtual string ReadFile(string path) => File.ReadAllText(path);
        protected abstract void ParseData(string rawData);
    }
}
