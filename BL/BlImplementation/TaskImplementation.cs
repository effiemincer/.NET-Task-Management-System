using BlApi;
using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlImplementation
{
    /// <summary>
    /// TaskImplementation class implements ITask interface and is responsible for the logic of the Task entity
    /// </summary>
    internal class TaskImplementation : ITask
    {
        /// <summary>
        /// dal instance to be used by the class
        /// </summary>
        private DalApi.IDal _dal = DalApi.Factory.Get;

        private readonly Bl _bl; 
        internal TaskImplementation(Bl bl) => _bl = bl;
        /// <summary>
        /// Create a new Task
        /// </summary>
        /// <param name="boTask">The Task object to create</param>
        /// <returns>The ID of the newly created Task</returns>
        /// <exception cref="BO.BlArgumentNullException">Thrown when the provided Task object is null</exception>
        /// <exception cref="BO.BlNullPropertyException">Thrown when Alias or Description is null</exception>
        /// <exception cref="BO.BlBadInputDataException">Thrown when missing ID, Alias, or Description, or when the Task cannot be completed within the deadline</exception>
        /// <exception cref="BO.BlDoesNotExistException">Thrown when the specified Engineer does not exist</exception>
        /// <exception cref="BO.BlAlreadyExistsException">Thrown when a Task with the same ID already exists</exception>
        public int Create(BO.Task boTask)
        {
            // Check for null Task object
            if (boTask is null)
                throw new BO.BlArgumentNullException("Task is null");

            // Check for null Alias or Description
            if (boTask.Alias is null || boTask.Description is null)
                throw new BO.BlNullPropertyException("Alias or Description is null");

            // Check for invalid Task data
            if (boTask.Id < 0 || string.IsNullOrEmpty(boTask.Alias) || string.IsNullOrEmpty(boTask.Description))
                throw new BO.BlBadInputDataException("Missing ID, Alias, or Description");

            // Check if Engineer exists
            if (boTask.Engineer is not null)
            {
                IEnumerable<DO.Engineer?> engineers = _dal.Engineer.ReadAll(eng => eng.Id == boTask.Engineer.Id);
                if (engineers.Count() == 0)
                    throw new BO.BlDoesNotExistException("Engineer with ID=" + boTask.Engineer.Id + " does not exist");
            }

            // Check if the Task can be completed within the deadline and end date
            if (boTask.Deadline < boTask.ProjectedStartDate + boTask.RequiredEffortTime && boTask.ProjectedStartDate < _bl.Config.GetProjectEndDate() && boTask.ProjectedStartDate + boTask.RequiredEffortTime < _bl.Config.GetProjectEndDate())
                throw new BO.BlBadInputDataException("Task cannot be completed within the deadline");

            if (boTask.ProjectedStartDate < _bl.Clock)
                throw new BO.BlBadInputDataException("Task cannot have a projected start date in the past");


            if (boTask.Deadline < _bl.Clock)
                throw new BO.BlBadInputDataException("Task cannot have a deadline in the past");

            if (boTask.ProjectedStartDate < _bl.Config.GetProjectStartDate())
                throw new BO.BlBadInputDataException("Task cannot have a projected start date before the project start date");

            if (boTask.Deadline > _bl.Config.GetProjectEndDate())
                throw new BO.BlBadInputDataException("Task cannot have a deadline after the project end date");

            if (boTask.ProjectedStartDate > boTask.Deadline)
                throw new BO.BlBadInputDataException("Task cannot have a projected start date after the deadline");

            if (boTask.ProjectedStartDate > _bl.Config.GetProjectEndDate())
                throw new BO.BlBadInputDataException("Task cannot have a projected start date after the project end date");

            if (boTask.RequiredEffortTime < TimeSpan.Zero)
                throw new BO.BlBadInputDataException("Task cannot have a negative required effort time");

            if (boTask.Deadline > _bl.Config.GetProjectEndDate())
                throw new BO.BlBadInputDataException("Task cannot have a deadline after the project end date");

            if (boTask.Deadline < _bl.Config.GetProjectStartDate())
                throw new BO.BlBadInputDataException("Task cannot have a deadline before the project start date");

            try
            {

                // Create the Task in the data access layer
                DO.Task doTask = new DO.Task
                (
                    boTask.Id,
                    boTask.Alias ?? "",
                    _bl.Clock,
                    boTask.Description ?? "",
                    boTask.RequiredEffortTime,
                    boTask.Deadline,
                    boTask.ProjectedStartDate,
                    (DO.Enums.EngineerExperience?)boTask.Complexity,
                    boTask.Engineer?.Id,
                    boTask.ActualEndDate,
                    (boTask.Milestone != null),
                    boTask.ActualStartDate,
                    boTask.Deliverable,
                    boTask.Remarks
                );

                

                // Check and create dependencies if any
                if (boTask.Dependencies is not null && boTask.Dependencies.Count() > 0)
                {
                    foreach (BO.TaskInList dep in boTask.Dependencies)
                    {
                        if (_dal.Task.Read(dep.Id) is null)
                            throw new BO.BlBadInputDataException($"Task with ID={dep.Id} does not exist");

                        //if (IsCircularDep(dep.Id, boTask.Id, 40))
                        //    throw new BO.BlBadInputDataException($"Task with ID={dep.Id} has a circular dependency");
                    }

                    foreach (BO.TaskInList dep in boTask.Dependencies)
                    {
                        DO.Dependency doDep = new DO.Dependency
                        {
                            DependentTaskId = doTask.Id,
                            RequisiteID = dep.Id
                        };
                        _dal.Dependency.Create(doDep);
                    }
                }

                int idTask = _dal.Task.Create(doTask);
                return idTask;
            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.BlAlreadyExistsException($"Entity with ID={boTask.Id} already exists", ex);
            }
        }

        /// <summary>
        /// Delete a Task by ID
        /// </summary>
        /// <param name="id">The ID of the Task to delete</param>
        /// <exception cref="BO.BlBadInputDataException">Thrown when the Task is a requisite ID for some dependencies and therefore cannot be deleted</exception>
        /// <exception cref="BO.BlDoesNotExistException">Thrown when the Task with the provided ID does not exist</exception>
        public void Delete(int id)
        {
            // Check if the Task is a requisite ID for some dependencies and therefore cannot be deleted
            IEnumerable<DO.Dependency?> dependencies = _dal.Dependency.ReadAll(dep => dep.RequisiteID == id);
            if (dependencies.Count() > 0)
            {
                //throw new BO.BlBadInputDataException("Task with ID=" + id + " is a requisite ID for some dependencies and therefore cannot be deleted");


                //delete dependencies with this task as the requisite ID
                for (int i = 0; i < dependencies.Count(); i++)
                {
                    _dal.Dependency.Delete((int)dependencies.ElementAt(i)!.Id);
                }
            }
            // Check if the Task is a requisite ID for some dependencies and therefore cannot be deleted
            IEnumerable<DO.Dependency?> dependencies2 = _dal.Dependency.ReadAll(dep => dep.DependentTaskId == id);
            if (dependencies2.Count() > 0)
            {
                //throw new BO.BlBadInputDataException("Task with ID=" + id + " is a requisite ID for some dependencies and therefore cannot be deleted");


                //delete dependencies with this task as the requisite ID
                for (int i = 0; i < dependencies2.Count(); i++)
                {
                    _dal.Dependency.Delete((int)dependencies2.ElementAt(i)!.Id);
                }
            }


            try
            {
                // Delete the Task from the data access layer
                _dal.Task.Delete(id);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Task with ID={id} does not exist", ex);
            }
        }

        /// <summary>
        /// Read a Task by ID
        /// </summary>
        /// <param name="id">The ID of the Task to read</param>
        /// <returns>The Task object if found, otherwise null</returns>
        /// <exception cref="BO.BlBadInputDataException">Thrown when the provided ID is negative</exception>
        /// <exception cref="BO.BlDoesNotExistException">Thrown when the Task with the provided ID does not exist</exception>
        public BO.Task? Read(int id)
        {
            // Check if the provided ID is negative
            if (id < 0)
                throw new BO.BlBadInputDataException("ID must be positive");

            // Read Task from the data access layer
            DO.Task? doTask = _dal.Task.Read(id);

            // Throw exception if Task does not exist
            if (doTask is null)
                throw new BO.BlDoesNotExistException($"Task with ID={id} does not exist");

            // Calculate status and projected start date
            BO.Enums.Status stat = classTools.StatusCalculator(doTask);
            DateTime? projectedStart = doTask.Deadline - doTask.Duration;

            return new BO.Task()
            {
                Id = doTask.Id,
                Alias = doTask.Alias,
                DateCreated = doTask.DateCreated,
                Description = doTask.Description,
                Status = stat,
                Dependencies = classTools.DependenciesCalculator(doTask, _dal),
                RequiredEffortTime = doTask.Duration,
                ActualStartDate = doTask.ActualStartDate,
                ProjectedStartDate = projectedStart,
                Deadline = doTask.Deadline,
                ActualEndDate = doTask.ActualEndDate,
                Deliverable = doTask.Deliverable,
                Remarks = doTask.Notes,
                Complexity = (BO.Enums.EngineerExperience?)doTask.DegreeOfDifficulty
            };
        }

        /// <summary>
        /// Read all Tasks with optional filtering
        /// </summary>
        /// <param name="filter">Optional filtering function</param>
        /// <returns>A collection of TaskInList objects</returns>
        public IEnumerable<BO.TaskInList> ReadAll(Func<BO.Task, bool>? filter = null)
        {
            IEnumerable<BO.TaskInList> tasks;

            if (filter != null)
            {
                // Read filtered Tasks from the data access layer
                tasks = from DO.Task doTask in _dal.Task.ReadAll()
                        where filter(Read(doTask.Id)!) && !doTask.IsMilestone
                        select new BO.TaskInList
                        {
                            Id = doTask.Id,
                            Description = doTask.Description,
                            Alias = doTask.Alias,
                            Status = Read(doTask.Id)?.Status ?? BO.Enums.Status.None
                        };
            }
            else
            {
                // Read all Tasks from the data access layer
                tasks = from DO.Task doTask in _dal.Task.ReadAll()
                        where !doTask.IsMilestone
                        select new BO.TaskInList
                        {
                            Id = doTask.Id,
                            Description = doTask.Description,
                            Alias = doTask.Alias,
                            Status = Read(doTask.Id)?.Status ?? BO.Enums.Status.None
                        };
            }
            return tasks;
        }

        /// <summary>
        /// Update a Task
        /// </summary>
        /// <param name="boTask">The Task object to update</param>
        /// <returns>The updated Task object</returns>
        /// <exception cref="BO.BlArgumentNullException">Thrown when the provided Task object is null</exception>
        /// <exception cref="BO.BlBadInputDataException">Thrown when the Task with the provided ID does not exist or has no name</exception>
        /// <exception cref="BO.BlAlreadyExistsException">Thrown when a Task with the same ID already exists</exception>
        public BO.Task Update(BO.Task? boTask)
        {
            // Check for null Task object
            if (boTask is null)
                throw new BO.BlArgumentNullException("Task is null");

            // Update Task in the data access layer
            DO.Task? doTask = _dal.Task.Read(boTask.Id);
            if (doTask == null)
                throw new BO.BlBadInputDataException("Task with ID=" + boTask.Id + " does not exist ");

            // Check if Task has a name
            if (string.IsNullOrEmpty(doTask.Alias))
                throw new BO.BlBadInputDataException("Task with ID=" + boTask.Id + " has no name ");

            //reassing engineer to the new task
            IEnumerable<DO.Task?> tasks = _dal.Task.ReadAll();
            foreach (DO.Task? task in tasks)
            {
                if (task?.AssignedEngineerId == boTask.Engineer?.Id)
                {
                    //removes the engineer ID from the old task and let's it be placed in the new task
                    DO.Task updatedTask = new DO.Task { Id = task!.Id, Alias = task!.Alias, Description = task!.Description, Deadline = task!.Deadline, AssignedEngineerId = null, 
                        DateCreated = task!.DateCreated, ActualEndDate = task!.ActualEndDate, ActualStartDate = task!.ActualStartDate, DegreeOfDifficulty = task!.DegreeOfDifficulty, 
                        Deliverable = task!.Deliverable, Duration = task!.Duration, Inactive = task!.Inactive, IsMilestone = task!.IsMilestone, Notes = task!.Notes, ProjectedStartDate = task!.ProjectedStartDate };
                    _dal.Task.Update(updatedTask);

                }
            }

            // Check if the Task is a milestone
            bool hasMilestone = (boTask.Milestone is not null);

            try
            {
                doTask = new DO.Task
                (
                    boTask.Id,
                    boTask.Alias ?? "",
                    boTask.DateCreated,
                    boTask.Description ?? "",
                    boTask.RequiredEffortTime,
                    boTask.Deadline,
                    doTask.ProjectedStartDate,
                    (DO.Enums.EngineerExperience?)boTask.Complexity,
                    boTask.Engineer?.Id,
                    boTask.ActualEndDate,
                    hasMilestone,
                    boTask.ActualStartDate,
                    boTask.Deliverable,
                    boTask.Remarks
                );
                _dal.Task.Update(doTask);
            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.BlAlreadyExistsException($"Task with ID={boTask!.Id} already exists", ex);
            }
            return boTask!;
        }

        /// <summary>
        /// Update the projected start date of a Task
        /// </summary>
        /// <param name="id">The ID of the Task</param>
        /// <param name="newDateTime">The new projected start date</param>
        /// <exception cref="BO.BlDoesNotExistException">Thrown when the Task with the provided ID does not exist</exception>
        /// <exception cref="BO.BlBadInputDataException">Thrown when the projected start date is later than the deadline of the dependent task</exception>
        public void UpdateProjectedStartDate(int id, DateTime? newDateTime)
        {
            // Read Task from the data access layer
            DO.Task doTask = _dal.Task.Read(id)!;

            // Throw exception if Task does not exist
            if (doTask is null)
                throw new BO.BlDoesNotExistException("Task with ID=" + id + " does not exist");

            // Create updated Task object
            DO.Task newDoTask = new DO.Task
            (
                doTask.Id,
                doTask.Alias ?? "",
                doTask.DateCreated,
                doTask.Description ?? "",
                doTask.Duration,
                doTask.Deadline,
                newDateTime,
                doTask.DegreeOfDifficulty,
                doTask.AssignedEngineerId,
                doTask.ActualEndDate,
                doTask.IsMilestone,
                doTask.ActualStartDate,
                doTask.Deliverable,
                doTask.Notes
            );

            // Check if the newDateTime is before the deadline of the dependent task
            IEnumerable<DO.Dependency?> dependencies = _dal.Dependency.ReadAll(dep => dep.DependentTaskId == newDoTask.Id);
            if (dependencies.Count() == 0)
            {
                _dal.Task.Update(newDoTask);
            }
            else
            {
                foreach (DO.Dependency? dep in dependencies)
                {
                    if (newDateTime <= _dal.Task.Read((int)dep!.RequisiteID!)!.Deadline)
                        throw new BO.BlBadInputDataException("Projected start date cannot be later than the deadline of the dependent task");
                }
                _dal.Task.Update(newDoTask);
            }
        }

        /// <summary>
        /// Recursive method to check circular dependencies
        /// </summary>
        /// <param name="depId">ID of the dependent task</param>
        /// <param name="reqId">ID of the requisite task</param>
        /// <param name="n">Maximum depth of recursion</param>
        /// <returns>
        /// if a circular dependency exists, otherwise false</returns>
        private bool IsCircularDep(int depId, int reqId, int n)
        {
            if (n == 0)
                return false;

            // Read requisite Task
            BO.Task? boTask = Read(reqId);

            // Throw exception if requisite Task does not exist
            if (boTask is null)
                throw new BO.BlDoesNotExistException("Task with ID=" + reqId + " does not exist");

            // Check dependencies of the requisite Task
            IEnumerable<DO.Dependency?> dependencies = _dal.Dependency.ReadAll(dep => dep.DependentTaskId == boTask.Id);

            if (dependencies == null)
                return false;

            // Check for circular dependencies
            foreach (DO.Dependency? dep in dependencies)
            {
                if (dep != null && dep.Id == depId)
                    return true;
            }

            foreach (DO.Dependency? dep in dependencies)
            {
                if (IsCircularDep(depId, (int)dep!.DependentTaskId!, n - 1))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Find all dependants of a task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<int> findDependants(int id)
        {
            var deps = _dal.Dependency.ReadAll();
            
            List<int> res = new List<int>();
            
            foreach (var dep in deps) {
                if (dep.RequisiteID == id)
                    res.Add((int)dep.DependentTaskId);
            }

            return res;
        }

        public void finishTask(int engID, int taskId)
        {
            BO.Task task = Read(taskId);

            task.Engineer = null;
            task.Status = BO.Enums.Status.Done;
            task.ActualEndDate = DateTime.Now;

            Update(task);
        }

        public void assignEng(int engId, int taskId)
        {
            BO.Task task = Read(taskId);

            DO.Task dTask = _dal.Task.Read(taskId);

            if (dTask == null) throw new DalDoesNotExistException("task does not exist");

            _dal.Task.Update(new DO.Task(
                Id: dTask.Id,
                Alias: dTask.Alias,
                DateCreated: dTask.DateCreated,
                Description: dTask.Description,
                Duration: dTask.Duration,
                Deadline: dTask.Deadline,
                ProjectedStartDate: dTask.ProjectedStartDate,
                DegreeOfDifficulty: dTask.DegreeOfDifficulty,
                AssignedEngineerId: engId,
                ActualEndDate: dTask.ActualEndDate,
                IsMilestone: dTask.IsMilestone,
                ActualStartDate: dTask.ActualStartDate,
                Deliverable: dTask.Deliverable,
                Notes: dTask.Notes,
                Inactive: dTask.Inactive
            ));

        }
    }
}
