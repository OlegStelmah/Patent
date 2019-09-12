namespace PatentNS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    /// <summary>
    /// ������ ��� ������� � �������������� � ����� ������,
    /// ����� Type ��� ��������� ������� Types � ��.
    /// ����� ���� � ������ � ������� Patent
    /// ������ ����� ����� ���������� ��� ��������, � ��� �� �� ���������
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
