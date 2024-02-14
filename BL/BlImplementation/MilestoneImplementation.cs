
namespace BlImplementation;
using BlApi;
using BO;
using System.Collections.Generic;

internal class MilestoneImplementation : IMilestone
{

    private DalApi.IDal _dal = DalApi.Factory.Get;

    public int Create(Milestone item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Milestone Read(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<MilestoneInList> ReadAll()
    {
        throw new NotImplementedException();
    }

    public void Update(Milestone item)
    {
        throw new NotImplementedException();
    }
}
