using System;
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
        public dynamic GetSummariesData(string configName = Constants.SUOSDefaultConfig)
        {
            dynamic meta = new ExpandoObject();

            var cookieData = this.ReadCookie<SUOS>(SUOS.CookieName);
            var data = SUOSService.GetSummaryRecords(cookieData, configName, ref meta);
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
        public dynamic GetSummariesDataByDay(string configName, string filter)
        {
            if (string.IsNullOrEmpty(configName))
            {
                throw new ArgumentNullException(nameof(configName));
            }

            if (string.IsNullOrEmpty(filter))
            {
                throw new ArgumentNullException(nameof(filter));
            }

            dynamic meta = new ExpandoObject();

            var cookieData = this.ReadCookie<SUOS>(SUOS.CookieName);
            var data = SUOSService.GetSummaryRecords(cookieData, configName, ref meta, filter);
            var query = data.AsParallel();

            return new
            {
                Meta = meta,
                Results = query.GroupBy(keySelector: x => x.MachineName,
                    resultSelector: (machine, groupByMachine) =>
                    {
                        var groupByMachineArray = groupByMachine as SummaryFile[] ?? groupByMachine.ToArray();

                        var result = new List<dynamic>();
                        foreach (var day in Extensions.EachDay(cookieData.StartDate, cookieData.EndDate))
                        {
                            var dailyFilterRecord = groupByMachineArray
                                .SingleOrDefault(x => x.Date == day)?
                                .Filters
                                .SingleOrDefault(x => x.Name == filter);
                            
                            if (dailyFilterRecord != null)
                            {
                                result.Add(new
                                {
                                    Date = day,
                                    Name = dailyFilterRecord.Name,
                                    Type = dailyFilterRecord.Type,
                                    Value = dailyFilterRecord.Value,
                                    Count = dailyFilterRecord.Count
                                });
                            }
                            else
                            {
                                result.Add(new
                                {
                                    Date = day,
                                    Name = filter,
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
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (string.IsNullOrEmpty(user))
            {
                throw new ArgumentNullException(nameof(user));
            }

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
                                    User = dailyUserRecord.User,
                                    Queries = dailyUserRecord.Queries,
                                    Count = dailyUserRecord.Count
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
