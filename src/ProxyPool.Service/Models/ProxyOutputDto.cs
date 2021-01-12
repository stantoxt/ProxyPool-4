using System;

namespace ProxyPool.Service.Models
{
    [Serializable]
    public class ProxyOutputDto
    {
        public int Port { get; set; }

        public int AnonymousDegree { get; set; }

        public int Score { get; set; }

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }
    }
}
