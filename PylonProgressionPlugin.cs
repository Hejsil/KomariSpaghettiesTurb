using System;
using System.Collections.Generic;
using System.Linq;
using Turbo.Plugins.Default;
using System.Reflection;

namespace Turbo.Plugins.KomariSpaghetti
{
    public class PylonProgressionPlugin : BasePlugin, IInGameWorldPainter
	{
        public HashSet<ShrineType> Pylons { get; set; }
        public Dictionary<ShrineType, string> Names { get; set; } 
        public Dictionary<ShrineType, double> FoundPercentages { get; set; } 

        public IFont PylonProgressionPluginFont { get; set; }

        public PylonProgressionPlugin()
		{
            Enabled = true;
            FoundPercentages = new Dictionary<ShrineType, double>();
            Names = new Dictionary<ShrineType, string>();
            Pylons = new HashSet<ShrineType>() 
            {
                ShrineType.PowerPylon,
                ShrineType.ConduitPylon,
                ShrineType.ShieldPylon,
                ShrineType.SpeedPylon,
                ShrineType.ChannelingPylon,
            };
		}

        public override void Load(IController hud)
        {
            base.Load(hud);
            PylonProgressionPluginFont = Hud.Render.CreateFont("tahoma", 7, 255, 255, 210, 150, true, false, 160, 0, 0, 0, true);
        }

		public void PaintWorld(WorldLayer layer)
		{
            IUiElement ui = null;
            switch (Hud.Game.SpecialArea)
            {
                case SpecialArea.Rift:
                    ui = Hud.Render.NephalemRiftBarUiElement;
                    break;
                case SpecialArea.GreaterRift:
                    ui = Hud.Render.GreaterRiftBarUiElement;
                    break;
                case SpecialArea.ChallengeRift:
                    ui = Hud.Render.ChallengeRiftBarUiElement;
                    break;
                default:
                    FoundPercentages.Clear();
                    return;
            }

            if (ui.Visible)
            {
                foreach (var shrine in Hud.Game.Shrines)
                {
                    if (!Pylons.Contains(shrine.Type)) continue;
                    if (!FoundPercentages.ContainsKey(shrine.Type)) FoundPercentages.Add(shrine.Type, Hud.Game.RiftPercentage);
                    if (!Names.ContainsKey(shrine.Type)) Names.Add(shrine.Type, shrine.SnoActor.NameLocalized);
                }

                var x = ui.Rectangle.Left;
                var y = ui.Rectangle.Bottom;

                var offset = 3;
                foreach (var pair in FoundPercentages) 
                {
                    var textLayout = PylonProgressionPluginFont.GetTextLayout(string.Format("{0} at {1:0.0}%", Names[pair.Key], pair.Value));                
                    PylonProgressionPluginFont.DrawText(textLayout, x, y + ((textLayout.Metrics.Height * 1.2f) * offset));
                    offset++;
                }
            }

        }
    }
}