﻿using System.Collections.Generic;

namespace ThingAppraiser.Models.Internal
{
    public sealed class ProcessedDataContainer
    {
        public IReadOnlyList<IReadOnlyList<ResultInfo>> Data { get; }

        public RatingsStorage RatingsStorage { get; }


        public ProcessedDataContainer(IReadOnlyList<IReadOnlyList<ResultInfo>> processedData,
            RatingsStorage ratingsStorage)
        {
            Data = processedData.ThrowIfNull(nameof(processedData));
            RatingsStorage = ratingsStorage.ThrowIfNull(nameof(ratingsStorage));
        }
    }
}