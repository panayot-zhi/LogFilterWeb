using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LogFilterWeb.Models.Domain;
using LogFilterWeb.Services;
using LogFilterWeb.Utility;

namespace LogFilterWeb.Controllers.Api
{
    [ApiController]
    public class SmartUCFController : ControllerBase
    {
        [HttpGet]
        [Route("api/smartUCF/stopwatchListData")]
        public dynamic GetStopwatchListData(string listName)
        {
            var cookieData = this.ReadCookie<DateRange>(Constants.SmartUCFConfigCookieName);

            dynamic meta = new ExpandoObject();

            var data = SmartUCFService.GetStopwatchRecordsForRange(cookieData.StartDate.Date, cookieData.EndDate.Date, ref meta);
            var query = data.AsParallel();

            if (!string.IsNullOrEmpty(listName))
            {
                query = query.Where(x => x.ListName == listName);
            }

            return new
            {
                Meta = meta,
                Results = query.GroupBy(keySelector: record => record.ListName,
                    resultSelector: (list, groupByList) =>
                    {
                        // NOTE: Enumerate to array to avoid multiple enumerations
                        var byListRecords = groupByList as StopwatchRecord[] ?? groupByList.ToArray();

                        var serverDataList = new List<dynamic>();
                        foreach (var machineName in Constants.SmartUCFMachines)
                        {
                            serverDataList.Add(new
                            {
                                Server = machineName,
                                Retrieved = byListRecords.Where(x => x.MachineName == machineName)
                                    .Sum(x => x.NumberOfRows)
                            });
                        }

                        return new
                        {
                            Id = list,
                            TotalRecords = byListRecords.Length,
                            ListName = Constants.SmartUCFListDisplayName[list],
                            TotalRowsRetrieved = byListRecords.Sum(x => x.NumberOfRows),
                            MaxRowsRetrieved = byListRecords.Max(x => x.NumberOfRows),
                            AvgRowsRetrieved = byListRecords.Average(x => x.NumberOfRows),
                            MaxRetrieveTime = byListRecords.Max(x => x.RetrieveMilliseconds),
                            AvgRetrieveTime = byListRecords.Average(x => x.RetrieveMilliseconds),

                            Servers = serverDataList

                            /*Servers = byListRecords.GroupBy(keySelector: x => x.MachineName,
                                resultSelector: (machineName, groupByMachine) =>
                                {
                                    // NOTE: Enumerate to array to avoid multiple enumerations
                                    var byMachineRecords = groupByMachine as StopwatchRecord[] ?? groupByMachine.ToArray();
    
    
                                    return new
                                    {
                                        Server = machineName,
                                        Retrieved = byMachineRecords.Sum(x => x.NumberOfRows)
                                    };
    
                                })*/
                        };

                    }).OrderByDescending(x => x.TotalRowsRetrieved)
            };
        }

        /*[HttpGet]
        [Route("api/smartUCF/stopwatchServerDataByDay")]
        public dynamic GetStopwatchServerDataByDay(string serverName)
        {
            var cookieData = this.ReadCookie<DateRange>(Constants.SmartUCFConfigCookieName);

            dynamic meta = new ExpandoObject();
            
            var data = SmartUCFService.GetStopwatchRecordsForRange(cookieData.StartDate.Date, cookieData.EndDate.Date, ref meta);
            var query = data.AsParallel();

            if (!string.IsNullOrEmpty(serverName))
            {
                query = query.Where(x => x.MachineName == serverName);
            }

            return query.GroupBy(x => x.MachineName,
                (machineName, groupByMachine) =>
                {
                    var groupByMachineArray = groupByMachine as StopwatchRecord[] ?? groupByMachine.ToArray();

                    return new
                    {
                        Server = machineName,
                        Retrieved = groupByMachineArray.Sum(x => x.NumberOfRows),
                        Records = groupByMachineArray.GroupBy(x => x.Date, 
                            (date, groupByDate) =>
                            {
                                var groupByDateArray = groupByDate as StopwatchRecord[] ?? groupByDate.ToArray();

                                return new
                                {
                                    Date = date,
                                    Retrieved = groupByDateArray.Sum(x => x.NumberOfRows),
                                    Lists = groupByDateArray.GroupBy(x => x.ListName,
                                        (list, groupByList) =>
                                        {
                                            return new
                                            {
                                                List = list, 
                                                Retrieved = groupByList.Sum(x => x.NumberOfRows)
                                            };

                                        }).OrderByDescending(x => x.Retrieved)
                                };

                            }).OrderBy(x => x.Date)
                    };
                });
        }*/
    }
}
