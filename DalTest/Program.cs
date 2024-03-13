using Dal;
using DalApi;
using DO;

namespace DalTest;

internal class Program
{
    //private static IConfig? s_dalConfig = new ConfigImplementation();    //stage 1
    //private static ITask? s_dalTask = new TaskImplementation(); //stage 1
    //private static IEngineer? s_dalEngineer = new EngineerImplementation(); //stage 1
    //private static IDependency? s_dalDependency = new DependencyImplementation(); //stage 1
    // private static IDal? s_dal = new DalList();   // stage 2
    // private static IDal? s_dal = new DalXml(); // stage 3
    static readonly IDal? s_dal = Factory.Get; // stage 4

    static void Main(string[] args)
    {
        Console.Write("Would you like to create Initial data? (Y/N)"); //stage 3
        string? ans = Console.ReadLine() ?? throw new FormatException("Wrong input"); //stage 3
        if (ans == "Y") //stage 3
            //DalTest.Initialization.Do(s_dal); // stage 2
            DalTest.Initialization.Do(); // stage 4

        string? userInput = null;

        while (true)
        {
            //Main Menu
            Console.WriteLine("\nEnter a number of the entity you want to test:\n" +
            "0. Exit Main Menu\n" +
            "1. Task\n" +
            "2. Engineer\n" +
            "3. Dependency\n" +
            "4. Project Data");

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
                    "f. Delete an object from the object list – Delete()\n"+
                    "g. Erase all data values (in memory)\n" //+ "h. Change Kickstart/Projected Start date of task\n" +
                   // "i. Change end date of task"
                     );

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
                        input = Console.ReadLine();
                        string description = input ?? "";
                            

                        Console.WriteLine("Enter duration of task (hours, hit enter then put minutes and press enter): ");
                        input = Console.ReadLine();
                        TimeSpan? duration = null;
                        int hours, mins;

                        if (input != "" && input is not null)
                        {
                            hours = Convert.ToInt32(input);

                            input = Console.ReadLine();
                            if (input != "" && input is not null)
                            {
                                mins = Convert.ToInt32(input);
                                duration = new TimeSpan(hours, mins, 0);
                            }
                        }

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

                        Console.WriteLine("Enter if task is deliverable (Y/N): ");
                        bool deliverable = (input! == "Y");

                        Console.WriteLine("Enter notes for the task: ");
                        string? notes = Console.ReadLine();

                        Console.WriteLine("Enter whether task is inactive (Y/N): ");
                        input = Console.ReadLine();
                        bool inactive = (input! == "Y");

                        try
                        {
                            DO.Task task = new DO.Task(0, name, dateCreated, description, duration, deadline,
                                projectedStart, difficulty, assignedEng, actualEnd,
                                isMilestone, actualStart, deliverable, notes!, inactive);
                            s_dal!.Task.Create(task);
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
                                DO.Task? task1 = s_dal!.Task.Read(intId);

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
                            foreach(DO.Task? var_task in s_dal!.Task.ReadAll())
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

                        //checks not empty string
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            
                            try
                            {
                                DO.Task? task = s_dal!.Task.Read(intId) ?? null;

                                //print out object
                                Console.WriteLine(task);
                                Console.WriteLine("\nEnter updated information below:\n");

                                //Collects Updated information from User - if input is blank then do not change
                                Console.WriteLine("Enter name of task: ");
                                input = Console.ReadLine();
                                name = (input == "" || input is null) ? task!.Alias : input;

                                Console.WriteLine("Date Created (mm/dd/yyyy): ");
                                input = Console.ReadLine();
                                dateCreated = input == "" ? task!.DateCreated : DateTime.Parse(input!);

                                Console.WriteLine("Enter description of task: ");
                                input = Console.ReadLine();
                                description = (input == "" || input is null) ? task!.Description : input;

                                Console.WriteLine("Enter duration of task (hours, hit enter then put minutes and press enter): ");
                                input = Console.ReadLine();
                                duration = task!.Duration;

                                if (input != "" && input is not null)
                                {
                                    hours = Convert.ToInt32(input);

                                    input = Console.ReadLine();
                                    if (input != "" && input is not null)
                                    {
                                        mins = Convert.ToInt32(input);
                                        duration = new TimeSpan(hours, mins, 0);
                                    }
                                }

                                Console.WriteLine("Enter deadline of task (mm/dd/yyyy): ");
                                input = Console.ReadLine();
                                deadline = (input == "" ? task!.Deadline : DateTime.Parse(input!));

                                Console.WriteLine("Enter projected start date of task (mm/dd/yyyy): ");
                                input = Console.ReadLine();
                                projectedStart = (input == "" ? task!.ProjectedStartDate : DateTime.Parse(input!));

                                Console.WriteLine("Enter difficulty of task (0-4): ");
                                //list of enums and variables 
                                allDifficulties = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));

                                //user input
                                input = Console.ReadLine();

                                if (input == "") difficulty = task!.DegreeOfDifficulty;
                                else difficulty = allDifficulties[Convert.ToInt32(input)];

                                //DO WE NEED TO check if the engineer is in our list of engineers?
                                Console.WriteLine("Enter ID of assigned Enginner of task: ");
                                input = Console.ReadLine();

                                if (input == "") assignedEng = task!.AssignedEngineerId;
                                else assignedEng = Convert.ToInt32(input);

                                Console.WriteLine("Enter actual end date of task (mm/dd/yyyy): ");
                                input = Console.ReadLine();
                                actualEnd = (input == "" ? task!.ActualEndDate : DateTime.Parse(input!));

                                Console.WriteLine("Enter whether task is a milestone (Y/N): ");
                                input = Console.ReadLine();
                                isMilestone = ((input! == "") ? task!.IsMilestone : (input! == "Y"));   //if input is blank then leave as previous value otherwise based on new input


                                Console.WriteLine("Enter actaul start date of task (mm/dd/yyyy): ");
                                input = Console.ReadLine();
                                actualStart = (input == "" ? task!.ActualStartDate : DateTime.Parse(input!));

                                Console.WriteLine("Enter if task is deliverable (Y/N): ");
                                input = Console.ReadLine();
                                deliverable = ((input! == "") ? task!.IsMilestone : (input! == "Y"));

                                Console.WriteLine("Enter notes for the task: ");
                                input = Console.ReadLine();
                                notes = (input == "" || input is null) ? task!.Notes : input;

                                Console.WriteLine("Enter whether task is inactive (Y/N): ");
                                input = Console.ReadLine();
                                inactive = ((input! == "") ? task!.Inactive : (input! == "Y"));

                                DO.Task updatedTask = new DO.Task(task!.Id, name, dateCreated, description, duration, deadline,
                                                        projectedStart, difficulty, assignedEng, actualEnd,
                                                        isMilestone, actualStart, deliverable!, notes!, inactive);
                            
                                s_dal!.Task.Update(updatedTask);

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
                                s_dal!.Task.Delete(intId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    //---------------------- Reset -----------------------------------
                    case "g":
                        Console.WriteLine("Erasing all tasks...");
                        try
                        {
                            s_dal!.Task.Reset();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

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
                    "f. Delete an object from the object list – Delete()\n" +
                    "g. Erase all data values (in memory)");

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
                        string email = (input == "" || input is null) ? "firstLast@gmail.com" : input;

                        Console.WriteLine("Enter experience level of the engineer (0-4): ");
                        //list of enums and variables 
                        Enums.EngineerExperience[] allExperiences = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
                        Enums.EngineerExperience? experience;

                        input = Console.ReadLine();

                        if (input == "") experience = null;
                        else experience = allExperiences[Convert.ToInt32(input)];

                        Console.WriteLine("Enter cost per hour of the engineer (XX.XX): ");
                        input = Console.ReadLine();
                        double costPerHour = (input == "") ? 0 : Convert.ToDouble(input);

                        Console.WriteLine("Enter whether engineer is inactive (Y/N): ");
                        input = Console.ReadLine();
                        bool inactive = input! == "Y" ? true : false;

                        try
                        {
                            Engineer eng = new Engineer(tz, name, email, costPerHour, experience, inactive);
                            s_dal!.Engineer.Create(eng);
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
                                Engineer? eng = s_dal!.Engineer.Read(intId);

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
                            foreach (Engineer? var_eng in s_dal!.Engineer.ReadAll())
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

                            try { 
                                engineer = s_dal!.Engineer.Read(intId);

                                //print out object
                                Console.WriteLine(engineer);
                                Console.WriteLine("\nEnter updated information below:\n");


                                Console.WriteLine("Enter name of the engineer: ");
                                input = Console.ReadLine();
                                name = (input == "" || input is null) ? engineer!.FullName : input;

                                Console.WriteLine("Enter email address of the engineer: ");
                                input = Console.ReadLine();
                                email = (input == "" || input is null) ? engineer!.EmailAddress : input;

                                Console.WriteLine("Enter experience level of the engineer (0-4): ");
                                //list of enums and variables 
                                allExperiences = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));

                                input = Console.ReadLine();

                                if (input == "") experience = engineer!.ExperienceLevel;
                                else experience = allExperiences[Convert.ToInt32(input)];

                                Console.WriteLine("Enter cost per hour of the engineer (XX.XX): ");
                                input = Console.ReadLine();
                                costPerHour = (input == "") ? engineer!.CostPerHour : Convert.ToDouble(input);

                                Console.WriteLine("Enter whether engineer is inactive (Y/N): ");
                                input = Console.ReadLine();
                                inactive = ((input! == "") ? engineer!.Inactive : (input! == "Y"));

                                Engineer updatedEng = new Engineer(engineer!.Id, name, email, costPerHour, experience, inactive);

                                if (engineer is not null) s_dal!.Engineer.Update(updatedEng);

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
                                s_dal!.Engineer.Delete(intId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    //---------------------- Reset -----------------------------------
                    case "g":
                        Console.WriteLine("Erasing all engineers...");
                        try
                        {
                            s_dal!.Engineer.Reset();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
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
                    "f. Delete an object from the object list – Delete()\n" +
                    "g. Erase all data values (in memory)");

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

                        /*
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
                        */

                        Console.WriteLine("Enter whether dependency is inactive (Y/N): ");
                        input = Console.ReadLine();
                        bool inactive = input! == "Y" ? true : false;

                        try
                        {
                            Dependency dep = new Dependency(0, depTaskID, reqID, inactive);
                            s_dal!.Dependency.Create(dep);
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
                                Dependency? dep = s_dal!.Dependency.Read(intId);

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
                            foreach (Dependency? var_dep in s_dal!.Dependency.ReadAll())
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
                            try { 
                                dependency = s_dal!.Dependency.Read(intId);

                                //print out object
                                Console.WriteLine(dependency);
                                Console.WriteLine("\nEnter updated information below:\n");

                                //Collects Updated information from User - if input is blank then do not change
                                Console.WriteLine("Enter dependent task ID of the Dependency: ");
                                input = Console.ReadLine();
                                depTaskID = (input == "" || input is null) ? dependency!.DependentTaskId : Convert.ToInt32(input);

                                Console.WriteLine("Enter requisite task ID of the Dependency: ");
                                input = Console.ReadLine();
                                reqID = (input == "" || input is null) ? dependency!.RequisiteID : Convert.ToInt32(input);

                                /*
                                Console.WriteLine("Enter customer email of the Dependency: ");
                                input = Console.ReadLine();
                                customerEmail = (input == "") ? dependency!.CustomerEmail : input;

                                Console.WriteLine("Enter shipping address of the Dependency: ");
                                input = Console.ReadLine();
                                shipAddress = (input == "") ? dependency!.ShippingAddress : input;

                                Console.WriteLine("Enter order creation date of the Dependency: ");
                                input = Console.ReadLine();
                                dateCreated = (input == "") ? dependency!.OrderCreationDate : DateTime.Parse(input!);

                                Console.WriteLine("Enter shipping date of the Dependency: ");
                                input = Console.ReadLine();
                                shipDate = (input == "" ? dependency!.ShippingDate : DateTime.Parse(input!));

                                Console.WriteLine("Enter delivery date date of the Dependency: ");
                                input = Console.ReadLine();
                                deliveryDate = (input == "" ? dependency!.DeliveryDate : DateTime.Parse(input!));
                                */

                                Console.WriteLine("Enter whether dependency is inactive (Y/N): ");
                                input = Console.ReadLine();
                                inactive = ((input! == "") ? dependency!.Inactive : (input! == "Y"));

                                Dependency updatedDependency = new Dependency(dependency!.Id, depTaskID, reqID, inactive);

                                s_dal!.Dependency.Update(updatedDependency);

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
                                s_dal!.Dependency.Delete(intId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    //---------------------- Reset -----------------------------------
                    case "g":
                        Console.WriteLine("Erasing all dependencies...");
                        try
                        {
                            s_dal!.Dependency.Reset();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;
                }
            }

//----------------------------------- Entire Config Maniuplation Menu --------------------------------
            else if (userInput == "4")
            {
                Console.WriteLine("\nEnter a character for which action to test:\n" +
                    "a. Go back\n" +
                    "b. Reset everything - Reset()\n" +
                    "c. Set new Project Start Date\n" +
                    "d. Set new Project End Date\n" + 
                    "e. Get Project Start Date\n" +
                    "f. Get Project End Date");

                //take in user input
                userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "a": break;

                    //Reset Everything
                    case "b":
                        try
                        {
                            Console.WriteLine("\nResetting entire project you absolute savage...");
                            s_dal!.Config.Reset();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;    

                    //------------------- Set Project Kickstart Date ------------------
                    case "c":
                        try
                        {
                            Console.WriteLine("\nEnter new Project Kickstart date:");
                            string? input = Console.ReadLine();
                            DateTime newDate = (input == "") ? DateTime.Now : DateTime.Parse(input!);

                            s_dal!.Config.SetProjectStartDate(newDate);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        
                        break;

                    //------------ set Prject End date ----------------
                    case "d":
                        try
                        {
                            Console.WriteLine("\nEnter new Project End date:");
                            string? input = Console.ReadLine();
                            DateTime newDate = (input == "") ? DateTime.Now : DateTime.Parse(input!);

                            s_dal!.Config.SetProjectEndDate(newDate);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;
                    //------------ get Prject start date ----------------
                    case "e":
                        try
                        {
                            Console.WriteLine("\nProject start date: ");
                            Console.WriteLine(s_dal!.Config.GetProjectStartDate());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;
                    //------------ get Prject End date ----------------
                    case "f":
                        try
                        {
                            Console.WriteLine("\nProject end date: ");
                            Console.WriteLine(s_dal!.Config.GetProjectEndDate());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
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

