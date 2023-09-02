using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace MasaoPlus
{
    public struct CustomPartsProperties
    {
        public int walk_speed;

        public int fall_speed;

        public int jump_vy;

        public int search_range;

        public int interval;

        public int period;

        [XmlElement("attack_timing")]
        public List<attack_timing> attack_timing;

        public int speed;

        public int accel;

        public int distance;

        public int attack_speed;

        public int return_speed;

        public int speed_x;

        public int speed_y;

        public int radius;

        public int init_vy;
    }
    public struct attack_timing
    {
        [XmlAttribute("AttackFrame")]
        public int AttackFrame;

        [XmlAttribute("IsPlaySoundFrame")]
        public bool IsPlaySoundFrame;
    }
}
