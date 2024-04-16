using F2.Application.BerthLock.Dtos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.BerthLock
{
    public interface IBerthLockService
    {
        BerthLockRespose LockDataReceive(BerthLockRequest dto);
        BerthLockRespose LockDownDataReceive(BerthLockRequest dto);
        BerthLockRespose LockBeatDataReceive(BerthLockRequest dto);
    }
}
