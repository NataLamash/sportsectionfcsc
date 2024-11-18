using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sportsectionfcsc
{
    public class DomStrategy : IXmlFilterStrategy
    {
        public void FilterXml(Filters filters, string inputFilePath, string outputFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(inputFilePath);

            XmlNode? root = doc.DocumentElement;

            // Прохід по кожному розділу
            foreach (XmlNode sectionNode in root.SelectNodes("Section"))
            {
                // Фільтрація розкладу
                XmlNode scheduleNode = sectionNode.SelectSingleNode("Schedule");
                List<XmlNode> trainingsToRemove = new List<XmlNode>();
                foreach (XmlNode trainingNode in scheduleNode.SelectNodes("Training"))
                {
                    string day = trainingNode.Attributes["day"].Value;
                    string time = trainingNode.Attributes["time"].Value;

                    bool remove = false;

                    if (filters.Days.Count > 0 && !filters.Days.Contains(day))
                    {
                        remove = true;
                    }
                    if (!string.IsNullOrEmpty(filters.TimeFrom) && string.Compare(time, filters.TimeFrom) < 0)
                    {
                        remove = true;
                    }
                    if (!string.IsNullOrEmpty(filters.TimeTo) && string.Compare(time, filters.TimeTo) > 0)
                    {
                        remove = true;
                    }

                    if (remove)
                    {
                        trainingsToRemove.Add(trainingNode);
                    }
                }
                foreach (XmlNode trainingNode in trainingsToRemove)
                {
                    scheduleNode.RemoveChild(trainingNode);
                }

                // Фільтрація учасників
                XmlNode membersNode = sectionNode.SelectSingleNode("Members");
                List<XmlNode> membersToRemove = new List<XmlNode>();
                foreach (XmlNode memberNode in membersNode.SelectNodes("*"))
                {
                    string name = memberNode.Attributes["name"].Value;
                    string gender = memberNode.Attributes["gender"].Value;
                    int age = int.Parse(memberNode.Attributes["age"].Value);

                    bool remove = false;

                    if (!string.IsNullOrEmpty(filters.Name) && !name.Contains(filters.Name))
                    {
                        remove = true;
                    }
                    if (!string.IsNullOrEmpty(filters.Gender) && gender != filters.Gender)
                    {
                        remove = true;
                    }
                    if (filters.AgeFrom.HasValue && age < filters.AgeFrom.Value)
                    {
                        remove = true;
                    }
                    if (filters.AgeTo.HasValue && age > filters.AgeTo.Value)
                    {
                        remove = true;
                    }

                    if (remove)
                    {
                        membersToRemove.Add(memberNode);
                    }
                }
                foreach (XmlNode memberNode in membersToRemove)
                {
                    membersNode.RemoveChild(memberNode);
                }
            }

            // Збереження відфільтрованого XML
            doc.Save(outputFilePath);
        }
    }
}
