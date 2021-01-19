using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;

namespace BeatTogether.UI
{
    public class BeatTogetherSettings : PersistentSingleton<BeatTogetherSettings>
    {
        [UIValue("enabled")]
        public bool Enabled
        {
            get => Plugin.IsEnabled();
            set
            {
                Plugin.SetEnabled(value);
            }
        }
    }
}
