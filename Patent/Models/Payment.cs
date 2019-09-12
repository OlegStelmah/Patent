namespace PatentNS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    /// <summary>
    /// ������ ��� ������� � �������������� � ����� ������,
    /// ����� Payment ��� ��������� ������� Payments � ��.
    /// ����� ���� � ������ � ������� Patent
    /// </summary>
    public partial class Payment
    {
        public int Id { get; set; }

        public int PatentId { get; set; }

        [Column(TypeName = "money")]
        public decimal Sum { get; set; }

        public virtual Patent Patent { get; set; }
    }
}
