using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sportsectionfcsc
{
    public class SaxStrategy : IXmlFilterStrategy
    {
        public void FilterXml(Filters filters, string inputFilePath, string outputFilePath)
        {
            using (XmlReader reader = XmlReader.Create(inputFilePath))
            {
                using (XmlWriter writer = XmlWriter.Create(outputFilePath, new XmlWriterSettings { Indent = true }))
                {
                    Stack<string> elementStack = new Stack<string>();
                    bool writeCurrentElement = true;
                    Stack<bool> writeStack = new Stack<bool>();

                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                elementStack.Push(reader.Name);
                                writeStack.Push(writeCurrentElement);

                                if (reader.Name == "Training")
                                {
                                    string day = reader.GetAttribute("day");
                                    string time = reader.GetAttribute("time");

                                    writeCurrentElement = (filters.Days.Count == 0 || filters.Days.Contains(day)) &&
                                                          (string.IsNullOrEmpty(filters.TimeFrom) || string.Compare(time, filters.TimeFrom) >= 0) &&
                                                          (string.IsNullOrEmpty(filters.TimeTo) || string.Compare(time, filters.TimeTo) <= 0);
                                }
                                else if (reader.Name == "Student" || reader.Name == "Teacher")
                                {
                                    string name = reader.GetAttribute("name");
                                    string gender = reader.GetAttribute("gender");
                                    int age = int.Parse(reader.GetAttribute("age"));

                                    writeCurrentElement = (string.IsNullOrEmpty(filters.Name) || name.Contains(filters.Name)) &&
                                                          (string.IsNullOrEmpty(filters.Gender) || gender == filters.Gender) &&
                                                          (!filters.AgeFrom.HasValue || age >= filters.AgeFrom.Value) &&
                                                          (!filters.AgeTo.HasValue || age <= filters.AgeTo.Value);
                                }
                                else
                                {
                                    writeCurrentElement = writeCurrentElement && true;
                                }

                                if (writeCurrentElement)
                                {
                                    writer.WriteStartElement(reader.Name);
                                    if (reader.HasAttributes)
                                    {
                                        while (reader.MoveToNextAttribute())
                                        {
                                            writer.WriteAttributeString(reader.Name, reader.Value);
                                        }
                                        reader.MoveToElement();
                                    }

                                    if (reader.IsEmptyElement)
                                    {
                                        writer.WriteEndElement();
                                        elementStack.Pop();
                                        writeCurrentElement = writeStack.Pop();
                                    }
                                }
                                else
                                {
                                    if (reader.IsEmptyElement)
                                    {
                                        elementStack.Pop();
                                        writeCurrentElement = writeStack.Pop();
                                    }
                                    else
                                    {
                                        SkipElement(reader);
                                    }
                                }
                                break;
                            case XmlNodeType.EndElement:
                                if (writeCurrentElement)
                                {
                                    writer.WriteEndElement();
                                }
                                elementStack.Pop();
                                writeCurrentElement = writeStack.Pop();
                                break;
                            case XmlNodeType.Text:
                                if (writeCurrentElement)
                                {
                                    writer.WriteString(reader.Value);
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void SkipElement(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            int depth = reader.Depth;
            while (reader.Read() && reader.Depth > depth) { }
        }
    }
}
