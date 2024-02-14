

using BlApi;

namespace BlImplementation;

internal class EngineerImplementation : IEngineer
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    public int Create(BO.Engineer item)
    {
        throw new NotImplementedException();
    }
    public void Delete(int id)
    {
        throw new NotImplementedException();
    }
    public BO.Engineer Read(int id)
    {
        throw new NotImplementedException();
    }
    public IEnumerable<BO.Engineer> ReadAll()
    {
        throw new NotImplementedException();
    }
    public void Update(BO.Engineer item)
    {
        throw new NotImplementedException();
    }
}
