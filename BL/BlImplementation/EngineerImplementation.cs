

using BlApi;
using DO;
using System.Text.RegularExpressions;

namespace BlImplementation;

internal class EngineerImplementation : IEngineer
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    public int Create(BO.Engineer e)
    {
        if (e.Id < 0) throw new Exception("ID provided was invalid");
        if (e.Name == null || e.Name == "") throw new Exception("Name provided was invalid");
        if (e.CostPerHour < 0) throw new Exception("Cost per hour provided was invalid");
        if ( !isEmail(e.EmailAddress)) throw new Exception("Email provided was invalid");

        DO.Engineer doEngineer = new DO.Engineer(
            e.Id,
            e.Name,
            e.EmailAddress!,
            e.CostPerHour,
            (DO.Enums.EngineerExperience?)e?.ExperienceLevel
            
        ); 
        try
        {
            int idEng = _dal.Engineer.Create(doEngineer);
            return idEng;
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException(
                    $"Engineer with ID={e.Id} already exists", ex
                );
        }
        
    }
    public void Delete(int id)
    {
        if (_dal.Task.ReadAll(task => task.AssignedEngineerId == id).Count() > 0) 
            throw new Exception("Cannot delete engineer that was assaigned to a task");

        try
        {
            _dal.Engineer.Delete(id);
        }
        catch(DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Engineer with ID={id} does not exist!", ex);
        }
    }
    public BO.Engineer? Read(int id)
    {
        DO.Engineer? doEngineer = _dal.Engineer.Read(id);
        if (doEngineer == null) 
            throw new BO.BlDoesNotExistException($"Engineer with ID={id} does not exist!");


        IEnumerable<DO.Task?> tasks = _dal.Task.ReadAll(task => task.AssignedEngineerId == id);
        if (tasks.Count() > 1) throw new Exception("multiple tasks assigned to engineer"); // might add something to handle this
        
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

    public IEnumerable<BO.Engineer> ReadAll(Func<BO.Engineer, bool>? filter = null)
    {
        IEnumerable<BO.Engineer> engineers;
        
        if (filter == null)
        {
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
    public void Update(BO.Engineer e)
    {
        if (e.Id < 0) 
            throw new Exception("ID provided was invalid");
        if (e.Name == null || e.Name == "") 
            throw new Exception("Name provided was invalid");
        if (e.CostPerHour < 0) 
            throw new Exception("Cost per hour provided was invalid");
        if (!isEmail(e.EmailAddress)) 
            throw new Exception("Email provided was invalid");

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

    private bool isEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        return Regex.IsMatch(email, emailPattern);
    } 
}
