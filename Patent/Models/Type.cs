namespace PatentNS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    /// <summary>
    /// Модель для доступа и взаемодействия с базой данных,
    /// класс Type что описывает таблицу Types в бд.
    /// Связь один к многим с классом Patent
    /// Хранит набор типов публикаций для патентов, а так же их стоимость
    /// </summary>
    public partial class Type
    {
        public Type()
        {
            Patents = new List<Patent>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public virtual ICollection<Patent> Patents { get; set; }
    }
}
