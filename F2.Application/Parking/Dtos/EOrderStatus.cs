using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    public enum EOrderStatus
    {
        未知状态 = 0,
        停车中 = 1,
        已出单 = 2,
        部分支付 = 3,
        已完成 = 4,
        逃逸 = 5,
        欠费出场 = 6
    }

    public enum EDeviceType
    {
        未知 = 0,
        PDA = 1,
        POS机 = 2,
        地磁 = 3,
        抓拍机 = 4
    }

    public enum ECarType
    {
        包月车=1,
        临时车=2
    }
}
