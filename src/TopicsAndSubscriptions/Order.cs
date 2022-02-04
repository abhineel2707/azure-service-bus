using System;
using System.Collections.Generic;
using System.Text;

namespace TopicsAndSubscriptions
{
    class Order
    {
        public string Name { get; set; }
        public DateTime OrderDate { get; set; }
        public int Items { get; set; }
        public double Value { get; set; }
        public string Priority { get; set; }
        public string Region { get; set; }
        public bool HasLoyalityCard { get; set; }
        public override string ToString()
        {
            return $"{Name}\t Item:{Items}\t ${Value}\t {Region}\t Loyal:{HasLoyalityCard}";
        }
    }
}
