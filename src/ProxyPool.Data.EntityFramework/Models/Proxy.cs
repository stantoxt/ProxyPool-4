using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProxyPool.Data.EntityFramework.Models
{
    [Table("t_proxys")]
    public class Proxy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("ip")]
        public string IP { get; set; }

        [Column("port")]
        public int Port { get; set; }

        [Column("anonymous_degree")]
        public int AnonymousDegree { get; set; }

        [Column("score")]
        public int Score { get; set; }

        [Column("created_time")]
        public DateTime? CreatedTime { get; set; }

        [Column("updated_time")]
        public DateTime? UpdatedTime { get; set; }
    }
}
