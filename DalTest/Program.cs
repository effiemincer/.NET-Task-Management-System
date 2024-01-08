using Dal;
using DalApi;
using DO;

namespace DalTest;

internal class Program
{
    private static ITask? s_dalTask = new TaskImplementation(); //stage 1
    private static IEngineer? s_dalEngineer = new EngineerImplementation(); //stage 1
    private static IDependency? s_dalDependency = new DependencyImplementation(); //stage 1

    static void Main(string[] args)
    {
        DalTest.Initialization.Do(s_dalTask, s_dalEngineer, s_dalDependency);

        int? userInput = null, ignoreKey;


        while (true)
        {
            //Main Menu
            Console.WriteLine("\nEnter a number:\n" +
            "0. Exit Main Menu\n" +
            "1. Task\n" +
            "2. Engineer\n" +
            "3. Dependency");

            //take in user input with 2 ignore keys for empty values
            userInput = Console.Read();
            ignoreKey = Console.Read();
            ignoreKey = Console.Read();

            //exit main menu
            if (userInput == '0') { return; }

            //Task Menu
            else if (userInput == '1')
            {
                Console.WriteLine("\nEnter a character for which action to do in Task:\n" +
                    "a. Go back\n" +
                    "b. Add an Object to the entity list - Create()\n" +
                    "c. Display and object using an object’s identifier - Read()\n" +
                    "d. Display the object list - ReadAll()\n" +
                    "e. Update an object - Update()\n" +
                    "f. Delete an object from the object list – Delete()");

                //take in user input with 2 ignore keys for empty values
                userInput = Console.Read();
                ignoreKey = Console.Read();
                ignoreKey = Console.Read();

                switch (userInput)
                {
                    case 'a': break; //go back to main menu

 //might need to handle if an empty string is put in to just assing that as null

                    case 'b': //create
                        Console.WriteLine("Enter name of task: ");
                        string name = Console.ReadLine() ?? "default name";

                        Console.WriteLine("Date Created (mm/dd/yyyy): ");
                        DateTime dateCreated = DateTime.Parse(Console.ReadLine() ?? "01/01/2024");

                        Console.WriteLine("Enter description of task: ");
                        string? description = Console.ReadLine();

                        Console.WriteLine("Enter duration of task (hours): ");
                        int? duration = Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine("Enter deadline of task (mm/dd/yyyy): ");
                        DateTime? deadline = DateTime.Parse(Console.ReadLine() ?? "01/01/2024");

                        Console.WriteLine("Enter projected start date of task (mm/dd/yyyy): ");
                        DateTime? projectedStart = DateTime.Parse(Console.ReadLine() ?? "01/01/2024");

                        Console.WriteLine("Enter difficulty of task (0-4): ");
                        Enums.EngineerExperience[] _allDifficulties = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
                        int difficultyIndex = Convert.ToInt32(Console.ReadLine());
                        Enums.EngineerExperience difficulty = _allDifficulties[difficultyIndex];

                        //DO WE NEED TO check if the engineer is in our list of engineers?
                        Console.WriteLine("Enter ID of assigned Enginner of task: ");
                        int? assignedEng = Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine("Enter actual end date of task (mm/dd/yyyy): ");
                        DateTime? actualEnd = DateTime.Parse(Console.ReadLine() ?? "01/01/2024");

                        Console.WriteLine("Enter whether task is a milestone (Y/N): ");
                        string isMilestoneString = Console.ReadLine() ?? "N";
                        bool isMilestone = false;
                        if (isMilestoneString == "Y") isMilestone = true;


                        Console.WriteLine("Enter actaul start date of task (mm/dd/yyyy): ");
                        DateTime? actualStart = DateTime.Parse(Console.ReadLine() ?? "01/01/2024"); ;

                        Console.WriteLine("Enter deliverable of task: ");
                        string? deliverable = Console.ReadLine();

                        Console.WriteLine("Enter notes for the task: ");
                        string? notes = Console.ReadLine();

                        Console.WriteLine("Enter whether task is inactive (Y/N): ");
                        string inactiveString = Console.ReadLine() ?? "N";
                        bool inactive = false;
                        if (inactiveString == "Y") inactive = true;

                        try
                        {
                            s_dalTask!.Create(new DO.Task(0, name, dateCreated, description, duration, deadline, 
                                projectedStart, difficulty, assignedEng, actualEnd,
                                isMilestone, actualStart, deliverable!, notes!, inactive));
                        }
                        catch   (Exception ex) { 
                            Console.WriteLine(ex);
                        }
                        break;


                    case 'c': //read
                        Console.WriteLine("Enter object ID: ");
                        string? stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            Console.WriteLine(s_dalTask!.Read(intId));
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    case 'd': //readAll
                        Console.WriteLine(s_dalTask!.ReadAll());
                        break;

                    case 'e':  //update
                        Console.WriteLine("Enter task ID to update: "); 
                        stringId = Console.ReadLine();
                        DO.Task? t;

                        //checks not empty string
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            t = s_dalTask!.Read(intId);
                            if (t is not null) s_dalTask.Update(t);
                        }
                        else Console.WriteLine("No ID entered.");

                        break;

                    case 'f':   //delete
                        Console.WriteLine("Enter object ID you want to delete: ");
                        stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            s_dalTask!.Delete(intId);
                        }
                        break;
                }

            }

