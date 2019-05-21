﻿using System;
using System.Collections.Generic;
using ThingAppraiser.Data;
using ThingAppraiser.Data.Models;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal class ThingSupplier : IThingSupplier, ITagable
    {
        private readonly IThingGrader _thingGrader;

        private readonly List<Thing> _things = new List<Thing>();

        public string StorageName { get; private set; }

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "ThingSupplier";

        #endregion


        public ThingSupplier(IThingGrader thingGrader)
        {
            _thingGrader = thingGrader.ThrowIfNull(nameof(thingGrader));
        }

        #region IThingSupplier Implementation

        public List<Thing> GetAllThings()
        {
            return _things;
        }

        public Thing GetThingById(Guid thingId)
        {
            return _things.Find(p => p.InternalId.Equals(thingId));
        }

        #endregion

        public bool SaveResults(ProcessingResponse response, string storageName)
        {
            StorageName = storageName;

            _thingGrader.ProcessMetaData(response.MetaData);

            if (_things.Count > 0)
            {
                _things.Clear();
            }
            foreach (List<RatingDataContainer> rating in response.RatingDataContainers)
            {
                _things.AddRange(_thingGrader.ProcessRatings(rating));
            }
            return true;
        }
    }
}