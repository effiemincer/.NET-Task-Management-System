   
using DalApi;
using DO;
using System.Data.Common;
using System.Xml.Linq;

namespace Dal;


internal class ConfigImplementation : IConfig
{
    static string s_data_config_xml = "data-config";

    /// <summary>
    /// set new KickStart Date of project
    /// </summary>
    /// <param name="newStartDate"></param>
    public void SetProjectStartDate(DateTime newStartDate)
    {
        XElement configXML = XMLTools.LoadListFromXMLElement(s_data_config_xml);

        XElement? projectStartDateElement = configXML.Element("ProjectStartDate");

        if (projectStartDateElement is null)
        {
            // "ProjectStartDate" element doesn't exist, so create and add it to the XML
            XElement newProjectStartDateElement = new XElement("ProjectStartDate", newStartDate);
            configXML.Add(newProjectStartDateElement);
        }
        else
        {
            // "ProjectStartDate" element already exists, update its value
            projectStartDateElement.SetValue(newStartDate);
        }

        XMLTools.SaveListToXMLElement(configXML, s_data_config_xml);
    }

    /// <summary>
    /// Set new end date for project
    /// </summary>
    /// <param name="newEndDate"></param>
    public void SetProjectEndDate(DateTime newEndDate)
    {
        XElement configXML = XMLTools.LoadListFromXMLElement(s_data_config_xml);

        XElement? projectEndDateElement = configXML.Element("ProjectEndDate");

        if (projectEndDateElement is null)
        {
            // "ProjectStartDate" element doesn't exist, so create and add it to the XML
            XElement newProjectEndDateElement = new XElement("ProjectEndDate", newEndDate);
            configXML.Add(newProjectEndDateElement);
        }
        else
        {
            // "ProjectStartDate" element already exists, update its value
            projectEndDateElement.SetValue(newEndDate);
        }

        XMLTools.SaveListToXMLElement(configXML, s_data_config_xml);
    }


    public DateTime? GetProjectStartDate()
    {
        XElement configXML = XMLTools.LoadListFromXMLElement(s_data_config_xml);
        // Extract the ProjectStartDate element value

        XElement? projectStartDateElement = configXML.Element("ProjectStartDate") != null ? configXML.Element("ProjectStartDate") : null;


        if (projectStartDateElement is not null && DateTime.TryParse(projectStartDateElement.Value, out DateTime projectStartDate))
        {
            // Successfully parsed, return the DateTime value
            return projectStartDate;
        }
        else
        {
            // Handle the case where the ProjectStartDate element is missing or not in the correct format
            throw new InvalidOperationException("Unable to retrieve or parse ProjectStartDate from the XML.");
        }
    }

    public DateTime? GetProjectEndDate()
    {
        XElement configXML = XMLTools.LoadListFromXMLElement(s_data_config_xml);
        // Extract the ProjectStartDate element value

        XElement? projectStartEndElement = configXML.Element("ProjectEndDate") != null ? configXML.Element("ProjectEndDate") : null;


        if (projectStartEndElement is not null && DateTime.TryParse(projectStartEndElement.Value, out DateTime projectEndDate))
        {
            // Successfully parsed, return the DateTime value
            return projectEndDate;
        }
        else
        {
            // Handle the case where the ProjectStartDate element is missing or not in the correct format
            throw new InvalidOperationException("Unable to retrieve or parse ProjectEndDate from the XML.");
        }
    }

    public void Reset()
    {
        //-----------Dependencies----------------
        List<Dependency> Dependencies = XMLTools.LoadListFromXMLSerializer<Dependency>("dependencies");
        Dependencies.Clear();
        XMLTools.SaveListToXMLSerializer<Dependency>(Dependencies, "dependencies");

        //----------------Engineers-------------------
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");
        Engineers.Clear();
        XMLTools.SaveListToXMLSerializer<Engineer>(Engineers, "engineers");

        //----------------Tasks----------------
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");
        Tasks.Clear();
        XMLTools.SaveListToXMLSerializer<DO.Task>(Tasks, "tasks");

        //-------------Config------------------
        XElement configXML = XMLTools.LoadListFromXMLElement(s_data_config_xml);

        if (configXML.Element("ProjectStartDate") is not null)
            configXML.Element("ProjectStartDate")!.Remove();

        if (configXML.Element("ProjectEndDate") is not null)
            configXML.Element("ProjectEndDate")!.Remove();
        XMLTools.SaveListToXMLElement(configXML, s_data_config_xml);
    }
}
