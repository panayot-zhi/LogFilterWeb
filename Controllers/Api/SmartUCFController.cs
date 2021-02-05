using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using LogFilterWeb.Models.Cookie;
using LogFilterWeb.Models.Domain;
using LogFilterWeb.Services;
using LogFilterWeb.Utility;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LogFilterWeb.Controllers.Api
{
    [ApiController]
    [Route("api/smartUCF/[action]")]
    public class SmartUCFController : ControllerBase
    {
        [HttpGet]
        public dynamic GetStopwatchListsData(string listName)
        {
            dynamic meta = new ExpandoObject();
            
            var cookieData = this.ReadCookie<SmartUCF>(SmartUCF.CookieName);
            var data = SmartUCFService.GetStopwatchRecordsForRange(cookieData, ref meta);
            var query = data.AsParallel();

            if (!string.IsNullOrEmpty(listName))
            {
                query = query.Where(x => x.ListName == listName);
            }

            query = query.Where(x => cookieData.MonitoredServers.Contains(x.MachineName));

            return new
            {
                Meta = meta,
                Results = query.GroupBy(keySelector: record => record.ListName,
                    resultSelector: (list, groupByList) =>
                    {
                        // NOTE: Enumerate to array to avoid multiple enumerations
                        var byListRecords = groupByList as StopwatchRecord[] ?? groupByList.ToArray();

                        var serverDataList = new List<dynamic>();
                        foreach (var machineName in cookieData.MonitoredServers)
                        {
                            serverDataList.Add(new
                            {
                                Name = machineName,
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

                            Servers = serverDataList.OrderBy(x => x.Name)

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

        [HttpGet]
        public dynamic GetStopwatchListDataByDay(string listName)
        {
            if (string.IsNullOrWhiteSpace(listName))
            {
                throw new ArgumentNullException(nameof(listName));
            }

            dynamic meta = new ExpandoObject();
            
            var cookieData = this.ReadCookie<SmartUCF>(SmartUCF.CookieName);
            var data = SmartUCFService.GetStopwatchRecordsForRange(cookieData, ref meta);
            var query = data.AsParallel();

            query = query.Where(x => x.ListName == listName);
            query = query.Where(x => cookieData.MonitoredServers.Contains(x.MachineName));

            return new
            {
                Meta = meta,
                Results = query.GroupBy(keySelector: x => x.MachineName,
                    resultSelector: (machine, groupByMachine) =>
                    {
                        var groupByMachineArray = groupByMachine as StopwatchRecord[] ?? groupByMachine.ToArray();

                        var result = new List<dynamic>();
                        foreach (var day in Extensions.EachDay(cookieData.StartDate, cookieData.EndDate))
                        {
                            var dailyRecords = groupByMachineArray.Where(x => x.Date == day).ToArray();

                            if (dailyRecords.Length > 0)
                            {
                                result.Add(new
                                {
                                    Date = day,
                                    TotalRecords = dailyRecords.Length,
                                    TotalRowsRetrieved = dailyRecords.Sum(x => x.NumberOfRows),
                                    MaxRowsRetrieved = dailyRecords.Max(x => x.NumberOfRows),
                                    AvgRowsRetrieved = dailyRecords.Average(x => x.NumberOfRows),
                                    MaxRetrieveTime = dailyRecords.Max(x => x.RetrieveMilliseconds),
                                    AvgRetrieveTime = dailyRecords.Average(x => x.RetrieveMilliseconds),
                                });
                            }
                            else
                            {
                                result.Add(new
                                {
                                    Date = day,
                                    TotalRecords = 0,
                                    TotalRowsRetrieved = 0,
                                    MaxRowsRetrieved = 0,
                                    MaxRetrieveTime = 0,
                                    AvgRetrieveTime = 0
                                });
                            }
                        }

                        return new
                        {
                            Name = machine,
                            Records = result
                        };

                    }).OrderByDescending(x => x.Name)
            };
        }


        [HttpGet]
        public dynamic GetStopwatchServerDataByDay(string listName)
        {
            if (string.IsNullOrWhiteSpace(listName))
            {
                throw new ArgumentNullException(nameof(listName));
            }

            dynamic meta = new ExpandoObject();

            var cookieData = this.ReadCookie<SmartUCF>(SmartUCF.CookieName);
            var data = SmartUCFService.GetStopwatchRecordsForRange(cookieData, ref meta);
            var query = data.AsParallel();

            query = query.Where(x => x.ListName == listName);
            query = query.Where(x => cookieData.MonitoredServers.Contains(x.MachineName));

            return new
            {
                Meta = meta,
                Results = query.GroupBy(keySelector: x => x.MachineName,
                    resultSelector: (machine, groupByMachine) =>
                    {
                        var groupByMachineArray = groupByMachine as StopwatchRecord[] ?? groupByMachine.ToArray();

                        var result = new List<dynamic>();
                        foreach (var day in Extensions.EachDay(cookieData.StartDate, cookieData.EndDate))
                        {
                            var dailyRecords = groupByMachineArray.Where(x => x.Date == day).ToArray();

                            if (dailyRecords.Length > 0)
                            {
                                result.Add(new
                                {
                                    Date = day,
                                    TotalRecords = dailyRecords.Length,
                                    TotalRowsRetrieved = dailyRecords.Sum(x => x.NumberOfRows),
                                    MaxRowsRetrieved = dailyRecords.Max(x => x.NumberOfRows),
                                    AvgRowsRetrieved = dailyRecords.Average(x => x.NumberOfRows),
                                    MaxRetrieveTime = dailyRecords.Max(x => x.RetrieveMilliseconds),
                                    AvgRetrieveTime = dailyRecords.Average(x => x.RetrieveMilliseconds),
                                });
                            }
                            else
                            {
                                result.Add(new
                                {
                                    Date = day,
                                    TotalRecords = 0,
                                    TotalRowsRetrieved = 0,
                                    MaxRowsRetrieved = 0,
                                    MaxRetrieveTime = 0,
                                    AvgRetrieveTime = 0
                                });
                            }
                        }

                        return new
                        {
                            Name = machine,
                            Records = result
                        };

                    }).OrderByDescending(x => x.Name)
            };
        }
    }
}
