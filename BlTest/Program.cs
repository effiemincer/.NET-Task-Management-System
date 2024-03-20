using BO;
using DalApi;
using DO;
using System.Threading.Tasks;

namespace BlTest;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    static readonly IDal? s_dal = Factory.Get;

    static void Main(string[] args)
    {
        Console.Write("Would you like to create Initial data? (Y/N)"); 
        string? ans = Console.ReadLine() ?? throw new FormatException("Wrong input");
        if (ans == "Y")
            DalTest.DalTest.Initialization.Do();

        string? userInput = null;

        while (true)
        {
            //Main Menu
            Console.WriteLine("\nEnter a number of the entity you want to test:\n" +
            "0. Exit Main Menu\n" +
            "1. Task\n" +
            "2. Engineer\n" +
            "3. Milestone\n" +
            "4. Project\n");

            //take in user input
            userInput = Console.ReadLine();

            //exit main menu
            if (userInput == "0") {
                return; }

            //=========================================== Task Menu ======================================================
            else if (userInput == "1")
            {
                Console.WriteLine("\nEnter a character for which action to test in Task:\n" +
                    "a. Go back\n" +
                    "b. Add an Object to the entity list - Create()\n" +
                    "c. Display and object using an object’s identifier - Read()\n" +
                    "d. Display the object list - ReadAll()\n" +
                    "e. Update an object - Update()\n" +
                    "f. Delete an object from the object list – Delete()\n" +
                    "g. Update a Projected Start date of a task - Update
                    ()\n");
                     

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

                        DateTime dateCreated = DateTime.Now;

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

                        Console.WriteLine("Enter projected start date of task (mm/dd/yyyy) or nothing for null: ");
                        input = Console.ReadLine();
                        DateTime? projectedStart = (input == "" ? null : DateTime.Parse(input!));

                        Console.WriteLine("Enter difficulty of task (0-4): ");
                        //list of enums and variables 
                        BO.Enums.EngineerExperience[] allDifficulties = (BO.Enums.EngineerExperience[])Enum.GetValues(typeof(BO.Enums.EngineerExperience));
                        BO.Enums.EngineerExperience? difficulty;

                        //user input
                        input = Console.ReadLine();

                        if (input == "") difficulty = null;
                        else difficulty = allDifficulties[Convert.ToInt32(input)];

                        Console.WriteLine("Enter ID of assigned Enginner of task: ");
                        input = Console.ReadLine();
                        int? assignedEng;

                        if (input == "") assignedEng = null;
                        else assignedEng = Convert.ToInt32(input);

                        Console.WriteLine("Enter actual end date of task (mm/dd/yyyy): ");
                        input = Console.ReadLine();
                        DateTime? actualEnd = (input == "" ? null : DateTime.Parse(input!));

                        Console.WriteLine("Enter actaul start date of task (mm/dd/yyyy): ");
                        input = Console.ReadLine();
                        DateTime? actualStart = (input == "" ? null : DateTime.Parse(input!));

                        Console.WriteLine("Enter if task is deliverable (Y/N): ");
                        bool deliverable = (input! == "Y");

                        Console.WriteLine("Enter notes for the task: ");
                        string? notes = Console.ReadLine();

                        Console.WriteLine("Enter number of dependencies for the task: ");
                        input = Console.ReadLine();
                        int numDependencies = (input == "" || input is null) ? 0 : Convert.ToInt32(input);
                        List<BO.TaskInList> dependencies = new List<TaskInList>();
                        for (int i=0; i < numDependencies; i++)
                        {
                            Console.WriteLine("Enter ID of requisite task: ");
                            input = Console.ReadLine();
                            int reqID = (input == "" || input is null) ? 0 : Convert.ToInt32(input);
                            try
                            {
                                dependencies.Add(new BO.TaskInList { Id = reqID });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }


                        try
                        {
                            BO.Task task = new BO.Task()
                            {
                                Id = 0,
                                Alias = name,
                                DateCreated = dateCreated,
                                Description = description,
                                Dependencies = dependencies,
                                RequiredEffortTime = duration,
                                ActualStartDate = actualStart,
                                ProjectedStartDate = projectedStart,
                                Deadline = deadline,
                                ActualEndDate = actualEnd,
                                Deliverable = deliverable,
                                Remarks = notes,
                                Complexity = difficulty
                            };

                            s_bl!.Task.Create(task);
                        }
                        catch (Exception ex)
                        {
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
                                BO.Task? task1 = s_bl!.Task.Read(intId);

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
                            foreach (BO.TaskInList? var_task in s_bl!.Task.ReadAll())
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
                                BO.Task? task = s_bl!.Task.Read(intId) ?? null;

                                //print out object
                                Console.WriteLine(task);
                                Console.WriteLine("\nEnter updated information below:\n");

                                //Collects Updated information from User - if input is blank then do not change
                                Console.WriteLine("Enter name of task: ");
                                input = Console.ReadLine();
                                name = ((input == "" || input is null) ? task!.Alias : input)!;

                                Console.WriteLine("Date Created (mm/dd/yyyy): ");
                                input = Console.ReadLine();
                                dateCreated = input == "" ? task!.DateCreated : DateTime.Parse(input!);

                                Console.WriteLine("Enter description of task: ");
                                input = Console.ReadLine();
                                description = ((input == "" || input is null) ? task!.Description : input)!;

                                Console.WriteLine("Enter duration of task (hours, hit enter then put minutes and press enter): ");
                                input = Console.ReadLine();
                                duration = task!.RequiredEffortTime;

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

                                Console.WriteLine("Enter difficulty of task (0-4): ");
                                //list of enums and variables 
                                allDifficulties = (BO.Enums.EngineerExperience[])Enum.GetValues(typeof(BO.Enums.EngineerExperience));

                                //user input
                                input = Console.ReadLine();

                                if (input == "") difficulty = task!.Complexity;
                                else difficulty = allDifficulties[Convert.ToInt32(input)];

                                //DO WE NEED TO check if the engineer is in our list of engineers?
                                Console.WriteLine("Enter ID of assigned Enginner of task: ");
                                input = Console.ReadLine();

                                if (task!.Engineer is null) 
                                    assignedEng = null;
                                else if (input == "") 
                                    assignedEng = task!.Engineer.Id;
                                else 
                                    assignedEng = Convert.ToInt32(input);

                                Console.WriteLine("Enter actual end date of task (mm/dd/yyyy): ");
                                input = Console.ReadLine();
                                actualEnd = (input == "" ? task!.ActualEndDate : DateTime.Parse(input!));

                                Console.WriteLine("Enter actaul start date of task (mm/dd/yyyy): ");
                                input = Console.ReadLine();
                                actualStart = (input == "" ? task!.ActualStartDate : DateTime.Parse(input!));

                                Console.WriteLine("Enter deliverable of task: ");
                                input = Console.ReadLine();
                                deliverable = ((input! == "") ? task!.Deliverable : (input! == "Y"));

                                Console.WriteLine("Enter notes for the task: ");
                                input = Console.ReadLine();
                                notes = (input == "" || input is null) ? task!.Remarks : input;

                                BO.Task updatedTask = new BO.Task()
                                {
                                    Id = task.Id,
                                    Alias = name,
                                    DateCreated = dateCreated,
                                    Description = description,
                                    RequiredEffortTime = duration,
                                    ActualStartDate = actualStart,
                                    Deadline = deadline,
                                    ActualEndDate = actualEnd,
                                    Deliverable = deliverable,
                                    Remarks = notes,
                                    Complexity = difficulty
                                };

                                s_bl!.Task.Update(updatedTask);

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
                                s_bl!.Task.Delete(intId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    //---------------------- Update Projected Start Date -----------------------------------
                    case "g":
                        Console.WriteLine("Enter task ID you want to update the projected Start date of: ");
                        stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);

                            Console.WriteLine("Enter projected start date of task (mm/dd/yyyy): ");
                            input = Console.ReadLine();
                            DateTime? newDateTime = (input == "" ? s_dal!.Task.Read(intId)!.ProjectedStartDate : DateTime.Parse(input!));


                            try
                            {
                                s_bl!.Task.UpdateProjectedStartDate(intId, newDateTime);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");

                        break;
                        /*
                        //---------------------- Reset -----------------------------------
                        case "g":
                            Console.WriteLine("Erasing all tasks...");
                            try
                            {
                                s_bl!.Task.Reset();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }

                            break;
                        */

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
                    "f. Delete an object from the object list – Delete()\n");
                    //"g. Erase all data values (in memory)");

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
                        BO.Enums.EngineerExperience[] allExperiences = (BO.Enums.EngineerExperience[])Enum.GetValues(typeof(BO.Enums.EngineerExperience));
                        BO.Enums.EngineerExperience? experience;

                        input = Console.ReadLine();

                        if (input == "") experience = null;
                        else experience = allExperiences[Convert.ToInt32(input)];

                        Console.WriteLine("Enter cost per hour of the engineer (XX.XX): ");
                        input = Console.ReadLine();
                        double costPerHour = (input == "") ? 0 : Convert.ToDouble(input);

                        try
                        {
                            BO.Engineer eng = new BO.Engineer { Id = tz, Name = name, EmailAddress = email, CostPerHour = costPerHour, ExperienceLevel = experience };
                            s_bl!.Engineer.Create(eng);
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
                                BO.Engineer? eng = s_bl!.Engineer.Read(intId);

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
                            foreach (BO.Engineer? var_eng in s_bl!.Engineer.ReadAll())
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
                        BO.Engineer? engineer;

                        //checks not empty string
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);

                            try
                            {
                                engineer = s_bl!.Engineer.Read(intId);

                                //print out object
                                Console.WriteLine(engineer);
                                Console.WriteLine("\nEnter updated information below:\n");


                                Console.WriteLine("Enter name of the engineer: ");
                                input = Console.ReadLine();
                                name = ((input == "" || input is null) ? engineer!.Name : input)!;

                                Console.WriteLine("Enter email address of the engineer: ");
                                input = Console.ReadLine();
                                email = ((input == "" || input is null) ? engineer!.EmailAddress : input)!;

                                Console.WriteLine("Enter experience level of the engineer (0-4): ");
                                //list of enums and variables 
                                allExperiences = (BO.Enums.EngineerExperience[])Enum.GetValues(typeof(BO.Enums.EngineerExperience));

                                input = Console.ReadLine();

                                if (input == "") experience = engineer!.ExperienceLevel;
                                else experience = allExperiences[Convert.ToInt32(input)];

                                Console.WriteLine("Enter cost per hour of the engineer (XX.XX): ");
                                input = Console.ReadLine();
                                costPerHour = (input == "") ? engineer!.CostPerHour : Convert.ToDouble(input);


                                BO.Engineer updatedEng = new BO.Engineer { Id = intId, Name = name, EmailAddress = email, CostPerHour = costPerHour, ExperienceLevel = experience };

                                if (engineer is not null) s_bl!.Engineer.Update(updatedEng);

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
                                s_bl!.Engineer.Delete(intId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    //---------------------- Reset -----------------------------------
                    //case "g":
                    //    Console.WriteLine("Erasing all engineers...");
                    //    try
                    //    {
                    //        s_bl!.Engineer.Reset();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Console.WriteLine(ex);
                    //    }
                    //    break;
                }

            }

            //================================================= Milestone Menu ===========================================================
            else if (userInput == "3")
            {
                Console.WriteLine("\nEnter a character for which action to test in Dependency:\n" +
                    "a. Go back\n" +
                    "b. Create the project schedule - CreateSchedule()\n" +
                    "c. Display and object using an object’s identifier - Read()\n" +
                    "d. Update an object - Update()\n");

                //take in user input
                userInput = Console.ReadLine();

                switch (userInput)
                {

                    case "a": break; //go back to main menu

                    //--------------------- Create Scehdule -----------------------------
                    case "b": //create
                        Console.WriteLine("Enter new project start date or leave blank to keep old one: ");
                        string? input = Console.ReadLine();
                        DateTime? newStartDate = (input == "" ? s_dal!.Config.GetProjectStartDate() : DateTime.Parse(input!));
                        if (input != "") s_dal!.Config.SetProjectStartDate((DateTime)newStartDate!);

                        Console.WriteLine("Enter new project end date or leave blank to keep old one: ");
                        input = Console.ReadLine();
                        DateTime? newEndDate = (input == "" ? s_dal!.Config.GetProjectEndDate() : DateTime.Parse(input!));
                        if (input != "") s_dal!.Config.SetProjectEndDate((DateTime)newEndDate!);


                        try
                        {

                            string res = s_bl!.Milestone.CreateSchedule((DateTime)s_dal!.Config.GetProjectStartDate()!, (DateTime)s_dal!.Config.GetProjectEndDate()!);
                            Console.WriteLine(res);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;

                    //--------------------- Read Milestone -----------------------------
                    case "c": //read
                        Console.WriteLine("Enter milestone ID: ");
                        string? stringId = Console.ReadLine();
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                Milestone? mlstone = s_bl!.Milestone.Read(intId);

                                if (mlstone is not null) Console.WriteLine(mlstone);    //if milestone is found

                                else Console.WriteLine("Dependency not found.");

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else Console.WriteLine("No ID entered.");
                        break;

                    //--------------------- Update Milestone -----------------------------
                    case "d":  //update
                        Console.WriteLine("Enter milestone ID to update: ");
                        stringId = Console.ReadLine();
                        Milestone? milestone;

                        //checks not empty string
                        if (stringId != "")
                        {
                            int intId = Convert.ToInt32(stringId);
                            try
                            {
                                milestone = s_bl!.Milestone.Read(intId);

                                //print out object
                                Console.WriteLine(milestone);
                                Console.WriteLine("\nEnter updated information below:\n");

                                //Description, Alias and Remarks

                                //Collects Updated information from User - if input is blank then do not change
                                Console.WriteLine("Enter description of Milestone: ");
                                input = Console.ReadLine();
                                string description = (input is null) ? "": input;

                                Console.WriteLine("Enter name of the milestone: ");
                                input = Console.ReadLine();
                                string alias = (input is null) ? "": input;

                                Console.WriteLine("Enter remarks about the milestone: ");
                                input = Console.ReadLine();
                                string remarks = (input is null) ? "" : input;

                                s_bl!.Milestone.Update(intId, alias, description, remarks);

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