            //Engineer Menu
            else if (userInput == '2')
            {
                Console.WriteLine("\nEnter a character for which action to do in Engineer:\n" +
                    "a. Go back\n" +
                    "b. Add an Object to the entity list - Create()\n" +
                    "c. Display and object using an object’s identifier - Read()\n" +
                    "d. Display the object list - ReadAll()\n" +
                    "e. Update an object - Update()\n" +
                    "f. Delete an object from the object list – Delete()");

                //take in user input with 2 ignore keys for empty values
                userInput = Console.Read();
                ignoreKey = Console.Read();
                ignoreKey = Console.Read();

                switch (userInput)
                {

                    case 'a': break; //go back to main menu

                    case 'b': //create
                        Console.WriteLine("Enter name of the engineer: ");
                        string? name = Console.ReadLine();

                        break;

                    case 'c': //read
                        Console.WriteLine("Enter object ID: ");
                        string? stringId = Console.ReadLine();
                        int? intId = Convert.ToInt32(stringId);

                        break;

                    case 'd': //readAll
                        Console.WriteLine();
                        break;

                    case 'e':  //update
                        Console.WriteLine();
                        break;

                    case 'f':   //delete
                        Console.WriteLine("Enter object ID you want to delete: ");
                        stringId = Console.ReadLine();
                        intId = Convert.ToInt32(stringId);
                        break;
                }

            }

            else if (userInput == '3')
            {
                Console.WriteLine("\nEnter a character for which action to do in Engineer:\n" +
                    "a. Go back\n" +
                    "b. Add an Object to the entity list - Create()\n" +
                    "c. Display and object using an object’s identifier - Read()\n" +
                    "d. Display the object list - ReadAll()\n" +
                    "e. Update an object - Update()\n" +
                    "f. Delete an object from the object list – Delete()");

                //take in user input with 2 ignore keys for empty values
                userInput = Console.Read();
                ignoreKey = Console.Read();
                ignoreKey = Console.Read();

                switch (userInput)
                {
                    case 'a': break; //go back to main menu

                    case 'b': //create
                        Console.WriteLine("Enter name of the engineer: ");
                        string? name = Console.ReadLine();

                        break;


                    case 'c': //read
                        Console.WriteLine("Enter object ID: ");
                        string? stringId = Console.ReadLine();
                        int? intId = Convert.ToInt32(stringId);

                        break;

                    case 'd': //readAll
                        Console.WriteLine();
                        break;

                    case 'e':  //update
                        Console.WriteLine();
                        break;

                    case 'f':   //delete
                        Console.WriteLine("Enter object ID you want to delete: ");
                        stringId = Console.ReadLine();
                        intId = Convert.ToInt32(stringId);
                        break;
                }
            }

            else
            {
                Console.WriteLine("Response not valid, try again.\n");
            }
        }
    }
}

