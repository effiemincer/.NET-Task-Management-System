using BlApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlImplementation;

/// <summary>
/// EngineerImplementation class implements IEngineer interface and is responsible for the logic of the Engineer entity
/// </summary>
internal class EngineerImplementation : IEngineer
{
    /// <summary>
    /// Dal instance to be used by the class
    /// </summary>
    private DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Create a new Engineer
    /// </summary>
    /// <param name="e">The Engineer object to create</param>
    /// <returns>The ID of the newly created Engineer</returns>
    /// <exception cref="Exception">Thrown when invalid parameters are provided</exception>
    /// <exception cref="BO.BlAlreadyExistsException">Thrown when an Engineer with the same ID already exists</exception>
    public int Create(BO.Engineer e)
    {
        // Check if the provided Engineer object contains valid data
        if (e.Id < 0) throw new Exception("ID provided was invalid");
        if (string.IsNullOrEmpty(e.Name)) throw new Exception("Name provided was invalid");
        if (e.CostPerHour < 0) throw new Exception("Cost per hour provided was invalid");
        if (!IsEmail(e.EmailAddress)) throw new Exception("Email provided was invalid");

        // Map BO.Engineer to DO.Engineer and create a new DO.Engineer object
        DO.Engineer doEngineer = new DO.Engineer(
            e.Id,
            e.Name,
            e.EmailAddress!,
            e.CostPerHour,
            (DO.Enums.EngineerExperience?)e?.ExperienceLevel
        );
        try
        {
            // Try to create the Engineer in the data access layer
            int idEng = _dal.Engineer.Create(doEngineer);
            return idEng;
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            // If Engineer already exists, throw a BO.BlAlreadyExistsException
            throw new BO.BlAlreadyExistsException(
                $"Engineer with ID={e.Id} already exists", ex
            );
        }

    }

    /// <summary>
    /// Delete an Engineer by ID
    /// </summary>
    /// <param name="id">The ID of the Engineer to delete</param>
    /// <exception cref="Exception">Thrown when attempting to delete an Engineer assigned to a task</exception>
    public void Delete(int id)
    {
        // Check if the Engineer is assigned to any task before deletion
        if (_dal.Task.ReadAll(task => task.AssignedEngineerId == id).Count() > 0)
            throw new Exception("Cannot delete engineer that was assigned to a task");

        try
        {
            // Try to delete the Engineer from the data access layer
            _dal.Engineer.Delete(id);
        }
        catch (DalDoesNotExistException ex)
        {
            // If Engineer does not exist, throw a BO.BlDoesNotExistException
            throw new BO.BlDoesNotExistException($"Engineer with ID={id} does not exist!", ex);
        }
    }

    /// <summary>
    /// Read an Engineer by ID
    /// </summary>
    /// <param name="id">The ID of the Engineer to read</param>
    /// <returns>The Engineer object if found, otherwise null</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the Engineer with the provided ID does not exist</exception>
    public BO.Engineer? Read(int id)
    {
        // Read Engineer from the data access layer
        DO.Engineer? doEngineer = _dal.Engineer.Read(id);
        if (doEngineer == null)
            throw new BO.BlDoesNotExistException($"Engineer with ID={id} does not exist!");

        // Check if Engineer is assigned to any task
        IEnumerable<DO.Task?> tasks = _dal.Task.ReadAll(task => task.AssignedEngineerId == id);
        if (tasks.Count() > 1) throw new Exception("Multiple tasks assigned to engineer");

        // Map DO.Engineer to BO.Engineer
        if (tasks.Count() == 0 || tasks == null) return new BO.Engineer()
        {
            Id = id,
            Name = doEngineer.FullName,
            EmailAddress = doEngineer.EmailAddress,
            ExperienceLevel = (BO.Enums.EngineerExperience?)doEngineer.ExperienceLevel,
            CostPerHour = doEngineer.CostPerHour,
            Task = null
        };
        else return new BO.Engineer()
        {
            Id = id,
            Name = doEngineer.FullName,
            EmailAddress = doEngineer.EmailAddress,
            ExperienceLevel = (BO.Enums.EngineerExperience?)doEngineer.ExperienceLevel,
            CostPerHour = doEngineer.CostPerHour,
            Task = new BO.TaskInEngineer(tasks.First()!.Id, tasks.First()!.Alias)
        };
    }

