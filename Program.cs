using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CsvHelper;
using Newtonsoft.Json;
using ProgramExporterImporter.Data;
using ProgramExporterImporter.Model;

namespace ProgramExporterImporter
{
    class Program
    {
        static ProgramExporterImporterContext context = new ProgramExporterImporterContext();

        static void Main(string[] args)
        {

            Console.CursorVisible = false;
            bool appliationRunning = true;

            do
            {
                Console.Clear();
                Console.WriteLine("-- Main Menu --");
                Console.WriteLine(" 1. Add product");
                Console.WriteLine(" 2. List products");
                Console.WriteLine(" 3. Export products");
                Console.WriteLine(" 4. Import products");
                Console.WriteLine(" 9. Clear products");
                Console.WriteLine(" 0. Exit");

                ConsoleKeyInfo input = Console.ReadKey(true);

                Console.Clear();

                switch (input.Key)
                {
                    case ConsoleKey.D1:

                        Console.CursorVisible = true;

                        Console.Write("Name: ");

                        string productName = Console.ReadLine();

                        Console.Write("Description: ");

                        string productDescription = Console.ReadLine();

                        Console.CursorVisible = false;

                        var product = new Product(productName, productDescription);

                        SaveProduct(product);


                        break;
                    case ConsoleKey.D2:

                        ListProducts();

                        break;
                    case ConsoleKey.D3:
                        {
                            var selectedDataType = SelectDataType();

                            ExportProducts(selectedDataType);
                        }

                        break;
                    case ConsoleKey.D4:
                        {
                            var selectedDataType = SelectDataType();

                            ImportProducts(selectedDataType);
                        }

                        break;
                    case ConsoleKey.D9:

                        context.Products.RemoveRange(context.Products);
                        context.SaveChanges();

                        break;
                    case ConsoleKey.D0:
                        appliationRunning = false;
                        break;
                }

            } while (appliationRunning);
        }

        private static DataType SelectDataType()
        {
            Console.Clear();
            Console.WriteLine("- Select format -");
            Console.WriteLine(" 1. XML");
            Console.WriteLine(" 2. CSV");
            Console.WriteLine(" 3. JSON");

            ConsoleKey consoleKey;
            while ((consoleKey = Console.ReadKey(true).Key) != ConsoleKey.D1 && consoleKey != ConsoleKey.D2 && consoleKey != ConsoleKey.D3) { }

            {
                switch (consoleKey)
                {
                    case ConsoleKey.D1:
                        return DataType.XML;
                    case ConsoleKey.D2:
                        return DataType.CSV;
                    default:
                        return DataType.JSON;
                }
            }
        }

        private static void ImportProducts(DataType dataType)
        {

            switch (dataType)
            {
                case DataType.XML:

                    using (var fileStream = File.Open("Products.xml", FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(ProductExport));

                        var productExport = (ProductExport)serializer.Deserialize(fileStream);

                        foreach (var product in productExport.ProductList)
                        {
                            product.Id = 0;
                            context.Products.Add(product);
                        }
                        context.SaveChanges();
                    }

                    break;
                case DataType.CSV:

                    var reader = new StreamReader("Products.csv");

                    var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

                    var records = csvReader.GetRecords<Product>().ToList();

                    foreach (var product in records)
                    {
                        product.Id = 0;
                        context.Products.Add(product);
                    }
                    context.SaveChanges();

                    break;
                case DataType.JSON:

                    var importList = JsonConvert.DeserializeObject<ProductExport>(File.ReadAllText("Products.json"));

                    foreach (var product in importList.ProductList)
                    {
                        product.Id = 0;
                        context.Products.Add(product);
                    }
                    context.SaveChanges();

                    break;
            }

        }

        private static void ExportProducts(DataType dataType)
        {
            var productList = context.Products.ToList();

            switch (dataType)
            {
                case DataType.XML:
                    using (var xmlWriter = XmlWriter.Create("Products.xml"))
                    {

                        xmlWriter.WriteStartDocument();


                        xmlWriter.WriteStartElement("ProductExport", "http://domain.com/ns");
                        xmlWriter.WriteStartElement("ProductList");

                        foreach (var product in productList)
                        {
                            xmlWriter.WriteStartElement("Product");

                            xmlWriter.WriteElementString("Id", product.Id.ToString());
                            xmlWriter.WriteElementString("Name", product.Name);
                            xmlWriter.WriteElementString("Description", product.Description);

                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteEndDocument();
                    }
                    break;
                case DataType.CSV:

                    using (var writer = new StreamWriter("Products.csv"))
                    using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csvWriter.WriteRecords(context.Products);
                    }

                    break;
                case DataType.JSON:

                    var productExportJson = new ProductExport
                    {
                        ProductList = productList
                    };

                    string json = JsonConvert.SerializeObject(productExportJson);

                    File.WriteAllText("Products.json", json);

                    break;
            }

        }

        private static void ListProducts()
        {
            var productList = context.Products;

            Console.WriteLine($"{"ID",-4} {"Product Name"}");
            Console.WriteLine("---------------------------------------------------");

            foreach (var product in productList)
            {
                Console.WriteLine($"{product.Id,-4} {product.Name}");
            }

            Console.WriteLine("\n<Any key to return>");

            Console.ReadKey(true);
        }

        private static void SaveProduct(Product product)
        {
            context.Products.Add(product);

            context.SaveChanges();
        }
    }

    [XmlRoot("ProductExport", Namespace = "http://domain.com/ns")]
    public class ProductExport
    {

        [XmlArray]
        [XmlArrayItem(ElementName = "Product")]
        public List<Product> ProductList { get; set; }
    }

    enum DataType
    {
        XML,
        CSV,
        JSON
    }
}
