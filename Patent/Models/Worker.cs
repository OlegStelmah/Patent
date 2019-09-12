namespace PatentNS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    /// <summary>
    /// ������ ��� ������� � �������������� � ����� ������,
    /// ����� Worker ��� ��������� ������� Workers � ��.
    /// ����� ���� � ������ � ������� Patent
    /// ������ ������ � ���������� ���������� ����, �� ������, ������ � ������������ ������
    /// </summary>
    public partial class Worker
    {
        public Worker()
        {
            Patents = new HashSet<Patent>();
        }
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FIO { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        public virtual ICollection<Patent> Patents { get; set; }
    }
}
