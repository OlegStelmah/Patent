namespace PatentNS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    /// <summary>
    /// ������ ��� ������� � �������������� � ����� ������,
    /// ����� Patent ��� ��������� ������� Patents � ��.
    /// ����� ���� � ������ � ������� Document, Payment, Schemes, Author, Type, Worker
    /// </summary>
    public partial class Patent
    {
        public Patent()
        {
            Documents = new List<Document>();
            Payments = new List<Payment>();
            Schemes = new List<Scheme>();
        }

        public int Id { get; set; }

        public int? PublicationId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime ApplicationDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PatentDate { get; set; }

        [Required]
        public string Description { get; set; }

        public bool Payment { get; set; }

        public int Status { get; set; }

        public int? TypeId { get; set; }

        public int AuthorId { get; set; }

        public int? WorkerId { get; set; }

        public virtual Author Author { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        public virtual Type Type { get; set; }

        public virtual Worker Worker { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

        public virtual ICollection<Scheme> Schemes { get; set; }

        public String AuthorName { get { return Author.FIO; } }

        /// <summary>
        /// �������� StatusStr ����� ��� ������������� ��������� ���� Status � ���� ������.
        /// ������������ � �������� ������ � �������� ��� Users, Admin
        /// </summary>
        public String StatusStr { get
            {
                switch (Status)
                {
                    case 0:
                        return "�� ����������";
                    case 1:
                        return "�� ������������";
                    case 2:
                        return "�� ������������� ������";
                    case 3:
                        return "������� ������";
                    case 4:
                        return "������������";
                    case 5:
                        return "���������";
                    default:
                        return "������";
                }
                }
            }
        }
    } 
