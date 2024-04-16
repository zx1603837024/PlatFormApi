using F2.Application.Inspectors.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Inspectors
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInspectorAppService
    {
        List<BerthDto> GetBerthList(int berthsecId);
    }
}
