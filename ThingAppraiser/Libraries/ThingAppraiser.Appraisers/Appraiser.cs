﻿using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Basic appraiser with default rating calculations. You should inherit this class if would 
    /// like to create your own appraiser with rating calculation.
    /// </summary>
    public abstract class Appraiser : AppraiserBase
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public override string Tag { get; } = nameof(Appraiser);

        #endregion

        /// <summary>
        /// Rating name which describes rating calculation.
        /// </summary>
        public override string RatingName { get; } = "Common rating";


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        protected Appraiser()
        {
        }

        /// <summary>
        /// Makes prior analysis through normalizers and calculates ratings based on average vote 
        /// and vote count.
        /// </summary>
        /// <param name="rawDataContainer">Entities to appraise with additional parameters.</param>
        /// <param name="outputResults">Flag to define need to output.</param>
        /// <returns>Collection of result object (data object with rating).</returns>
        /// <remarks>
        /// Entities collection must be unique because rating calculation errors can occur in such
        /// situations.
        /// </remarks>
        public virtual IReadOnlyList<ResultInfo> GetRatings(RawDataContainer rawDataContainer,
            bool outputResults)
        {
            rawDataContainer.ThrowIfNull(nameof(rawDataContainer));

            CheckRatingId();

            var ratings = new List<ResultInfo>();
            IReadOnlyList<BasicInfo> rawData = rawDataContainer.RawData;
            if (!rawData.Any()) return ratings;

            MinMaxDenominator voteCountMMD = rawDataContainer.GetParameter(
                 nameof(BasicInfo.VoteCount)
            );
            MinMaxDenominator voteAverageMMD = rawDataContainer.GetParameter(
                nameof(BasicInfo.VoteAverage)
            );

            foreach (BasicInfo entityInfo in rawData)
            {
                double ratingValue = CalculateRating(entityInfo, voteCountMMD, voteAverageMMD);

                var resultInfo = new ResultInfo(entityInfo.ThingId, ratingValue, RatingId);
                ratings.Add(resultInfo);

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage(resultInfo.ToString());
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }

        /// <summary>
        /// Calculates rating for <see cref="BasicInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <param name="voteCountMMD">Additional value to normalize vote count property.</param>
        /// <param name="voteAverageMMD">
        /// Additional value to normalize vote average property.
        /// </param>
        /// <returns>Normalized sum of vote count and vote average values.</returns>
        protected static double CalculateRating(BasicInfo entity, MinMaxDenominator voteCountMMD,
            MinMaxDenominator voteAverageMMD)
        {
            double vcValue = (entity.VoteCount - voteCountMMD.MinValue) /
                             voteCountMMD.Denominator;
            double vaValue = (entity.VoteAverage - voteAverageMMD.MinValue) /
                             voteAverageMMD.Denominator;

            return vcValue + vaValue;
        }

        /// <summary>
        /// Checks that class was registered rating and has proper rating ID.
        /// </summary>
        protected void CheckRatingId()
        {
            if (RatingId == Guid.Empty)
            {
                throw new InvalidOperationException("Rating ID has no value.");
            }
        }
    }
}