    /// <summary>
    /// Read all Engineers with optional filtering
    /// </summary>
    /// <param name="filter">Optional filtering function</param>
    /// <returns>A collection of Engineer objects</returns>
    public IEnumerable<BO.Engineer> ReadAll(Func<BO.Engineer, bool>? filter = null)
    {
        IEnumerable<BO.Engineer> engineers;

        if (filter == null)
        {
            // Read all Engineers and map to BO.Engineer
            engineers = from DO.Engineer e in _dal.Engineer.ReadAll()
                        select new BO.Engineer
                        {
                            Id = e.Id,
                            Name = e.FullName,
                            EmailAddress = e.EmailAddress,
                            ExperienceLevel = (BO.Enums.EngineerExperience?)e.ExperienceLevel,
                            CostPerHour = e.CostPerHour,
                            Task = new BO.TaskInEngineer(
                                _dal.Task.ReadAll(task => task.AssignedEngineerId == e.Id).Count() != 0 ? _dal.Task.ReadAll(task => task.AssignedEngineerId == e.Id).FirstOrDefault(task => task != null)!.Id : null,
                                _dal.Task.ReadAll(task => task.AssignedEngineerId == e.Id).Count() != 0 ? _dal.Task.ReadAll(task => task.AssignedEngineerId == e.Id).FirstOrDefault(task => task != null)!.Alias : null
                            ),
                        };
        }
        else
        {
            // Read Engineers with filtered condition and map to BO.Engineer
            engineers = from DO.Engineer e in _dal.Engineer.ReadAll()
                        where e != null && filter(Read(e.Id)!)
                        select new BO.Engineer
                        {
                            Id = e.Id,
                            Name = e.FullName,
                            EmailAddress = e.EmailAddress,
                            ExperienceLevel = (BO.Enums.EngineerExperience?)e.ExperienceLevel,
                            CostPerHour = e.CostPerHour,
                            Task = new BO.TaskInEngineer(
                                _dal.Task.ReadAll(task => task.AssignedEngineerId == e.Id).Count() != 0 ? _dal.Task.ReadAll(task => task.AssignedEngineerId == e.Id).FirstOrDefault(task => task != null)!.Id : null,
                                _dal.Task.ReadAll(task => task.AssignedEngineerId == e.Id).Count() != 0 ? _dal.Task.ReadAll(task => task.AssignedEngineerId == e.Id).FirstOrDefault(task => task != null)!.Alias : null
                            ),
                        };
        }

        return engineers;
    }

    /// <summary>
    /// Update an Engineer
    /// </summary>
    /// <param name="e">The Engineer object to update</param>
    /// <exception cref="Exception">Thrown when invalid parameters are provided</exception>
    public void Update(BO.Engineer e)
    {
        // Check if the provided Engineer object contains valid data
        if (e.Id < 0)
            throw new Exception("ID provided was invalid");
        if (string.IsNullOrEmpty(e.Name))
            throw new Exception("Name provided was invalid");
        if (e.CostPerHour < 0)
            throw new Exception("Cost per hour provided was invalid");
        if (!IsEmail(e.EmailAddress))
            throw new Exception("Email provided was invalid");

        // Map BO.Engineer to DO.Engineer and update in the data access layer
        _dal.Engineer.Update
        (
            new DO.Engineer
            (
                e.Id,
                e.Name,
                e.EmailAddress!,
                e.CostPerHour,
                (DO.Enums.EngineerExperience?)e?.ExperienceLevel
            )
        );
    }

    /// <summary>
    /// Validate an email address
    /// </summary>
    /// <param name="email">The email address to validate</param>
    /// <returns>True if the email is valid, otherwise false</returns>
    private bool IsEmail(string? email)
    {
        // Check if the email address matches the email pattern
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        return Regex.IsMatch(email, emailPattern);
    }

    public bool isEngineer(int id)
    {
        return _dal.Engineer.Read(id) != null;
    }
}
