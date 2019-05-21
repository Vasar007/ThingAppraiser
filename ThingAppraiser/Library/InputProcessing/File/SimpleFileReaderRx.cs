﻿using System.Collections.Generic;
using System.IO;
using CsvHelper;
using FileHelpers;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public class SimpleFileReaderRx : IFileReaderRx
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<SimpleFileReaderRx>();

        private readonly string _thingNameHeader = "Thing Name";


        public SimpleFileReaderRx(string thingNameHeader = "Thing Name")
        {
            _thingNameHeader = thingNameHeader.ThrowIfNullOrWhiteSpace(nameof(thingNameHeader));
        }

        #region IFileReaderAsync Implementation

        public IEnumerable<string> ReadFile(string filename)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();
            var engine = new FileHelperAsyncEngine<InputFileData>();
            using (engine.BeginReadFile(filename))
            {
                foreach (var record in engine)
                {
                    if (result.Add(record.thingName))
                    {
                        yield return record.thingName;
                    }
                }
            }
        }

        public IEnumerable<string> ReadCsvFile(string filename)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();
            using (var reader = new StreamReader(filename))
            {
                var csv = new CsvReader(
                    reader, new CsvHelper.Configuration.Configuration { HasHeaderRecord = true }
                );

                if (!csv.Read() || !csv.ReadHeader())
                {
                    var ex = new InvalidDataException("CSV file doesn't contain header!");
                    _logger.Error(ex, "Got CSV file without header.");
                    throw ex;
                }
                while (csv.Read())
                {
                    string thingName = csv[_thingNameHeader];
                    if (result.Add(thingName))
                    {
                        yield return thingName;
                    }
                }
            }
        }

        #endregion
    }
}