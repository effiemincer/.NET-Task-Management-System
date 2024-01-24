using DalApi;
using DO;
using System.Xml.Linq;

namespace Dal
{
    internal class DependencyImplementation : IDependency
    {
        // XML file name
        readonly string s_dependencies_xml = "dependencies";

        // Helper method to retrieve a Dependency based on ID
        private Dependency? getDependency(int id)
        {
            // Load the XML document
            XElement Dependencies = XMLTools.LoadListFromXMLElement("dependencies");

            try
            {
                // Query to find a specific Dependency element based on ID
                return (from s in Dependencies.Elements()
                        where Int32.Parse(s.Element("Id")!.Value) == id
                        select new Dependency(
                            Int32.Parse(s.Element("Id")!.Value)
                            )).FirstOrDefault();
            }
            catch { return null; } // Handle parsing exceptions
        }

        // Create a new Dependency
        public int Create(Dependency dependency)
        {
            // Load the XML document
            XElement Dependencies = XMLTools.LoadListFromXMLElement("dependencies");

            // Generate a new ID for the Dependency
            int Id = Config.NextDependencyId;

            // Check if a Dependency with the generated ID already exists
            if (getDependency(Id) is not null)
                throw new DalAlreadyExistsException("Object with that id already exists!");

            // Create a copy of the Dependency with the new ID
            Dependency dependencyCopy = new Dependency(
                Id,
                dependency?.DependentTaskId,
                dependency?.RequisiteID
            );

            // Add the new Dependency to the XML document
            Dependencies.Add(dependencyCopy);

            // Save the updated XML document
            XMLTools.SaveListToXMLElement(Dependencies, "dependencies");

            return Id; // Return the generated ID
        }

        // Delete a Dependency based on ID
        public void Delete(int id)
        {
            // Load the XML document
            XElement Dependencies = XMLTools.LoadListFromXMLElement("dependencies");

            // Check if the Dependency with the specified ID exists
            if (getDependency(id) is null)
                throw new DalDoesNotExistException($"Object of type Dependency with identifier {id} does not exist");

            // Find the Dependency element with the specified ID
            var selectedDependency = Dependencies.Elements()
                .Where(s => (int)s.Element("Id")! == id)
                .FirstOrDefault();

            // Set the Inactive attribute to true for soft deletion
            selectedDependency?.Element("Inactive")?.SetValue(true.ToString());

            // Save the updated XML document
            XMLTools.SaveListToXMLElement(Dependencies, "dependencies");
        }

        // Read a Dependency based on ID
        public Dependency? Read(int id)
        {
            // Load the XML document
            XElement Dependencies = XMLTools.LoadListFromXMLElement("dependencies");

            // Check if the Dependency with the specified ID exists
            if (getDependency(id) is null)
                throw new DalDoesNotExistException($"Object of type Dependency with identifier {id} does not exist");

            // Query to find a specific Dependency element based on ID
            return (from s in Dependencies.Elements()
                    where Int32.Parse(s.Element("Id")!.Value) == id
                    select new Dependency(
                        (int)s.Element("Id")!,
                        Int32.Parse(s.Element("DependentTaskId")!.Value),
                        Int32.Parse(s.Element("RequisiteID")!.Value)
                    )).FirstOrDefault();
        }

        // Read a Dependency based on a custom filter
        public Dependency? Read(Func<Dependency, bool> filter)
        {
            // Load the XML document
            XElement dependenciesElement = XMLTools.LoadListFromXMLElement("dependencies");

            // Check if the filter or XML document is null
            if (filter == null || dependenciesElement == null)
            {
                return null;
            }

            // Query to select and create Dependency objects from XML
            List<Dependency> Dependencies = dependenciesElement.Elements("Dependency")
                .Select(depElement => new Dependency
                {
                    Id = Int32.Parse(depElement.Element("Id")!.Value),
                    DependentTaskId = depElement.Element("DependentTaskId") != null ? (int?)depElement.Element("DependentTaskId") : null,
                    RequisiteID = depElement.Element("RequisiteID") != null ? (int?)depElement.Element("RequisiteID") : null,
                    Inactive = depElement.Element("Inactive")!.Value == "true" ? true : false
                })
                .Where(dep => dep.Inactive is not true)
                .ToList();

            // Return the first Dependency that matches the filter
            return Dependencies.FirstOrDefault(filter);
        }

