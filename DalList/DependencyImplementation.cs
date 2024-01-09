
namespace Dal;
using  DO;
using DalApi;
using System.Threading.Tasks;

public class DependencyImplementation : IDependency
{
    public int Create(Dependency dependency)
    {
        int Id = DataSource.Config.NextDependencyId;
        if (DataSource.Dependencies.Any(dependencyItem => dependencyItem.Id == Id))
        {
            throw new Exception("object with that id already exists!");
        }
        Dependency dependencyCopy = new Dependency(
            Id,
            dependency?.DependentTaskId ?? 0,
            dependency?.RequisiteID ?? 0,
            dependency?.CustomerEmail ?? "",
            dependency?.ShippingAddress ?? "",
            dependency?.OrderCreationDate ?? DateTime.Now,
            dependency?.ShippingDate,
            dependency?.DeliveryDate
            );
        DataSource.Dependencies.Add(dependencyCopy);
        return Id;
    }

    public Dependency? Read(int id)
    {
        Dependency? foundDependency = DataSource.Dependencies.FirstOrDefault(dependency => dependency.Id == id && dependency.Inactive == false);

        // If an object with the specified Id exists, return a reference to the object; otherwise, return null
        return foundDependency;
    }

    public List<Dependency> ReadAll()
    {
        return new List<Dependency>(DataSource.Dependencies);
    }

    public void Update(Dependency dependency)
    {
        int index = DataSource.Dependencies.FindIndex(d => d.Id == dependency.Id && d.Inactive == false);
        if (index == -1)
        {
            throw new Exception($"object of type Dependency with identifier {dependency.Id} does not exist");
        }

        //print out object
        Console.WriteLine(dependency);
        Console.WriteLine("\nEnter updated information below:\n");

        //Collects Updated information from User - if input is blank then do not change
        Console.WriteLine("Enter dependent task ID of the Dependency: ");
        string? input = Console.ReadLine();
        int? depTaskID = (input == "" || input is null) ? dependency.DependentTaskId : Convert.ToInt32(input);

        Console.WriteLine("Enter requisite task ID of the Dependency: ");
        input = Console.ReadLine();
        int? reqID = (input == "" || input is null) ? dependency.RequisiteID : Convert.ToInt32(input);

        Console.WriteLine("Enter customer email of the Dependency: ");
        input = Console.ReadLine();
        string? customerEmail = (input == "") ? dependency.CustomerEmail : input;

        Console.WriteLine("Enter shipping address of the Dependency: ");
        input = Console.ReadLine();
        string? shipAddress = (input == "") ? dependency.ShippingAddress : input;

        Console.WriteLine("Enter order creation date of the Dependency: ");
        input = Console.ReadLine();
        DateTime dateCreated = (input == "") ? dependency.OrderCreationDate : DateTime.Parse(input!);

        Console.WriteLine("Enter shipping date of the Dependency: ");
        input = Console.ReadLine();
        DateTime? shipDate = (input == "" ? dependency.ShippingDate : DateTime.Parse(input!));

        Console.WriteLine("Enter delivery date date of the Dependency: ");
        input = Console.ReadLine();
        DateTime? deliveryDate = (input == "" ? dependency.DeliveryDate : DateTime.Parse(input!));

        Console.WriteLine("Enter whether dependency is inactive (Y/N): ");
        input = Console.ReadLine();
        bool inactive = ((input! == "") ? dependency.Inactive : (input! == "Y"));

        // Remove the old dependency
        DataSource.Dependencies.RemoveAt(index);


        Dependency updatedDependency = new Dependency(dependency.Id, depTaskID, reqID, customerEmail, shipAddress, dateCreated, shipDate, deliveryDate, inactive);

        // Add the updated dependency
        DataSource.Dependencies.Insert(index, updatedDependency);
    }

    public void Delete(int id)
    {
        int index = DataSource.Dependencies.FindIndex(d => d.Id == id);
        if (index == -1)
        {
            throw new Exception($"object of type Dependency with identifier {id} does not exist");
        }

        Dependency inactiveDependency= DataSource.Dependencies[index] with { Inactive = true };

        DataSource.Dependencies.RemoveAt(index);

        // Add the Inactive dependency
        DataSource.Dependencies.Add(inactiveDependency);
    }   
    
}
