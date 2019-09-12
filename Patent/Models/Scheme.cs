namespace PatentNS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    /// <summary>
    /// Модель для доступа и взаемодействия с базой данных,
    /// класс Scheme что описывает таблицу Schemes в бд.
    /// Связь один к многим с классом Patent
    /// </summary>
    public partial class Scheme
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public byte[] Data { get; set; }

        public int PatentId { get; set; }

        public virtual Patent Patent { get; set; }
    }
}
