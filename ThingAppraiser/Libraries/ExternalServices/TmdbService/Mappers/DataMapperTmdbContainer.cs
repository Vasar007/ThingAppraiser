﻿using System.Linq;
using System.Collections.Generic;
using ThingAppraiser.TmdbService.Models;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.TmdbService.Mappers
{
    internal sealed class DataMapperTmdbContainer :
        IDataMapper<SearchContainer<SearchMovie>, TmdbSearchContainer>
    {
        private readonly DataMapperTmdbMovie _mapperTmdbMovie = new DataMapperTmdbMovie();


        public DataMapperTmdbContainer()
        {
        }

        #region IDataMapper<SearchContainer<SearchMovie>, TmdbSearchContainer> Implementation

        public TmdbSearchContainer Transform(SearchContainer<SearchMovie> dataObject)
        {
            List<TmdbMovieInfo> results = dataObject.Results
                .Select(tmdb => _mapperTmdbMovie.Transform(tmdb))
                .ToList();

            return new TmdbSearchContainer(
                page:         dataObject.Page,
                results:      results,
                totalPages:   dataObject.TotalPages,
                totalResults: dataObject.TotalResults
            );
        }

        #endregion
    }
}