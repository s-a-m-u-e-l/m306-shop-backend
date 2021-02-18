using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public abstract class EntityBase
    {
        /// <summary>
        /// The entitie's id
        /// </summary>
        [Column("id")]
        public Guid Id { get; set; }
    }
}