using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace sportsectionfcsc
{
    public class LinqToXmlStrategy : IXmlFilterStrategy
    {
        public void FilterXml(Filters filters, string inputFilePath, string outputFilePath)
        {
            // Завантаження XML-файлу
            XDocument doc = XDocument.Load(inputFilePath);

            // Фільтрація даних
            var filteredSections = doc.Root.Elements("Section").Select(section =>
            {
                var filteredSchedule = section.Element("Schedule").Elements("Training")
                    .Where(training =>
                        (filters.Days.Count == 0 || filters.Days.Contains(training.Attribute("day").Value)) &&
                        (string.IsNullOrEmpty(filters.TimeFrom) || string.Compare(training.Attribute("time").Value, filters.TimeFrom) >= 0) &&
                        (string.IsNullOrEmpty(filters.TimeTo) || string.Compare(training.Attribute("time").Value, filters.TimeTo) <= 0));

                var filteredMembers = section.Element("Members").Elements()
                    .Where(member =>
                        (string.IsNullOrEmpty(filters.Name) || member.Attribute("name").Value.Contains(filters.Name)) &&
                        (string.IsNullOrEmpty(filters.Gender) || member.Attribute("gender").Value == filters.Gender) &&
                        (!filters.AgeFrom.HasValue || int.Parse(member.Attribute("age").Value) >= filters.AgeFrom.Value) &&
                        (!filters.AgeTo.HasValue || int.Parse(member.Attribute("age").Value) <= filters.AgeTo.Value));

                // Повернення відфільтрованого розділу
                return new XElement("Section",
                    section.Attributes(),
                    new XElement("Schedule", filteredSchedule),
                    new XElement("Members", filteredMembers)
                );
            });

            // Створення нового документа
            XDocument newDoc = new XDocument(new XElement("SportsFaculty", filteredSections));

            // Збереження відфільтрованого XML
            newDoc.Save(outputFilePath);
        }
    }
}
