using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MeetOL.Filters;

namespace MeetOL
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {         
            filters.Add(new TimerAttribute());
            filters.Add(new ExceptionFilterAttribute());     

        }
    }
}
