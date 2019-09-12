namespace PatentNS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    /// <summary>
    /// Модель для доступа и взаемодействия с базой данных,
    /// класс Author что описывает таблицу Authors в бд.
    /// Связь много к одному с классом Patent
    /// </summary>
    public partial class Author
    {
        public Author()
        {
            Patents = new List<Patent>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FIO { get; set; }

        [Required]
        [StringLength(50)]
        public string Mail { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(20)]
        public string Card { get; set; }

        public virtual ICollection<Patent> Patents { get; set; }
    }
}
