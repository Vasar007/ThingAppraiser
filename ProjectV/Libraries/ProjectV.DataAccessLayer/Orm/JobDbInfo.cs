﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acolyte.Assertions;

namespace ProjectV.DataAccessLayer.Orm
{
    [Table("jobs")]
    public sealed class JobDbInfo
    {
        [Key, Required]
        [Column("id")]
        public Guid Id { get; }

        [Required]
        [Column("name")]
        public string Name { get; }

        [Required]
        [Column("state")]
        public int State { get; }

        [Required]
        [Column("result")]
        public int Result { get; }

        [Required]
        [Column("config")]
        public string Config { get; }


        public JobDbInfo(
            Guid id,
            string name,
            int state,
            int result,
            string config)
        {
            Id = id;
            Name = name.ThrowIfNullOrWhiteSpace(nameof(config));
            State = state;
            Result = result;
            Config = config.ThrowIfNullOrWhiteSpace(nameof(config));
        }
    }
}