﻿using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface ITrackingServices
    {
        public BuildTrackingLocationSQL BuildTrackingLocationSQL(List<Table> itableQuery, string ConnectionString, string sCurrentSQL, Table oTables);
        Task<bool> IsOutDestination(string oDestinationTable, string oDestinationId, string ConnectionString);
        Task<DateTime> GetDueBackDate(string oDestinationTable, string oDestinationId, string ConnectionString);
        Task PrepareDataForTransfer(string trackableType, string trackableID, string destinationType,
            string destinationID, DateTime DueBackDate, string userName, Passport passport, string trackingAdditionalField1 = null, string trackingAdditionalField2 = null);
    }
}
