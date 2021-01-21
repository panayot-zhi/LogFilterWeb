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
            // TODO: Resolve 'from' and 'to' from cookie here
            var to = FilesHelper.ToDateTime("2019-11-22");
            var from = to;

            var data = SmartUCFService.GetStopwatchRecordsForRange(from, to);

            var query = data.AsParallel();

            if (!string.IsNullOrEmpty(listName))
            {
                query = query.Where(x => x.ListName == listName);
            }

            return query.GroupBy(keySelector: record => record.ListName,
                resultSelector: (key, records) =>
                {
                    // NOTE: Enumerate to array to avoid multiple enumerations
                    var stopwatchRecords = records as StopwatchRecord[] ?? records.ToArray();

                    return new
                    {
                        ListName = key,
                        TotalRecords = stopwatchRecords.Length,
                        TotalRowsRetrieved = stopwatchRecords.Sum(x => x.NumberOfRows),
                        MaxRowsRetrieved = stopwatchRecords.Max(x => x.NumberOfRows),
                        AvgRowsRetrieved = stopwatchRecords.Average(x => x.NumberOfRows),
                        MaxRetrieveTime = stopwatchRecords.Max(x => x.RetrieveMilliseconds),
                        AvgRetrieveTime = stopwatchRecords.Average(x => x.RetrieveMilliseconds)
                    };

                }).OrderByDescending(x => x.TotalRowsRetrieved);
        }

        [HttpGet]
        [Route("api/smartUCF/stopwatchServerDataByDay")]
        public dynamic GetStopwatchServerDataByDay(string serverName)
        {
            // TODO: Resolve 'from' and 'to' from cookie here
            var to = FilesHelper.ToDateTime("2019-11-22");
            var from = to.AddDays(-7);

            var data = SmartUCFService.GetStopwatchRecordsForRange(from, to);

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
        }
    }
}
