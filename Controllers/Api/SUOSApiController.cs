using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using LogFilterWeb.Models.Cookie;
using LogFilterWeb.Models.Domain;
using LogFilterWeb.Services;
using LogFilterWeb.Utility;

namespace LogFilterWeb.Controllers.Api
{
    [ApiController]
    [Route("api/suos/[action]")]
    public class SUOSApiController : ControllerBase
    {
        [HttpGet]
        public dynamic GetSummariesData(string config = Constants.SUOSDefaultConfig)
        {
            dynamic meta = new ExpandoObject();

            var cookieData = this.ReadCookie<SUOS>(SUOS.CookieName);
            var data = SUOSService.GetSummaryRecords(cookieData, config, ref meta);
            var query = data.AsParallel();

            return new
            {
                Meta = meta,
                Results = query.GroupBy(keySelector: x => x.MachineName, 
                    resultSelector: (machineName, groupByMachine) =>
                    {
                        return new
                        {
                            Machine = machineName,
                            Filters = FilesHelper.Combine(groupByMachine).Filters
                        };

                    }).OrderBy(x => x.Machine)
            };
        }

        [HttpGet]
        public dynamic GetUsersData(string serviceName)
        {
            dynamic meta = new ExpandoObject();

            var cookieData = this.ReadCookie<SUOS>(SUOS.CookieName);
            var fullServiceName = serviceName?.ToLowerInvariant() + "requests";
            var data = SUOSService.GetUserQueryData(cookieData, fullServiceName, ref meta);
            var query = data.AsParallel();

            return new
            {
                Meta = meta,
                Results = query.GroupBy(keySelector: x => x.MachineName,
                    resultSelector: (machineName, groupByServer) =>
                    {
                        return new
                        {
                            Machine = machineName,
                            Records = FilesHelper.Combine(groupByServer).Records
                                .OrderByDescending(x => x.Count)
                        };

                    }).OrderBy(x => x.Machine)
            };
        }

        [HttpGet]
        public dynamic GetUserDataByDay(string serviceName, string user)
        {
            dynamic meta = new ExpandoObject();

            var cookieData = this.ReadCookie<SUOS>(SUOS.CookieName);
            var fullServiceName = serviceName?.ToLowerInvariant() + "requests";
            var data = SUOSService.GetUserQueryData(cookieData, fullServiceName, ref meta, user);
            var query = data.AsParallel();

            return new
            {
                Meta = meta,
                Results = query.GroupBy(keySelector: x => x.MachineName,
                    resultSelector: (machine, groupByMachine) =>
                    {
                        var groupByMachineArray = groupByMachine as UserQueryFile[] ?? groupByMachine.ToArray();

                        var result = new List<dynamic>();
                        foreach (var day in Extensions.EachDay(cookieData.StartDate, cookieData.EndDate))
                        {
                            if (groupByMachineArray
                                .SingleOrDefault(x => x.Date == day)?
                                .Records
                                .SingleOrDefault() is UserQueryRecordExtended dailyUserRecord)
                            {
                                result.Add(new
                                {
                                    Date = day,
                                    dailyUserRecord.User,
                                    dailyUserRecord.Queries,
                                    dailyUserRecord.Count
                                });
                            }
                            else
                            {
                                result.Add(new
                                {
                                    Date = day,
                                    User = user,
                                    Count = 0
                                });
                            }
                        }

                        return new
                        {
                            Name = machine,
                            Records = result
                        };
                    })
            };
        }
    }
}
