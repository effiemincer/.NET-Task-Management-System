using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DO;


//<param name = "Id" > unique ID(created automatically)</param>

//<param name = "Nickname"></param>
//<param name = "Description"></param>
//<param name = "Is it a milestone?">(T/F)</param>
//<param name = "Date created"></param>
//<param name = "Projected start Date"></param>
//<param name = "Actual start Date"></param>
//<param name = "Duration"></param>
//<param name = "Deadline"></param>
//<param name = "Actual end Date"></param>
//<param name = "Deliverable"></param>
//<param name = "Notes"></param>
//<param name = "Assigned engineer ID"></param>
//<param name = "Degree of difficulty"></param>
public record Task
(
    int ID
)
{

}
