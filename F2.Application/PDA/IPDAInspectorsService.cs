using F2.Application.PDA.Dtos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PDA
{
    public interface IPDAInspectorsService
    {
        AbpLoginResult InspectorsLogin(EmployeeLoginInput input, int tenantId);
        GetAllBerthsecListOutput GetBerthsecList(long inspectorsId, int tenantId);
        LoginModel LoginToken(LoginTokenInput input);
        PdaModel DownParameterForInspectors(string access_token); 
        int InspectorsOutLineLogout(string berthsecid, string access_token);
        Hashtable GetStopCarList(Hashtable input);
        Hashtable GetAllFeeList(Hashtable input);
        Hashtable GetArrearageData(Hashtable input);
        Hashtable GetInspectorsInfo(Hashtable input);
        Hashtable GetIBerthInfo(Hashtable input);
        Hashtable ModifyPassword(Hashtable input);
        Hashtable InsertTaskFeedbacks(string access_token, string TaskId, string BerthsecId, string BerthNumber, string TaskContent, byte[] PicUrl1, byte[] PicUrl2, byte[] PicUrl3);
        Hashtable InsertInspectorsEvent(string access_token, string BerthsecId, string BerthNumber, string EventContent, byte[] PicUrl);
        Hashtable GetInspectorTasks(Hashtable input);
        Hashtable GetInspectorEvent(Hashtable input);
        Hashtable GetInspectorTaskFeedbacks(Hashtable input);
        int UpdateInspectorGps(string x, string y, string access_token);
        Hashtable GetInspectorsTaskNum(Hashtable input);
        string CheckInspVersion(string OldVersion, string PDA, int Type, string access_token);
        Hashtable GetTypeCarDataList(Hashtable input);
        Hashtable GetTypeCarDataCount(Hashtable input);
    }
}
