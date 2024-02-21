using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

internal class EngineerExperienceCollection : System.Collections.IEnumerable
{
    static readonly IEnumerable<BO.Enums.EngineerExperience> e_enums = (Enum.GetValues(typeof(BO.Enums.EngineerExperience)) as IEnumerable<BO.Enums.EngineerExperience>)!;
    public System.Collections.IEnumerator GetEnumerator() => e_enums.GetEnumerator();
}

internal class StatusCollection : System.Collections.IEnumerable
{
    static readonly IEnumerable<BO.Enums.Status> s_enums = (Enum.GetValues(typeof(BO.Enums.Status)) as IEnumerable<BO.Enums.Status>)!;
    public System.Collections.IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
