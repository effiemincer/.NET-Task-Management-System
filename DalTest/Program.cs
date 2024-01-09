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

        string? userInput = null;

        while (true)
        {
            //Main Menu
            Console.WriteLine("\nEnter a number of the entity you want to test:\n" +
            "0. Exit Main Menu\n" +
            "1. Task\n" +
            "2. Engineer\n" +
            "3. Dependency");

            //take in user input
            userInput = Console.ReadLine();

            //exit main menu
            if (userInput == "0") { return; }

//=========================================== Task Menu ======================================================
            else if (userInput == "1")
            {
                Console.WriteLine("\nEnter a character for which action to test in Task:\n" +
                    "a. Go back\n" +
                    "b. Add an Object to the entity list - Create()\n" +
                    "c. Display and object using an object’s identifier - Read()\n" +
                    "d. Display the object list - ReadAll()\n" +
                    "e. Update an object - Update()\n" +
                    "f. Delete an object from the object list – Delete()");

                //take in user input
                userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "a": break; //go back to main menu

                    //------------------ Create Task ----------------------------
                    case "b":
                        Console.WriteLine("Enter name of task: ");
                        string? input = Console.ReadLine();
                        string name = (input == "" || input is null) ? "Placeholder Name" : input;

                        Console.WriteLine("Date Created (mm/dd/yyyy): ");
                        input = Console.ReadLine();
                        DateTime dateCreated = input == "" ? DateTime.Now : DateTime.Parse(input!);

                        Console.WriteLine("Enter description of task: ");
                        string? description = Console.ReadLine();

                        Console.WriteLine("Enter duration of task (hours): ");
                        input = Console.ReadLine();
                        int? duration;

                        if (input == "") duration = null;
                        else duration = Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine("Enter deadline of task (mm/dd/yyyy): ");
                        input = Console.ReadLine();
                        DateTime? deadline = (input == "" ? null : DateTime.Parse(input!));

                        Console.WriteLine("Enter projected start date of task (mm/dd/yyyy): ");
                        input = Console.ReadLine();
                        DateTime? projectedStart = (input == "" ? null : DateTime.Parse(input!));

                        Console.WriteLine("Enter difficulty of task (0-4): ");
                        //list of enums and variables 
                        Enums.EngineerExperience[] allDifficulties = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
                        Enums.EngineerExperience? difficulty;

                        //user input
                        input = Console.ReadLine();

                        if (input == "") difficulty = null;
                        else difficulty = allDifficulties[Convert.ToInt32(input)];

              
                        //DO WE NEED TO check if the engineer is in our list of engineers?
                        Console.WriteLine("Enter ID of assigned Enginner of task: ");
                        input = Console.ReadLine();
                        int? assignedEng;

                        if (input == "") assignedEng = null;
                        else assignedEng = Convert.ToInt32(input);

                        Console.WriteLine("Enter actual end date of task (mm/dd/yyyy): ");
                        input = Console.ReadLine();
                        DateTime? actualEnd = (input == "" ? null : DateTime.Parse(input!));

                        Console.WriteLine("Enter whether task is a milestone (Y/N): ");
                        input = Console.ReadLine();
                        bool isMilestone = (input! == "Y");


                        Console.WriteLine("Enter actaul start date of task (mm/dd/yyyy): ");
                        input = Console.ReadLine();
                        DateTime? actualStart = (input == "" ? null : DateTime.Parse(input!));

                        Console.WriteLine("Enter deliverable of task: ");
                        string? deliverable = Console.ReadLine();

                        Console.WriteLine("Enter notes for the task: ");
                        string? notes = Console.ReadLine();

                        Console.WriteLine("Enter whether task is inactive (Y/N): ");
                        input = Console.ReadLine();
                        bool inactive = (input! == "Y");

                        try
                        {
                            DO.Task task = new DO.Task(0, name, dateCreated, description, duration, deadline,
                                projectedStart, difficulty, assignedEng, actualEnd,
                                isMilestone, actualStart, deliverable!, notes!, inactive);
                            s_dalTask!.Create(task);
                        }
                        catch   (Exception ex) { 
                            Console.WriteLine(ex);
                        }
                        break;


            //---------------- Read Task ---------------------------
                    case "c": //read
                        Console.WriteLine("Enter task ID: ");
                        string? stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                DO.Task? task1 = s_dalTask!.Read(intId);

                                if (task1 is not null) Console.WriteLine(task1);    //if task is found

                                else Console.WriteLine("Task not found.");

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

             //------------------- Read All tasks -------------------------
                    case "d": //readAll
                        try
                        {
                            //for loop to print them all
                            foreach(DO.Task var_task in s_dalTask!.ReadAll())
                            {
                                Console.WriteLine(var_task);
                                Console.WriteLine();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        break;

        //------------------ Update task ----------------------
                    case "e":  //update
                        Console.WriteLine("Enter task ID to update: "); 
                        stringId = Console.ReadLine();
                        DO.Task? t;

                        //checks not empty string
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                t = s_dalTask!.Read(intId);
                                if (t is not null) s_dalTask.Update(t);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");

                        break;

        //--------------------- Delete Task ---------------------
                    case "f":   //delete
                        Console.WriteLine("Enter task ID you want to delete: ");
                        stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                s_dalTask!.Delete(intId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;
                }

            }

//=============================================== Engineer Menu ==========================================================
            else if (userInput == "2")
            {
                Console.WriteLine("\nEnter a character for which action to test in Engineer:\n" +
                    "a. Go back\n" +
                    "b. Add an Object to the entity list - Create()\n" +
                    "c. Display and object using an object’s identifier - Read()\n" +
                    "d. Display the object list - ReadAll()\n" +
                    "e. Update an object - Update()\n" +
                    "f. Delete an object from the object list – Delete()");

                //take in user input
                userInput = Console.ReadLine();

                switch (userInput)
                {

                    case "a": break; //go back to main menu

                    //--------------------- Create Engineer -----------------------------
                    case "b": //create
                        Console.WriteLine("Enter Teudat Zehut of the engineer: ");
                        string? input = Console.ReadLine();
                        int tz = (input == "" || input is null) ? 346291982 : Convert.ToInt32(input);

                        Console.WriteLine("Enter name of the engineer: ");
                        input = Console.ReadLine();
                        string name = (input == "" || input is null) ? "FirstName LastName" : input;

                        Console.WriteLine("Enter email address of the engineer: ");
                        input = Console.ReadLine();
                        string email = (input == "" || input is null) ? "FirstName LastName" : input;

                        Console.WriteLine("Enter experience level of the engineer (0-4): ");
                        //list of enums and variables 
                        Enums.EngineerExperience[] allExperiences = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
                        int experienceIndex;
                        Enums.EngineerExperience? experience;

                        input = Console.ReadLine();

                        if (input == "") experience = null;
                        else
                        {
                            experienceIndex = Convert.ToInt32(input);
                            experience = allExperiences[experienceIndex];
                        }

                        Console.WriteLine("Enter cost per hour of the engineer (XX.XX): ");
                        input = Console.ReadLine();
                        double? costPerHour = (input == "") ? null : Convert.ToDouble(input);

                        Console.WriteLine("Enter whether engineer is inactive (Y/N): ");
                        input = Console.ReadLine();
                        bool inactive = input! == "Y" ? true : false;

                        try
                        {
                            Engineer eng = new Engineer(tz, name, email, experience, costPerHour, inactive);
                            s_dalEngineer!.Create(eng);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        break;

                    //--------------------- Read Engineer -----------------------------
                    case "c": //read
                        Console.WriteLine("Enter engineer ID: ");
                        string? stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                Engineer? eng = s_dalEngineer!.Read(intId);

                                if (eng is not null) Console.WriteLine(eng);    //if eng is found

                                else Console.WriteLine("Engineer not found.");

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    //--------------------- Read All Engineers -----------------------------
                    case "d": //readAll
                        try
                        {
                            //for loop to print them all
                            foreach (Engineer var_eng in s_dalEngineer!.ReadAll())
                            {
                                Console.WriteLine(var_eng);
                                Console.WriteLine();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        break;

                    //--------------------- Update Engineer -----------------------------
                    case "e":  //update
                        Console.WriteLine("Enter engineer ID to update: ");
                        stringId = Console.ReadLine();
                        Engineer? engineer;

                        //checks not empty string
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                engineer = s_dalEngineer!.Read(intId);
                                if (engineer is not null) s_dalEngineer.Update(engineer);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");

                        break;

                    //--------------------- Delete Engineer -----------------------------
                    case "f":   //delete
                        Console.WriteLine("Enter Engineer ID you want to delete: ");
                        stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                s_dalEngineer!.Delete(intId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;
                }

            }

//================================================= Dependency Menu ===========================================================
            else if (userInput == "3")
            {
                Console.WriteLine("\nEnter a character for which action to test in Dependency:\n" +
                    "a. Go back\n" +
                    "b. Add an Object to the entity list - Create()\n" +
                    "c. Display and object using an object’s identifier - Read()\n" +
                    "d. Display the object list - ReadAll()\n" +
                    "e. Update an object - Update()\n" +
                    "f. Delete an object from the object list – Delete()");

                //take in user input
                userInput = Console.ReadLine();

                switch (userInput)
                {

                    case "a": break; //go back to main menu

                    //--------------------- Create Dependency -----------------------------
                    case "b": //create
                        Console.WriteLine("Enter dependent task ID of the Dependency: ");
                        string? input = Console.ReadLine();
                        int? depTaskID = (input == "" || input is null) ? null : Convert.ToInt32(input);

                        Console.WriteLine("Enter requisite task ID of the Dependency: ");
                        input = Console.ReadLine();
                        int? reqID = (input == "" || input is null) ? null : Convert.ToInt32(input);

                        Console.WriteLine("Enter customer email of the Dependency: ");
                        input = Console.ReadLine();
                        string? customerEmail = (input == "") ? null : input;

                        Console.WriteLine("Enter shipping address of the Dependency: ");
                        input = Console.ReadLine();
                        string? shipAddress = (input == "") ? null : input;

                        Console.WriteLine("Enter order creation date of the Dependency: ");
                        input = Console.ReadLine();
                        DateTime dateCreated = (input == "") ? DateTime.Now : DateTime.Parse(input!);

                        Console.WriteLine("Enter shipping date of the Dependency: ");
                        input = Console.ReadLine();
                        DateTime? shipDate = (input == "" ? null : DateTime.Parse(input!));

                        Console.WriteLine("Enter delivery date date of the Dependency: ");
                        input = Console.ReadLine();
                        DateTime? deliveryDate = (input == "" ? null : DateTime.Parse(input!));

                        Console.WriteLine("Enter whether dependency is inactive (Y/N): ");
                        input = Console.ReadLine();
                        bool inactive = input! == "Y" ? true : false;

                        try
                        {
                            Dependency dep = new Dependency(0, depTaskID, reqID, customerEmail, shipAddress,
                                dateCreated, shipDate, deliveryDate, inactive);
                            s_dalDependency!.Create(dep);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;

                    //--------------------- Read Dependency -----------------------------
                    case "c": //read
                        Console.WriteLine("Enter dependency ID: ");
                        string? stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                Dependency? dep = s_dalDependency!.Read(intId);

                                if (dep is not null) Console.WriteLine(dep);    //if eng is found

                                else Console.WriteLine("Dependency not found.");

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    //--------------------- Read all Dependencies -----------------------------
                    case "d": //readAll
                        try
                        {
                            //for loop to print them all
                            foreach (Dependency var_dep in s_dalDependency!.ReadAll())
                            {
                                Console.WriteLine(var_dep);
                                Console.WriteLine();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;

                    //--------------------- Update Dependency -----------------------------
                    case "e":  //update
                        Console.WriteLine("Enter dependency ID to update: ");
                        stringId = Console.ReadLine();
                        Dependency? dependency;

                        //checks not empty string
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                dependency = s_dalDependency!.Read(intId);
                                if (dependency is not null) s_dalDependency.Update(dependency);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    //--------------------- Delete Dependency -----------------------------
                    case "f":   //delete
                        Console.WriteLine("Enter dependency ID you want to delete: ");
                        stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                s_dalDependency!.Delete(intId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;
                }
            }

            else
            {
                Console.WriteLine("Response not valid, try again.");
            }
        }
    }
}

