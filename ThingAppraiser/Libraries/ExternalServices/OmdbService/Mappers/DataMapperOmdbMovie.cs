﻿using System;
using System.Globalization;
using System.Linq;
using OMDbApiNet.Model;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers.Omdb
{
    public sealed class DataMapperOmdbMovie : IDataMapper<Item, OmdbMovieInfo>
    {
        public DataMapperOmdbMovie()
        {
        }

        #region IDataMapper<SearchMovie, OmdbMovieInfo> Implementation

        public OmdbMovieInfo Transform(Item dataObject)
        {
            var thingId = int.Parse(dataObject.ImdbId.Substring(2));
            var voteCount = int.Parse(dataObject.ImdbVotes, NumberStyles.AllowThousands);
            var voteAverage = double.Parse(dataObject.ImdbRating);
            var releaseDate = DateTime.Parse(dataObject.Released);
            var metascore = dataObject.Metascore.IsEqualWithInvariantCulture("N/A")
                ? 0
                : int.Parse(dataObject.Metascore);
            var genreIds = dataObject.Genre.Split(',').Select(genre => genre.Trim()).ToList();

            return new OmdbMovieInfo(
                thingId:     thingId,
                title:       dataObject.Title,
                voteCount:   voteCount,
                voteAverage: voteAverage,
                overview:    dataObject.Plot,
                releaseDate: releaseDate,
                metascore:   metascore,
                rated:       dataObject.Rated,
                genreIds:    genreIds,
                posterPath:  dataObject.Poster
            );
        }

        #endregion
    }
}