        // Read all Dependencies with an optional filter
        public IEnumerable<Dependency> ReadAll(Func<Dependency, bool>? filter = null)
        {
            // Load the XML document
            XElement dependenciesElement = XMLTools.LoadListFromXMLElement("dependencies");

            // Query to select and create Dependency objects from XML
            List<Dependency> Dependencies = dependenciesElement.Elements("Dependency")
                .Select(depElement => new Dependency
                {
                    Id = Int32.Parse(depElement.Element("Id")!.Value),
                    DependentTaskId = depElement.Element("DependentTaskId") != null ? (int?)depElement.Element("DependentTaskId") : null,
                    RequisiteID = depElement.Element("RequisiteID") != null ? (int?)depElement.Element("RequisiteID") : null,
                    Inactive = depElement.Element("Inactive")!.Value == "true" ? true : false
                })
                .Where(dep => dep.Inactive is false)
                .ToList();

            // Apply the filter if provided
            if (filter != null)
            {
                return from item in Dependencies
                       where filter(item) && !item.Inactive
                       select item;
            }
            // Return all active Dependencies if no filter is provided
            return from item in Dependencies
                   where !item.Inactive
                   select item;
        }

        // Reset the XML document by removing all Dependency elements
        public void Reset()
        {
            // Load the XML document
            XElement Dependencies = XMLTools.LoadListFromXMLElement("dependencies");

            // Check if the XML document is not null
            if (Dependencies != null)
            {
                // Remove all Dependency elements to reset the document
                Dependencies.Elements("Dependency").Remove();

                // Save the updated XML document
                XMLTools.SaveListToXMLElement(Dependencies, "dependencies");
            }
        }

        // Update a Dependency based on ID
        public void Update(Dependency dependency)
        {
            // Load the XML document
            XElement Dependencies = XMLTools.LoadListFromXMLElement("dependencies");

            // Check if the Dependency with the specified ID exists
            if (getDependency(dependency.Id) is null)
                throw new DalDoesNotExistException($"Object of type Dependency with identifier {dependency.Id} does not exist");

            // Find the Dependency element with the specified ID
            XElement dependencyElement = Dependencies.Elements("Dependency")!.FirstOrDefault(dep => (int)dep.Element("Id")! == dependency.Id)!;

            // Update the ID
            dependencyElement.Element("Id")?.SetValue(dependency.Id);

            // Update the DependentTaskId
            if (dependencyElement.Element("DependentTaskId") is not null && dependency.DependentTaskId is not null)
                dependencyElement.Element("DependentTaskId")!.SetValue(dependency.DependentTaskId!);
            else if (dependencyElement.Element("DependentTaskId") is not null && dependency.DependentTaskId is null)
                dependencyElement.Element("DependentTaskId")!.Remove();
            else if (dependencyElement.Element("DependentTaskId") is null && dependency.DependentTaskId is not null)
                dependencyElement.Add(new XElement("DependentTaskId", dependency.DependentTaskId));

            // Update the RequisiteID
            if (dependencyElement.Element("RequisiteID") is not null && dependency.RequisiteID is not null)
                dependencyElement.Element("RequisiteID")!.SetValue(dependency.RequisiteID!);
            else if (dependencyElement.Element("RequisiteID") is not null && dependency.RequisiteID is null)
                dependencyElement.Element("RequisiteID")!.Remove();
            else if (dependencyElement.Element("RequisiteID") is null && dependency.RequisiteID is not null)
                dependencyElement.Add(new XElement("RequisiteID", dependency.RequisiteID));

            // Save the updated XML document
            XMLTools.SaveListToXMLElement(Dependencies, "dependencies");
        }
    }
}
